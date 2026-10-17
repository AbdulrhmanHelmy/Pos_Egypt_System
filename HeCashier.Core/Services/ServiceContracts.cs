using HeCashier.Models;

namespace HeCashier.Services
{
    public sealed record AuthenticatedUser(int Id, string UserName);

    public interface IAuthenticationService
    {
        Task<AuthenticatedUser?> AuthenticateAsync(string userName, string password);
    }

    public interface IShiftService
    {
        Task<Shift?> GetActiveShiftAsync(int userId);
        Task<Shift> StartShiftAsync(int userId);
    }

    public sealed class ProductSearchFilter
    {
        public string? SearchText { get; init; }
        public int? CategoryId { get; init; }
        public bool? IsActive { get; init; }
        public bool LowStockOnly { get; init; }
    }

    public sealed class ProductDraft
    {
        public string Code { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string? ArabicName { get; init; }
        public int? CategoryId { get; init; }
        public int? UnitId { get; init; }
        public decimal PurchasePrice { get; init; }
        public decimal CostPrice { get; init; }
        public decimal WholesalePrice { get; init; }
        public decimal SellingPrice { get; init; }
        public decimal Quantity { get; init; }
        public decimal ReorderPoint { get; init; }
        public DateTime? ExpiryDate { get; init; }
        public bool IsActive { get; init; } = true;
        public string? Notes { get; init; }
        public string? PrimaryBarcode { get; init; }
        public IReadOnlyCollection<string> AdditionalBarcodes { get; init; } = Array.Empty<string>();
        public string? SubCategory { get; init; }
        public string? GroupCode { get; init; }
        public string? Color { get; init; }
        public string? Size { get; init; }
        public decimal? Weight { get; init; }
        public string? CostCenter { get; init; }
        public string? PackagingUnit { get; init; }
        public decimal? PackagingQuantity { get; init; }
    }

    public sealed class QuickProductUpdate
    {
        public int ProductId { get; init; }
        public decimal? PurchasePrice { get; init; }
        public decimal? CostPrice { get; init; }
        public decimal? SellingPrice { get; init; }
        public decimal? Quantity { get; init; }
        public string? PrimaryBarcode { get; init; }
        public bool? IsActive { get; init; }
    }

    public interface IProductService
    {
        Task<IReadOnlyList<Product>> SearchAsync(ProductSearchFilter filter);
        Task<Product?> FindByBarcodeOrNameAsync(string query);
        Task<Product> CreateAsync(ProductDraft draft, int userId);
        Task<Product> UpdateAsync(int productId, ProductDraft draft, int userId);
        Task<Product> QuickUpdateAsync(QuickProductUpdate update, int userId);
        Task<string> GenerateBarcodeAsync();
    }

    public interface ICategoryAndUnitService
    {
        Task<IReadOnlyList<Category>> GetCategoriesAsync();
        Task<Category> AddCategoryAsync(string name);
        Task<Category> UpdateCategoryAsync(int id, string name);
        Task DeleteCategoryAsync(int id);
        Task<IReadOnlyList<Unit>> GetUnitsAsync();
        Task<Unit> AddUnitAsync(string name);
        Task<Unit> UpdateUnitAsync(int id, string name);
        Task DeleteUnitAsync(int id);
    }

    public interface IInventoryService
    {
        Task<IReadOnlyList<Store>> GetStoresAsync();
        Task<IReadOnlyList<InventoryMovement>> GetMovementsAsync(int? productId = null);
        Task<InventoryMovement> AdjustStockAsync(int productId, int storeId, decimal quantityDelta, InventoryMovementType type, string notes, int userId);
        Task<StocktakeRecord> RecordStocktakeAsync(int productId, int storeId, decimal actualQuantity, string notes, int userId);
        Task<DamagedStockRecord> RemoveDamagedStockAsync(int productId, int storeId, decimal quantity, string reason, int userId);
        Task<InventoryTransfer> TransferStockAsync(int productId, int sourceStoreId, int destinationStoreId, decimal quantity, string notes, int userId);
    }

    public interface ISystemSettingsService
    {
        Task<bool> GetAllowNegativeStockAsync();
        Task SetAllowNegativeStockAsync(bool allow);
    }
}
