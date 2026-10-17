using HeCashier.Data;
using HeCashier.Models;
using HeCashier.Services;
using Microsoft.EntityFrameworkCore;

namespace HeCashier.Infrastructure.Services;

public sealed class TreasuryService(IDbContextFactory<HeCashierDbContext> factory) : ITreasuryService
{
    public async Task<IReadOnlyList<Safe>> GetSafesAsync(bool includeInactive = false)
    {
        await using var db = await factory.CreateDbContextAsync();
        var query = db.Safes.AsNoTracking();
        if (!includeInactive)
            query = query.Where(x => x.IsActive);
        return await query.OrderBy(x => x.Name).ToListAsync();
    }

    public async Task<Safe> CreateSafeAsync(SafeDraft draft, int userId)
    {
        ValidateOpeningBalance(draft.OpeningBalance);
        var name = FinancialServiceRules.RequiredText(draft.Name, nameof(draft.Name), 150);
        await using var db = await factory.CreateDbContextAsync();
        await using var transaction = await db.Database.BeginTransactionAsync();
        if (await db.Safes.AnyAsync(x => x.Name == name))
            throw new InvalidOperationException("اسم الخزينة مستخدم بالفعل.");
        var safe = new Safe { Name = name, OpeningBalance = draft.OpeningBalance, CurrentBalance = draft.OpeningBalance, IsActive = draft.IsActive };
        db.Safes.Add(safe);
        await db.SaveChangesAsync();
        if (safe.OpeningBalance != 0)
            db.FinancialTransactions.Add(FinancialServiceRules.CreateTransaction(FinancialTransactionType.OpeningBalance, "OpeningBalance", safe.OpeningBalance, 0, true, "رصيد افتتاحي للخزينة", "رصيد افتتاحي", userId, safeId: safe.Id));
        db.AuditEntries.Add(FinancialServiceRules.CreateAudit(userId, "Safe.Created", "Safe", safe.Id, safe.Name));
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        return safe;
    }

    public async Task<Safe> UpdateSafeAsync(int safeId, SafeDraft draft, int userId)
    {
        ValidateOpeningBalance(draft.OpeningBalance);
        var name = FinancialServiceRules.RequiredText(draft.Name, nameof(draft.Name), 150);
        await using var db = await factory.CreateDbContextAsync();
        await using var transaction = await db.Database.BeginTransactionAsync();
        var safe = await db.Safes.SingleOrDefaultAsync(x => x.Id == safeId)
            ?? throw new KeyNotFoundException("الخزينة غير موجودة.");
        if (await db.Safes.AnyAsync(x => x.Id != safeId && x.Name == name))
            throw new InvalidOperationException("اسم الخزينة مستخدم بالفعل.");
        ApplyOpeningBalanceChange(db, safe, draft.OpeningBalance, userId);
        safe.Name = name;
        safe.IsActive = draft.IsActive;
        db.AuditEntries.Add(FinancialServiceRules.CreateAudit(userId, "Safe.Updated", "Safe", safe.Id, safe.Name));
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        return safe;
    }

    public async Task DeleteSafeAsync(int safeId, int userId)
    {
        await using var db = await factory.CreateDbContextAsync();
        await using var transaction = await db.Database.BeginTransactionAsync();
        var safe = await db.Safes.SingleOrDefaultAsync(x => x.Id == safeId)
            ?? throw new KeyNotFoundException("الخزينة غير موجودة.");
        if (safe.Name == "الخزينة الرئيسية")
            throw new InvalidOperationException("لا يمكن حذف الخزينة الرئيسية.");
        if (await db.FinancialTransactions.AnyAsync(x => x.SafeId == safeId) ||
            await db.Expenses.AnyAsync(x => x.SafeId == safeId) ||
            await db.EmployeeFinancialTransactions.AnyAsync(x => x.SafeId == safeId) ||
            await db.SalaryPayments.AnyAsync(x => x.SafeId == safeId))
            throw new InvalidOperationException("لا يمكن حذف خزينة لها حركات مالية.");
        db.Safes.Remove(safe);
        db.AuditEntries.Add(FinancialServiceRules.CreateAudit(userId, "Safe.Deleted", "Safe", safe.Id, safe.Name));
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    public async Task<IReadOnlyList<BankAccount>> GetBankAccountsAsync(bool includeInactive = false)
    {
        await using var db = await factory.CreateDbContextAsync();
        var query = db.BankAccounts.AsNoTracking();
        if (!includeInactive)
            query = query.Where(x => x.IsActive);
        return await query.OrderBy(x => x.Name).ToListAsync();
    }

    public async Task<BankAccount> CreateBankAccountAsync(BankAccountDraft draft, int userId)
    {
        ValidateOpeningBalance(draft.OpeningBalance);
        var name = FinancialServiceRules.RequiredText(draft.Name, nameof(draft.Name), 150);
        await using var db = await factory.CreateDbContextAsync();
        await using var transaction = await db.Database.BeginTransactionAsync();
        var account = new BankAccount
        {
            Name = name,
            AccountNumber = NormalizeOptional(draft.AccountNumber, 100),
            BankName = NormalizeOptional(draft.BankName, 100),
            OpeningBalance = draft.OpeningBalance,
            CurrentBalance = draft.OpeningBalance,
            IsActive = draft.IsActive
        };
        db.BankAccounts.Add(account);
        await db.SaveChangesAsync();
        if (account.OpeningBalance != 0)
            db.FinancialTransactions.Add(FinancialServiceRules.CreateTransaction(FinancialTransactionType.OpeningBalance, "OpeningBalance", account.OpeningBalance, 0, true, "رصيد افتتاحي للحساب البنكي", "رصيد افتتاحي", userId, bankAccountId: account.Id));
        db.AuditEntries.Add(FinancialServiceRules.CreateAudit(userId, "BankAccount.Created", "BankAccount", account.Id, account.Name));
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        return account;
    }

    public async Task<BankAccount> UpdateBankAccountAsync(int bankAccountId, BankAccountDraft draft, int userId)
    {
        ValidateOpeningBalance(draft.OpeningBalance);
        var name = FinancialServiceRules.RequiredText(draft.Name, nameof(draft.Name), 150);
        await using var db = await factory.CreateDbContextAsync();
        await using var transaction = await db.Database.BeginTransactionAsync();
        var account = await db.BankAccounts.SingleOrDefaultAsync(x => x.Id == bankAccountId)
            ?? throw new KeyNotFoundException("الحساب البنكي غير موجود.");
        ApplyOpeningBalanceChange(db, account, draft.OpeningBalance, userId);
        account.Name = name;
        account.AccountNumber = NormalizeOptional(draft.AccountNumber, 100);
        account.BankName = NormalizeOptional(draft.BankName, 100);
        account.IsActive = draft.IsActive;
        db.AuditEntries.Add(FinancialServiceRules.CreateAudit(userId, "BankAccount.Updated", "BankAccount", account.Id, account.Name));
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        return account;
    }

    public async Task DeleteBankAccountAsync(int bankAccountId, int userId)
    {
        await using var db = await factory.CreateDbContextAsync();
        await using var transaction = await db.Database.BeginTransactionAsync();
        var account = await db.BankAccounts.SingleOrDefaultAsync(x => x.Id == bankAccountId)
            ?? throw new KeyNotFoundException("الحساب البنكي غير موجود.");
        if (await db.FinancialTransactions.AnyAsync(x => x.BankAccountId == bankAccountId) ||
            await db.Expenses.AnyAsync(x => x.BankAccountId == bankAccountId))
            throw new InvalidOperationException("لا يمكن حذف حساب بنكي له حركات مالية.");
        db.BankAccounts.Remove(account);
        db.AuditEntries.Add(FinancialServiceRules.CreateAudit(userId, "BankAccount.Deleted", "BankAccount", account.Id, account.Name));
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    public Task<FinancialTransaction> DepositToSafeAsync(int safeId, decimal amount, string depositorName, int userId, int? shiftId = null, string? description = null) =>
        ChangeSafeBalanceAsync(safeId, amount, depositorName, userId, shiftId, incoming: true, FinancialTransactionType.Deposit, "Safe.Deposit", description);

    public Task<FinancialTransaction> WithdrawFromSafeAsync(int safeId, decimal amount, string beneficiaryName, int userId, int? shiftId = null, string? description = null) =>
        ChangeSafeBalanceAsync(safeId, amount, beneficiaryName, userId, shiftId, incoming: false, FinancialTransactionType.Withdrawal, "Safe.Withdrawal", description);

    public Task<FinancialTransaction> DepositToBankAsync(int bankAccountId, decimal amount, string depositorName, int userId, int? shiftId = null, string? description = null) =>
        ChangeBankBalanceAsync(bankAccountId, amount, depositorName, userId, shiftId, incoming: true, FinancialTransactionType.Deposit, "Bank.Deposit", description);

    public Task<FinancialTransaction> WithdrawFromBankAsync(int bankAccountId, decimal amount, string beneficiaryName, int userId, int? shiftId = null, string? description = null) =>
        ChangeBankBalanceAsync(bankAccountId, amount, beneficiaryName, userId, shiftId, incoming: false, FinancialTransactionType.Withdrawal, "Bank.Withdrawal", description);

    public async Task<IReadOnlyList<FinancialTransaction>> TransferBetweenSafeAndBankAsync(int safeId, int bankAccountId, decimal amount, bool safeToBank, string beneficiaryName, int userId, int? shiftId = null, string? description = null)
    {
        FinancialServiceRules.ValidateAmount(amount);
        var name = FinancialServiceRules.RequiredName(beneficiaryName, nameof(beneficiaryName));
        await using var db = await factory.CreateDbContextAsync();
        await using var transaction = await db.Database.BeginTransactionAsync();
        var safe = await db.Safes.SingleOrDefaultAsync(x => x.Id == safeId && x.IsActive)
            ?? throw new KeyNotFoundException("الخزينة غير موجودة أو غير نشطة.");
        var bank = await db.BankAccounts.SingleOrDefaultAsync(x => x.Id == bankAccountId && x.IsActive)
            ?? throw new KeyNotFoundException("الحساب البنكي غير موجود أو غير نشط.");
        var actualShiftId = await FinancialServiceRules.GetOpenShiftIdAsync(db, shiftId);
        if (safeToBank && safe.CurrentBalance < amount || !safeToBank && bank.CurrentBalance < amount)
            throw new InvalidOperationException("الرصيد غير كافٍ لإتمام التحويل.");

        var safeBefore = safe.CurrentBalance;
        var bankBefore = bank.CurrentBalance;
        safe.CurrentBalance += safeToBank ? -amount : amount;
        bank.CurrentBalance += safeToBank ? amount : -amount;
        var time = DateTime.UtcNow;
        var safeTransaction = FinancialServiceRules.CreateTransaction(
            FinancialTransactionType.Transfer, safeToBank ? "TransferOut" : "TransferIn", amount,
            safeBefore, !safeToBank, description ?? "تحويل بين الخزينة والبنك", name, userId,
            safeId: safe.Id, shiftId: actualShiftId, referenceEntityType: "BankAccount", referenceEntityId: bank.Id);
        var bankTransaction = FinancialServiceRules.CreateTransaction(
            FinancialTransactionType.Transfer, safeToBank ? "TransferIn" : "TransferOut", amount,
            bankBefore, safeToBank, description ?? "تحويل بين الخزينة والبنك", name, userId,
            bankAccountId: bank.Id, shiftId: actualShiftId, referenceEntityType: "Safe", referenceEntityId: safe.Id);
        db.FinancialTransactions.AddRange(safeTransaction, bankTransaction);
        db.AuditEntries.Add(FinancialServiceRules.CreateAudit(userId, "Treasury.Transfer", "Safe", safe.Id, $"BankAccount:{bank.Id};Amount:{amount}"));
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        return [safeTransaction, bankTransaction];
    }

    public async Task<IReadOnlyList<FinancialTransaction>> GetFinancialTransactionsAsync(FinancialTransactionFilter filter)
    {
        await using var db = await factory.CreateDbContextAsync();
        var query = db.FinancialTransactions.AsNoTracking().AsQueryable();
        if (filter.FromUtc.HasValue)
            query = query.Where(x => x.OccurredAtUtc >= filter.FromUtc.Value);
        if (filter.ToUtc.HasValue)
            query = query.Where(x => x.OccurredAtUtc <= filter.ToUtc.Value);
        if (filter.SafeId.HasValue)
            query = query.Where(x => x.SafeId == filter.SafeId.Value);
        if (filter.BankAccountId.HasValue)
            query = query.Where(x => x.BankAccountId == filter.BankAccountId.Value);
        if (filter.Type.HasValue)
            query = query.Where(x => x.Type == filter.Type.Value);
        return await query.OrderBy(x => x.OccurredAtUtc).ThenBy(x => x.Id).ToListAsync();
    }

    private async Task<FinancialTransaction> ChangeSafeBalanceAsync(int safeId, decimal amount, string name, int userId, int? shiftId, bool incoming, FinancialTransactionType type, string operation, string? description)
    {
        FinancialServiceRules.ValidateAmount(amount);
        name = FinancialServiceRules.RequiredName(name, nameof(name));
        await using var db = await factory.CreateDbContextAsync();
        await using var transaction = await db.Database.BeginTransactionAsync();
        var safe = await db.Safes.SingleOrDefaultAsync(x => x.Id == safeId && x.IsActive)
            ?? throw new KeyNotFoundException("الخزينة غير موجودة أو غير نشطة.");
        if (!incoming && safe.CurrentBalance < amount)
            throw new InvalidOperationException("رصيد الخزينة غير كافٍ.");
        var actualShiftId = await FinancialServiceRules.GetOpenShiftIdAsync(db, shiftId);
        var balanceBefore = safe.CurrentBalance;
        safe.CurrentBalance += incoming ? amount : -amount;
        var financial = FinancialServiceRules.CreateTransaction(type, incoming ? "Deposit" : "Withdrawal", amount, balanceBefore, incoming,
            description ?? (incoming ? "إيداع في الخزينة" : "سحب من الخزينة"), name, userId, safeId: safe.Id, shiftId: actualShiftId);
        db.FinancialTransactions.Add(financial);
        db.AuditEntries.Add(FinancialServiceRules.CreateAudit(userId, operation, "Safe", safe.Id, $"Amount:{amount};Name:{name}"));
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        return financial;
    }

    private async Task<FinancialTransaction> ChangeBankBalanceAsync(int bankAccountId, decimal amount, string name, int userId, int? shiftId, bool incoming, FinancialTransactionType type, string operation, string? description)
    {
        FinancialServiceRules.ValidateAmount(amount);
        name = FinancialServiceRules.RequiredName(name, nameof(name));
        await using var db = await factory.CreateDbContextAsync();
        await using var transaction = await db.Database.BeginTransactionAsync();
        var bank = await db.BankAccounts.SingleOrDefaultAsync(x => x.Id == bankAccountId && x.IsActive)
            ?? throw new KeyNotFoundException("الحساب البنكي غير موجود أو غير نشط.");
        if (!incoming && bank.CurrentBalance < amount)
            throw new InvalidOperationException("رصيد الحساب البنكي غير كافٍ.");
        var actualShiftId = await FinancialServiceRules.GetOpenShiftIdAsync(db, shiftId);
        var balanceBefore = bank.CurrentBalance;
        bank.CurrentBalance += incoming ? amount : -amount;
        var financial = FinancialServiceRules.CreateTransaction(type, incoming ? "Deposit" : "Withdrawal", amount, balanceBefore, incoming,
            description ?? (incoming ? "إيداع في الحساب البنكي" : "سحب من الحساب البنكي"), name, userId, bankAccountId: bank.Id, shiftId: actualShiftId);
        db.FinancialTransactions.Add(financial);
        db.AuditEntries.Add(FinancialServiceRules.CreateAudit(userId, operation, "BankAccount", bank.Id, $"Amount:{amount};Name:{name}"));
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        return financial;
    }

    private static void ApplyOpeningBalanceChange(HeCashierDbContext db, Safe safe, decimal newOpeningBalance, int userId)
    {
        var delta = newOpeningBalance - safe.OpeningBalance;
        if (delta != 0)
        {
            var nextBalance = safe.CurrentBalance + delta;
            if (nextBalance < 0)
                throw new InvalidOperationException("لا يمكن أن ينتج عن تعديل الرصيد الافتتاحي رصيد سالب.");
            var adjustment = FinancialServiceRules.CreateTransaction(FinancialTransactionType.Adjustment, "OpeningBalanceAdjustment", Math.Abs(delta), safe.CurrentBalance, delta > 0,
                "تعديل الرصيد الافتتاحي للخزينة", "تسوية الرصيد الافتتاحي", userId, safeId: safe.Id);
            db.FinancialTransactions.Add(adjustment);
            safe.CurrentBalance = nextBalance;
            safe.OpeningBalance = newOpeningBalance;
        }
    }

    private static void ApplyOpeningBalanceChange(HeCashierDbContext db, BankAccount bank, decimal newOpeningBalance, int userId)
    {
        var delta = newOpeningBalance - bank.OpeningBalance;
        if (delta != 0)
        {
            var nextBalance = bank.CurrentBalance + delta;
            if (nextBalance < 0)
                throw new InvalidOperationException("لا يمكن أن ينتج عن تعديل الرصيد الافتتاحي رصيد سالب.");
            var adjustment = FinancialServiceRules.CreateTransaction(FinancialTransactionType.Adjustment, "OpeningBalanceAdjustment", Math.Abs(delta), bank.CurrentBalance, delta > 0,
                "تعديل الرصيد الافتتاحي للحساب البنكي", "تسوية الرصيد الافتتاحي", userId, bankAccountId: bank.Id);
            db.FinancialTransactions.Add(adjustment);
            bank.CurrentBalance = nextBalance;
            bank.OpeningBalance = newOpeningBalance;
        }
    }

    private static void ValidateOpeningBalance(decimal openingBalance)
    {
        if (openingBalance < 0)
            throw new ArgumentOutOfRangeException(nameof(openingBalance), "الرصيد الافتتاحي لا يمكن أن يكون سالباً.");
    }

    private static string? NormalizeOptional(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;
        var normalized = value.Trim();
        if (normalized.Length > maxLength)
            throw new ArgumentOutOfRangeException(nameof(value), "النص أطول من الحد المسموح.");
        return normalized;
    }
}
