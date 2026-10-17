using HeCashier.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.IO;

namespace HeCashier.Data;

public sealed class DatabaseInitializer
{
    public string DataRoot { get; }
    public string DatabasePath { get; }
    public string? ConfigurationError { get; }
    public IDbContextFactory<HeCashierDbContext> Factory { get; }

    private readonly IPasswordHasher _passwordHasher;

    public DatabaseInitializer(
        string? environmentRoot,
        string? configuredRoot,
        string localApplicationData,
        IPasswordHasher? passwordHasher = null)
    {
        var resolution = DataPathResolver.Resolve(
            environmentRoot,
            configuredRoot,
            localApplicationData);
        DataRoot = resolution.DataRoot;
        DatabasePath = resolution.DatabasePath;
        ConfigurationError = resolution.Error;
        _passwordHasher = passwordHasher ?? new PasswordHasher();
        var options = new DbContextOptionsBuilder<HeCashierDbContext>().UseSqlite($"Data Source={DatabasePath}").Options;
        Factory = new PooledDbContextFactory<HeCashierDbContext>(options);
    }

    public async Task<bool> InitializeAsync(bool seedDevelopmentUser)
    {
        if (ConfigurationError is not null)
            throw new InvalidOperationException(ConfigurationError);

        Directory.CreateDirectory(DataRoot);
        await using var db = await Factory.CreateDbContextAsync();
        await db.Database.EnsureCreatedAsync();
        if (!await db.Categories.AnyAsync())
            db.Categories.Add(new Models.Category { Name = "عام" });
        if (!await db.Units.AnyAsync())
            db.Units.Add(new Models.Unit { Name = "قطعة" });
        if (!await db.Stores.AnyAsync())
            db.Stores.Add(new Models.Store { Name = "محل" });
        if (!await db.SystemSettings.AnyAsync(x => x.Key == "AllowNegativeStock"))
            db.SystemSettings.Add(new Models.SystemSetting { Key = "AllowNegativeStock", Value = "false" });
#if DEBUG
        if (!await db.Users.AnyAsync() && seedDevelopmentUser)
        {
            db.Users.Add(new Models.User { UserName = "devadmin", PasswordHash = _passwordHasher.Hash("HeCashierDev2026!") });
        }
#endif
        await db.SaveChangesAsync();

        return !await db.Users.AnyAsync();
    }
}
