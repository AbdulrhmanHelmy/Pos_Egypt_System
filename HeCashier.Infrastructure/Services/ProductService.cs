using System.Security.Cryptography;
using HeCashier.Data;
using HeCashier.Models;
using HeCashier.Services;
using Microsoft.EntityFrameworkCore;

namespace HeCashier.Infrastructure.Services;

public sealed class ProductService(IDbContextFactory<HeCashierDbContext> factory) : IProductService
{
    public async Task<IReadOnlyList<Product>> SearchAsync(ProductSearchFilter filter)
    {
        await using var db = await factory.CreateDbContextAsync();
        var query = db.Products
            .Include(x => x.Category)
            .Include(x => x.Unit)
            .Include(x => x.Barcodes)
            .AsNoTracking()
            .AsQueryable();
        if (!string.IsNullOrWhiteSpace(filter.SearchText))
        {
            var search = filter.SearchText.Trim();
            query = query.Where(x => x.Name.Contains(search) ||
                                     (x.ArabicName != null && x.ArabicName.Contains(search)) ||
                                     x.Code.Contains(search) ||
                                     x.Barcodes.Any(barcode => barcode.Value.Contains(search)));
        }
        if (filter.CategoryId.HasValue)
            query = query.Where(x => x.CategoryId == filter.CategoryId.Value);
        if (filter.IsActive.HasValue)
            query = query.Where(x => x.IsActive == filter.IsActive.Value);
        if (filter.LowStockOnly)
            query = query.Where(x => x.Quantity <= x.ReorderPoint);
        return await query.OrderBy(x => x.Name).ToListAsync();
    }

    public async Task<Product?> FindByBarcodeOrNameAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return null;
        var search = query.Trim();
        await using var db = await factory.CreateDbContextAsync();
        var byBarcode = await db.Products
            .Include(x => x.Category)
            .Include(x => x.Unit)
            .Include(x => x.Barcodes)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Barcodes.Any(barcode => barcode.Value == search));
        if (byBarcode is not null)
            return byBarcode;
        return await db.Products
            .Include(x => x.Category)
            .Include(x => x.Unit)
            .Include(x => x.Barcodes)
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .FirstOrDefaultAsync(x => x.Name.Contains(search) || (x.ArabicName != null && x.ArabicName.Contains(search)) || x.Code.Contains(search));
    }

    public async Task<Product> CreateAsync(ProductDraft draft, int userId)
    {
        ValidateDraft(draft);
        await using var db = await factory.CreateDbContextAsync();
        await using var transaction = await db.Database.BeginTransactionAsync();
        var product = new Product();
        CopyDraft(draft, product);
        product.Code = await GetUniqueCodeAsync(db, draft.Code);
        await ValidateLookupsAsync(db, draft);
        var barcodes = NormalizeBarcodes(draft.PrimaryBarcode, draft.AdditionalBarcodes);
        await ValidateBarcodesAsync(db, barcodes, null);
        await InventoryRules.ValidateQuantityAsync(db, draft.Quantity, draft.Quantity);
        var store = await InventoryRules.GetDefaultStoreAsync(db);
        product.Barcodes = barcodes.Select((value, index) => new ProductBarcode { Value = value, IsPrimary = index == 0 }).ToList();
        db.Products.Add(product);
        await db.SaveChangesAsync();

        db.WarehouseStocks.Add(new WarehouseStock { ProductId = product.Id, StoreId = store.Id, Quantity = draft.Quantity });
        if (draft.Quantity != 0)
        {
            InventoryRules.AddMovement(db, product, store, draft.Quantity, 0, draft.Quantity, InventoryMovementType.OpeningBalance, "رصيد افتتاحي", userId);
        }
        db.AuditEntries.Add(InventoryRules.CreateAudit(userId, "Product.Created", product.Id, product.Name));
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        return product;
    }

    public async Task<Product> UpdateAsync(int productId, ProductDraft draft, int userId)
    {
        ValidateDraft(draft);
        await using var db = await factory.CreateDbContextAsync();
        await using var transaction = await db.Database.BeginTransactionAsync();
        var product = await db.Products.Include(x => x.Barcodes).SingleOrDefaultAsync(x => x.Id == productId)
            ?? throw new KeyNotFoundException("الصنف غير موجود.");
        await ValidateLookupsAsync(db, draft);
        var code = string.IsNullOrWhiteSpace(draft.Code) ? product.Code : draft.Code.Trim();
        if (await db.Products.AnyAsync(x => x.Id != productId && x.Code == code))
            throw new InvalidOperationException("كود الصنف مستخدم بالفعل.");
        var barcodes = NormalizeBarcodes(draft.PrimaryBarcode, draft.AdditionalBarcodes);
        await ValidateBarcodesAsync(db, barcodes, productId);
        var quantityDelta = draft.Quantity - product.Quantity;
        if (quantityDelta != 0)
        {
            var store = await InventoryRules.GetDefaultStoreAsync(db);
            var stock = await InventoryRules.GetOrCreateStockAsync(db, product.Id, store.Id);
            var nextStoreQuantity = stock.Quantity + quantityDelta;
            var nextProductQuantity = product.Quantity + quantityDelta;
            await InventoryRules.ValidateQuantityAsync(db, nextStoreQuantity, nextProductQuantity);
            stock.Quantity = nextStoreQuantity;
            InventoryRules.AddMovement(db, product, store, quantityDelta, stock.Quantity - quantityDelta, nextStoreQuantity, InventoryMovementType.ManualAdjustment, "تعديل كمية الصنف", userId);
        }
        CopyDraft(draft, product);
        product.Code = code;
        db.Barcodes.RemoveRange(product.Barcodes);
        product.Barcodes = barcodes.Select((value, index) => new ProductBarcode { Value = value, IsPrimary = index == 0 }).ToList();
        db.AuditEntries.Add(InventoryRules.CreateAudit(userId, "Product.Updated", product.Id, product.Name));
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        return product;
    }

    public async Task<Product> QuickUpdateAsync(QuickProductUpdate update, int userId)
    {
        await using var db = await factory.CreateDbContextAsync();
        await using var transaction = await db.Database.BeginTransactionAsync();
        var product = await db.Products.Include(x => x.Barcodes).SingleOrDefaultAsync(x => x.Id == update.ProductId)
            ?? throw new KeyNotFoundException("الصنف غير موجود.");
        ValidatePrice(update.PurchasePrice, nameof(update.PurchasePrice));
        ValidatePrice(update.CostPrice, nameof(update.CostPrice));
        ValidatePrice(update.SellingPrice, nameof(update.SellingPrice));
        var oldQuantity = product.Quantity;
        if (update.Quantity.HasValue && update.Quantity.Value != oldQuantity)
        {
            var store = await InventoryRules.GetDefaultStoreAsync(db);
            var stock = await InventoryRules.GetOrCreateStockAsync(db, product.Id, store.Id);
            var delta = update.Quantity.Value - oldQuantity;
            var nextStoreQuantity = stock.Quantity + delta;
            await InventoryRules.ValidateQuantityAsync(db, nextStoreQuantity, update.Quantity.Value);
            stock.Quantity = nextStoreQuantity;
            product.Quantity = update.Quantity.Value;
            InventoryRules.AddMovement(db, product, store, delta, nextStoreQuantity - delta, nextStoreQuantity, InventoryMovementType.ManualAdjustment, "تعديل سريع للكمية", userId);
        }
        if (update.PurchasePrice.HasValue) product.PurchasePrice = update.PurchasePrice.Value;
        if (update.CostPrice.HasValue) product.CostPrice = update.CostPrice.Value;
        if (update.SellingPrice.HasValue) product.SellingPrice = update.SellingPrice.Value;
        if (update.IsActive.HasValue) product.IsActive = update.IsActive.Value;
        if (update.PrimaryBarcode is not null)
        {
            var primary = string.IsNullOrWhiteSpace(update.PrimaryBarcode) ? null : update.PrimaryBarcode.Trim();
            var additional = product.Barcodes.Where(x => !x.IsPrimary).Select(x => x.Value).ToArray();
            var values = NormalizeBarcodes(primary, additional);
            await ValidateBarcodesAsync(db, values, product.Id);
            db.Barcodes.RemoveRange(product.Barcodes);
            product.Barcodes = values.Select((value, index) => new ProductBarcode { Value = value, IsPrimary = index == 0 }).ToList();
        }
        db.AuditEntries.Add(InventoryRules.CreateAudit(userId, "Product.QuickUpdated", product.Id, product.Name));
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        return product;
    }

    public async Task<string> GenerateBarcodeAsync()
    {
        await using var db = await factory.CreateDbContextAsync();
        for (var attempt = 0; attempt < 100; attempt++)
        {
            var barcode = CreateEan13();
            if (!await db.Barcodes.AnyAsync(x => x.Value == barcode))
                return barcode;
        }
        throw new InvalidOperationException("تعذر إنشاء باركود فريد.");
    }

    private static void ValidateDraft(ProductDraft draft)
    {
        if (string.IsNullOrWhiteSpace(draft.Name))
            throw new ArgumentException("اسم الصنف مطلوب.", nameof(draft));
        ValidatePrice(draft.PurchasePrice, nameof(draft.PurchasePrice));
        ValidatePrice(draft.CostPrice, nameof(draft.CostPrice));
        ValidatePrice(draft.WholesalePrice, nameof(draft.WholesalePrice));
        ValidatePrice(draft.SellingPrice, nameof(draft.SellingPrice));
        if (draft.ReorderPoint < 0)
            throw new ArgumentOutOfRangeException(nameof(draft.ReorderPoint), "حد إعادة الطلب لا يمكن أن يكون سالباً.");
    }

    private static void ValidatePrice(decimal? value, string name)
    {
        if (value is < 0)
            throw new ArgumentOutOfRangeException(name, "لا يمكن أن يكون السعر سالباً.");
    }

    private static async Task ValidateLookupsAsync(HeCashierDbContext db, ProductDraft draft)
    {
        if (draft.CategoryId.HasValue && !await db.Categories.AnyAsync(x => x.Id == draft.CategoryId.Value && x.IsActive))
            throw new ArgumentException("القسم المحدد غير موجود أو غير نشط.", nameof(draft));
        if (draft.UnitId.HasValue && !await db.Units.AnyAsync(x => x.Id == draft.UnitId.Value && x.IsActive))
            throw new ArgumentException("الوحدة المحددة غير موجودة أو غير نشطة.", nameof(draft));
    }

    private static List<string> NormalizeBarcodes(string? primary, IEnumerable<string> additional)
    {
        var values = new List<string>();
        if (!string.IsNullOrWhiteSpace(primary))
            values.Add(primary.Trim());
        values.AddRange(additional.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()));
        if (values.Any(x => x.Length > 100))
            throw new ArgumentException("الباركود أطول من الحد المسموح.");
        if (values.Distinct(StringComparer.OrdinalIgnoreCase).Count() != values.Count)
            throw new InvalidOperationException("يوجد باركود مكرر ضمن بيانات الصنف.");
        return values;
    }

    private static async Task ValidateBarcodesAsync(HeCashierDbContext db, IReadOnlyCollection<string> values, int? exceptProductId)
    {
        if (values.Count == 0)
            return;
        var conflicts = await db.Barcodes
            .Where(x => values.Contains(x.Value) && (!exceptProductId.HasValue || x.ProductId != exceptProductId.Value))
            .Select(x => x.Value)
            .ToListAsync();
        if (conflicts.Count > 0)
            throw new InvalidOperationException($"الباركود {conflicts[0]} مستخدم بالفعل لصنف آخر.");
    }

    private static async Task<string> GetUniqueCodeAsync(HeCashierDbContext db, string code)
    {
        if (!string.IsNullOrWhiteSpace(code))
        {
            var normalized = code.Trim();
            if (await db.Products.AnyAsync(x => x.Code == normalized))
                throw new InvalidOperationException("كود الصنف مستخدم بالفعل.");
            return normalized;
        }
        for (var attempt = 0; attempt < 100; attempt++)
        {
            var generated = $"PRD-{Guid.NewGuid():N}"[..16];
            if (!await db.Products.AnyAsync(x => x.Code == generated))
                return generated;
        }
        throw new InvalidOperationException("تعذر إنشاء كود صنف فريد.");
    }

    private static string CreateEan13()
    {
        var digits = new int[13];
        for (var i = 0; i < 12; i++)
            digits[i] = RandomNumberGenerator.GetInt32(0, 10);
        var weightedSum = 0;
        for (var i = 0; i < 12; i++)
            weightedSum += digits[i] * (i % 2 == 0 ? 1 : 3);
        digits[12] = (10 - weightedSum % 10) % 10;
        return string.Concat(digits);
    }

    private static void CopyDraft(ProductDraft draft, Product product)
    {
        product.Name = draft.Name.Trim();
        product.ArabicName = draft.ArabicName?.Trim();
        product.CategoryId = draft.CategoryId;
        product.UnitId = draft.UnitId;
        product.PurchasePrice = draft.PurchasePrice;
        product.CostPrice = draft.CostPrice;
        product.WholesalePrice = draft.WholesalePrice;
        product.SellingPrice = draft.SellingPrice;
        product.Quantity = draft.Quantity;
        product.ReorderPoint = draft.ReorderPoint;
        product.ExpiryDate = draft.ExpiryDate;
        product.IsActive = draft.IsActive;
        product.Notes = draft.Notes?.Trim();
        product.SubCategory = draft.SubCategory?.Trim();
        product.GroupCode = draft.GroupCode?.Trim();
        product.Color = draft.Color?.Trim();
        product.Size = draft.Size?.Trim();
        product.Weight = draft.Weight;
        product.CostCenter = draft.CostCenter?.Trim();
        product.PackagingUnit = draft.PackagingUnit?.Trim();
        product.PackagingQuantity = draft.PackagingQuantity;
    }
}