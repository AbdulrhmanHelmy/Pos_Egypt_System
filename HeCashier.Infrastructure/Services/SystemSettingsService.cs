using HeCashier.Data;
using HeCashier.Models;
using HeCashier.Services;
using Microsoft.EntityFrameworkCore;

namespace HeCashier.Infrastructure.Services;

public sealed class SystemSettingsService(IDbContextFactory<HeCashierDbContext> factory) : ISystemSettingsService
{
    private const string AllowNegativeStockKey = "AllowNegativeStock";

    public async Task<bool> GetAllowNegativeStockAsync()
    {
        await using var db = await factory.CreateDbContextAsync();
        var setting = await db.SystemSettings.AsNoTracking().SingleOrDefaultAsync(x => x.Key == AllowNegativeStockKey);
        return bool.TryParse(setting?.Value, out var allow) && allow;
    }

    public async Task SetAllowNegativeStockAsync(bool allow)
    {
        await using var db = await factory.CreateDbContextAsync();
        var setting = await db.SystemSettings.SingleOrDefaultAsync(x => x.Key == AllowNegativeStockKey);
        if (setting is null)
            db.SystemSettings.Add(new SystemSetting { Key = AllowNegativeStockKey, Value = allow.ToString().ToLowerInvariant() });
        else
            setting.Value = allow.ToString().ToLowerInvariant();
        await db.SaveChangesAsync();
    }
}