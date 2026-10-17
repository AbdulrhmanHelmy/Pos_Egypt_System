using HeCashier.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Configuration;
using System.IO;

namespace HeCashier.Data;

public sealed class DatabaseInitializer
{
    public string DataRoot { get; }
    public string DatabasePath { get; }
    public string? ConfigurationError { get; }
    public IDbContextFactory<HeCashierDbContext> Factory { get; }

    public DatabaseInitializer()
    {
        var resolution = DataPathResolver.Resolve(
            Environment.GetEnvironmentVariable("HECASHIER_DATA_ROOT"),
            ConfigurationManager.AppSettings["DataRoot"],
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
        DataRoot = resolution.DataRoot;
        DatabasePath = resolution.DatabasePath;
        ConfigurationError = resolution.Error;
        var options = new DbContextOptionsBuilder<HeCashierDbContext>().UseSqlite($"Data Source={DatabasePath}").Options;
        Factory = new PooledDbContextFactory<HeCashierDbContext>(options);
    }

    public async Task InitializeAsync()
    {
        if (ConfigurationError is not null)
            throw new InvalidOperationException(ConfigurationError);

        Directory.CreateDirectory(DataRoot);
        await using var db = await Factory.CreateDbContextAsync();
        await db.Database.EnsureCreatedAsync();
        if (!await db.Users.AnyAsync())
        {
            db.Users.Add(new Models.User { UserName = "devadmin", PasswordHash = PasswordHasher.Hash("HeCashierDev2026!") });
            await db.SaveChangesAsync();
        }
    }
}
