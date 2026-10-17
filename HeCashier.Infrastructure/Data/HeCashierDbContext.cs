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
    public DbSet<ProductGroup> ProductGroups => Set<ProductGroup>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductBarcode> Barcodes => Set<ProductBarcode>();
    public DbSet<Store> Stores => Set<Store>();
    public DbSet<WarehouseStock> WarehouseStocks => Set<WarehouseStock>();
    public DbSet<WarehouseStock> ProductStoreStocks => Set<WarehouseStock>();
    public DbSet<InventoryMovement> InventoryMovements => Set<InventoryMovement>();
    public DbSet<InventoryTransfer> InventoryTransfers => Set<InventoryTransfer>();
    public DbSet<StocktakeRecord> StocktakeRecords => Set<StocktakeRecord>();
    public DbSet<StocktakeRecord> Stocktakes => Set<StocktakeRecord>();
    public DbSet<DamagedStockRecord> DamagedStockRecords => Set<DamagedStockRecord>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<CustomerTransaction> CustomerTransactions => Set<CustomerTransaction>();
    public DbSet<ExternalDebt> ExternalDebts => Set<ExternalDebt>();
    public DbSet<CustomerCheck> CustomerChecks => Set<CustomerCheck>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<SupplierTransaction> SupplierTransactions => Set<SupplierTransaction>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleLine> SaleLines => Set<SaleLine>();
    public DbSet<Purchase> Purchases => Set<Purchase>();
    public DbSet<PurchaseLine> PurchaseLines => Set<PurchaseLine>();
    public DbSet<SaleReturn> SaleReturns => Set<SaleReturn>();
    public DbSet<SaleReturnLine> SaleReturnLines => Set<SaleReturnLine>();
    public DbSet<PurchaseReturn> PurchaseReturns => Set<PurchaseReturn>();
    public DbSet<PurchaseReturnLine> PurchaseReturnLines => Set<PurchaseReturnLine>();
    public DbSet<InstallmentPlan> InstallmentPlans => Set<InstallmentPlan>();
    public DbSet<InstallmentContract> InstallmentContracts => Set<InstallmentContract>();
    public DbSet<InstallmentSchedule> InstallmentSchedules => Set<InstallmentSchedule>();
    public DbSet<InstallmentPayment> InstallmentPayments => Set<InstallmentPayment>();
    public DbSet<Safe> Safes => Set<Safe>();
    public DbSet<BankAccount> BankAccounts => Set<BankAccount>();
    public DbSet<FinancialTransaction> FinancialTransactions => Set<FinancialTransaction>();
    public DbSet<ExpenseType> ExpenseTypes => Set<ExpenseType>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<EmployeeAttendance> EmployeeAttendances => Set<EmployeeAttendance>();
    public DbSet<EmployeeFinancialTransaction> EmployeeFinancialTransactions => Set<EmployeeFinancialTransaction>();
    public DbSet<SalaryPayment> SalaryPayments => Set<SalaryPayment>();
    public DbSet<AuditEntry> AuditEntries => Set<AuditEntry>();
    public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<BarcodeLabelTemplate> BarcodeLabelTemplates => Set<BarcodeLabelTemplate>();
    public DbSet<BarcodeLabelElement> BarcodeLabelElements => Set<BarcodeLabelElement>();
    public DbSet<ProductBatch> ProductBatches => Set<ProductBatch>();
    public DbSet<EWallet> EWallets => Set<EWallet>();
    public DbSet<EWalletMovement> EWalletMovements => Set<EWalletMovement>();
    public DbSet<RepairOrder> RepairOrders => Set<RepairOrder>();
    public DbSet<ManufacturingBatch> ManufacturingBatches => Set<ManufacturingBatch>();
    public DbSet<ManufacturingMaterial> ManufacturingMaterials => Set<ManufacturingMaterial>();
    public DbSet<BillOfMaterials> BillsOfMaterials => Set<BillOfMaterials>();
    public DbSet<BillOfMaterialsLine> BillOfMaterialsLines => Set<BillOfMaterialsLine>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().Property(x => x.UserName).UseCollation("NOCASE");
        modelBuilder.Entity<User>().HasIndex(x => x.UserName).IsUnique();
        modelBuilder.Entity<Role>().Property(x => x.Name).UseCollation("NOCASE");
        modelBuilder.Entity<Role>().HasIndex(x => x.Name).IsUnique();
        modelBuilder.Entity<Permission>().Property(x => x.Code).UseCollation("NOCASE");
        modelBuilder.Entity<Permission>().HasIndex(x => x.Code).IsUnique();
        modelBuilder.Entity<SystemSetting>().Property(x => x.Key).UseCollation("NOCASE");
        modelBuilder.Entity<SystemSetting>().HasIndex(x => x.Key).IsUnique();
        modelBuilder.Entity<Product>().Property(x => x.Code).UseCollation("NOCASE");
        modelBuilder.Entity<Product>().HasIndex(x => x.Code).IsUnique();
        modelBuilder.Entity<ProductBarcode>().Property(x => x.Value).UseCollation("NOCASE");
        modelBuilder.Entity<ProductBarcode>().HasIndex(x => x.Value).IsUnique();
        modelBuilder.Entity<Category>().Property(x => x.Name).UseCollation("NOCASE");
        modelBuilder.Entity<Category>().HasIndex(x => x.Name).IsUnique();
        modelBuilder.Entity<Unit>().Property(x => x.Name).UseCollation("NOCASE");
        modelBuilder.Entity<Unit>().HasIndex(x => x.Name).IsUnique();
        modelBuilder.Entity<ProductGroup>().HasIndex(x => x.Code).IsUnique();
        modelBuilder.Entity<Store>().Property(x => x.Name).UseCollation("NOCASE");
        modelBuilder.Entity<Store>().HasIndex(x => x.Name).IsUnique();
        modelBuilder.Entity<Safe>().Property(x => x.Name).UseCollation("NOCASE");
        modelBuilder.Entity<Safe>().HasIndex(x => x.Name).IsUnique();
        modelBuilder.Entity<ExpenseType>().Property(x => x.Name).UseCollation("NOCASE");
        modelBuilder.Entity<ExpenseType>().HasIndex(x => x.Name).IsUnique();
        modelBuilder.Entity<Customer>().HasIndex(x => x.Name);
        modelBuilder.Entity<Supplier>().HasIndex(x => x.Name);
        modelBuilder.Entity<Sale>().Property(x => x.InvoiceNumber).UseCollation("NOCASE");
        modelBuilder.Entity<Sale>().HasIndex(x => x.InvoiceNumber).IsUnique();
        modelBuilder.Entity<Purchase>().Property(x => x.InvoiceNumber).UseCollation("NOCASE");
        modelBuilder.Entity<Purchase>().HasIndex(x => x.InvoiceNumber).IsUnique();
        modelBuilder.Entity<SaleReturn>().HasIndex(x => x.ReturnNumber).IsUnique();
        modelBuilder.Entity<PurchaseReturn>().HasIndex(x => x.ReturnNumber).IsUnique();
        modelBuilder.Entity<InstallmentContract>().HasIndex(x => x.ContractNumber).IsUnique();
        modelBuilder.Entity<Expense>().HasIndex(x => x.VoucherNumber).IsUnique();
        modelBuilder.Entity<CustomerCheck>().HasIndex(x => new { x.CustomerId, x.CheckNumber }).IsUnique();
        modelBuilder.Entity<EWallet>().HasIndex(x => x.Name).IsUnique();
        modelBuilder.Entity<RepairOrder>().HasIndex(x => x.OrderNumber).IsUnique();
        modelBuilder.Entity<BarcodeLabelTemplate>().HasIndex(x => x.Name).IsUnique();
        modelBuilder.Entity<BarcodeLabelElement>().HasIndex(x => new { x.BarcodeLabelTemplateId, x.FieldKey }).IsUnique();
        modelBuilder.Entity<InstallmentSchedule>().HasIndex(x => new { x.InstallmentContractId, x.SequenceNumber }).IsUnique();

        modelBuilder.Entity<UserRole>().HasKey(x => new { x.UserId, x.RoleId });
        modelBuilder.Entity<RolePermission>().HasKey(x => new { x.RoleId, x.PermissionId });
        modelBuilder.Entity<WarehouseStock>().HasKey(x => new { x.ProductId, x.StoreId });
        modelBuilder.Entity<WarehouseStock>().ToTable("ProductStoreStocks");
        modelBuilder.Entity<StocktakeRecord>().ToTable("Stocktakes");
        modelBuilder.Entity<DamagedStockRecord>().ToTable("DamagedStock");

        modelBuilder.Entity<Category>().HasOne(x => x.ParentCategory).WithMany(x => x.SubCategories).HasForeignKey(x => x.ParentCategoryId);
        modelBuilder.Entity<Product>().HasOne(x => x.Category).WithMany(x => x.Products).HasForeignKey(x => x.CategoryId);
        modelBuilder.Entity<Product>().HasOne(x => x.Unit).WithMany(x => x.Products).HasForeignKey(x => x.UnitId);
        modelBuilder.Entity<Product>().HasOne(x => x.ProductGroup).WithMany(x => x.Products).HasForeignKey(x => x.ProductGroupId);
        modelBuilder.Entity<ProductBarcode>().HasOne(x => x.Product).WithMany(x => x.Barcodes).HasForeignKey(x => x.ProductId);
        modelBuilder.Entity<WarehouseStock>().HasOne(x => x.Product).WithMany(x => x.WarehouseStocks).HasForeignKey(x => x.ProductId);
        modelBuilder.Entity<WarehouseStock>().HasOne(x => x.Store).WithMany(x => x.Stocks).HasForeignKey(x => x.StoreId);
        modelBuilder.Entity<ProductBatch>().HasOne(x => x.Product).WithMany(x => x.Batches).HasForeignKey(x => x.ProductId);
        modelBuilder.Entity<ProductBatch>().HasOne(x => x.Store).WithMany().HasForeignKey(x => x.StoreId);
        modelBuilder.Entity<InventoryMovement>().HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId);
        modelBuilder.Entity<InventoryMovement>().HasOne(x => x.Store).WithMany().HasForeignKey(x => x.StoreId);
        modelBuilder.Entity<InventoryMovement>().HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
        modelBuilder.Entity<InventoryTransfer>().HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId);
        modelBuilder.Entity<InventoryTransfer>().HasOne(x => x.SourceStore).WithMany().HasForeignKey(x => x.SourceStoreId);
        modelBuilder.Entity<InventoryTransfer>().HasOne(x => x.DestinationStore).WithMany().HasForeignKey(x => x.DestinationStoreId);
        modelBuilder.Entity<InventoryTransfer>().HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
        modelBuilder.Entity<StocktakeRecord>().HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId);
        modelBuilder.Entity<StocktakeRecord>().HasOne(x => x.Store).WithMany().HasForeignKey(x => x.StoreId);
        modelBuilder.Entity<StocktakeRecord>().HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
        modelBuilder.Entity<DamagedStockRecord>().HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId);
        modelBuilder.Entity<DamagedStockRecord>().HasOne(x => x.Store).WithMany().HasForeignKey(x => x.StoreId);
        modelBuilder.Entity<DamagedStockRecord>().HasOne(x => x.InventoryMovement).WithMany().HasForeignKey(x => x.InventoryMovementId);

        modelBuilder.Entity<Shift>().HasOne(x => x.User).WithMany(x => x.Shifts).HasForeignKey(x => x.UserId);
        modelBuilder.Entity<Sale>().HasOne(x => x.Customer).WithMany(x => x.Sales).HasForeignKey(x => x.CustomerId);
        modelBuilder.Entity<Sale>().HasOne(x => x.Shift).WithMany(x => x.Sales).HasForeignKey(x => x.ShiftId);
        modelBuilder.Entity<Sale>().HasOne(x => x.Store).WithMany().HasForeignKey(x => x.StoreId);
        modelBuilder.Entity<Sale>().HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
        modelBuilder.Entity<SaleLine>().HasOne(x => x.Sale).WithMany(x => x.Lines).HasForeignKey(x => x.SaleId);
        modelBuilder.Entity<SaleLine>().HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId);
        modelBuilder.Entity<SaleLine>().HasOne(x => x.Store).WithMany().HasForeignKey(x => x.StoreId);
        modelBuilder.Entity<Purchase>().HasOne(x => x.Supplier).WithMany(x => x.Purchases).HasForeignKey(x => x.SupplierId);
        modelBuilder.Entity<Purchase>().HasOne(x => x.Store).WithMany().HasForeignKey(x => x.StoreId);
        modelBuilder.Entity<Purchase>().HasOne(x => x.Shift).WithMany(x => x.Purchases).HasForeignKey(x => x.ShiftId);
        modelBuilder.Entity<Purchase>().HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
        modelBuilder.Entity<PurchaseLine>().HasOne(x => x.Purchase).WithMany(x => x.Lines).HasForeignKey(x => x.PurchaseId);
        modelBuilder.Entity<PurchaseLine>().HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId);
        modelBuilder.Entity<PurchaseLine>().HasOne(x => x.Store).WithMany().HasForeignKey(x => x.StoreId);
        modelBuilder.Entity<SaleReturn>().HasOne(x => x.Sale).WithMany(x => x.Returns).HasForeignKey(x => x.SaleId);
        modelBuilder.Entity<SaleReturn>().HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
        modelBuilder.Entity<SaleReturn>().HasOne(x => x.Store).WithMany().HasForeignKey(x => x.StoreId);
        modelBuilder.Entity<SaleReturn>().HasOne(x => x.FinancialTransaction).WithMany().HasForeignKey(x => x.FinancialTransactionId);
        modelBuilder.Entity<SaleReturnLine>().HasOne(x => x.SaleReturn).WithMany(x => x.Lines).HasForeignKey(x => x.SaleReturnId);
        modelBuilder.Entity<SaleReturnLine>().HasOne(x => x.SaleLine).WithMany(x => x.ReturnLines).HasForeignKey(x => x.SaleLineId);
        modelBuilder.Entity<SaleReturnLine>().HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId);
        modelBuilder.Entity<PurchaseReturn>().HasOne(x => x.Purchase).WithMany(x => x.Returns).HasForeignKey(x => x.PurchaseId);
        modelBuilder.Entity<PurchaseReturn>().HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
        modelBuilder.Entity<PurchaseReturnLine>().HasOne(x => x.PurchaseReturn).WithMany(x => x.Lines).HasForeignKey(x => x.PurchaseReturnId);
        modelBuilder.Entity<PurchaseReturnLine>().HasOne(x => x.PurchaseLine).WithMany(x => x.ReturnLines).HasForeignKey(x => x.PurchaseLineId);

        modelBuilder.Entity<CustomerTransaction>().HasOne(x => x.Customer).WithMany(x => x.Transactions).HasForeignKey(x => x.CustomerId);
        modelBuilder.Entity<CustomerTransaction>().HasOne(x => x.Sale).WithMany().HasForeignKey(x => x.SaleId);
        modelBuilder.Entity<CustomerTransaction>().HasOne(x => x.ExternalDebt).WithMany(x => x.Transactions).HasForeignKey(x => x.ExternalDebtId);
        modelBuilder.Entity<CustomerTransaction>().HasOne(x => x.FinancialTransaction).WithMany(x => x.CustomerTransactions).HasForeignKey(x => x.FinancialTransactionId);
        modelBuilder.Entity<CustomerTransaction>().HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
        modelBuilder.Entity<ExternalDebt>().HasOne(x => x.Customer).WithMany(x => x.ExternalDebts).HasForeignKey(x => x.CustomerId);
        modelBuilder.Entity<ExternalDebt>().HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
        modelBuilder.Entity<CustomerCheck>().HasOne(x => x.Customer).WithMany(x => x.Checks).HasForeignKey(x => x.CustomerId);
        modelBuilder.Entity<SupplierTransaction>().HasOne(x => x.Supplier).WithMany(x => x.Transactions).HasForeignKey(x => x.SupplierId);
        modelBuilder.Entity<SupplierTransaction>().HasOne(x => x.Purchase).WithMany().HasForeignKey(x => x.PurchaseId);
        modelBuilder.Entity<SupplierTransaction>().HasOne(x => x.FinancialTransaction).WithMany(x => x.SupplierTransactions).HasForeignKey(x => x.FinancialTransactionId);
        modelBuilder.Entity<SupplierTransaction>().HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);

        modelBuilder.Entity<FinancialTransaction>().HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
        modelBuilder.Entity<FinancialTransaction>().HasOne(x => x.Safe).WithMany(x => x.Transactions).HasForeignKey(x => x.SafeId);
        modelBuilder.Entity<FinancialTransaction>().HasOne(x => x.BankAccount).WithMany(x => x.Transactions).HasForeignKey(x => x.BankAccountId);
        modelBuilder.Entity<FinancialTransaction>().HasOne(x => x.Shift).WithMany().HasForeignKey(x => x.ShiftId);
        modelBuilder.Entity<Expense>().HasOne(x => x.ExpenseType).WithMany(x => x.Expenses).HasForeignKey(x => x.ExpenseTypeId);
        modelBuilder.Entity<Expense>().HasOne(x => x.Safe).WithMany().HasForeignKey(x => x.SafeId);
        modelBuilder.Entity<Expense>().HasOne(x => x.BankAccount).WithMany().HasForeignKey(x => x.BankAccountId);
        modelBuilder.Entity<Expense>().HasOne(x => x.Shift).WithMany().HasForeignKey(x => x.ShiftId);
        modelBuilder.Entity<Expense>().HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
        modelBuilder.Entity<Expense>().HasOne(x => x.FinancialTransaction).WithMany().HasForeignKey(x => x.FinancialTransactionId);

        modelBuilder.Entity<InstallmentContract>().HasOne(x => x.Customer).WithMany(x => x.InstallmentContracts).HasForeignKey(x => x.CustomerId);
        modelBuilder.Entity<InstallmentContract>().HasOne(x => x.Sale).WithMany().HasForeignKey(x => x.SaleId);
        modelBuilder.Entity<InstallmentContract>().HasOne(x => x.InstallmentPlan).WithMany(x => x.Contracts).HasForeignKey(x => x.InstallmentPlanId);
        modelBuilder.Entity<InstallmentContract>().HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
        modelBuilder.Entity<InstallmentSchedule>().HasOne(x => x.InstallmentContract).WithMany(x => x.Schedule).HasForeignKey(x => x.InstallmentContractId);
        modelBuilder.Entity<InstallmentPayment>().HasOne(x => x.InstallmentSchedule).WithMany(x => x.Payments).HasForeignKey(x => x.InstallmentScheduleId);
        modelBuilder.Entity<InstallmentPayment>().HasOne(x => x.FinancialTransaction).WithMany().HasForeignKey(x => x.FinancialTransactionId);
        modelBuilder.Entity<InstallmentPayment>().HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);

        modelBuilder.Entity<Employee>().HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
        modelBuilder.Entity<EmployeeAttendance>().HasOne(x => x.Employee).WithMany(x => x.Attendances).HasForeignKey(x => x.EmployeeId);
        modelBuilder.Entity<EmployeeFinancialTransaction>().HasOne(x => x.Employee).WithMany(x => x.FinancialTransactions).HasForeignKey(x => x.EmployeeId);
        modelBuilder.Entity<EmployeeFinancialTransaction>().HasOne(x => x.Safe).WithMany().HasForeignKey(x => x.SafeId);
        modelBuilder.Entity<EmployeeFinancialTransaction>().HasOne(x => x.Shift).WithMany().HasForeignKey(x => x.ShiftId);
        modelBuilder.Entity<EmployeeFinancialTransaction>().HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
        modelBuilder.Entity<SalaryPayment>().HasOne(x => x.Employee).WithMany(x => x.SalaryPayments).HasForeignKey(x => x.EmployeeId);
        modelBuilder.Entity<SalaryPayment>().HasOne(x => x.Safe).WithMany().HasForeignKey(x => x.SafeId);
        modelBuilder.Entity<SalaryPayment>().HasOne(x => x.FinancialTransaction).WithMany().HasForeignKey(x => x.FinancialTransactionId);
        modelBuilder.Entity<SalaryPayment>().HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);

        modelBuilder.Entity<Notification>().HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
        modelBuilder.Entity<Notification>().HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId);
        modelBuilder.Entity<Notification>().HasOne(x => x.Customer).WithMany().HasForeignKey(x => x.CustomerId);
        modelBuilder.Entity<Notification>().HasOne(x => x.Supplier).WithMany().HasForeignKey(x => x.SupplierId);
        modelBuilder.Entity<Notification>().HasOne(x => x.InstallmentSchedule).WithMany().HasForeignKey(x => x.InstallmentScheduleId);
        modelBuilder.Entity<BarcodeLabelElement>().HasOne(x => x.BarcodeLabelTemplate).WithMany(x => x.Elements).HasForeignKey(x => x.BarcodeLabelTemplateId);
        modelBuilder.Entity<EWalletMovement>().HasOne(x => x.EWallet).WithMany(x => x.Movements).HasForeignKey(x => x.EWalletId);
        modelBuilder.Entity<EWalletMovement>().HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
        modelBuilder.Entity<RepairOrder>().HasOne(x => x.Customer).WithMany().HasForeignKey(x => x.CustomerId);
        modelBuilder.Entity<RepairOrder>().HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
        modelBuilder.Entity<ManufacturingBatch>().HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId);
        modelBuilder.Entity<ManufacturingBatch>().HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
        modelBuilder.Entity<ManufacturingMaterial>().HasOne(x => x.ManufacturingBatch).WithMany(x => x.Materials).HasForeignKey(x => x.ManufacturingBatchId);
        modelBuilder.Entity<ManufacturingMaterial>().HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId);
        modelBuilder.Entity<BillOfMaterials>().HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId);
        modelBuilder.Entity<BillOfMaterialsLine>().HasOne(x => x.BillOfMaterials).WithMany(x => x.Lines).HasForeignKey(x => x.BillOfMaterialsId);
        modelBuilder.Entity<BillOfMaterialsLine>().HasOne(x => x.RawMaterialProduct).WithMany().HasForeignKey(x => x.RawMaterialProductId);
        modelBuilder.Entity<AuditEntry>().HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var foreignKey in entityType.GetForeignKeys())
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;

            foreach (var property in entityType.GetProperties())
            {
                if ((Nullable.GetUnderlyingType(property.ClrType) ?? property.ClrType) != typeof(decimal))
                    continue;

                var scale = property.Name is "Quantity" or "QuantityReceived" or "QuantityRemaining" or "ReorderPoint" or "Weight" or
                    "PackagingQuantity" or "BeforeQuantity" or "AfterQuantity" or "SystemQuantity" or "ActualQuantity" or
                    "Difference" or "BreakHours" or "HoursWorked" or "OutputQuantity" ? 3 : 2;
                modelBuilder.Entity(entityType.ClrType).Property(property.Name).HasPrecision(18, scale);
            }
        }
    }
}