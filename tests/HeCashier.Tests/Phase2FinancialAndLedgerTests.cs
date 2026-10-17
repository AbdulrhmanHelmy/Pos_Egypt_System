using HeCashier.Data;
using HeCashier.Diagnostics;
using HeCashier.Infrastructure.Services;
using HeCashier.Models;
using HeCashier.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Xunit;

namespace HeCashier.Tests;

public sealed class Phase2FinancialAndLedgerTests
{
    [Fact]
    public async Task Shift_summary_and_close_reconcile_cash_and_only_one_open_shift_is_created()
    {
        await using var database = await TestDatabase.CreateAsync();
        var service = new ShiftService(database.Factory, new FileLogger(() => Path.GetTempPath()));
        var shift = await service.StartShiftAsync(database.UserId, 100m, database.SafeId);
        var second = await service.StartShiftAsync(database.UserId, 999m, database.SafeId);
        Assert.Equal(shift.Id, second.Id);
        Assert.Equal(100m, shift.OpeningBalance);
        Assert.Equal(database.SafeId, shift.SafeId);

        await using (var db = await database.Factory.CreateDbContextAsync())
        {
            db.Sales.Add(new Sale
            {
                InvoiceNumber = "PHASE2-CASH-1",
                CreatedAtUtc = DateTime.UtcNow,
                ShiftId = shift.Id,
                UserId = database.UserId,
                PaymentType = PaymentType.Cash,
                NetTotal = 90m,
                PaidAmount = 90m,
                Status = TransactionStatus.Completed
            });
            db.FinancialTransactions.AddRange(
                Financial(FinancialTransactionType.Refund, 5m, shift.Id, database.SafeId),
                Financial(FinancialTransactionType.CustomerCollection, 20m, shift.Id, database.SafeId),
                Financial(FinancialTransactionType.SupplierPayment, 10m, shift.Id, database.SafeId),
                Financial(FinancialTransactionType.Expense, 15m, shift.Id, database.SafeId));
            await db.SaveChangesAsync();
        }

        var summary = await service.GetShiftSummaryAsync(shift.Id);
        Assert.Equal(90m, summary.CashSales);
        Assert.Equal(5m, summary.CashReturns);
        Assert.Equal(20m, summary.CustomerCashCollections);
        Assert.Equal(10m, summary.SupplierCashPayments);
        Assert.Equal(15m, summary.ShiftExpenses);
        Assert.Equal(180m, summary.ExpectedCash);

        var closed = await service.CloseShiftAsync(shift.Id, 175m, "تم العد", database.UserId);
        Assert.Equal(ShiftStatus.Closed, closed.Status);
        Assert.Equal(180m, closed.ExpectedCash);
        Assert.Equal(-5m, closed.Difference);
        Assert.NotNull(closed.ClosedAtUtc);
        await using var verify = await database.Factory.CreateDbContextAsync();
        Assert.Contains(await verify.AuditEntries.ToListAsync(), x => x.Operation == "Shift.Closed" && x.UserId == database.UserId);
        Assert.Equal(1, await verify.Shifts.CountAsync(x => x.UserId == database.UserId && x.Status == ShiftStatus.Closed));
    }

    [Fact]
    public async Task Treasury_deposits_withdrawals_and_safe_bank_transfers_preserve_balances_atomically()
    {
        await using var database = await TestDatabase.CreateAsync();
        var treasury = new TreasuryService(database.Factory);
        var safe = await treasury.CreateSafeAsync(new SafeDraft("اختبار الخزينة", 100m), database.UserId);
        var bank = await treasury.CreateBankAccountAsync(new BankAccountDraft("اختبار البنك", OpeningBalance: 50m), database.UserId);

        var safeDeposit = await treasury.DepositToSafeAsync(safe.Id, 25m, "مودع", database.UserId);
        var safeWithdrawal = await treasury.WithdrawFromSafeAsync(safe.Id, 5m, "مستفيد", database.UserId);
        var bankDeposit = await treasury.DepositToBankAsync(bank.Id, 10m, "مودع بنك", database.UserId);
        var bankWithdrawal = await treasury.WithdrawFromBankAsync(bank.Id, 5m, "مستفيد بنك", database.UserId);
        var transfer = await treasury.TransferBetweenSafeAndBankAsync(safe.Id, bank.Id, 30m, true, "تحويل داخلي", database.UserId);

        Assert.Equal((100m, 125m), (safeDeposit.BalanceBefore, safeDeposit.BalanceAfter));
        Assert.Equal((125m, 120m), (safeWithdrawal.BalanceBefore, safeWithdrawal.BalanceAfter));
        Assert.Equal((50m, 60m), (bankDeposit.BalanceBefore, bankDeposit.BalanceAfter));
        Assert.Equal((60m, 55m), (bankWithdrawal.BalanceBefore, bankWithdrawal.BalanceAfter));
        Assert.Equal(2, transfer.Count);
        Assert.Contains(transfer, x => x.SafeId == safe.Id && x.BalanceBefore == 120m && x.BalanceAfter == 90m);
        Assert.Contains(transfer, x => x.BankAccountId == bank.Id && x.BalanceBefore == 55m && x.BalanceAfter == 85m);
        await Assert.ThrowsAsync<InvalidOperationException>(() => treasury.WithdrawFromSafeAsync(safe.Id, 91m, "مستفيد", database.UserId));
        await Assert.ThrowsAsync<ArgumentException>(() => treasury.DepositToSafeAsync(safe.Id, 1m, " ", database.UserId));

        var history = await treasury.GetFinancialTransactionsAsync(new FinancialTransactionFilter(SafeId: safe.Id));
        Assert.Equal(4, history.Count);
        await using var db = await database.Factory.CreateDbContextAsync();
        Assert.Equal(90m, (await db.Safes.SingleAsync(x => x.Id == safe.Id)).CurrentBalance);
        Assert.Equal(85m, (await db.BankAccounts.SingleAsync(x => x.Id == bank.Id)).CurrentBalance);
        Assert.Equal(1, await db.AuditEntries.CountAsync(x => x.Operation == "Treasury.Transfer"));
    }

    [Fact]
    public async Task Expense_deducts_safe_links_active_shift_and_records_financial_history()
    {
        await using var database = await TestDatabase.CreateAsync();
        var safe = await new TreasuryService(database.Factory).CreateSafeAsync(new SafeDraft("مصروفات", 100m), database.UserId);
        var expenseService = new ExpenseService(database.Factory);
        var expenseType = await expenseService.CreateExpenseTypeAsync(new ExpenseTypeDraft("نقل"), database.UserId);
        var shift = await new ShiftService(database.Factory, new FileLogger(() => Path.GetTempPath()))
            .StartShiftAsync(database.UserId, 0m, safe.Id);

        var expense = await expenseService.RecordExpenseAsync(
            new ExpenseDraft(expenseType.Id, safe.Id, 25m, "مصاريف نقل", ShiftId: shift.Id), database.UserId);

        await using var db = await database.Factory.CreateDbContextAsync();
        var stored = await db.Expenses.Include(x => x.FinancialTransaction).SingleAsync(x => x.Id == expense.Id);
        Assert.Equal(75m, (await db.Safes.SingleAsync(x => x.Id == safe.Id)).CurrentBalance);
        Assert.Equal(shift.Id, stored.ShiftId);
        Assert.Equal(FinancialTransactionType.Expense, stored.FinancialTransaction!.Type);
        Assert.Equal(100m, stored.FinancialTransaction.BalanceBefore);
        Assert.Equal(75m, stored.FinancialTransaction.BalanceAfter);
        Assert.Equal(shift.Id, stored.FinancialTransaction.ShiftId);
        Assert.Contains(await db.AuditEntries.ToListAsync(), x => x.Operation == "Expense.Recorded" && x.UserId == database.UserId);
        await Assert.ThrowsAsync<InvalidOperationException>(() => expenseService.RecordExpenseAsync(
            new ExpenseDraft(expenseType.Id, safe.Id, 100m, "مبلغ زائد"), database.UserId));
    }

    [Fact]
    public async Task Party_collections_and_payments_update_ledgers_and_safe_and_protect_cash_accounts()
    {
        await using var database = await TestDatabase.CreateAsync();
        var treasury = new TreasuryService(database.Factory);
        var safe = await treasury.CreateSafeAsync(new SafeDraft("تحصيلات", 200m), database.UserId);
        var parties = new CustomerSupplierService(database.Factory);
        var customer = await parties.CreateCustomerAsync(new CustomerDraft("عميل آجل", OpeningBalance: 100m), database.UserId);
        var supplier = await parties.CreateSupplierAsync(new SupplierDraft("مورد آجل", OpeningBalance: 80m), database.UserId);

        var collection = await parties.CollectFromCustomerAsync(customer.Id, 25m, safe.Id, null, "محصل", database.UserId);
        var payment = await parties.PaySupplierAsync(supplier.Id, 30m, safe.Id, null, "مستفيد", database.UserId);
        var customerStatement = await parties.GetCustomerStatementAsync(customer.Id);
        var supplierStatement = await parties.GetSupplierStatementAsync(supplier.Id);

        Assert.Equal(100m, collection.BalanceBefore);
        Assert.Equal(75m, collection.BalanceAfter);
        Assert.Equal(80m, payment.BalanceBefore);
        Assert.Equal(50m, payment.BalanceAfter);
        Assert.Equal(2, customerStatement.Count);
        Assert.Equal(75m, customerStatement[^1].BalanceAfter);
        Assert.Equal(2, supplierStatement.Count);
        Assert.Equal(50m, supplierStatement[^1].BalanceAfter);

        await using var db = await database.Factory.CreateDbContextAsync();
        Assert.Equal(195m, (await db.Safes.SingleAsync(x => x.Id == safe.Id)).CurrentBalance);
        Assert.Equal(225m, (await db.FinancialTransactions.SingleAsync(x => x.Type == FinancialTransactionType.CustomerCollection && x.ReferenceEntityId == customer.Id)).BalanceAfter);
        Assert.Equal(195m, (await db.FinancialTransactions.SingleAsync(x => x.Type == FinancialTransactionType.SupplierPayment && x.ReferenceEntityId == supplier.Id)).BalanceAfter);
        var cashCustomer = await db.Customers.SingleAsync(x => x.Name == "عميل نقدي");
        var cashSupplier = await db.Suppliers.SingleAsync(x => x.Name == "مورد نقدي");
        await Assert.ThrowsAsync<InvalidOperationException>(() => parties.DeleteCustomerAsync(cashCustomer.Id, database.UserId));
        await Assert.ThrowsAsync<InvalidOperationException>(() => parties.UpdateCustomerAsync(cashCustomer.Id,
            new CustomerDraft(cashCustomer.Name, IsCashCustomer: true, IsActive: false), database.UserId));
        await Assert.ThrowsAsync<InvalidOperationException>(() => parties.DeleteSupplierAsync(cashSupplier.Id, database.UserId));
        await Assert.ThrowsAsync<InvalidOperationException>(() => parties.UpdateSupplierAsync(cashSupplier.Id,
            new SupplierDraft(cashSupplier.Name, IsActive: false), database.UserId));
    }

    [Fact]
    public async Task External_debt_and_customer_check_lifecycles_settle_into_accounts()
    {
        await using var database = await TestDatabase.CreateAsync();
        var treasury = new TreasuryService(database.Factory);
        var safe = await treasury.CreateSafeAsync(new SafeDraft("تحصيل الشيكات", 0m), database.UserId);
        var bank = await treasury.CreateBankAccountAsync(new BankAccountDraft("بنك الشيكات"), database.UserId);
        var parties = new CustomerSupplierService(database.Factory);
        var customer = await parties.CreateCustomerAsync(new CustomerDraft("عميل الشيكات", OpeningBalance: 100m), database.UserId);
        var debt = await parties.RecordExternalDebtAsync(new ExternalDebtDraft(customer.Id, "دين خارجي", 40m), database.UserId);
        await parties.CollectExternalDebtAsync(debt.Id, 15m, safe.Id, null, "محصل", database.UserId);
        var check = await parties.CreateCustomerCheckAsync(new CustomerCheckDraft(customer.Id, "CHK-1", 30m, DateTime.UtcNow), database.UserId);
        var collected = await parties.CollectCustomerCheckAsync(check.Id, null, bank.Id, database.UserId);
        var rejected = await parties.CreateCustomerCheckAsync(new CustomerCheckDraft(customer.Id, "CHK-2", 10m, DateTime.UtcNow), database.UserId);
        var bounced = await parties.RejectCustomerCheckAsync(rejected.Id, "شيك مرتجع", database.UserId);

        Assert.Equal(25m, (await parties.GetExternalDebtsAsync(customer.Id, isSettled: false)).Single().RemainingAmount);
        Assert.Equal(CustomerCheckStatus.Collected, collected.Status);
        Assert.NotNull(collected.FinancialTransactionId);
        Assert.Equal(CustomerCheckStatus.Rejected, bounced.Status);
        await using var db = await database.Factory.CreateDbContextAsync();
        Assert.Equal(95m, (await db.Customers.SingleAsync(x => x.Id == customer.Id)).CurrentBalance);
        Assert.Equal(15m, (await db.Safes.SingleAsync(x => x.Id == safe.Id)).CurrentBalance);
        Assert.Equal(30m, (await db.BankAccounts.SingleAsync(x => x.Id == bank.Id)).CurrentBalance);
        Assert.Equal(2, await db.CustomerTransactions.CountAsync(x => x.CustomerId == customer.Id && x.Type == PartyTransactionType.Collection));
    }

    private static FinancialTransaction Financial(FinancialTransactionType type, decimal amount, int shiftId, int safeId) => new()
    {
        Type = type,
        Amount = amount,
        BalanceBefore = 0,
        BalanceAfter = 0,
        OccurredAtUtc = DateTime.UtcNow,
        Description = type.ToString(),
        TransactionType = type.ToString(),
        ShiftId = shiftId,
        SafeId = safeId
    };

    private sealed class TestDatabase : IAsyncDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly PooledDbContextFactory<HeCashierDbContext> _factory;

        public IDbContextFactory<HeCashierDbContext> Factory => _factory;
        public int UserId { get; private set; }
        public int SafeId { get; private set; }

        private TestDatabase(SqliteConnection connection, PooledDbContextFactory<HeCashierDbContext> factory)
        {
            _connection = connection;
            _factory = factory;
        }

        public static async Task<TestDatabase> CreateAsync()
        {
            var connection = new SqliteConnection("Data Source=:memory:");
            await connection.OpenAsync();
            var options = new DbContextOptionsBuilder<HeCashierDbContext>().UseSqlite(connection).Options;
            var factory = new PooledDbContextFactory<HeCashierDbContext>(options);
            var database = new TestDatabase(connection, factory);
            await using var db = await factory.CreateDbContextAsync();
            await db.Database.EnsureCreatedAsync();
            var user = new User { UserName = $"phase2-{Guid.NewGuid():N}", PasswordHash = "test" };
            var safe = new Safe { Name = "الخزينة الرئيسية", IsActive = true };
            db.Users.Add(user);
            db.Safes.Add(safe);
            db.Customers.Add(new Customer { Name = "عميل نقدي", IsCashCustomer = true });
            db.Suppliers.Add(new Supplier { Name = "مورد نقدي" });
            await db.SaveChangesAsync();
            database.UserId = user.Id;
            database.SafeId = safe.Id;
            return database;
        }

        public async ValueTask DisposeAsync() => await _connection.DisposeAsync();
    }
}
