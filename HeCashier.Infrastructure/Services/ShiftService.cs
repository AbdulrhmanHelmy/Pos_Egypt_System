using HeCashier.Data;
using HeCashier.Diagnostics;
using HeCashier.Models;
using Microsoft.EntityFrameworkCore;

namespace HeCashier.Services;

public sealed class ShiftService(
    IDbContextFactory<HeCashierDbContext> factory,
    FileLogger logger) : IShiftService
{
    public async Task<Shift?> GetActiveShiftAsync(int userId)
    {
        try
        {
            await using var db = await factory.CreateDbContextAsync();
            return await db.Shifts.SingleOrDefaultAsync(x => x.UserId == userId && x.Status == ShiftStatus.Open);
        }
        catch (Exception exception)
        {
            logger.Log(exception);
            throw;
        }
    }

    public async Task<Shift> StartShiftAsync(int userId, decimal openingCash = 0m, int? safeId = null)
    {
        if (openingCash < 0)
            throw new ArgumentOutOfRangeException(nameof(openingCash), "رصيد افتتاح الوردية لا يمكن أن يكون سالباً.");
        try
        {
            await using var db = await factory.CreateDbContextAsync();
            await using var transaction = await db.Database.BeginTransactionAsync();
            var active = await db.Shifts.SingleOrDefaultAsync(x => x.UserId == userId && x.Status == ShiftStatus.Open);
            if (active is not null)
            {
                await transaction.CommitAsync();
                return active;
            }

            var safe = safeId.HasValue
                ? await db.Safes.SingleOrDefaultAsync(x => x.Id == safeId.Value && x.IsActive)
                : await db.Safes.SingleOrDefaultAsync(x => x.Name == "الخزينة الرئيسية" && x.IsActive);
            if (safe is null && !safeId.HasValue)
            {
                safe = new Safe { Name = "الخزينة الرئيسية", IsActive = true };
                db.Safes.Add(safe);
                await db.SaveChangesAsync();
            }
            if (safe is null)
                throw new KeyNotFoundException("الخزينة غير موجودة أو غير نشطة.");

            var startedAt = DateTime.UtcNow;
            var shift = new Shift
            {
                UserId = userId,
                SafeId = safe.Id,
                Number = $"SH-{startedAt:yyyyMMddHHmmssfff}-{Guid.NewGuid():N}"[..27],
                StartedAtUtc = startedAt,
                OpeningBalance = openingCash
            };
            db.Shifts.Add(shift);
            db.AuditEntries.Add(new AuditEntry
            {
                UserId = userId,
                Operation = "Shift.Opened",
                EntityReference = $"Shift:{shift.Number};Safe:{safe.Id};OpeningCash:{openingCash}",
                OccurredAtUtc = startedAt
            });
            await db.SaveChangesAsync();
            await transaction.CommitAsync();
            return shift;
        }
        catch (Exception exception)
        {
            logger.Log(exception);
            throw;
        }
    }

    public async Task<ShiftSummary> GetShiftSummaryAsync(int shiftId)
    {
        await using var db = await factory.CreateDbContextAsync();
        var shift = await db.Shifts.AsNoTracking().SingleOrDefaultAsync(x => x.Id == shiftId)
            ?? throw new KeyNotFoundException("الوردية غير موجودة.");
        return await CalculateShiftSummaryAsync(db, shift);
    }

    private static async Task<ShiftSummary> CalculateShiftSummaryAsync(HeCashierDbContext db, Shift shift)
    {
        var shiftId = shift.Id;
        var cashSales = await SumAsync(db.Sales
            .Where(x => x.ShiftId == shiftId && x.Status == TransactionStatus.Completed && x.PaymentType == PaymentType.Cash)
            .Select(x => x.PaidAmount));
        var cashReturns = await SumAsync(db.FinancialTransactions
            .Where(x => x.ShiftId == shiftId && x.Type == FinancialTransactionType.Refund && x.SafeId != null)
            .Select(x => x.Amount));
        var customerCollections = await SumAsync(db.FinancialTransactions
            .Where(x => x.ShiftId == shiftId && x.Type == FinancialTransactionType.CustomerCollection && x.SafeId != null)
            .Select(x => x.Amount));
        var supplierPayments = await SumAsync(db.FinancialTransactions
            .Where(x => x.ShiftId == shiftId && x.Type == FinancialTransactionType.SupplierPayment && x.SafeId != null)
            .Select(x => x.Amount));
        var expenses = await SumAsync(db.FinancialTransactions
            .Where(x => x.ShiftId == shiftId && x.Type == FinancialTransactionType.Expense && x.SafeId != null)
            .Select(x => x.Amount));
        var expected = shift.OpeningBalance + cashSales - cashReturns + customerCollections - supplierPayments - expenses;
        return new ShiftSummary(shift.Id, shift.OpeningBalance, cashSales, cashReturns, customerCollections, supplierPayments, expenses, expected);
    }

    private static async Task<decimal> SumAsync(IQueryable<decimal> query) => (await query.ToListAsync()).Sum();

    public async Task<Shift> CloseShiftAsync(int shiftId, decimal actualCash, string? notes = null, int? userId = null)
    {
        if (actualCash < 0)
            throw new ArgumentOutOfRangeException(nameof(actualCash), "النقد الفعلي لا يمكن أن يكون سالباً.");

        await using var db = await factory.CreateDbContextAsync();
        await using var transaction = await db.Database.BeginTransactionAsync();
        var shift = await db.Shifts.SingleOrDefaultAsync(x => x.Id == shiftId)
            ?? throw new KeyNotFoundException("الوردية غير موجودة.");
        if (shift.Status != ShiftStatus.Open)
            throw new InvalidOperationException("الوردية مغلقة بالفعل.");

        var summary = await CalculateShiftSummaryAsync(db, shift);
        var closedAt = DateTime.UtcNow;
        shift.ClosingBalance = actualCash;
        shift.ExpectedCash = summary.ExpectedCash;
        shift.Difference = actualCash - summary.ExpectedCash;
        shift.ClosedAtUtc = closedAt;
        shift.Status = ShiftStatus.Closed;
        if (!string.IsNullOrWhiteSpace(notes))
            shift.Notes = notes.Trim();
        db.AuditEntries.Add(new AuditEntry
        {
            UserId = userId ?? shift.UserId,
            Operation = "Shift.Closed",
            EntityReference = $"Shift:{shift.Id};ExpectedCash:{summary.ExpectedCash};ActualCash:{actualCash};Difference:{shift.Difference}",
            OccurredAtUtc = closedAt
        });
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        return shift;
    }
}
