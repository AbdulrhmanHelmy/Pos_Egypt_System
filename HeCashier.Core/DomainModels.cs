using System.ComponentModel.DataAnnotations;

namespace HeCashier.Models;

public enum ShiftStatus
{
    Open,
    Closed
}

public enum CustomerCheckStatus
{
    Pending,
    Collected,
    Rejected
}

public enum FinancialTransactionType
{
    OpeningBalance,
    Deposit,
    Withdrawal,
    Transfer,
    Sale,
    Purchase,
    CustomerCollection,
    SupplierPayment,
    Expense,
    Salary,
    Advance,
    Refund,
    Adjustment
}

public enum PaymentType
{
    Cash,
    Credit,
    Mixed,
    Visa
}

public enum TransactionStatus
{
    Draft,
    Completed,
    Voided
}

public enum ReturnFinancialMode
{
    CashRefund,
    CustomerCredit,
    SupplierCredit,
    SafeAdjustment,
    BankAdjustment
}

public enum InstallmentStatus
{
    Pending,
    PartiallyPaid,
    Paid,
    Overdue,
    Cancelled
}

public enum PartyTransactionType
{
    OpeningBalance,
    Invoice,
    Payment,
    Collection,
    Discount,
    Return,
    ExternalDebt,
    Adjustment,
    InstallmentPayment
}

public enum EmployeeFinancialTransactionType
{
    Advance,
    Loan,
    Bonus,
    Deduction,
    Salary
}

public enum NotificationType
{
    LowStock,
    SupplierPayable,
    CustomerReceivable,
    ExpiringProduct,
    InstallmentDue,
    System
}

public enum InventoryMovementType
{
    OpeningBalance,
    ManualAdjustment,
    Damaged,
    Stocktake,
    TransferIn,
    TransferOut,
    Purchase,
    Sale,
    Return,
    Manufacturing
}

public enum EWalletMovementType
{
    CashIn,
    CashOut
}

public enum RepairOrderStatus
{
    Received,
    InProgress,
    Ready,
    Delivered
}

public class User
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? DisplayName { get; set; }

    public bool CanDepositToMainSafe { get; set; }

    public bool IsActive { get; set; } = true;
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<Shift> Shifts { get; set; } = new List<Shift>();
}

public class Role
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}

public class Permission
{
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string Code { get; set; } = string.Empty;

    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Module { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string Action { get; set; } = string.Empty;

    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}

public class UserRole
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;
}

public class RolePermission
{
    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;
    public int PermissionId { get; set; }
    public Permission Permission { get; set; } = null!;
}

public class Shift
{
    public int Id { get; set; }

    [Required, MaxLength(30)]
    public string Number { get; set; } = string.Empty;

    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int? SafeId { get; set; }
    public Safe? Safe { get; set; }
    public DateTime StartedAtUtc { get; set; }
    public DateTime? ClosedAtUtc { get; set; }
    public decimal OpeningBalance { get; set; }
    public decimal? ClosingBalance { get; set; }
    public decimal? ExpectedCash { get; set; }
    public decimal? Difference { get; set; }
    [MaxLength(1000)]
    public string Notes { get; set; } = string.Empty;
    public ShiftStatus Status { get; set; } = ShiftStatus.Open;
    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
    public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
}

public class Category
{
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public int? ParentCategoryId { get; set; }
    public Category? ParentCategory { get; set; }
    public ICollection<Category> SubCategories { get; set; } = new List<Category>();

    public ICollection<Product> Products { get; set; } = new List<Product>();
}

public class Unit
{
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}

public class Product
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Code { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? ArabicName { get; set; }

    public decimal SellingPrice { get; set; }
    public decimal WholesalePrice { get; set; }
    public decimal HalfWholesalePrice { get; set; }
    public decimal CostPrice { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal Quantity { get; set; }
    public decimal ReorderPoint { get; set; }
    public DateTime? ExpiryDate { get; set; }
    [MaxLength(100)]
    public string? SubCategory { get; set; }
    [MaxLength(100)]
    public string? GroupCode { get; set; }
    [MaxLength(100)]
    public string? Color { get; set; }
    [MaxLength(100)]
    public string? Size { get; set; }
    public decimal? Weight { get; set; }
    [MaxLength(150)]
    public string? CostCenter { get; set; }
    [MaxLength(100)]
    public string? PackagingUnit { get; set; }
    public decimal? PackagingQuantity { get; set; }
    [MaxLength(100)]
    public string? ModelNumber { get; set; }
    public bool DoesNotAffectInventory { get; set; }
    public bool IsBarcodeScaleItem { get; set; }
    public bool IsSoldByLength { get; set; }
    public bool IsActive { get; set; } = true;
    [MaxLength(1000)]
    public string? Notes { get; set; }
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }
    public int? UnitId { get; set; }
    public Unit? Unit { get; set; }
    public int? ProductGroupId { get; set; }
    public ProductGroup? ProductGroup { get; set; }
    public ICollection<ProductBarcode> Barcodes { get; set; } = new List<ProductBarcode>();
    public ICollection<WarehouseStock> WarehouseStocks { get; set; } = new List<WarehouseStock>();
    public ICollection<ProductBatch> Batches { get; set; } = new List<ProductBatch>();
}

public class ProductBarcode
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Value { get; set; } = string.Empty;

    public bool IsPrimary { get; set; }

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
}

public class Store
{
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
    public bool IsDefault { get; set; }
    public ICollection<WarehouseStock> Stocks { get; set; } = new List<WarehouseStock>();
}

public class WarehouseStock
{
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public int StoreId { get; set; }
    public Store Store { get; set; } = null!;
    public decimal Quantity { get; set; }
}

public class InventoryMovement
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public int? StoreId { get; set; }
    public Store? Store { get; set; }
    public InventoryMovementType Type { get; set; }
    public decimal Quantity { get; set; }
    public decimal BeforeQuantity { get; set; }
    public decimal AfterQuantity { get; set; }
    public DateTime OccurredAtUtc { get; set; }
    public int? UserId { get; set; }
    public User? User { get; set; }
    [MaxLength(1000)]
    public string Notes { get; set; } = string.Empty;
}

public class InventoryTransfer
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public int SourceStoreId { get; set; }
    public Store SourceStore { get; set; } = null!;
    public int DestinationStoreId { get; set; }
    public Store DestinationStore { get; set; } = null!;
    public decimal Quantity { get; set; }
    public DateTime OccurredAtUtc { get; set; }
    public int? UserId { get; set; }
    public User? User { get; set; }
    [MaxLength(1000)]
    public string Notes { get; set; } = string.Empty;
}

public class StocktakeRecord
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public int StoreId { get; set; }
    public Store Store { get; set; } = null!;
    public decimal SystemQuantity { get; set; }
    public decimal ActualQuantity { get; set; }
    public decimal Difference { get; set; }
    public DateTime OccurredAtUtc { get; set; }
    public int? UserId { get; set; }
    public User? User { get; set; }
    [MaxLength(1000)]
    public string Notes { get; set; } = string.Empty;
}

public class DamagedStockRecord
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public int StoreId { get; set; }
    public Store Store { get; set; } = null!;
    public int InventoryMovementId { get; set; }
    public InventoryMovement InventoryMovement { get; set; } = null!;
    public decimal Quantity { get; set; }
    public DateTime OccurredAtUtc { get; set; }
    [Required, MaxLength(500)]
    public string Reason { get; set; } = string.Empty;
}

public class Customer
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? CompanyName { get; set; }

    [MaxLength(50)]
    public string? Phone { get; set; }

    [MaxLength(50)]
    public string? SecondaryPhone { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    [MaxLength(50)]
    public string? NationalId { get; set; }

    public decimal OpeningBalance { get; set; }
    public decimal CurrentBalance { get; set; }
    public decimal? CreditLimit { get; set; }
    public bool IsCashCustomer { get; set; }
    public bool IsActive { get; set; } = true;
    [MaxLength(1000)]
    public string? Notes { get; set; }
    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
    public ICollection<CustomerTransaction> Transactions { get; set; } = new List<CustomerTransaction>();
    public ICollection<ExternalDebt> ExternalDebts { get; set; } = new List<ExternalDebt>();
    public ICollection<CustomerCheck> Checks { get; set; } = new List<CustomerCheck>();
    public ICollection<InstallmentContract> InstallmentContracts { get; set; } = new List<InstallmentContract>();
}

public class Supplier
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? CompanyName { get; set; }

    [MaxLength(50)]
    public string? Phone { get; set; }

    [MaxLength(50)]
    public string? SecondaryPhone { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    public decimal OpeningBalance { get; set; }
    public decimal CurrentBalance { get; set; }
    public decimal? CreditLimit { get; set; }
    public bool IsActive { get; set; } = true;
    [MaxLength(1000)]
    public string? Notes { get; set; }
    public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
    public ICollection<SupplierTransaction> Transactions { get; set; } = new List<SupplierTransaction>();
}

public class Sale
{
    public int Id { get; set; }
    [Required, MaxLength(50)]
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public int? ShiftId { get; set; }
    public Shift? Shift { get; set; }
    public int? StoreId { get; set; }
    public Store? Store { get; set; }
    public int? UserId { get; set; }
    public User? User { get; set; }
    public PaymentType PaymentType { get; set; }
    public decimal Subtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal NetTotal { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public TransactionStatus Status { get; set; } = TransactionStatus.Completed;
    [MaxLength(1000)]
    public string Notes { get; set; } = string.Empty;
    public ICollection<SaleLine> Lines { get; set; } = new List<SaleLine>();
    public ICollection<SaleReturn> Returns { get; set; } = new List<SaleReturn>();
}

public class SaleLine
{
    public int Id { get; set; }
    public int SaleId { get; set; }
    public Sale Sale { get; set; } = null!;
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal CostSnapshot { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal LineTotal { get; set; }
    public int? StoreId { get; set; }
    public Store? Store { get; set; }
    public ICollection<SaleReturnLine> ReturnLines { get; set; } = new List<SaleReturnLine>();
}

public class Purchase
{
    public int Id { get; set; }
    [Required, MaxLength(50)]
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public int? SupplierId { get; set; }
    public Supplier? Supplier { get; set; }
    public int? StoreId { get; set; }
    public Store? Store { get; set; }
    public int? ShiftId { get; set; }
    public Shift? Shift { get; set; }
    public int? UserId { get; set; }
    public User? User { get; set; }
    public PaymentType PaymentType { get; set; }
    public decimal Subtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal NetTotal { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public TransactionStatus Status { get; set; } = TransactionStatus.Completed;
    [MaxLength(1000)]
    public string Notes { get; set; } = string.Empty;
    public ICollection<PurchaseLine> Lines { get; set; } = new List<PurchaseLine>();
    public ICollection<PurchaseReturn> Returns { get; set; } = new List<PurchaseReturn>();
}

public class PurchaseLine
{
    public int Id { get; set; }
    public int PurchaseId { get; set; }
    public Purchase Purchase { get; set; } = null!;
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal CostSnapshot { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal LineTotal { get; set; }
    public int? StoreId { get; set; }
    public Store? Store { get; set; }
    public ICollection<PurchaseReturnLine> ReturnLines { get; set; } = new List<PurchaseReturnLine>();
}

public class Safe
{
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    public decimal OpeningBalance { get; set; }
    public decimal CurrentBalance { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<FinancialTransaction> Transactions { get; set; } = new List<FinancialTransaction>();
}

public class BankAccount
{
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? AccountNumber { get; set; }
    [MaxLength(100)]
    public string? BankName { get; set; }
    public decimal OpeningBalance { get; set; }
    public decimal CurrentBalance { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<FinancialTransaction> Transactions { get; set; } = new List<FinancialTransaction>();
}

public class FinancialTransaction
{
    public int Id { get; set; }
    public FinancialTransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public decimal BalanceBefore { get; set; }
    public decimal BalanceAfter { get; set; }
    public DateTime OccurredAtUtc { get; set; }
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;
    [MaxLength(50)]
    public string TransactionType { get; set; } = string.Empty;
    [MaxLength(100)]
    public string? InvoiceNumber { get; set; }
    public int? UserId { get; set; }
    public User? User { get; set; }
    public int? SafeId { get; set; }
    public Safe? Safe { get; set; }
    public int? BankAccountId { get; set; }
    public BankAccount? BankAccount { get; set; }
    public int? ShiftId { get; set; }
    public Shift? Shift { get; set; }
    [MaxLength(100)]
    public string? ReferenceEntityType { get; set; }
    public int? ReferenceEntityId { get; set; }
    [MaxLength(200)]
    public string? DepositorOrBeneficiaryName { get; set; }
    public ICollection<CustomerTransaction> CustomerTransactions { get; set; } = new List<CustomerTransaction>();
    public ICollection<SupplierTransaction> SupplierTransactions { get; set; } = new List<SupplierTransaction>();
}

public class ExpenseType
{
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
}

public class Expense
{
    public int Id { get; set; }
    [Required, MaxLength(50)]
    public string VoucherNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime OccurredAtUtc { get; set; }
    public int ExpenseTypeId { get; set; }
    public ExpenseType ExpenseType { get; set; } = null!;
    public int? SafeId { get; set; }
    public Safe? Safe { get; set; }
    public int? BankAccountId { get; set; }
    public BankAccount? BankAccount { get; set; }
    public int? ShiftId { get; set; }
    public Shift? Shift { get; set; }
    public int? UserId { get; set; }
    public User? User { get; set; }
    public int? FinancialTransactionId { get; set; }
    public FinancialTransaction? FinancialTransaction { get; set; }
    [MaxLength(100)]
    public string? InvoiceNumber { get; set; }
    [MaxLength(1000)]
    public string? Notes { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class AuditEntry
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public User? User { get; set; }
    public string Operation { get; set; } = string.Empty;
    public string? EntityReference { get; set; }
    public DateTime OccurredAtUtc { get; set; }
}

public class SystemSetting
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Key { get; set; } = string.Empty;

    [Required, MaxLength(500)]
    public string Value { get; set; } = string.Empty;
}

public class EWallet
{
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Provider { get; set; }

    [MaxLength(50)]
    public string? PhoneNumber { get; set; }

    public bool IsActive { get; set; } = true;
    public ICollection<EWalletMovement> Movements { get; set; } = new List<EWalletMovement>();
}

public class EWalletMovement
{
    public int Id { get; set; }
    public int EWalletId { get; set; }
    public EWallet EWallet { get; set; } = null!;
    public EWalletMovementType Type { get; set; }
    public decimal Amount { get; set; }
    public DateTime OccurredAtUtc { get; set; }
    public int? UserId { get; set; }
    public User? User { get; set; }
    [MaxLength(100)]
    public string? InvoiceNumber { get; set; }
    [MaxLength(1000)]
    public string Notes { get; set; } = string.Empty;
}

public class RepairOrder
{
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string OrderNumber { get; set; } = string.Empty;

    [MaxLength(100)]
    public string DeviceType { get; set; } = string.Empty;
    [MaxLength(100)]
    public string Brand { get; set; } = string.Empty;
    [MaxLength(100)]
    public string Model { get; set; } = string.Empty;
    [MaxLength(100)]
    public string SerialNumber { get; set; } = string.Empty;
    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }
    [MaxLength(1000)]
    public string Problem { get; set; } = string.Empty;
    [MaxLength(1000)]
    public string Notes { get; set; } = string.Empty;
    public RepairOrderStatus Status { get; set; } = RepairOrderStatus.Received;
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public DateTime ReceivedAtUtc { get; set; }
    public DateTime? DeliveredAtUtc { get; set; }
    public int? UserId { get; set; }
    public User? User { get; set; }
}

public class ManufacturingBatch
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public decimal Quantity { get; set; }
    public decimal TotalCost { get; set; }
    public DateTime OccurredAtUtc { get; set; }
    public int? UserId { get; set; }
    public User? User { get; set; }
    public ICollection<ManufacturingMaterial> Materials { get; set; } = new List<ManufacturingMaterial>();
}

public class ManufacturingMaterial
{
    public int Id { get; set; }
    public int ManufacturingBatchId { get; set; }
    public ManufacturingBatch ManufacturingBatch { get; set; } = null!;
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
}

public class SaleReturn
{
    public int Id { get; set; }
    [Required, MaxLength(50)]
    public string ReturnNumber { get; set; } = string.Empty;
    public int SaleId { get; set; }
    public Sale Sale { get; set; } = null!;
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal NetTotal { get; set; }
    public ReturnFinancialMode FinancialMode { get; set; }
    public int? FinancialTransactionId { get; set; }
    public FinancialTransaction? FinancialTransaction { get; set; }
    public int? StoreId { get; set; }
    public Store? Store { get; set; }
    public DateTime OccurredAtUtc { get; set; }
    public int? UserId { get; set; }
    public User? User { get; set; }
    [MaxLength(500)]
    public string Reason { get; set; } = string.Empty;
    public ICollection<SaleReturnLine> Lines { get; set; } = new List<SaleReturnLine>();
}

public class SaleReturnLine
{
    public int Id { get; set; }
    public int SaleReturnId { get; set; }
    public SaleReturn SaleReturn { get; set; } = null!;
    public int SaleLineId { get; set; }
    public SaleLine SaleLine { get; set; } = null!;
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal CostSnapshot { get; set; }
    public decimal LineTotal { get; set; }
}
