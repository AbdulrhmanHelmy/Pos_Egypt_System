using System.ComponentModel.DataAnnotations;

namespace HeCashier.Models;

public class ProductGroup
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Code { get; set; } = string.Empty;

    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
    public ICollection<Product> Products { get; set; } = new List<Product>();
}

public class ProductBatch
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public int? StoreId { get; set; }
    public Store? Store { get; set; }
    [MaxLength(100)]
    public string? BatchNumber { get; set; }
    public DateTime? ProductionDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public decimal QuantityReceived { get; set; }
    public decimal QuantityRemaining { get; set; }
    public decimal UnitCost { get; set; }
    public DateTime ReceivedAtUtc { get; set; }
}

public class CustomerTransaction
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public PartyTransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public DateTime OccurredAtUtc { get; set; }
    [MaxLength(50)]
    public string? ReferenceNumber { get; set; }
    public int? SaleId { get; set; }
    public Sale? Sale { get; set; }
    public int? ExternalDebtId { get; set; }
    public ExternalDebt? ExternalDebt { get; set; }
    public int? FinancialTransactionId { get; set; }
    public FinancialTransaction? FinancialTransaction { get; set; }
    public int? UserId { get; set; }
    public User? User { get; set; }
    [MaxLength(1000)]
    public string Notes { get; set; } = string.Empty;
}

public class SupplierTransaction
{
    public int Id { get; set; }
    public int SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;
    public PartyTransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public DateTime OccurredAtUtc { get; set; }
    [MaxLength(50)]
    public string? ReferenceNumber { get; set; }
    public int? PurchaseId { get; set; }
    public Purchase? Purchase { get; set; }
    public int? FinancialTransactionId { get; set; }
    public FinancialTransaction? FinancialTransaction { get; set; }
    public int? UserId { get; set; }
    public User? User { get; set; }
    [MaxLength(1000)]
    public string Notes { get; set; } = string.Empty;
}

public class ExternalDebt
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    [Required, MaxLength(200)]
    public string Description { get; set; } = string.Empty;
    public decimal OriginalAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public DateTime OccurredAtUtc { get; set; }
    public DateTime? DueAtUtc { get; set; }
    public int? UserId { get; set; }
    public User? User { get; set; }
    public bool IsSettled { get; set; }
    [MaxLength(1000)]
    public string Notes { get; set; } = string.Empty;
    public ICollection<CustomerTransaction> Transactions { get; set; } = new List<CustomerTransaction>();
}

public class CustomerCheck
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    [Required, MaxLength(100)]
    public string CheckNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime IssuedAtUtc { get; set; }
    public DateTime? DueAtUtc { get; set; }
    public DateTime? ClearedAtUtc { get; set; }
    [MaxLength(150)]
    public string? BankName { get; set; }
    public bool IsPromissoryNote { get; set; }
    [MaxLength(1000)]
    public string Notes { get; set; } = string.Empty;
}

public class InstallmentPlan
{
    public int Id { get; set; }
    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;
    public int InstallmentCount { get; set; }
    public int IntervalMonths { get; set; } = 1;
    public decimal InterestOrMarkupRate { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<InstallmentContract> Contracts { get; set; } = new List<InstallmentContract>();
}

public class InstallmentContract
{
    public int Id { get; set; }
    [Required, MaxLength(50)]
    public string ContractNumber { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public int? SaleId { get; set; }
    public Sale? Sale { get; set; }
    public int? InstallmentPlanId { get; set; }
    public InstallmentPlan? InstallmentPlan { get; set; }
    public int? UserId { get; set; }
    public User? User { get; set; }
    [MaxLength(50)]
    public string? NationalId { get; set; }
    [MaxLength(200)]
    public string? GuarantorName { get; set; }
    [MaxLength(50)]
    public string? GuarantorNationalId { get; set; }
    [MaxLength(50)]
    public string? GuarantorPhone { get; set; }
    [MaxLength(500)]
    public string? GuarantorAddress { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public decimal ProductTotal { get; set; }
    public decimal DownPayment { get; set; }
    public decimal InterestOrMarkupAmount { get; set; }
    public decimal ContractTotal { get; set; }
    public decimal InstallmentAmount { get; set; }
    public int InstallmentCount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public InstallmentStatus Status { get; set; } = InstallmentStatus.Pending;
    [MaxLength(1000)]
    public string Notes { get; set; } = string.Empty;
    public ICollection<InstallmentSchedule> Schedule { get; set; } = new List<InstallmentSchedule>();
}

public class InstallmentSchedule
{
    public int Id { get; set; }
    public int InstallmentContractId { get; set; }
    public InstallmentContract InstallmentContract { get; set; } = null!;
    public int SequenceNumber { get; set; }
    public DateTime DueAtUtc { get; set; }
    public decimal AmountDue { get; set; }
    public decimal AmountPaid { get; set; }
    public InstallmentStatus Status { get; set; } = InstallmentStatus.Pending;
    public ICollection<InstallmentPayment> Payments { get; set; } = new List<InstallmentPayment>();
}

public class InstallmentPayment
{
    public int Id { get; set; }
    public int InstallmentScheduleId { get; set; }
    public InstallmentSchedule InstallmentSchedule { get; set; } = null!;
    public decimal Amount { get; set; }
    public DateTime PaidAtUtc { get; set; }
    public PaymentType PaymentType { get; set; }
    public int? FinancialTransactionId { get; set; }
    public FinancialTransaction? FinancialTransaction { get; set; }
    public int? UserId { get; set; }
    public User? User { get; set; }
    [MaxLength(500)]
    public string Notes { get; set; } = string.Empty;
}

public class PurchaseReturn
{
    public int Id { get; set; }
    [Required, MaxLength(50)]
    public string ReturnNumber { get; set; } = string.Empty;
    public int PurchaseId { get; set; }
    public Purchase Purchase { get; set; } = null!;
    public DateTime OccurredAtUtc { get; set; }
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal NetTotal { get; set; }
    public ReturnFinancialMode FinancialMode { get; set; }
    public int? UserId { get; set; }
    public User? User { get; set; }
    [MaxLength(500)]
    public string Reason { get; set; } = string.Empty;
    public ICollection<PurchaseReturnLine> Lines { get; set; } = new List<PurchaseReturnLine>();
}

public class PurchaseReturnLine
{
    public int Id { get; set; }
    public int PurchaseReturnId { get; set; }
    public PurchaseReturn PurchaseReturn { get; set; } = null!;
    public int PurchaseLineId { get; set; }
    public PurchaseLine PurchaseLine { get; set; } = null!;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal CostSnapshot { get; set; }
    public decimal LineTotal { get; set; }
}

public class Employee
{
    public int Id { get; set; }
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    [MaxLength(150)]
    public string? JobTitle { get; set; }
    public decimal MonthlySalary { get; set; }
    [MaxLength(50)]
    public string? NationalId { get; set; }
    [MaxLength(50)]
    public string? Phone { get; set; }
    [MaxLength(500)]
    public string? Address { get; set; }
    public bool IsActive { get; set; } = true;
    public int? UserId { get; set; }
    public User? User { get; set; }
    public ICollection<EmployeeAttendance> Attendances { get; set; } = new List<EmployeeAttendance>();
    public ICollection<EmployeeFinancialTransaction> FinancialTransactions { get; set; } = new List<EmployeeFinancialTransaction>();
    public ICollection<SalaryPayment> SalaryPayments { get; set; } = new List<SalaryPayment>();
}

public class EmployeeAttendance
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;
    public DateTime CheckInAtUtc { get; set; }
    public DateTime? BreakStartedAtUtc { get; set; }
    public DateTime? BreakEndedAtUtc { get; set; }
    public decimal BreakHours { get; set; }
    public DateTime? CheckOutAtUtc { get; set; }
    public decimal? HoursWorked { get; set; }
    [MaxLength(1000)]
    public string Notes { get; set; } = string.Empty;
}

public class EmployeeFinancialTransaction
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;
    public EmployeeFinancialTransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public DateTime OccurredAtUtc { get; set; }
    public int? SafeId { get; set; }
    public Safe? Safe { get; set; }
    public int? ShiftId { get; set; }
    public Shift? Shift { get; set; }
    public int? UserId { get; set; }
    public User? User { get; set; }
    [MaxLength(1000)]
    public string Notes { get; set; } = string.Empty;
}

public class SalaryPayment
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;
    public decimal GrossAmount { get; set; }
    public decimal AdvancesDeducted { get; set; }
    public decimal Deductions { get; set; }
    public decimal Bonuses { get; set; }
    public decimal NetAmount { get; set; }
    public DateTime PaidAtUtc { get; set; }
    public int? SafeId { get; set; }
    public Safe? Safe { get; set; }
    public int? FinancialTransactionId { get; set; }
    public FinancialTransaction? FinancialTransaction { get; set; }
    public int? UserId { get; set; }
    public User? User { get; set; }
    [MaxLength(500)]
    public string Notes { get; set; } = string.Empty;
}

public class Notification
{
    public int Id { get; set; }
    public NotificationType Type { get; set; }
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    [MaxLength(1000)]
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? DismissedAtUtc { get; set; }
    public int? UserId { get; set; }
    public User? User { get; set; }
    public int? ProductId { get; set; }
    public Product? Product { get; set; }
    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public int? SupplierId { get; set; }
    public Supplier? Supplier { get; set; }
    public int? InstallmentScheduleId { get; set; }
    public InstallmentSchedule? InstallmentSchedule { get; set; }
    public bool IsRead { get; set; }
}

public class BarcodeLabelTemplate
{
    public int Id { get; set; }
    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;
    public decimal Width { get; set; }
    public decimal Height { get; set; }
    public string? PrinterName { get; set; }
    public bool IsDefault { get; set; }
    public ICollection<BarcodeLabelElement> Elements { get; set; } = new List<BarcodeLabelElement>();
}

public class BarcodeLabelElement
{
    public int Id { get; set; }
    public int BarcodeLabelTemplateId { get; set; }
    public BarcodeLabelTemplate BarcodeLabelTemplate { get; set; } = null!;
    [Required, MaxLength(100)]
    public string FieldKey { get; set; } = string.Empty;
    public bool IsVisible { get; set; } = true;
    public decimal X { get; set; }
    public decimal Y { get; set; }
    public decimal Width { get; set; }
    public decimal Height { get; set; }
    [MaxLength(100)]
    public string? FontFamily { get; set; }
    public decimal FontSize { get; set; }
    [MaxLength(30)]
    public string? Alignment { get; set; }
    public int SortOrder { get; set; }
}

public class BillOfMaterials
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public decimal OutputQuantity { get; set; } = 1m;
    public bool IsActive { get; set; } = true;
    public ICollection<BillOfMaterialsLine> Lines { get; set; } = new List<BillOfMaterialsLine>();
}

public class BillOfMaterialsLine
{
    public int Id { get; set; }
    public int BillOfMaterialsId { get; set; }
    public BillOfMaterials BillOfMaterials { get; set; } = null!;
    public int RawMaterialProductId { get; set; }
    public Product RawMaterialProduct { get; set; } = null!;
    public decimal Quantity { get; set; }
}
