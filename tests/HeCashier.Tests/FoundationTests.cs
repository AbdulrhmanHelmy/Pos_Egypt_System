using System.ComponentModel.DataAnnotations;
using HeCashier.Diagnostics;
using HeCashier.Data;
using HeCashier.Models;
using HeCashier.Security;
using HeCashier.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Xunit;

namespace HeCashier.Tests;

public sealed class FoundationTests
{
    [Fact]
    public void Password_hash_is_not_plaintext_and_verifies()
    {
        var hasher = new PasswordHasher();
        var hash = hasher.Hash("secret");
        Assert.NotEqual("secret", hash);
        Assert.True(hasher.Verify("secret", hash));
        Assert.False(hasher.Verify("wrong", hash));
    }

    [Fact]
    public void Product_requires_a_name()
    {
        var product = new Product();
        var results = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(product, new ValidationContext(product), results, true));
    }

    [Fact]
    public async Task Sqlite_initializes_and_active_shift_is_reused()
    {
        await using var connection = new Microsoft.Data.Sqlite.SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var liveOptions = new DbContextOptionsBuilder<HeCashierDbContext>().UseSqlite(connection).Options;
        var factory = new PooledDbContextFactory<HeCashierDbContext>(liveOptions);
        var hasher = new PasswordHasher();
        var logger = new FileLogger(() => Path.GetTempPath());
        await using (var db = await factory.CreateDbContextAsync())
        {
            await db.Database.EnsureCreatedAsync();
            db.Users.Add(new User { UserName = "test", PasswordHash = hasher.Hash("x") });
            await db.SaveChangesAsync();
        }

        await using (var db = await factory.CreateDbContextAsync())
        {
            var user = await db.Users.SingleAsync();
            var service = new ShiftService(factory, logger);
            var first = await service.StartShiftAsync(user.Id);
            var second = await service.StartShiftAsync(user.Id);
            Assert.Equal(first.Id, second.Id);
            Assert.Equal(ShiftStatus.Open, (await service.GetActiveShiftAsync(user.Id))!.Status);
        }
    }

    [Fact]
    public async Task Sqlite_enforces_unique_user_role_and_barcode_values()
    {
        await using var connection = new Microsoft.Data.Sqlite.SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<HeCashierDbContext>().UseSqlite(connection).Options;
        await using var db = new HeCashierDbContext(options);
        await db.Database.EnsureCreatedAsync();

        db.Users.Add(new User { UserName = "cashier", PasswordHash = "hash" });
        db.Roles.Add(new Role { Name = "Admin" });
        var firstProduct = new Product { Code = "P-1", Name = "First" };
        var secondProduct = new Product { Code = "P-2", Name = "Second" };
        db.Products.AddRange(firstProduct, secondProduct);
        await db.SaveChangesAsync();

        db.Users.Add(new User { UserName = "cashier", PasswordHash = "hash" });
        await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
        db.ChangeTracker.Clear();

        db.Roles.Add(new Role { Name = "Admin" });
        await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
        db.ChangeTracker.Clear();

        db.Barcodes.Add(new ProductBarcode { Value = "12345", ProductId = firstProduct.Id });
        await db.SaveChangesAsync();
        db.Barcodes.Add(new ProductBarcode { Value = "12345", ProductId = secondProduct.Id });
        await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
    }

    [Fact]
    public async Task All_decimal_properties_have_explicit_precision()
    {
        await using var connection = new Microsoft.Data.Sqlite.SqliteConnection("Data Source=:memory:");
        var options = new DbContextOptionsBuilder<HeCashierDbContext>().UseSqlite(connection).Options;
        await using var db = new HeCashierDbContext(options);

        var decimalProperties = db.Model.GetEntityTypes()
            .SelectMany(entity => entity.GetProperties())
            .Where(property => (Nullable.GetUnderlyingType(property.ClrType) ?? property.ClrType) == typeof(decimal))
            .ToArray();

        Assert.NotEmpty(decimalProperties);
        Assert.All(decimalProperties, property =>
        {
            Assert.Equal(18, property.GetPrecision());
            Assert.Equal(property.Name is "Quantity" or "ReorderPoint" or "Weight" or "PackagingQuantity" or
                "BeforeQuantity" or "AfterQuantity" or "SystemQuantity" or "ActualQuantity" or "Difference" ? 3 : 2, property.GetScale());
        });
    }

    [Fact]
    public void Unavailable_configured_drive_falls_back_to_local_application_data()
    {
        var localApplicationData = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        var fallbackRoot = Path.GetFullPath(Path.Combine(localApplicationData, "HeCashier"));
        var result = DataPathResolver.Resolve(
            environmentRoot: null,
            configuredRoot: @"F:\HeCashier",
            localApplicationData,
            driveExists: root => !string.Equals(root, @"F:\", StringComparison.OrdinalIgnoreCase));

        Assert.Equal(fallbackRoot, result.DataRoot);
        Assert.Equal(Path.Combine(fallbackRoot, "hecashier.db"), result.DatabasePath);
        Assert.Null(result.Error);
    }

    [Fact]
    public async Task Database_initializer_creates_schema_and_seeds_only_in_debug()
    {
        var localApplicationData = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        var initializer = new DatabaseInitializer(null, null, localApplicationData);

        try
        {
            var hasNoUsers = await initializer.InitializeAsync(seedDevelopmentUser: true);
            Assert.True(File.Exists(initializer.DatabasePath));
#if DEBUG
            Assert.False(hasNoUsers);
            await using var db = await initializer.Factory.CreateDbContextAsync();
            var user = await db.Users.SingleAsync();
            Assert.Equal("devadmin", user.UserName);
            Assert.True(new PasswordHasher().Verify("HeCashierDev2026!", user.PasswordHash));
#else
            Assert.True(hasNoUsers);
#endif
        }
        finally
        {
            Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
            if (Directory.Exists(localApplicationData))
                Directory.Delete(localApplicationData, recursive: true);
        }
    }
}
