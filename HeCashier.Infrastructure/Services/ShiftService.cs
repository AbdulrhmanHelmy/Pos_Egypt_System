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

    public async Task<Shift> StartShiftAsync(int userId)
    {
        try
        {
            await using var db = await factory.CreateDbContextAsync();
            var active = await db.Shifts.SingleOrDefaultAsync(x => x.UserId == userId && x.Status == ShiftStatus.Open);
            if (active is not null) return active;
            var shift = new Shift { UserId = userId, Number = $"SH-{DateTime.UtcNow:yyyyMMddHHmmssfff}", StartedAtUtc = DateTime.UtcNow };
            db.Shifts.Add(shift);
            await db.SaveChangesAsync();
            return shift;
        }
        catch (Exception exception)
        {
            logger.Log(exception);
            throw;
        }
    }
}
