using HeCashier.Data;
using HeCashier.Models;
using Microsoft.EntityFrameworkCore;

namespace HeCashier.Services;

public sealed class ShiftService(IDbContextFactory<HeCashierDbContext> factory)
{
    public async Task<Shift?> GetActiveShiftAsync(int userId)
    {
        await using var db = await factory.CreateDbContextAsync();
        return await db.Shifts.SingleOrDefaultAsync(x => x.UserId == userId && x.Status == ShiftStatus.Open);
    }

    public async Task<Shift> StartShiftAsync(int userId)
    {
        await using var db = await factory.CreateDbContextAsync();
        var active = await db.Shifts.SingleOrDefaultAsync(x => x.UserId == userId && x.Status == ShiftStatus.Open);
        if (active is not null) return active;
        var shift = new Shift { UserId = userId, Number = $"SH-{DateTime.UtcNow:yyyyMMddHHmmssfff}", StartedAtUtc = DateTime.UtcNow };
        db.Shifts.Add(shift);
        await db.SaveChangesAsync();
        return shift;
    }
}
