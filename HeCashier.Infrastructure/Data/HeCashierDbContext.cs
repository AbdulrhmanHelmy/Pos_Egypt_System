using HeCashier.Models;
using Microsoft.EntityFrameworkCore;

namespace HeCashier.Data;

public sealed class HeCashierDbContext(DbContextOptions<HeCashierDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<Shift> Shifts => Set<Shift>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Unit> Units => Set<Unit>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductBarcode> Barcodes => Set<ProductBarcode>();
    public DbSet<Store> Stores => Set<Store>();
    public DbSet<WarehouseStock> WarehouseStocks => Set<WarehouseStock>();
    public DbSet<InventoryMovement> InventoryMovements => Set<InventoryMovement>();
    public DbSet<InventoryTransfer> InventoryTransfers => Set<InventoryTransfer>();
    public DbSet<StocktakeRecord> StocktakeRecords => Set<StocktakeRecord>();
    public DbSet<DamagedStockRecord> DamagedStockRecords => Set<DamagedStockRecord>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleLine> SaleLines => Set<SaleLine>();
    public DbSet<Purchase> Purchases => Set<Purchase>();
    public DbSet<PurchaseLine> PurchaseLines => Set<PurchaseLine>();
    public DbSet<Safe> Safes => Set<Safe>();
    public DbSet<BankAccount> BankAccounts => Set<BankAccount>();
    public DbSet<FinancialTransaction> FinancialTransactions => Set<FinancialTransaction>();
    public DbSet<ExpenseType> ExpenseTypes => Set<ExpenseType>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<AuditEntry> AuditEntries => Set<AuditEntry>();
    public DbSet<EWallet> EWallets => Set<EWallet>();
    public DbSet<EWalletMovement> EWalletMovements => Set<EWalletMovement>();
    public DbSet<RepairOrder> RepairOrders => Set<RepairOrder>();
    public DbSet<ManufacturingBatch> ManufacturingBatches => Set<ManufacturingBatch>();
    public DbSet<ManufacturingMaterial> ManufacturingMaterials => Set<ManufacturingMaterial>();
    public DbSet<SaleReturn> SaleReturns => Set<SaleReturn>();
    public DbSet<SaleReturnLine> SaleReturnLines => Set<SaleReturnLine>();
    public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasIndex(x => x.UserName).IsUnique();
        modelBuilder.Entity<ProductBarcode>().Property(x => x.Value).UseCollation("NOCASE");
        modelBuilder.Entity<ProductBarcode>().HasIndex(x => x.Value).IsUnique();
        modelBuilder.Entity<Product>().Property(x => x.Code).UseCollation("NOCASE");
        modelBuilder.Entity<Product>().HasIndex(x => x.Code).IsUnique();
        modelBuilder.Entity<Category>().Property(x => x.Name).UseCollation("NOCASE");
        modelBuilder.Entity<Category>().HasIndex(x => x.Name).IsUnique();
        modelBuilder.Entity<Unit>().Property(x => x.Name).UseCollation("NOCASE");
        modelBuilder.Entity<Unit>().HasIndex(x => x.Name).IsUnique();
        modelBuilder.Entity<Store>().Property(x => x.Name).UseCollation("NOCASE");
        modelBuilder.Entity<Store>().HasIndex(x => x.Name).IsUnique();
        modelBuilder.Entity<EWallet>().HasIndex(x => x.Name).IsUnique();
        modelBuilder.Entity<RepairOrder>().HasIndex(x => x.OrderNumber).IsUnique();
        modelBuilder.Entity<SystemSetting>().HasIndex(x => x.Key).IsUnique();
        modelBuilder.Entity<Role>().HasIndex(x => x.Name).IsUnique();

        modelBuilder.Entity<UserRole>().HasKey(x => new { x.UserId, x.RoleId });
        modelBuilder.Entity<RolePermission>().HasKey(x => new { x.RoleId, x.PermissionId });
        modelBuilder.Entity<UserRole>().HasOne(x => x.User).WithMany(x => x.UserRoles).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<UserRole>().HasOne(x => x.Role).WithMany(x => x.UserRoles).HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Product>().HasOne(x => x.Category).WithMany(x => x.Products).HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Product>().HasOne(x => x.Unit).WithMany(x => x.Products).HasForeignKey(x => x.UnitId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ProductBarcode>().HasOne(x => x.Product).WithMany(x => x.Barcodes).HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<WarehouseStock>().HasKey(x => new { x.ProductId, x.StoreId });
        modelBuilder.Entity<WarehouseStock>().HasOne(x => x.Product).WithMany(x => x.WarehouseStocks).HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<WarehouseStock>().HasOne(x => x.Store).WithMany(x => x.Stocks).HasForeignKey(x => x.StoreId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<InventoryMovement>().HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<InventoryMovement>().HasOne(x => x.Store).WithMany().HasForeignKey(x => x.StoreId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<InventoryMovement>().HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<InventoryTransfer>().HasOne(x => x.SourceStore).WithMany().HasForeignKey(x => x.SourceStoreId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<InventoryTransfer>().HasOne(x => x.DestinationStore).WithMany().HasForeignKey(x => x.DestinationStoreId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<StocktakeRecord>().HasOne(x => x.Store).WithMany().HasForeignKey(x => x.StoreId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<DamagedStockRecord>().HasOne(x => x.InventoryMovement).WithMany().HasForeignKey(x => x.InventoryMovementId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<AuditEntry>().HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<FinancialTransaction>().HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<EWalletMovement>().HasOne(x => x.EWallet).WithMany(x => x.Movements).HasForeignKey(x => x.EWalletId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<EWalletMovement>().HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<RepairOrder>().HasOne(x => x.Customer).WithMany().HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<RepairOrder>().HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<ManufacturingBatch>().HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ManufacturingBatch>().HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<ManufacturingMaterial>().HasOne(x => x.ManufacturingBatch).WithMany(x => x.Materials).HasForeignKey(x => x.ManufacturingBatchId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<ManufacturingMaterial>().HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<SaleReturn>().HasOne(x => x.Sale).WithMany().HasForeignKey(x => x.SaleId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<SaleReturn>().HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<SaleReturnLine>().HasOne(x => x.SaleReturn).WithMany(x => x.Lines).HasForeignKey(x => x.SaleReturnId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<SaleReturnLine>().HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if ((Nullable.GetUnderlyingType(property.ClrType) ?? property.ClrType) != typeof(decimal))
                    continue;

                var scale = property.Name is "Quantity" or "ReorderPoint" or "Weight" or "PackagingQuantity" or
                    "BeforeQuantity" or "AfterQuantity" or "SystemQuantity" or "ActualQuantity" or "Difference" ? 3 : 2;
                modelBuilder.Entity(entityType.ClrType).Property(property.Name).HasPrecision(18, scale);
            }
        }
    }
}
