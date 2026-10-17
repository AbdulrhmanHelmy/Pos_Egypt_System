using HeCashier.Data;
using HeCashier.Models;
using HeCashier.Services;
using Microsoft.EntityFrameworkCore;

namespace HeCashier.Infrastructure.Services;

public sealed class ExpenseService(IDbContextFactory<HeCashierDbContext> factory) : IExpenseService
{
    public async Task<IReadOnlyList<ExpenseType>> GetExpenseTypesAsync()
    {
        await using var db = await factory.CreateDbContextAsync();
        return await db.ExpenseTypes.AsNoTracking().OrderBy(x => x.Name).ToListAsync();
    }

    public async Task<ExpenseType> CreateExpenseTypeAsync(ExpenseTypeDraft draft, int userId)
    {
        var name = FinancialServiceRules.RequiredText(draft.Name, nameof(draft.Name), 150);
        await using var db = await factory.CreateDbContextAsync();
        await using var transaction = await db.Database.BeginTransactionAsync();
        if (await db.ExpenseTypes.AnyAsync(x => x.Name == name))
            throw new InvalidOperationException("نوع المصروف موجود بالفعل.");
        var expenseType = new ExpenseType { Name = name };
        db.ExpenseTypes.Add(expenseType);
        await db.SaveChangesAsync();
        db.AuditEntries.Add(FinancialServiceRules.CreateAudit(userId, "ExpenseType.Created", "ExpenseType", expenseType.Id, expenseType.Name));
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        return expenseType;
    }

    public async Task<ExpenseType> UpdateExpenseTypeAsync(int expenseTypeId, ExpenseTypeDraft draft, int userId)
    {
        var name = FinancialServiceRules.RequiredText(draft.Name, nameof(draft.Name), 150);
        await using var db = await factory.CreateDbContextAsync();
        await using var transaction = await db.Database.BeginTransactionAsync();
        var expenseType = await db.ExpenseTypes.SingleOrDefaultAsync(x => x.Id == expenseTypeId)
            ?? throw new KeyNotFoundException("نوع المصروف غير موجود.");
        if (await db.ExpenseTypes.AnyAsync(x => x.Id != expenseTypeId && x.Name == name))
            throw new InvalidOperationException("نوع المصروف موجود بالفعل.");
        expenseType.Name = name;
        db.AuditEntries.Add(FinancialServiceRules.CreateAudit(userId, "ExpenseType.Updated", "ExpenseType", expenseType.Id, expenseType.Name));
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        return expenseType;
    }

    public async Task DeleteExpenseTypeAsync(int expenseTypeId, int userId)
    {
        await using var db = await factory.CreateDbContextAsync();
        await using var transaction = await db.Database.BeginTransactionAsync();
        var expenseType = await db.ExpenseTypes.SingleOrDefaultAsync(x => x.Id == expenseTypeId)
            ?? throw new KeyNotFoundException("نوع المصروف غير موجود.");
        if (await db.Expenses.AnyAsync(x => x.ExpenseTypeId == expenseTypeId))
            throw new InvalidOperationException("لا يمكن حذف نوع مصروف مرتبط بمصروفات مسجلة.");
        db.ExpenseTypes.Remove(expenseType);
        db.AuditEntries.Add(FinancialServiceRules.CreateAudit(userId, "ExpenseType.Deleted", "ExpenseType", expenseType.Id, expenseType.Name));
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    public async Task<Expense> RecordExpenseAsync(ExpenseDraft draft, int userId)
    {
        FinancialServiceRules.ValidateAmount(draft.Amount);
        var description = FinancialServiceRules.RequiredText(draft.Description, nameof(draft.Description), 1000);
        await using var db = await factory.CreateDbContextAsync();
        await using var transaction = await db.Database.BeginTransactionAsync();
        var expenseType = await db.ExpenseTypes.SingleOrDefaultAsync(x => x.Id == draft.ExpenseTypeId)
            ?? throw new KeyNotFoundException("نوع المصروف غير موجود.");
        var safe = await db.Safes.SingleOrDefaultAsync(x => x.Id == draft.SafeId && x.IsActive)
            ?? throw new KeyNotFoundException("الخزينة غير موجودة أو غير نشطة.");
        if (safe.CurrentBalance < draft.Amount)
            throw new InvalidOperationException("رصيد الخزينة غير كافٍ.");
        var shiftId = await FinancialServiceRules.GetOpenShiftIdAsync(db, draft.ShiftId);
        var notes = string.IsNullOrWhiteSpace(draft.Notes) ? null : draft.Notes.Trim();
        if (notes?.Length > 1000)
            throw new ArgumentOutOfRangeException(nameof(draft.Notes), "الملاحظات أطول من الحد المسموح.");
        var occurredAt = draft.OccurredAtUtc ?? DateTime.UtcNow;
        var voucherNumber = string.IsNullOrWhiteSpace(draft.VoucherNumber)
            ? $"EXP-{occurredAt:yyyyMMddHHmmssfff}-{Guid.NewGuid():N}"[..40]
            : FinancialServiceRules.RequiredText(draft.VoucherNumber, nameof(draft.VoucherNumber), 50);
        if (await db.Expenses.AnyAsync(x => x.VoucherNumber == voucherNumber))
            throw new InvalidOperationException("رقم سند المصروف مستخدم بالفعل.");

        var balanceBefore = safe.CurrentBalance;
        safe.CurrentBalance -= draft.Amount;
        var financial = FinancialServiceRules.CreateTransaction(
            FinancialTransactionType.Expense, "Expense", draft.Amount, balanceBefore, false,
            description, "مصروف", userId, safeId: safe.Id, shiftId: shiftId, referenceEntityType: "Expense");
        var expense = new Expense
        {
            VoucherNumber = voucherNumber,
            Amount = draft.Amount,
            OccurredAtUtc = occurredAt,
            ExpenseTypeId = expenseType.Id,
            SafeId = safe.Id,
            ShiftId = shiftId,
            UserId = userId,
            FinancialTransaction = financial,
            Notes = notes,
            Description = description
        };
        db.Expenses.Add(expense);
        await db.SaveChangesAsync();
        financial.ReferenceEntityId = expense.Id;
        db.AuditEntries.Add(FinancialServiceRules.CreateAudit(userId, "Expense.Recorded", "Expense", expense.Id, $"Voucher:{voucherNumber};Amount:{draft.Amount}"));
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        return expense;
    }

    public async Task<IReadOnlyList<Expense>> GetExpensesAsync(DateTime? fromUtc = null, DateTime? toUtc = null, int? expenseTypeId = null, int? shiftId = null, int? userId = null)
    {
        await using var db = await factory.CreateDbContextAsync();
        var query = db.Expenses
            .Include(x => x.ExpenseType)
            .Include(x => x.Safe)
            .Include(x => x.Shift)
            .AsNoTracking()
            .AsQueryable();
        if (fromUtc.HasValue)
            query = query.Where(x => x.OccurredAtUtc >= fromUtc.Value);
        if (toUtc.HasValue)
            query = query.Where(x => x.OccurredAtUtc <= toUtc.Value);
        if (expenseTypeId.HasValue)
            query = query.Where(x => x.ExpenseTypeId == expenseTypeId.Value);
        if (shiftId.HasValue)
            query = query.Where(x => x.ShiftId == shiftId.Value);
        if (userId.HasValue)
            query = query.Where(x => x.UserId == userId.Value);
        return await query.OrderByDescending(x => x.OccurredAtUtc).ToListAsync();
    }
}
