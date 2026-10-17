using System.ComponentModel.DataAnnotations;
using HeCashier.Data;
using HeCashier.Models;
using HeCashier.Security;
using HeCashier.Services;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Xunit;

namespace HeCashier.Tests;

public sealed class FoundationTests
{
    [Fact]
    public void Password_hash_is_not_plaintext_and_verifies()
    {
        var hash = PasswordHasher.Hash("secret");
        Assert.NotEqual("secret", hash);
        Assert.True(PasswordHasher.Verify("secret", hash));
        Assert.False(PasswordHasher.Verify("wrong", hash));
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
        var options = new DbContextOptionsBuilder<HeCashierDbContext>().UseSqlite("Data Source=:memory:").Options;
        await using var connection = new Microsoft.Data.Sqlite.SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var liveOptions = new DbContextOptionsBuilder<HeCashierDbContext>().UseSqlite(connection).Options;
        var factory = new PooledDbContextFactory<HeCashierDbContext>(liveOptions);
        await using (var db = await factory.CreateDbContextAsync()) { await db.Database.EnsureCreatedAsync(); db.Users.Add(new User { UserName = "test", PasswordHash = PasswordHasher.Hash("x") }); await db.SaveChangesAsync(); }
        await using (var db = await factory.CreateDbContextAsync()) { var user = await db.Users.SingleAsync(); var service = new ShiftService(factory); var first = await service.StartShiftAsync(user.Id); var second = await service.StartShiftAsync(user.Id); Assert.Equal(first.Id, second.Id); Assert.Equal(ShiftStatus.Open, (await service.GetActiveShiftAsync(user.Id))!.Status); }
    }
}
