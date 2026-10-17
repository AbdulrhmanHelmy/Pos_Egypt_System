using HeCashier.Data;
using HeCashier.Models;
using HeCashier.Services;
using Microsoft.EntityFrameworkCore;

namespace HeCashier.Infrastructure.Services;

public sealed class InventoryService(IDbContextFactory<HeCashierDbContext> factory) : IInventoryService
{
    public async Task<IReadOnlyList<Store>> GetStoresAsync()
    {
        await using var db = await factory.CreateDbContextAsync();
        return await db.Stores.Where(x => x.IsActive).OrderBy(x => x.Name).AsNoTracking().ToListAsync();
    }

    public async Task<IReadOnlyList<InventoryMovement>> GetMovementsAsync(int? productId = null)
    {
        await using var db = await factory.CreateDbContextAsync();
        var query = db.InventoryMovements
            .Include(x => x.Product)
            .Include(x => x.Store)
            .Include(x => x.User)
            .AsNoTracking();
        if (productId.HasValue)
            query = query.Where(x => x.ProductId == productId.Value);
        return await query.OrderByDescending(x => x.OccurredAtUtc).ToListAsync();
    }

    public async Task<InventoryMovement> AdjustStockAsync(int productId, int storeId, decimal quantityDelta, InventoryMovementType type, string notes, int userId)
    {
        if (type is not InventoryMovementType.ManualAdjustment and not InventoryMovementType.OpeningBalance)
            throw new ArgumentException("استخدم إجراء المخزون المخصص لهذا النوع من الحركة.", nameof(type));
        if (quantityDelta == 0)
            throw new ArgumentOutOfRangeException(nameof(quantityDelta), "يجب أن تكون كمية التسوية غير صفرية.");

        await using var db = await factory.CreateDbContextAsync();
        await using var transaction = await db.Database.BeginTransactionAsync();
        var product = await db.Products.SingleOrDefaultAsync(x => x.Id == productId)
            ?? throw new KeyNotFoundException("الصنف غير موجود.");
        var store = await db.Stores.SingleOrDefaultAsync(x => x.Id == storeId && x.IsActive)
            ?? throw new KeyNotFoundException("المخزن غير موجود أو غير نشط.");
        var stock = await InventoryRules.GetOrCreateStockAsync(db, productId, storeId);
        var nextStoreQuantity = stock.Quantity + quantityDelta;
        var nextProductQuantity = product.Quantity + quantityDelta;
        await InventoryRules.ValidateQuantityAsync(db, nextStoreQuantity, nextProductQuantity);
        var movement = InventoryRules.AddMovement(db, product, store, quantityDelta, stock.Quantity, nextStoreQuantity, type, notes, userId);
        stock.Quantity = nextStoreQuantity;
        product.Quantity = nextProductQuantity;
        db.AuditEntries.Add(InventoryRules.CreateAudit(userId, "Inventory.StockAdjusted", product.Id, notes));
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        return movement;
    }

    public async Task<StocktakeRecord> RecordStocktakeAsync(int productId, int storeId, decimal actualQuantity, string notes, int userId)
    {
        await using var db = await factory.CreateDbContextAsync();
        await using var transaction = await db.Database.BeginTransactionAsync();
        var product = await db.Products.SingleOrDefaultAsync(x => x.Id == productId)
            ?? throw new KeyNotFoundException("الصنف غير موجود.");
        var store = await db.Stores.SingleOrDefaultAsync(x => x.Id == storeId && x.IsActive)
            ?? throw new KeyNotFoundException("المخزن غير موجود أو غير نشط.");
        var stock = await InventoryRules.GetOrCreateStockAsync(db, productId, storeId);
        var systemQuantity = stock.Quantity;
        var difference = actualQuantity - systemQuantity;
        var nextProductQuantity = product.Quantity + difference;
        await InventoryRules.ValidateQuantityAsync(db, actualQuantity, nextProductQuantity);
        var record = new StocktakeRecord
        {
            ProductId = productId,
            StoreId = storeId,
            SystemQuantity = systemQuantity,
            ActualQuantity = actualQuantity,
            Difference = difference,
            OccurredAtUtc = DateTime.UtcNow,
            UserId = userId,
            Notes = notes.Trim()
        };
        db.StocktakeRecords.Add(record);
        if (difference != 0)
        {
            InventoryRules.AddMovement(db, product, store, difference, systemQuantity, actualQuantity, InventoryMovementType.Stocktake, notes, userId);
            stock.Quantity = actualQuantity;
            product.Quantity = nextProductQuantity;
        }
        db.AuditEntries.Add(InventoryRules.CreateAudit(userId, "Inventory.StocktakeRecorded", product.Id, notes));
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        return record;
    }

    public async Task<DamagedStockRecord> RemoveDamagedStockAsync(int productId, int storeId, decimal quantity, string reason, int userId)
    {
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "يجب إدخال كمية تالفة موجبة.");
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("سبب إخراج التالف مطلوب.", nameof(reason));

        await using var db = await factory.CreateDbContextAsync();
        await using var transaction = await db.Database.BeginTransactionAsync();
        var product = await db.Products.SingleOrDefaultAsync(x => x.Id == productId)
            ?? throw new KeyNotFoundException("الصنف غير موجود.");
        var store = await db.Stores.SingleOrDefaultAsync(x => x.Id == storeId && x.IsActive)
            ?? throw new KeyNotFoundException("المخزن غير موجود أو غير نشط.");
        var stock = await InventoryRules.GetOrCreateStockAsync(db, productId, storeId);
        var nextStoreQuantity = stock.Quantity - quantity;
        var nextProductQuantity = product.Quantity - quantity;
        await InventoryRules.ValidateQuantityAsync(db, nextStoreQuantity, nextProductQuantity);
        stock.Quantity = nextStoreQuantity;
        product.Quantity = nextProductQuantity;
        var movement = InventoryRules.AddMovement(db, product, store, -quantity, stock.Quantity + quantity, nextStoreQuantity, InventoryMovementType.Damaged, reason, userId);
        var record = new DamagedStockRecord
        {
            ProductId = productId,
            StoreId = storeId,
            InventoryMovement = movement,
            Quantity = quantity,
            OccurredAtUtc = DateTime.UtcNow,
            Reason = reason.Trim()
        };
        db.DamagedStockRecords.Add(record);
        db.AuditEntries.Add(InventoryRules.CreateAudit(userId, "Inventory.DamagedStockRemoved", product.Id, reason));
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        return record;
    }

    public async Task<InventoryTransfer> TransferStockAsync(int productId, int sourceStoreId, int destinationStoreId, decimal quantity, string notes, int userId)
    {
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "يجب إدخال كمية تحويل موجبة.");
        if (sourceStoreId == destinationStoreId)
            throw new ArgumentException("يجب اختيار مخزنين مختلفين.");

        await using var db = await factory.CreateDbContextAsync();
        await using var transaction = await db.Database.BeginTransactionAsync();
        var product = await db.Products.SingleOrDefaultAsync(x => x.Id == productId)
            ?? throw new KeyNotFoundException("الصنف غير موجود.");
        var source = await db.Stores.SingleOrDefaultAsync(x => x.Id == sourceStoreId && x.IsActive)
            ?? throw new KeyNotFoundException("مخزن المصدر غير موجود أو غير نشط.");
        var destination = await db.Stores.SingleOrDefaultAsync(x => x.Id == destinationStoreId && x.IsActive)
            ?? throw new KeyNotFoundException("مخزن الوجهة غير موجود أو غير نشط.");
        var sourceStock = await InventoryRules.GetOrCreateStockAsync(db, productId, sourceStoreId);
        var destinationStock = await InventoryRules.GetOrCreateStockAsync(db, productId, destinationStoreId);
        var nextSourceQuantity = sourceStock.Quantity - quantity;
        await InventoryRules.ValidateQuantityAsync(db, nextSourceQuantity, product.Quantity);
        var transfer = new InventoryTransfer
        {
            ProductId = productId,
            SourceStoreId = sourceStoreId,
            DestinationStoreId = destinationStoreId,
            Quantity = quantity,
            OccurredAtUtc = DateTime.UtcNow,
            UserId = userId,
            Notes = notes.Trim()
        };
        db.InventoryTransfers.Add(transfer);
        InventoryRules.AddMovement(db, product, source, -quantity, sourceStock.Quantity, nextSourceQuantity, InventoryMovementType.TransferOut, notes, userId);
        InventoryRules.AddMovement(db, product, destination, quantity, destinationStock.Quantity, destinationStock.Quantity + quantity, InventoryMovementType.TransferIn, notes, userId);
        sourceStock.Quantity = nextSourceQuantity;
        destinationStock.Quantity += quantity;
        db.AuditEntries.Add(InventoryRules.CreateAudit(userId, "Inventory.StockTransferred", product.Id, notes));
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        return transfer;
    }
}

internal static class InventoryRules
{
    public static async Task<WarehouseStock> GetOrCreateStockAsync(HeCashierDbContext db, int productId, int storeId)
    {
        var stock = await db.WarehouseStocks.SingleOrDefaultAsync(x => x.ProductId == productId && x.StoreId == storeId);
        if (stock is not null)
            return stock;
        stock = new WarehouseStock { ProductId = productId, StoreId = storeId };
        db.WarehouseStocks.Add(stock);
        return stock;
    }

    public static async Task ValidateQuantityAsync(HeCashierDbContext db, decimal storeQuantity, decimal productQuantity)
    {
        var setting = await db.SystemSettings.AsNoTracking().SingleOrDefaultAsync(x => x.Key == "AllowNegativeStock");
        var allowNegativeStock = bool.TryParse(setting?.Value, out var value) && value;
        if (!allowNegativeStock && (storeQuantity < 0 || productQuantity < 0))
            throw new InvalidOperationException("لا يمكن تنفيذ الحركة لأن المخزون سيصبح سالباً. يمكن تغيير ذلك من الإعدادات.");
    }

    public static InventoryMovement AddMovement(HeCashierDbContext db, Product product, Store store, decimal quantity, decimal beforeQuantity, decimal afterQuantity, InventoryMovementType type, string notes, int userId)
    {
        var movement = new InventoryMovement
        {
            ProductId = product.Id,
            StoreId = store.Id,
            Type = type,
            Quantity = quantity,
            BeforeQuantity = beforeQuantity,
            AfterQuantity = afterQuantity,
            OccurredAtUtc = DateTime.UtcNow,
            UserId = userId,
            Notes = notes.Trim()
        };
        db.InventoryMovements.Add(movement);
        return movement;
    }

    public static AuditEntry CreateAudit(int userId, string operation, int productId, string? notes) => new()
    {
        UserId = userId,
        Operation = operation,
        EntityReference = $"Product:{productId};{notes?.Trim()}",
        OccurredAtUtc = DateTime.UtcNow
    };

    public static async Task<Store> GetDefaultStoreAsync(HeCashierDbContext db)
    {
        var store = await db.Stores
            .Where(x => x.IsActive)
            .OrderByDescending(x => x.IsDefault)
            .ThenBy(x => x.Id)
            .FirstOrDefaultAsync();
        if (store is not null)
            return store;
        store = new Store { Name = "المخزن الرئيسي", IsDefault = true };
        db.Stores.Add(store);
        await db.SaveChangesAsync();
        return store;
    }
}