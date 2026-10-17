using System.ComponentModel.DataAnnotations;

namespace HeCashier.Models;

public enum ShiftStatus
{
    Open,
    Closed
}

public class User
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

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
    public DateTime StartedAtUtc { get; set; }
    public DateTime? ClosedAtUtc { get; set; }
    public ShiftStatus Status { get; set; } = ShiftStatus.Open;
}

public class Category
{
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}

public class Unit
{
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}

public class Product
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public decimal SellingPrice { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal Quantity { get; set; }
    public decimal ReorderPoint { get; set; }
    public bool IsActive { get; set; } = true;
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }
    public int? UnitId { get; set; }
    public Unit? Unit { get; set; }
    public ICollection<Barcode> Barcodes { get; set; } = new List<Barcode>();
}

public class Barcode
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Value { get; set; } = string.Empty;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
}

public class InventoryMovement
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public decimal Quantity { get; set; }
    public DateTime OccurredAtUtc { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class Customer
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public string? Phone { get; set; }
}

public class Supplier
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public string? Phone { get; set; }
}

public class Sale
{
    public int Id { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public int? ShiftId { get; set; }
    public Shift? Shift { get; set; }
    public ICollection<SaleLine> Lines { get; set; } = new List<SaleLine>();
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
}

public class Purchase
{
    public int Id { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public int? SupplierId { get; set; }
    public Supplier? Supplier { get; set; }
    public ICollection<PurchaseLine> Lines { get; set; } = new List<PurchaseLine>();
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
}

public class Safe
{
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    public ICollection<FinancialTransaction> Transactions { get; set; } = new List<FinancialTransaction>();
}

public class BankAccount
{
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    public ICollection<FinancialTransaction> Transactions { get; set; } = new List<FinancialTransaction>();
}

public class FinancialTransaction
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime OccurredAtUtc { get; set; }
    public string Description { get; set; } = string.Empty;
    public int? SafeId { get; set; }
    public Safe? Safe { get; set; }
    public int? BankAccountId { get; set; }
    public BankAccount? BankAccount { get; set; }
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
    public decimal Amount { get; set; }
    public DateTime OccurredAtUtc { get; set; }
    public int ExpenseTypeId { get; set; }
    public ExpenseType ExpenseType { get; set; } = null!;
    public string Description { get; set; } = string.Empty;
}

public class AuditEntry
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public string Operation { get; set; } = string.Empty;
    public string? EntityReference { get; set; }
    public DateTime OccurredAtUtc { get; set; }
}
