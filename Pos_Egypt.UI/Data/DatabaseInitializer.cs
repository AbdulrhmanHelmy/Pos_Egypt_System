using HeCashier.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.IO;

namespace HeCashier.Data;

public sealed class DatabaseInitializer
{
    public string DatabasePath { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "HeCashier", "hecashier.db");
    public IDbContextFactory<HeCashierDbContext> Factory { get; }

    public DatabaseInitializer()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(DatabasePath)!);
        var options = new DbContextOptionsBuilder<HeCashierDbContext>().UseSqlite($"Data Source={DatabasePath}").Options;
        Factory = new PooledDbContextFactory<HeCashierDbContext>(options);
    }

    public async Task InitializeAsync()
    {
        await using var db = await Factory.CreateDbContextAsync();
        await db.Database.EnsureCreatedAsync();
        if (!await db.Users.AnyAsync())
        {
            db.Users.Add(new Models.User { UserName = "devadmin", PasswordHash = PasswordHasher.Hash("HeCashierDev2026!") });
            await db.SaveChangesAsync();
        }
    }
}
