using HeCashier.Models;
using Microsoft.EntityFrameworkCore;

namespace HeCashier.Data;

public sealed class HeCashierDbContext(DbContextOptions<HeCashierDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>(); public DbSet<Role> Roles => Set<Role>(); public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>(); public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<Shift> Shifts => Set<Shift>(); public DbSet<Category> Categories => Set<Category>(); public DbSet<Unit> Units => Set<Unit>();
    public DbSet<Product> Products => Set<Product>(); public DbSet<Barcode> Barcodes => Set<Barcode>(); public DbSet<InventoryMovement> InventoryMovements => Set<InventoryMovement>();
    public DbSet<Customer> Customers => Set<Customer>(); public DbSet<Supplier> Suppliers => Set<Supplier>(); public DbSet<Sale> Sales => Set<Sale>(); public DbSet<SaleLine> SaleLines => Set<SaleLine>();
    public DbSet<Purchase> Purchases => Set<Purchase>(); public DbSet<PurchaseLine> PurchaseLines => Set<PurchaseLine>(); public DbSet<Safe> Safes => Set<Safe>(); public DbSet<BankAccount> BankAccounts => Set<BankAccount>();
    public DbSet<FinancialTransaction> FinancialTransactions => Set<FinancialTransaction>(); public DbSet<ExpenseType> ExpenseTypes => Set<ExpenseType>(); public DbSet<Expense> Expenses => Set<Expense>(); public DbSet<AuditEntry> AuditEntries => Set<AuditEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserRole>().HasKey(x => new { x.UserId, x.RoleId });
        modelBuilder.Entity<RolePermission>().HasKey(x => new { x.RoleId, x.PermissionId });
        modelBuilder.Entity<UserRole>().HasOne(x => x.User).WithMany(x => x.UserRoles).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<UserRole>().HasOne(x => x.Role).WithMany(x => x.UserRoles).HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Cascade);
        foreach (var entity in new[] { typeof(SaleLine), typeof(PurchaseLine), typeof(FinancialTransaction), typeof(Expense) })
            foreach (var property in entity.GetProperties().Where(p => p.PropertyType == typeof(decimal)))
                modelBuilder.Entity(entity).Property(property.Name).HasPrecision(18, 4);
    }
}
