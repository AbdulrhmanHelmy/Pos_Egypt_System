using HeCashier.Data;
using HeCashier.Models;
using HeCashier.Services;
using Microsoft.EntityFrameworkCore;

namespace HeCashier.Infrastructure.Services;

public sealed class CustomerSupplierService(IDbContextFactory<HeCashierDbContext> factory) : ICustomerSupplierService
{
    public async Task<IReadOnlyList<Customer>> SearchCustomersAsync(string? searchText = null, bool includeInactive = false)
    {
        await using var db = await factory.CreateDbContextAsync();
        var query = db.Customers.AsNoTracking().AsQueryable();
        if (!includeInactive)
            query = query.Where(x => x.IsActive);
        if (!string.IsNullOrWhiteSpace(searchText))
        {
            var term = searchText.Trim();
            query = query.Where(x => x.Name.Contains(term) ||
                                     (x.CompanyName != null && x.CompanyName.Contains(term)) ||
                                     (x.Phone != null && x.Phone.Contains(term)) ||
                                     (x.SecondaryPhone != null && x.SecondaryPhone.Contains(term)) ||
                                     (x.NationalId != null && x.NationalId.Contains(term)));
        }
        return await query.OrderBy(x => x.Name).ToListAsync();
    }

    public async Task<Customer> CreateCustomerAsync(CustomerDraft draft, int userId)
    {
        var name = FinancialServiceRules.RequiredText(draft.Name, nameof(draft.Name), 200);
        await using var db = await factory.CreateDbContextAsync();
        await using var transaction = await db.Database.BeginTransactionAsync();
        var customer = new Customer
        {
            Name = name,
            CompanyName = NormalizeOptional(draft.CompanyName, 200),
            Phone = NormalizeOptional(draft.Phone, 50),
            SecondaryPhone = NormalizeOptional(draft.SecondaryPhone, 50),
            Address = NormalizeOptional(draft.Address, 500),
            NationalId = NormalizeOptional(draft.NationalId, 50),
            OpeningBalance = draft.OpeningBalance,
            CurrentBalance = draft.OpeningBalance,
            CreditLimit = draft.CreditLimit,
            IsCashCustomer = draft.IsCashCustomer,
            IsActive = draft.IsActive,
            Notes = NormalizeOptional(draft.Notes, 1000)
        };
        db.Customers.Add(customer);
        await db.SaveChangesAsync();
        if (customer.OpeningBalance != 0)
            db.CustomerTransactions.Add(new CustomerTransaction
            {
                CustomerId = customer.Id,
                Type = PartyTransactionType.OpeningBalance,
                Amount = customer.OpeningBalance,
                BalanceBefore = 0,
                BalanceAfter = customer.OpeningBalance,
                OccurredAtUtc = DateTime.UtcNow,
                UserId = userId,
                Notes = "رصيد افتتاحي"
            });
        db.AuditEntries.Add(FinancialServiceRules.CreateAudit(userId, "Customer.Created", "Customer", customer.Id, customer.Name));
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        return customer;
    }

    public async Task<Customer> UpdateCustomerAsync(int customerId, CustomerDraft draft, int userId)
    {
        var name = FinancialServiceRules.RequiredText(draft.Name, nameof(draft.Name), 200);
        await using var db = await factory.CreateDbContextAsync();
        await using var transaction = await db.Database.BeginTransactionAsync();
        var customer = await db.Customers.SingleOrDefaultAsync(x => x.Id == customerId)
            ?? throw new KeyNotFoundException("العميل غير موجود.");
        if (IsProtectedCashCustomer(customer) && (!draft.IsActive || !draft.IsCashCustomer || name != "عميل نقدي"))
            throw new InvalidOperationException("لا يمكن حذف أو تعطيل العميل النقدي.");
        AdjustCustomerOpeningBalance(db, customer, draft.OpeningBalance, userId);
        customer.Name = name;
        customer.CompanyName = NormalizeOptional(draft.CompanyName, 200);
        customer.Phone = NormalizeOptional(draft.Phone, 50);
        customer.SecondaryPhone = NormalizeOptional(draft.SecondaryPhone, 50);
        customer.Address = NormalizeOptional(draft.Address, 500);
        customer.NationalId = NormalizeOptional(draft.NationalId, 50);
        customer.CreditLimit = draft.CreditLimit;
        customer.IsCashCustomer = draft.IsCashCustomer;
        customer.IsActive = draft.IsActive;
        customer.Notes = NormalizeOptional(draft.Notes, 1000);
        db.AuditEntries.Add(FinancialServiceRules.CreateAudit(userId, "Customer.Updated", "Customer", customer.Id, customer.Name));
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        return customer;
    }

    public async Task DeleteCustomerAsync(int customerId, int userId)
    {
        await using var db = await factory.CreateDbContextAsync();
        await using var transaction = await db.Database.BeginTransactionAsync();
        var customer = await db.Customers.SingleOrDefaultAsync(x => x.Id == customerId)
            ?? throw new KeyNotFoundException("العميل غير موجود.");
        if (IsProtectedCashCustomer(customer))
            throw new InvalidOperationException("لا يمكن حذف العميل النقدي.");
        if (await db.CustomerTransactions.AnyAsync(x => x.CustomerId == customerId) ||
            await db.Sales.AnyAsync(x => x.CustomerId == customerId) ||
            await db.ExternalDebts.AnyAsync(x => x.CustomerId == customerId) ||
            await db.CustomerChecks.AnyAsync(x => x.CustomerId == customerId) ||
            await db.InstallmentContracts.AnyAsync(x => x.CustomerId == customerId) ||
            await db.RepairOrders.AnyAsync(x => x.CustomerId == customerId))
            throw new InvalidOperationException("لا يمكن حذف عميل مرتبط بسجلات أو حركات.");
        db.Customers.Remove(customer);
        db.AuditEntries.Add(FinancialServiceRules.CreateAudit(userId, "Customer.Deleted", "Customer", customer.Id, customer.Name));
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    public async Task<IReadOnlyList<Supplier>> SearchSuppliersAsync(string? searchText = null, bool includeInactive = false)
    {
        await using var db = await factory.CreateDbContextAsync();
        var query = db.Suppliers.AsNoTracking().AsQueryable();
        if (!includeInactive)
            query = query.Where(x => x.IsActive);
        if (!string.IsNullOrWhiteSpace(searchText))
        {
            var term = searchText.Trim();
            query = query.Where(x => x.Name.Contains(term) ||
                                     (x.CompanyName != null && x.CompanyName.Contains(term)) ||
                                     (x.Phone != null && x.Phone.Contains(term)) ||
                                     (x.SecondaryPhone != null && x.SecondaryPhone.Contains(term)));
        }
        return await query.OrderBy(x => x.Name).ToListAsync();
    }

    public async Task<Supplier> CreateSupplierAsync(SupplierDraft draft, int userId)
    {
        var name = FinancialServiceRules.RequiredText(draft.Name, nameof(draft.Name), 200);
        await using var db = await factory.CreateDbContextAsync();
        await using var transaction = await db.Database.BeginTransactionAsync();
        var supplier = new Supplier
        {
            Name = name,
            CompanyName = NormalizeOptional(draft.CompanyName, 200),
            Phone = NormalizeOptional(draft.Phone, 50),
            SecondaryPhone = NormalizeOptional(draft.SecondaryPhone, 50),
            Address = NormalizeOptional(draft.Address, 500),
            OpeningBalance = draft.OpeningBalance,
            CurrentBalance = draft.OpeningBalance,
            CreditLimit = draft.CreditLimit,
            IsActive = draft.IsActive,
            Notes = NormalizeOptional(draft.Notes, 1000)
        };
        db.Suppliers.Add(supplier);
        await db.SaveChangesAsync();
        if (supplier.OpeningBalance != 0)
            db.SupplierTransactions.Add(new SupplierTransaction
            {
                SupplierId = supplier.Id,
                Type = PartyTransactionType.OpeningBalance,
                Amount = supplier.OpeningBalance,
                BalanceBefore = 0,
                BalanceAfter = supplier.OpeningBalance,
                OccurredAtUtc = DateTime.UtcNow,
                UserId = userId,
                Notes = "رصيد افتتاحي"
            });
        db.AuditEntries.Add(FinancialServiceRules.CreateAudit(userId, "Supplier.Created", "Supplier", supplier.Id, supplier.Name));
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        return supplier;
    }

    public async Task<Supplier> UpdateSupplierAsync(int supplierId, SupplierDraft draft, int userId)
    {
        var name = FinancialServiceRules.RequiredText(draft.Name, nameof(draft.Name), 200);
        await using var db = await factory.CreateDbContextAsync();
        await using var transaction = await db.Database.BeginTransactionAsync();
        var supplier = await db.Suppliers.SingleOrDefaultAsync(x => x.Id == supplierId)
            ?? throw new KeyNotFoundException("المورد غير موجود.");
        if (IsProtectedCashSupplier(supplier) && (!draft.IsActive || name != "مورد نقدي"))
            throw new InvalidOperationException("لا يمكن حذف أو تعطيل المورد النقدي.");
        AdjustSupplierOpeningBalance(db, supplier, draft.OpeningBalance, userId);
        supplier.Name = name;
        supplier.CompanyName = NormalizeOptional(draft.CompanyName, 200);
        supplier.Phone = NormalizeOptional(draft.Phone, 50);
        supplier.SecondaryPhone = NormalizeOptional(draft.SecondaryPhone, 50);
        supplier.Address = NormalizeOptional(draft.Address, 500);
        supplier.CreditLimit = draft.CreditLimit;
        supplier.IsActive = draft.IsActive;
        supplier.Notes = NormalizeOptional(draft.Notes, 1000);
        db.AuditEntries.Add(FinancialServiceRules.CreateAudit(userId, "Supplier.Updated", "Supplier", supplier.Id, supplier.Name));
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        return supplier;
    }

    public async Task DeleteSupplierAsync(int supplierId, int userId)
    {
        await using var db = await factory.CreateDbContextAsync();
        await using var transaction = await db.Database.BeginTransactionAsync();
        var supplier = await db.Suppliers.SingleOrDefaultAsync(x => x.Id == supplierId)
            ?? throw new KeyNotFoundException("المورد غير موجود.");
        if (IsProtectedCashSupplier(supplier))
            throw new InvalidOperationException("لا يمكن حذف المورد النقدي.");
        if (await db.SupplierTransactions.AnyAsync(x => x.SupplierId == supplierId) || await db.Purchases.AnyAsync(x => x.SupplierId == supplierId))
            throw new InvalidOperationException("لا يمكن حذف مورد مرتبط بسجلات أو حركات.");
        db.Suppliers.Remove(supplier);
        db.AuditEntries.Add(FinancialServiceRules.CreateAudit(userId, "Supplier.Deleted", "Supplier", supplier.Id, supplier.Name));
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    public async Task<CustomerTransaction> CollectFromCustomerAsync(int customerId, decimal amount, int? safeId, int? bankAccountId, string depositorName, int userId, int? shiftId = null, string? notes = null)
    {
        FinancialServiceRules.ValidateAmount(amount);
        var name = FinancialServiceRules.RequiredName(depositorName, nameof(depositorName));
        await using var db = await factory.CreateDbContextAsync();
        await using var transaction = await db.Database.BeginTransactionAsync();
        var customer = await db.Customers.SingleOrDefaultAsync(x => x.Id == customerId && x.IsActive)
            ?? throw new KeyNotFoundException("العميل غير موجود أو غير نشط.");
        var (safe, bank) = await GetAccountAsync(db, safeId, bankAccountId);
        var actualShiftId = await FinancialServiceRules.GetOpenShiftIdAsync(db, shiftId);
        var balanceBefore = customer.CurrentBalance;
        customer.CurrentBalance -= amount;
        var financial = AddAccountTransaction(db, safe, bank, FinancialTransactionType.CustomerCollection, "CustomerCollection", amount, true, name, userId, actualShiftId, "Customer", customer.Id, "تحصيل من عميل");
        var ledger = new CustomerTransaction
        {
            CustomerId = customer.Id,
            Type = PartyTransactionType.Collection,
            Amount = amount,
            BalanceBefore = balanceBefore,
            BalanceAfter = customer.CurrentBalance,
            OccurredAtUtc = DateTime.UtcNow,
            FinancialTransaction = financial,
            UserId = userId,
            Notes = NormalizeNotes(notes)
        };
        db.CustomerTransactions.Add(ledger);
        db.AuditEntries.Add(FinancialServiceRules.CreateAudit(userId, "Customer.Collected", "Customer", customer.Id, $"Amount:{amount}"));
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        return ledger;
    }

    public async Task<SupplierTransaction> PaySupplierAsync(int supplierId, decimal amount, int? safeId, int? bankAccountId, string beneficiaryName, int userId, int? shiftId = null, string? notes = null)
    {
        FinancialServiceRules.ValidateAmount(amount);
        var name = FinancialServiceRules.RequiredName(beneficiaryName, nameof(beneficiaryName));
        await using var db = await factory.CreateDbContextAsync();
        await using var transaction = await db.Database.BeginTransactionAsync();
        var supplier = await db.Suppliers.SingleOrDefaultAsync(x => x.Id == supplierId && x.IsActive)
            ?? throw new KeyNotFoundException("المورد غير موجود أو غير نشط.");
        var (safe, bank) = await GetAccountAsync(db, safeId, bankAccountId);
        var currentBalance = safe?.CurrentBalance ?? bank!.CurrentBalance;
        if (currentBalance < amount)
            throw new InvalidOperationException("الرصيد غير كافٍ للسداد.");
        var actualShiftId = await FinancialServiceRules.GetOpenShiftIdAsync(db, shiftId);
        var balanceBefore = supplier.CurrentBalance;
        supplier.CurrentBalance -= amount;
        var financial = AddAccountTransaction(db, safe, bank, FinancialTransactionType.SupplierPayment, "SupplierPayment", amount, false, name, userId, actualShiftId, "Supplier", supplier.Id, "سداد لمورد");
        var ledger = new SupplierTransaction
        {
            SupplierId = supplier.Id,
            Type = PartyTransactionType.Payment,
            Amount = amount,
            BalanceBefore = balanceBefore,
            BalanceAfter = supplier.CurrentBalance,
            OccurredAtUtc = DateTime.UtcNow,
            FinancialTransaction = financial,
            UserId = userId,
            Notes = NormalizeNotes(notes)
        };
        db.SupplierTransactions.Add(ledger);
        db.AuditEntries.Add(FinancialServiceRules.CreateAudit(userId, "Supplier.Paid", "Supplier", supplier.Id, $"Amount:{amount}"));
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        return ledger;
    }

    public async Task<IReadOnlyList<AccountStatementEntry>> GetCustomerStatementAsync(int customerId, DateTime? fromUtc = null, DateTime? toUtc = null)
    {
        await using var db = await factory.CreateDbContextAsync();
        if (!await db.Customers.AnyAsync(x => x.Id == customerId))
            throw new KeyNotFoundException("العميل غير موجود.");
        var query = db.CustomerTransactions.AsNoTracking().Where(x => x.CustomerId == customerId);
        if (fromUtc.HasValue)
            query = query.Where(x => x.OccurredAtUtc >= fromUtc.Value);
        if (toUtc.HasValue)
            query = query.Where(x => x.OccurredAtUtc <= toUtc.Value);
        return await query.OrderBy(x => x.OccurredAtUtc).ThenBy(x => x.Id)
            .Select(x => new AccountStatementEntry(x.OccurredAtUtc, x.Type, x.Amount, x.BalanceBefore, x.BalanceAfter, x.ReferenceNumber, x.Notes))
            .ToListAsync();
    }

    public async Task<IReadOnlyList<AccountStatementEntry>> GetSupplierStatementAsync(int supplierId, DateTime? fromUtc = null, DateTime? toUtc = null)
    {
        await using var db = await factory.CreateDbContextAsync();
        if (!await db.Suppliers.AnyAsync(x => x.Id == supplierId))
            throw new KeyNotFoundException("المورد غير موجود.");
        var query = db.SupplierTransactions.AsNoTracking().Where(x => x.SupplierId == supplierId);
        if (fromUtc.HasValue)
            query = query.Where(x => x.OccurredAtUtc >= fromUtc.Value);
        if (toUtc.HasValue)
            query = query.Where(x => x.OccurredAtUtc <= toUtc.Value);
        return await query.OrderBy(x => x.OccurredAtUtc).ThenBy(x => x.Id)
            .Select(x => new AccountStatementEntry(x.OccurredAtUtc, x.Type, x.Amount, x.BalanceBefore, x.BalanceAfter, x.ReferenceNumber, x.Notes))
            .ToListAsync();
    }

    public async Task<IReadOnlyList<ExternalDebt>> GetExternalDebtsAsync(int? customerId = null, bool? isSettled = null)
    {
        await using var db = await factory.CreateDbContextAsync();
        var query = db.ExternalDebts.Include(x => x.Customer).AsNoTracking().AsQueryable();
        if (customerId.HasValue)
            query = query.Where(x => x.CustomerId == customerId.Value);
        if (isSettled.HasValue)
            query = query.Where(x => x.IsSettled == isSettled.Value);
        return await query.OrderBy(x => x.OccurredAtUtc).ThenBy(x => x.Id).ToListAsync();
    }

    public async Task<ExternalDebt> RecordExternalDebtAsync(ExternalDebtDraft draft, int userId)
    {
        var description = FinancialServiceRules.RequiredText(draft.Description, nameof(draft.Description), 200);
        FinancialServiceRules.ValidateAmount(draft.Amount);
        await using var db = await factory.CreateDbContextAsync();
        await using var transaction = await db.Database.BeginTransactionAsync();
        var customer = await db.Customers.SingleOrDefaultAsync(x => x.Id == draft.CustomerId && x.IsActive)
            ?? throw new KeyNotFoundException("العميل غير موجود أو غير نشط.");
        var notes = NormalizeNotes(draft.Notes);
        var before = customer.CurrentBalance;
        customer.CurrentBalance += draft.Amount;
        var debt = new ExternalDebt
        {
            CustomerId = customer.Id,
            Description = description,
            OriginalAmount = draft.Amount,
            RemainingAmount = draft.Amount,
            OccurredAtUtc = DateTime.UtcNow,
            DueAtUtc = draft.DueAtUtc,
            UserId = userId,
            Notes = notes ?? string.Empty
        };
        var ledger = new CustomerTransaction
        {
            Customer = customer,
            ExternalDebt = debt,
            Type = PartyTransactionType.ExternalDebt,
            Amount = draft.Amount,
            BalanceBefore = before,
            BalanceAfter = customer.CurrentBalance,
            OccurredAtUtc = DateTime.UtcNow,
            UserId = userId,
            Notes = description
        };
        db.ExternalDebts.Add(debt);
        db.CustomerTransactions.Add(ledger);
        await db.SaveChangesAsync();
        db.AuditEntries.Add(FinancialServiceRules.CreateAudit(userId, "ExternalDebt.Recorded", "ExternalDebt", debt.Id, $"Customer:{customer.Id};Amount:{draft.Amount}"));
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        return debt;
    }

    public async Task<CustomerTransaction> CollectExternalDebtAsync(int externalDebtId, decimal amount, int? safeId, int? bankAccountId, string depositorName, int userId, int? shiftId = null, string? notes = null)
    {
        FinancialServiceRules.ValidateAmount(amount);
        var name = FinancialServiceRules.RequiredName(depositorName, nameof(depositorName));
        await using var db = await factory.CreateDbContextAsync();
        await using var transaction = await db.Database.BeginTransactionAsync();
        var debt = await db.ExternalDebts.Include(x => x.Customer).SingleOrDefaultAsync(x => x.Id == externalDebtId)
            ?? throw new KeyNotFoundException("الدين الخارجي غير موجود.");
        if (debt.IsSettled || amount > debt.RemainingAmount)
            throw new InvalidOperationException("مبلغ التحصيل يتجاوز الرصيد المتبقي للدين.");
        var (safe, bank) = await GetAccountAsync(db, safeId, bankAccountId);
        var actualShiftId = await FinancialServiceRules.GetOpenShiftIdAsync(db, shiftId);
        var customer = debt.Customer;
        var balanceBefore = customer.CurrentBalance;
        customer.CurrentBalance -= amount;
        debt.RemainingAmount -= amount;
        debt.IsSettled = debt.RemainingAmount == 0;
        var financial = AddAccountTransaction(db, safe, bank, FinancialTransactionType.CustomerCollection, "CustomerCollection", amount, true, name, userId, actualShiftId, "ExternalDebt", debt.Id, "تحصيل دين خارجي");
        var ledger = new CustomerTransaction
        {
            CustomerId = customer.Id,
            Type = PartyTransactionType.Collection,
            Amount = amount,
            BalanceBefore = balanceBefore,
            BalanceAfter = customer.CurrentBalance,
            OccurredAtUtc = DateTime.UtcNow,
            ExternalDebtId = debt.Id,
            FinancialTransaction = financial,
            UserId = userId,
            Notes = NormalizeNotes(notes)
        };
        db.CustomerTransactions.Add(ledger);
        db.AuditEntries.Add(FinancialServiceRules.CreateAudit(userId, "ExternalDebt.Collected", "ExternalDebt", debt.Id, $"Amount:{amount};Remaining:{debt.RemainingAmount}"));
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        return ledger;
    }

    public async Task<IReadOnlyList<CustomerCheck>> GetCustomerChecksAsync(int? customerId = null, CustomerCheckStatus? status = null)
    {
        await using var db = await factory.CreateDbContextAsync();
        var query = db.CustomerChecks.Include(x => x.Customer).AsNoTracking().AsQueryable();
        if (customerId.HasValue)
            query = query.Where(x => x.CustomerId == customerId.Value);
        if (status.HasValue)
            query = query.Where(x => x.Status == status.Value);
        return await query.OrderBy(x => x.DueAtUtc).ThenBy(x => x.Id).ToListAsync();
    }

    public async Task<CustomerCheck> CreateCustomerCheckAsync(CustomerCheckDraft draft, int userId)
    {
        FinancialServiceRules.ValidateAmount(draft.Amount);
        var checkNumber = FinancialServiceRules.RequiredText(draft.CheckNumber, nameof(draft.CheckNumber), 100);
        await using var db = await factory.CreateDbContextAsync();
        await using var transaction = await db.Database.BeginTransactionAsync();
        if (!await db.Customers.AnyAsync(x => x.Id == draft.CustomerId && x.IsActive))
            throw new KeyNotFoundException("العميل غير موجود أو غير نشط.");
        if (await db.CustomerChecks.AnyAsync(x => x.CustomerId == draft.CustomerId && x.CheckNumber == checkNumber))
            throw new InvalidOperationException("رقم الشيك مسجل لهذا العميل بالفعل.");
        var check = new CustomerCheck
        {
            CustomerId = draft.CustomerId,
            CheckNumber = checkNumber,
            Amount = draft.Amount,
            IssuedAtUtc = draft.IssuedAtUtc,
            DueAtUtc = draft.DueAtUtc,
            BankName = NormalizeOptional(draft.BankName, 150),
            IsPromissoryNote = draft.IsPromissoryNote,
            Notes = NormalizeNotes(draft.Notes) ?? string.Empty,
            Status = CustomerCheckStatus.Pending
        };
        db.CustomerChecks.Add(check);
        await db.SaveChangesAsync();
        db.AuditEntries.Add(FinancialServiceRules.CreateAudit(userId, "CustomerCheck.Created", "CustomerCheck", check.Id, $"Customer:{check.CustomerId};Number:{check.CheckNumber}"));
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        return check;
    }

    public async Task<CustomerCheck> CollectCustomerCheckAsync(int checkId, int? safeId, int? bankAccountId, int userId, int? shiftId = null)
    {
        await using var db = await factory.CreateDbContextAsync();
        await using var transaction = await db.Database.BeginTransactionAsync();
        var check = await db.CustomerChecks.Include(x => x.Customer).SingleOrDefaultAsync(x => x.Id == checkId)
            ?? throw new KeyNotFoundException("الشيك غير موجود.");
        if (check.Status != CustomerCheckStatus.Pending)
            throw new InvalidOperationException("الشيك غير معلق ولا يمكن تحصيله.");
        var (safe, bank) = await GetAccountAsync(db, safeId, bankAccountId);
        var actualShiftId = await FinancialServiceRules.GetOpenShiftIdAsync(db, shiftId);
        var balanceBefore = check.Customer.CurrentBalance;
        check.Customer.CurrentBalance -= check.Amount;
        var financial = AddAccountTransaction(db, safe, bank, FinancialTransactionType.CustomerCollection, "CustomerCheckCollection", check.Amount, true, check.CheckNumber, userId, actualShiftId, "CustomerCheck", check.Id, "تحصيل شيك عميل");
        check.Status = CustomerCheckStatus.Collected;
        check.ClearedAtUtc = DateTime.UtcNow;
        check.FinancialTransaction = financial;
        db.CustomerTransactions.Add(new CustomerTransaction
        {
            CustomerId = check.CustomerId,
            Type = PartyTransactionType.Collection,
            Amount = check.Amount,
            BalanceBefore = balanceBefore,
            BalanceAfter = check.Customer.CurrentBalance,
            OccurredAtUtc = check.ClearedAtUtc.Value,
            ReferenceNumber = check.CheckNumber,
            FinancialTransaction = financial,
            UserId = userId,
            Notes = "تحصيل شيك"
        });
        db.AuditEntries.Add(FinancialServiceRules.CreateAudit(userId, "CustomerCheck.Collected", "CustomerCheck", check.Id, $"Amount:{check.Amount}"));
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        return check;
    }

    public async Task<CustomerCheck> RejectCustomerCheckAsync(int checkId, string reason, int userId)
    {
        var rejectionReason = FinancialServiceRules.RequiredText(reason, nameof(reason), 500);
        await using var db = await factory.CreateDbContextAsync();
        await using var transaction = await db.Database.BeginTransactionAsync();
        var check = await db.CustomerChecks.SingleOrDefaultAsync(x => x.Id == checkId)
            ?? throw new KeyNotFoundException("الشيك غير موجود.");
        if (check.Status != CustomerCheckStatus.Pending)
            throw new InvalidOperationException("الشيك غير معلق ولا يمكن رفضه.");
        check.Status = CustomerCheckStatus.Rejected;
        check.Notes = string.IsNullOrWhiteSpace(check.Notes) ? rejectionReason : $"{check.Notes}; {rejectionReason}";
        if (check.Notes.Length > 1000)
            check.Notes = check.Notes[..1000];
        db.AuditEntries.Add(FinancialServiceRules.CreateAudit(userId, "CustomerCheck.Rejected", "CustomerCheck", check.Id, rejectionReason));
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        return check;
    }

    private static async Task<(Safe? Safe, BankAccount? Bank)> GetAccountAsync(HeCashierDbContext db, int? safeId, int? bankAccountId)
    {
        if (safeId.HasValue == bankAccountId.HasValue)
            throw new ArgumentException("يجب تحديد خزينة أو حساب بنكي واحد فقط.");
        var safe = safeId.HasValue
            ? await db.Safes.SingleOrDefaultAsync(x => x.Id == safeId.Value && x.IsActive)
            : null;
        var bank = bankAccountId.HasValue
            ? await db.BankAccounts.SingleOrDefaultAsync(x => x.Id == bankAccountId.Value && x.IsActive)
            : null;
        if (safeId.HasValue && safe is null)
            throw new KeyNotFoundException("الخزينة غير موجودة أو غير نشطة.");
        if (bankAccountId.HasValue && bank is null)
            throw new KeyNotFoundException("الحساب البنكي غير موجود أو غير نشط.");
        return (safe, bank);
    }

    private static FinancialTransaction AddAccountTransaction(
        HeCashierDbContext db,
        Safe? safe,
        BankAccount? bank,
        FinancialTransactionType type,
        string transactionType,
        decimal amount,
        bool incoming,
        string name,
        int userId,
        int? shiftId,
        string referenceType,
        int referenceId,
        string description)
    {
        var balanceBefore = safe?.CurrentBalance ?? bank!.CurrentBalance;
        if (safe is not null)
            safe.CurrentBalance += incoming ? amount : -amount;
        else
            bank!.CurrentBalance += incoming ? amount : -amount;
        var financial = FinancialServiceRules.CreateTransaction(type, transactionType, amount, balanceBefore, incoming,
            description, name, userId, safeId: safe?.Id, bankAccountId: bank?.Id, shiftId: shiftId,
            referenceEntityType: referenceType, referenceEntityId: referenceId);
        db.FinancialTransactions.Add(financial);
        return financial;
    }

    private static void AdjustCustomerOpeningBalance(HeCashierDbContext db, Customer customer, decimal newOpeningBalance, int userId)
    {
        var delta = newOpeningBalance - customer.OpeningBalance;
        if (delta == 0)
            return;
        var before = customer.CurrentBalance;
        customer.OpeningBalance = newOpeningBalance;
        customer.CurrentBalance += delta;
        db.CustomerTransactions.Add(new CustomerTransaction
        {
            CustomerId = customer.Id,
            Type = PartyTransactionType.Adjustment,
            Amount = delta,
            BalanceBefore = before,
            BalanceAfter = customer.CurrentBalance,
            OccurredAtUtc = DateTime.UtcNow,
            UserId = userId,
            Notes = "تعديل الرصيد الافتتاحي"
        });
    }

    private static void AdjustSupplierOpeningBalance(HeCashierDbContext db, Supplier supplier, decimal newOpeningBalance, int userId)
    {
        var delta = newOpeningBalance - supplier.OpeningBalance;
        if (delta == 0)
            return;
        var before = supplier.CurrentBalance;
        supplier.OpeningBalance = newOpeningBalance;
        supplier.CurrentBalance += delta;
        db.SupplierTransactions.Add(new SupplierTransaction
        {
            SupplierId = supplier.Id,
            Type = PartyTransactionType.Adjustment,
            Amount = delta,
            BalanceBefore = before,
            BalanceAfter = supplier.CurrentBalance,
            OccurredAtUtc = DateTime.UtcNow,
            UserId = userId,
            Notes = "تعديل الرصيد الافتتاحي"
        });
    }

    private static bool IsProtectedCashCustomer(Customer customer) => customer.IsCashCustomer && customer.Name == "عميل نقدي";
    private static bool IsProtectedCashSupplier(Supplier supplier) => supplier.Name == "مورد نقدي";

    private static string? NormalizeOptional(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;
        var normalized = value.Trim();
        if (normalized.Length > maxLength)
            throw new ArgumentOutOfRangeException(nameof(value), "النص أطول من الحد المسموح.");
        return normalized;
    }

    private static string NormalizeNotes(string? value)
    {
        var notes = value?.Trim() ?? string.Empty;
        if (notes.Length > 1000)
            throw new ArgumentOutOfRangeException(nameof(value), "الملاحظات أطول من الحد المسموح.");
        return notes;
    }
}
