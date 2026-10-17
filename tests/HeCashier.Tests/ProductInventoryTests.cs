using HeCashier.Data;
using HeCashier.Infrastructure.Services;
using HeCashier.Models;
using HeCashier.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Xunit;

namespace HeCashier.Tests;

public sealed class ProductInventoryTests
{
    [Fact]
    public async Task Product_creation_records_opening_stock_and_multiple_barcodes_with_generated_id()
    {
        await using var database = await TestDatabase.CreateAsync();
        var products = new ProductService(database.Factory);
        var product = await products.CreateAsync(CreateDraft(database, 10, "BC-PRIMARY", "BC-SECONDARY"), database.UserId);

        await using var db = await database.Factory.CreateDbContextAsync();
        var storedProduct = await db.Products.SingleAsync(x => x.Id == product.Id);
        var movement = await db.InventoryMovements.SingleAsync(x => x.ProductId == product.Id);
        var audit = await db.AuditEntries.SingleAsync(x => x.Operation == "Product.Created");
        Assert.Equal(10m, storedProduct.Quantity);
        Assert.Equal(2, await db.Barcodes.CountAsync(x => x.ProductId == product.Id));
        Assert.Equal("BC-PRIMARY", await db.Barcodes.Where(x => x.ProductId == product.Id && x.IsPrimary).Select(x => x.Value).SingleAsync());
        Assert.Equal(InventoryMovementType.OpeningBalance, movement.Type);
        Assert.Equal(0m, movement.BeforeQuantity);
        Assert.Equal(10m, movement.AfterQuantity);
        Assert.Equal(database.UserId, movement.UserId);
        Assert.Contains($"Product:{product.Id};", audit.EntityReference);
        Assert.Equal(database.UserId, audit.UserId);
    }

    [Fact]
    public async Task Product_creation_rejects_duplicate_barcodes_and_secondary_barcode_search_works()
    {
        await using var database = await TestDatabase.CreateAsync();
        var products = new ProductService(database.Factory);
        var first = await products.CreateAsync(CreateDraft(database, 3, "BC-ONE", "BC-SECOND"), database.UserId);

        var found = await products.FindByBarcodeOrNameAsync("BC-SECOND");
        Assert.Equal(first.Id, found?.Id);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            products.CreateAsync(CreateDraft(database, 0, "BC-THIRD", "bc-second"), database.UserId));

        await using var db = await database.Factory.CreateDbContextAsync();
        Assert.Equal(1, await db.Products.CountAsync());
        Assert.Equal(1, await db.InventoryMovements.CountAsync());
        Assert.Equal(1, await db.AuditEntries.CountAsync());
    }

    [Fact]
    public async Task Stock_adjustment_updates_product_movement_and_audit_atomically()
    {
        await using var database = await TestDatabase.CreateAsync();
        var product = await new ProductService(database.Factory).CreateAsync(CreateDraft(database, 10, "BC-ADJ"), database.UserId);
        var storeId = await GetStoreIdAsync(database.Factory);
        var inventory = new InventoryService(database.Factory);

        var movement = await inventory.AdjustStockAsync(product.Id, storeId, -3, InventoryMovementType.ManualAdjustment, "جرد يدوي", database.UserId);

        await using var db = await database.Factory.CreateDbContextAsync();
        Assert.Equal(7m, (await db.Products.SingleAsync(x => x.Id == product.Id)).Quantity);
        Assert.Equal(7m, (await db.WarehouseStocks.SingleAsync(x => x.ProductId == product.Id && x.StoreId == storeId)).Quantity);
        Assert.Equal(10m, movement.BeforeQuantity);
        Assert.Equal(7m, movement.AfterQuantity);
        Assert.Equal(-3m, movement.Quantity);
        Assert.Equal(2, await db.InventoryMovements.CountAsync(x => x.ProductId == product.Id));
        Assert.Contains(await db.AuditEntries.ToListAsync(), x => x.Operation == "Inventory.StockAdjusted" && x.UserId == database.UserId && x.EntityReference!.Contains($"Product:{product.Id};"));
    }

    [Fact]
    public async Task Negative_stock_is_rejected_by_default_without_partial_records()
    {
        await using var database = await TestDatabase.CreateAsync();
        var product = await new ProductService(database.Factory).CreateAsync(CreateDraft(database, 5, "BC-NEG"), database.UserId);
        var storeId = await GetStoreIdAsync(database.Factory);
        var inventory = new InventoryService(database.Factory);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            inventory.AdjustStockAsync(product.Id, storeId, -6, InventoryMovementType.ManualAdjustment, "تسوية", database.UserId));

        await using var db = await database.Factory.CreateDbContextAsync();
        Assert.Equal(5m, (await db.Products.SingleAsync(x => x.Id == product.Id)).Quantity);
        Assert.Equal(1, await db.InventoryMovements.CountAsync(x => x.ProductId == product.Id));
        Assert.Equal(1, await db.AuditEntries.CountAsync(x => x.Operation == "Product.Created"));
        Assert.False(await new SystemSettingsService(database.Factory).GetAllowNegativeStockAsync());
    }

    [Fact]
    public async Task Quick_update_saves_price_quantity_barcode_and_active_state_with_movement()
    {
        await using var database = await TestDatabase.CreateAsync();
        var products = new ProductService(database.Factory);
        var product = await products.CreateAsync(CreateDraft(database, 8, "BC-OLD", "BC-ALT"), database.UserId);

        var updated = await products.QuickUpdateAsync(new QuickProductUpdate
        {
            ProductId = product.Id,
            PurchasePrice = 8.5m,
            SellingPrice = 12m,
            Quantity = 4m,
            PrimaryBarcode = "BC-NEW",
            IsActive = false
        }, database.UserId);

        await using var db = await database.Factory.CreateDbContextAsync();
        var stored = await db.Products.SingleAsync(x => x.Id == product.Id);
        Assert.Equal(8.5m, stored.PurchasePrice);
        Assert.Equal(12m, stored.SellingPrice);
        Assert.Equal(4m, stored.Quantity);
        Assert.False(stored.IsActive);
        Assert.Contains(updated.Barcodes, x => x.IsPrimary && x.Value == "BC-NEW");
        Assert.Contains(updated.Barcodes, x => !x.IsPrimary && x.Value == "BC-ALT");
        Assert.DoesNotContain(updated.Barcodes, x => x.Value == "BC-OLD");
        Assert.Contains(await db.InventoryMovements.ToListAsync(), x => x.Type == InventoryMovementType.ManualAdjustment && x.Quantity == -4m && x.BeforeQuantity == 8m && x.AfterQuantity == 4m);
        Assert.Contains(await db.AuditEntries.ToListAsync(), x => x.Operation == "Product.QuickUpdated" && x.UserId == database.UserId);
    }

    [Fact]
    public async Task Category_and_unit_deletion_is_protected_while_linked_to_products()
    {
        await using var database = await TestDatabase.CreateAsync();
        await new ProductService(database.Factory).CreateAsync(CreateDraft(database, 0, "BC-LINKED"), database.UserId);
        var lookups = new CategoryAndUnitService(database.Factory);

        await Assert.ThrowsAsync<InvalidOperationException>(() => lookups.DeleteCategoryAsync(database.CategoryId));
        await Assert.ThrowsAsync<InvalidOperationException>(() => lookups.DeleteUnitAsync(database.UnitId));
    }

    private static ProductDraft CreateDraft(TestDatabase database, decimal quantity, string primaryBarcode, params string[] additionalBarcodes) => new()
    {
        Code = $"SKU-{Guid.NewGuid():N}",
        Name = "صنف اختبار",
        CategoryId = database.CategoryId,
        UnitId = database.UnitId,
        PurchasePrice = 4,
        CostPrice = 3.5m,
        SellingPrice = 7,
        Quantity = quantity,
        PrimaryBarcode = primaryBarcode,
        AdditionalBarcodes = additionalBarcodes
    };

    private static async Task<int> GetStoreIdAsync(IDbContextFactory<HeCashierDbContext> factory)
    {
        await using var db = await factory.CreateDbContextAsync();
        return await db.Stores.Select(x => x.Id).SingleAsync();
    }

    private sealed class TestDatabase : IAsyncDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly PooledDbContextFactory<HeCashierDbContext> _factory;
        public IDbContextFactory<HeCashierDbContext> Factory => _factory;
        public int UserId { get; private set; }
        public int CategoryId { get; private set; }
        public int UnitId { get; private set; }

        private TestDatabase(SqliteConnection connection, PooledDbContextFactory<HeCashierDbContext> factory)
        {
            _connection = connection;
            _factory = factory;
        }

        public static async Task<TestDatabase> CreateAsync()
        {
            var connection = new SqliteConnection("Data Source=:memory:");
            await connection.OpenAsync();
            var options = new DbContextOptionsBuilder<HeCashierDbContext>().UseSqlite(connection).Options;
            var factory = new PooledDbContextFactory<HeCashierDbContext>(options);
            var database = new TestDatabase(connection, factory);
            await using var db = await factory.CreateDbContextAsync();
            await db.Database.EnsureCreatedAsync();
            var user = new User { UserName = $"user-{Guid.NewGuid():N}", PasswordHash = "test" };
            var category = new Category { Name = $"category-{Guid.NewGuid():N}" };
            var unit = new Unit { Name = $"unit-{Guid.NewGuid():N}" };
            db.Users.Add(user);
            db.Categories.Add(category);
            db.Units.Add(unit);
            db.Stores.Add(new Store { Name = "محل" });
            db.SystemSettings.Add(new SystemSetting { Key = "AllowNegativeStock", Value = "false" });
            await db.SaveChangesAsync();
            database.UserId = user.Id;
            database.CategoryId = category.Id;
            database.UnitId = unit.Id;
            return database;
        }

        public async ValueTask DisposeAsync()
        {
            await _connection.DisposeAsync();
        }
    }
}
