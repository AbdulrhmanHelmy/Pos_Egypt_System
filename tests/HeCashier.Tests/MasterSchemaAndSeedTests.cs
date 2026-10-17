using HeCashier.Data;
using HeCashier.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Xunit;

namespace HeCashier.Tests;

public sealed class MasterSchemaAndSeedTests
{
    [Fact]
    public async Task Initializer_seeds_canonical_permissions_settings_and_master_records()
    {
        await using var testDatabase = await TemporaryDatabase.CreateAsync();
        await using var db = await testDatabase.Initializer.Factory.CreateDbContextAsync();

        var admin = await db.Roles.SingleAsync(x => x.Name == "Admin");
        var permissions = await db.Permissions.Select(x => x.Code).ToListAsync();
        var adminGrants = await db.RolePermissions.CountAsync(x => x.RoleId == admin.Id);
        Assert.Equal(DatabaseInitializer.CanonicalPermissionCodes.Count, permissions.Count);
        Assert.All(DatabaseInitializer.CanonicalPermissionCodes, code => Assert.Contains(code, permissions));
        Assert.Equal(DatabaseInitializer.CanonicalPermissionCodes.Count, adminGrants);

        var settings = await db.SystemSettings.ToDictionaryAsync(x => x.Key, x => x.Value);
        Assert.All(DatabaseInitializer.DefaultSettingKeys, key => Assert.Contains(key, settings.Keys, StringComparer.OrdinalIgnoreCase));
        Assert.Equal("false", settings["AllowNegativeStock"]);
        Assert.Equal("true", settings["WarnOnSaleBelowCost"]);
        Assert.Equal("", settings["InvoicePrinterName"]);

        Assert.True(await db.Stores.AnyAsync(x => x.Name == "المخزن الرئيسي" && x.IsDefault));
        Assert.True(await db.Safes.AnyAsync(x => x.Name == "الخزينة الرئيسية"));
        Assert.True(await db.Categories.AnyAsync(x => x.Name == "عام"));
        Assert.True(await db.Units.AnyAsync(x => x.Name == "قطعة"));
        Assert.True(await db.Customers.AnyAsync(x => x.Name == "عميل نقدي" && x.IsCashCustomer));
        Assert.True(await db.Suppliers.AnyAsync(x => x.Name == "مورد نقدي"));
        Assert.Contains("إيجار", await db.ExpenseTypes.Select(x => x.Name).ToListAsync());

        var template = await db.BarcodeLabelTemplates.Include(x => x.Elements).SingleAsync(x => x.IsDefault);
        Assert.Equal("Default", template.Name);
        Assert.Equal(50m, template.Width);
        Assert.Equal(30m, template.Height);
        Assert.Contains(template.Elements, x => x.FieldKey == "Barcode" && x.IsVisible);
    }

    [Fact]
    public async Task Initializer_is_idempotent_and_does_not_overwrite_saved_settings()
    {
        await using var testDatabase = await TemporaryDatabase.CreateAsync();
        await using (var db = await testDatabase.Initializer.Factory.CreateDbContextAsync())
        {
            var setting = await db.SystemSettings.SingleAsync(x => x.Key == "StoreName");
            setting.Value = "Existing shop";
            await db.SaveChangesAsync();
        }

        var firstSnapshot = await GetSeedCountsAsync(testDatabase.Initializer.Factory);
        await testDatabase.Initializer.InitializeAsync(seedDevelopmentUser: false);
        var secondSnapshot = await GetSeedCountsAsync(testDatabase.Initializer.Factory);

        Assert.Equal(firstSnapshot, secondSnapshot);
        await using var verifyDb = await testDatabase.Initializer.Factory.CreateDbContextAsync();
        Assert.Equal("Existing shop", await verifyDb.SystemSettings.Where(x => x.Key == "StoreName").Select(x => x.Value).SingleAsync());
    }

    [Fact]
    public async Task Schema_contains_phase_15_tables_and_enforces_unique_keys()
    {
        await using var testDatabase = await TemporaryDatabase.CreateAsync();
        await using var db = await testDatabase.Initializer.Factory.CreateDbContextAsync();
        var expectedTables = new[]
        {
            "Sales", "SaleLines", "Purchases", "PurchaseLines", "SaleReturns", "SaleReturnLines",
            "PurchaseReturns", "PurchaseReturnLines", "InstallmentPlans", "InstallmentContracts",
            "InstallmentSchedules", "InstallmentPayments", "Safes", "FinancialTransactions",
            "Employees", "EmployeeAttendances", "EmployeeFinancialTransactions", "SalaryPayments",
            "Notifications", "ProductStoreStocks", "InventoryMovements", "ProductBatches",
            "BarcodeLabelTemplates", "BarcodeLabelElements", "BillsOfMaterials", "BillOfMaterialsLines"
        };
        var actualTables = db.Model.GetEntityTypes()
            .Select(entity => entity.GetTableName())
            .Where(name => name is not null)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        Assert.All(expectedTables, table => Assert.Contains(table, actualTables));
        await db.Database.OpenConnectionAsync();
        await using (var command = db.Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = "SELECT name FROM sqlite_master WHERE type = 'table'";
            await using var reader = await command.ExecuteReaderAsync();
            var createdTables = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            while (await reader.ReadAsync())
                createdTables.Add(reader.GetString(0));
            Assert.All(expectedTables, table => Assert.Contains(table, createdTables));
        }

        AssertUniqueIndex<User>(db, nameof(User.UserName));
        AssertUniqueIndex<Permission>(db, nameof(Permission.Code));
        AssertUniqueIndex<SystemSetting>(db, nameof(SystemSetting.Key));
        AssertUniqueIndex<ProductBarcode>(db, nameof(ProductBarcode.Value));
        AssertUniqueIndex<Sale>(db, nameof(Sale.InvoiceNumber));
        AssertUniqueIndex<Purchase>(db, nameof(Purchase.InvoiceNumber));

        db.Users.Add(new User { UserName = "unique-test", PasswordHash = "hash" });
        await db.SaveChangesAsync();
        db.Users.Add(new User { UserName = "unique-test", PasswordHash = "hash" });
        await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
        db.ChangeTracker.Clear();

        db.Sales.Add(new Sale { InvoiceNumber = "UNIQUE-SALE" });
        await db.SaveChangesAsync();
        db.Sales.Add(new Sale { InvoiceNumber = "UNIQUE-SALE" });
        await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
    }

    [Fact]
    public async Task Phase_15_transactions_persist_and_history_foreign_keys_restrict_deletion()
    {
        await using var testDatabase = await TemporaryDatabase.CreateAsync();
        await using var db = await testDatabase.Initializer.Factory.CreateDbContextAsync();
        var store = await db.Stores.SingleAsync(x => x.Name == "المخزن الرئيسي");
        var safe = await db.Safes.SingleAsync(x => x.Name == "الخزينة الرئيسية");
        var customer = await db.Customers.SingleAsync(x => x.Name == "عميل نقدي");
        var supplier = await db.Suppliers.SingleAsync(x => x.Name == "مورد نقدي");
        var category = await db.Categories.SingleAsync(x => x.Name == "عام");
        var unit = await db.Units.SingleAsync(x => x.Name == "قطعة");
        var user = new User { UserName = "schema-user", PasswordHash = "hash" };
        var product = new Product { Code = "SCHEMA-PRODUCT", Name = "Schema Product", Category = category, Unit = unit };
        var employee = new Employee { Name = "Schema Employee", MonthlySalary = 1000m };
        db.AddRange(user, product, employee);
        await db.SaveChangesAsync();

        var saleLine = new SaleLine { Product = product, Store = store, Quantity = 2m, UnitPrice = 10m, CostSnapshot = 6m, LineTotal = 20m };
        var sale = new Sale
        {
            InvoiceNumber = "SCHEMA-SALE",
            CreatedAtUtc = DateTime.UtcNow,
            Customer = customer,
            Store = store,
            User = user,
            PaymentType = PaymentType.Cash,
            Subtotal = 20m,
            NetTotal = 20m,
            PaidAmount = 20m,
            Lines = [saleLine]
        };
        var purchaseLine = new PurchaseLine { Product = product, Store = store, Quantity = 5m, UnitPrice = 5m, CostSnapshot = 5m, LineTotal = 25m };
        var purchase = new Purchase
        {
            InvoiceNumber = "SCHEMA-PURCHASE",
            CreatedAtUtc = DateTime.UtcNow,
            Supplier = supplier,
            Store = store,
            User = user,
            PaymentType = PaymentType.Credit,
            Subtotal = 25m,
            NetTotal = 25m,
            RemainingAmount = 25m,
            Lines = [purchaseLine]
        };
        db.AddRange(sale, purchase);
        await db.SaveChangesAsync();

        db.SaleReturns.Add(new SaleReturn
        {
            ReturnNumber = "SCHEMA-SALE-RETURN",
            Sale = sale,
            Store = store,
            User = user,
            OccurredAtUtc = DateTime.UtcNow,
            Subtotal = 10m,
            NetTotal = 10m,
            FinancialMode = ReturnFinancialMode.CustomerCredit,
            Lines = [new SaleReturnLine { SaleLine = saleLine, Product = product, Quantity = 1m, UnitPrice = 10m, CostSnapshot = 6m, LineTotal = 10m }]
        });
        db.PurchaseReturns.Add(new PurchaseReturn
        {
            ReturnNumber = "SCHEMA-PURCHASE-RETURN",
            Purchase = purchase,
            User = user,
            OccurredAtUtc = DateTime.UtcNow,
            Subtotal = 5m,
            NetTotal = 5m,
            FinancialMode = ReturnFinancialMode.SupplierCredit,
            Lines = [new PurchaseReturnLine { PurchaseLine = purchaseLine, Quantity = 1m, UnitPrice = 5m, CostSnapshot = 5m, LineTotal = 5m }]
        });
        var schedule = new InstallmentSchedule
        {
            SequenceNumber = 1,
            DueAtUtc = DateTime.UtcNow.AddMonths(1),
            AmountDue = 5m
        };
        var contract = new InstallmentContract
        {
            ContractNumber = "SCHEMA-CONTRACT",
            Customer = customer,
            Sale = sale,
            User = user,
            CreatedAtUtc = DateTime.UtcNow,
            ProductTotal = 20m,
            DownPayment = 5m,
            ContractTotal = 20m,
            InstallmentCount = 3,
            InstallmentAmount = 5m,
            PaidAmount = 5m,
            RemainingAmount = 15m,
            Schedule = [schedule]
        };
        db.InstallmentContracts.Add(contract);
        db.InstallmentPayments.Add(new InstallmentPayment
        {
            InstallmentSchedule = schedule,
            Amount = 1m,
            PaidAtUtc = DateTime.UtcNow,
            PaymentType = PaymentType.Cash,
            User = user
        });
        db.FinancialTransactions.Add(new FinancialTransaction
        {
            Type = FinancialTransactionType.Deposit,
            TransactionType = "Deposit",
            Amount = 20m,
            BalanceBefore = 0m,
            BalanceAfter = 20m,
            OccurredAtUtc = DateTime.UtcNow,
            Safe = safe,
            User = user,
            DepositorOrBeneficiaryName = "Schema User"
        });
        db.EmployeeAttendances.Add(new EmployeeAttendance { Employee = employee, CheckInAtUtc = DateTime.UtcNow, BreakHours = 1m });
        db.SalaryPayments.Add(new SalaryPayment { Employee = employee, Safe = safe, GrossAmount = 1000m, NetAmount = 1000m, PaidAtUtc = DateTime.UtcNow, User = user });
        await db.SaveChangesAsync();

        Assert.Equal(1, await db.Sales.CountAsync(x => x.InvoiceNumber == "SCHEMA-SALE"));
        Assert.Equal(1, await db.Purchases.CountAsync(x => x.InvoiceNumber == "SCHEMA-PURCHASE"));
        Assert.Equal(1, await db.SaleReturns.CountAsync());
        Assert.Equal(1, await db.PurchaseReturns.CountAsync());
        Assert.Equal(1, await db.InstallmentSchedules.CountAsync());
        Assert.Equal(1, await db.InstallmentContracts.CountAsync());
        Assert.Equal(1, await db.InstallmentPayments.CountAsync());
        Assert.Equal(1, await db.FinancialTransactions.CountAsync());
        Assert.Equal(1, await db.EmployeeAttendances.CountAsync());
        Assert.Equal(1, await db.SalaryPayments.CountAsync());

        db.ChangeTracker.Clear();
        var saleToDelete = await db.Sales.SingleAsync(x => x.InvoiceNumber == "SCHEMA-SALE");
        db.Sales.Remove(saleToDelete);
        await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
        db.ChangeTracker.Clear();

        var purchaseToDelete = await db.Purchases.SingleAsync(x => x.InvoiceNumber == "SCHEMA-PURCHASE");
        db.Purchases.Remove(purchaseToDelete);
        await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
        db.ChangeTracker.Clear();

        var contractToDelete = await db.InstallmentContracts.SingleAsync(x => x.ContractNumber == "SCHEMA-CONTRACT");
        db.InstallmentContracts.Remove(contractToDelete);
        await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
        db.ChangeTracker.Clear();

        var returnToDelete = await db.SaleReturns.SingleAsync(x => x.ReturnNumber == "SCHEMA-SALE-RETURN");
        db.SaleReturns.Remove(returnToDelete);
        await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
        db.ChangeTracker.Clear();

        var purchaseReturnToDelete = await db.PurchaseReturns.SingleAsync(x => x.ReturnNumber == "SCHEMA-PURCHASE-RETURN");
        db.PurchaseReturns.Remove(purchaseReturnToDelete);
        await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
        db.ChangeTracker.Clear();

        var safeToDelete = await db.Safes.SingleAsync(x => x.Name == "الخزينة الرئيسية");
        db.Safes.Remove(safeToDelete);
        await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
        db.ChangeTracker.Clear();

        var employeeToDelete = await db.Employees.SingleAsync(x => x.Name == "Schema Employee");
        db.Employees.Remove(employeeToDelete);
        await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
    }

    private static void AssertUniqueIndex<TEntity>(HeCashierDbContext db, string propertyName) where TEntity : class
    {
        var entity = db.Model.FindEntityType(typeof(TEntity));
        Assert.NotNull(entity);
        Assert.Contains(entity!.GetIndexes(), index => index.IsUnique && index.Properties.Any(property => property.Name == propertyName));
    }

    private static async Task<SeedCounts> GetSeedCountsAsync(IDbContextFactory<HeCashierDbContext> factory)
    {
        await using var db = await factory.CreateDbContextAsync();
        return new SeedCounts(
            await db.Permissions.CountAsync(),
            await db.RolePermissions.CountAsync(),
            await db.SystemSettings.CountAsync(),
            await db.Roles.CountAsync(),
            await db.Stores.CountAsync(),
            await db.Safes.CountAsync(),
            await db.Categories.CountAsync(),
            await db.Units.CountAsync(),
            await db.Customers.CountAsync(),
            await db.Suppliers.CountAsync(),
            await db.ExpenseTypes.CountAsync(),
            await db.BarcodeLabelTemplates.CountAsync(),
            await db.BarcodeLabelElements.CountAsync());
    }

    private sealed record SeedCounts(
        int Permissions,
        int RolePermissions,
        int Settings,
        int Roles,
        int Stores,
        int Safes,
        int Categories,
        int Units,
        int Customers,
        int Suppliers,
        int ExpenseTypes,
        int BarcodeTemplates,
        int BarcodeElements);

    private sealed class TemporaryDatabase : IAsyncDisposable
    {
        private TemporaryDatabase(string root, DatabaseInitializer initializer)
        {
            Root = root;
            Initializer = initializer;
        }

        private string Root { get; }
        public DatabaseInitializer Initializer { get; }

        public static async Task<TemporaryDatabase> CreateAsync()
        {
            var root = Path.Combine(Path.GetTempPath(), $"HeCashierSchemaTests-{Guid.NewGuid():N}");
            var initializer = new DatabaseInitializer(null, null, root);
            await initializer.InitializeAsync(seedDevelopmentUser: false);
            return new TemporaryDatabase(root, initializer);
        }

        public ValueTask DisposeAsync()
        {
            Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
            if (Directory.Exists(Root))
                Directory.Delete(Root, recursive: true);
            return ValueTask.CompletedTask;
        }
    }
}
