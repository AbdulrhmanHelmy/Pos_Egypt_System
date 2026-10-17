using HeCashier.Security;
using HeCashier.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.IO;

namespace HeCashier.Data;

public sealed class DatabaseInitializer
{
    private static readonly (string Module, string Action)[] PermissionDefinitions =
    [
        ("Dashboard", "View"), ("Shifts", "View"), ("Shifts", "Open"), ("Shifts", "Close"), ("Shifts", "Print"),
        ("Sales", "View"), ("Sales", "Add"), ("Sales", "Edit"), ("Sales", "Delete"), ("Sales", "Print"), ("Sales", "Pay"), ("Sales", "OverrideBelowCost"),
        ("FastPos", "View"), ("FastPos", "Add"), ("FastPos", "Pay"),
        ("Purchases", "View"), ("Purchases", "Add"), ("Purchases", "Edit"), ("Purchases", "Delete"), ("Purchases", "Print"), ("Purchases", "Pay"),
        ("SalesReturns", "View"), ("SalesReturns", "Add"), ("SalesReturns", "Delete"), ("SalesReturns", "Print"),
        ("PurchaseReturns", "View"), ("PurchaseReturns", "Add"), ("PurchaseReturns", "Delete"), ("PurchaseReturns", "Print"),
        ("Products", "View"), ("Products", "Add"), ("Products", "Edit"), ("Products", "Delete"), ("Products", "Export"),
        ("Categories", "View"), ("Categories", "Manage"), ("Units", "View"), ("Units", "Manage"),
        ("Inventory", "View"), ("Inventory", "Adjust"), ("Inventory", "Print"), ("Inventory", "Export"),
        ("Stocktakes", "View"), ("Stocktakes", "Add"), ("StockTransfers", "View"), ("StockTransfers", "Add"), ("DamagedStock", "View"), ("DamagedStock", "Add"),
        ("Customers", "View"), ("Customers", "Add"), ("Customers", "Edit"), ("Customers", "Delete"), ("Customers", "Export"),
        ("CustomerAccounts", "View"), ("CustomerAccounts", "Collect"), ("CustomerAccounts", "Print"),
        ("ExternalDebts", "View"), ("ExternalDebts", "Manage"), ("CustomerChecks", "View"), ("CustomerChecks", "Manage"),
        ("Installments", "View"), ("Installments", "Add"), ("Installments", "Edit"), ("Installments", "Collect"), ("Installments", "Print"),
        ("Suppliers", "View"), ("Suppliers", "Add"), ("Suppliers", "Edit"), ("Suppliers", "Delete"), ("Suppliers", "Export"),
        ("SupplierAccounts", "View"), ("SupplierAccounts", "Pay"), ("SupplierAccounts", "Print"),
        ("Safes", "View"), ("Safes", "Manage"), ("Safes", "Deposit"), ("Safes", "Withdraw"),
        ("Bank", "View"), ("Bank", "Manage"), ("Bank", "Deposit"), ("Bank", "Withdraw"), ("Bank", "Transfer"),
        ("FinancialMovement", "View"), ("FinancialMovement", "Export"),
        ("Expenses", "View"), ("Expenses", "Add"), ("Expenses", "Edit"), ("Expenses", "Delete"), ("Expenses", "Print"),
        ("ExpenseTypes", "Manage"),
        ("Employees", "View"), ("Employees", "Manage"), ("Attendance", "View"), ("Attendance", "Manage"), ("Payroll", "View"), ("Payroll", "Pay"),
        ("Notifications", "View"), ("Notifications", "Dismiss"),
        ("Reports", "View"), ("Reports", "Print"), ("Reports", "Export"), ("Reports", "Design"),
        ("Statistics", "View"), ("BarcodePrinting", "View"), ("BarcodePrinting", "Design"), ("BarcodePrinting", "Print"),
        ("Users", "View"), ("Users", "Manage"), ("Permissions", "Manage"), ("SystemSettings", "View"), ("SystemSettings", "Manage"),
        ("Backup", "Create"), ("Backup", "Restore"),
        ("Repairs", "View"), ("Repairs", "Manage"), ("Manufacturing", "View"), ("Manufacturing", "Manage"),
        ("EWallets", "View"), ("EWallets", "Manage"), ("Audit", "View")
    ];

    private static readonly IReadOnlyDictionary<string, string> DefaultSettings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["StoreName"] = "",
        ["StoreAddress"] = "",
        ["StorePhone"] = "",
        ["StoreMobile"] = "",
        ["StoreTaxNumber"] = "",
        ["StoreLogoPath"] = "",
        ["ReceiptHeader"] = "",
        ["ReceiptFooter"] = "",
        ["InvoicePrinterName"] = "",
        ["A4PrinterName"] = "",
        ["BarcodePrinterName"] = "",
        ["InvoiceCopies"] = "1",
        ["ReceiptPaperSize"] = "80mm",
        ["ReportPaperSize"] = "A4",
        ["InvoicePrintMode"] = "Preview",
        ["EnableVAT"] = "false",
        ["VatRate"] = "0.00",
        ["DeliveryWorkerAmount"] = "0.00",
        ["AllowManualQuantity"] = "false",
        ["AllowBarcodeFreeSales"] = "true",
        ["AllowNegativeStock"] = "false",
        ["WarnOnSaleBelowCost"] = "true",
        ["AllowSaleBelowCost"] = "true",
        ["OpenCashDrawerAfterPurchase"] = "false",
        ["PrintBarcodeAfterPurchase"] = "false",
        ["BackupPath"] = "",
        ["AutomaticBackupPath"] = "",
        ["AutomaticBackupEnabled"] = "false",
        ["AutomaticBackupIntervalHours"] = "24",
        ["BackupOnExit"] = "true",
        ["ThemePreference"] = "Dark",
        ["ThemeAccentColor"] = "Blue",
        ["CustomerAccountCode"] = "",
        ["SupplierAccountCode"] = "",
        ["WhatsAppApiEnabled"] = "false",
        ["WhatsAppApiEndpoint"] = ""
    };

    private static readonly string[] DefaultExpenseTypes = ["إيجار", "مرتبات", "سلفيات", "خسائر", "أخرى"];

    public static IReadOnlyCollection<string> CanonicalPermissionCodes { get; } =
        Array.AsReadOnly(PermissionDefinitions.Select(x => $"{x.Module}.{x.Action}").ToArray());

    public static IReadOnlyCollection<string> DefaultSettingKeys { get; } =
        Array.AsReadOnly(DefaultSettings.Keys.ToArray());

    public string DataRoot { get; }
    public string DatabasePath { get; }
    public string? ConfigurationError { get; }
    public IDbContextFactory<HeCashierDbContext> Factory { get; }

    private readonly IPasswordHasher _passwordHasher;

    public DatabaseInitializer(
        string? environmentRoot,
        string? configuredRoot,
        string localApplicationData,
        IPasswordHasher? passwordHasher = null)
    {
        var resolution = DataPathResolver.Resolve(
            environmentRoot,
            configuredRoot,
            localApplicationData);
        DataRoot = resolution.DataRoot;
        DatabasePath = resolution.DatabasePath;
        ConfigurationError = resolution.Error;
        _passwordHasher = passwordHasher ?? new PasswordHasher();
        var options = new DbContextOptionsBuilder<HeCashierDbContext>().UseSqlite($"Data Source={DatabasePath}").Options;
        Factory = new PooledDbContextFactory<HeCashierDbContext>(options);
    }

    public async Task<bool> InitializeAsync(bool seedDevelopmentUser)
    {
        if (ConfigurationError is not null)
            throw new InvalidOperationException(ConfigurationError);

        Directory.CreateDirectory(DataRoot);
        await using var db = await Factory.CreateDbContextAsync();
        await db.Database.EnsureCreatedAsync();
        await SeedPermissionsAsync(db);
        await SeedSettingsAsync(db);
        await SeedOperationalRecordsAsync(db);

#if DEBUG
        if (!await db.Users.AnyAsync() && seedDevelopmentUser)
        {
            var adminRole = db.Roles.Local.Single(x => x.Name == "Admin");
            var user = new User
            {
                UserName = "devadmin",
                DisplayName = "مدير النظام",
                PasswordHash = _passwordHasher.Hash("HeCashierDev2026!")
            };
            db.Users.Add(user);
            db.UserRoles.Add(new UserRole { User = user, Role = adminRole });
        }
#endif
        await db.SaveChangesAsync();

        return !await db.Users.AnyAsync();
    }

    private static async Task SeedPermissionsAsync(HeCashierDbContext db)
    {
        var adminRole = await db.Roles.SingleOrDefaultAsync(x => x.Name == "Admin");
        if (adminRole is null)
        {
            adminRole = new Role { Name = "Admin" };
            db.Roles.Add(adminRole);
        }

        var permissions = await db.Permissions.ToListAsync();
        var byCode = permissions.ToDictionary(x => x.Code, StringComparer.OrdinalIgnoreCase);
        var existingGrants = adminRole.Id == 0
            ? new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            : (await db.RolePermissions
                .Where(x => x.RoleId == adminRole.Id)
                .Select(x => x.Permission.Code)
                .ToListAsync())
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (var (module, action) in PermissionDefinitions)
        {
            var code = $"{module}.{action}";
            if (!byCode.TryGetValue(code, out var permission))
            {
                permission = new Permission
                {
                    Code = code,
                    Name = $"{module} - {action}",
                    Module = module,
                    Action = action
                };
                db.Permissions.Add(permission);
                byCode.Add(code, permission);
            }

            if (existingGrants.Add(code))
                db.RolePermissions.Add(new RolePermission { Role = adminRole, Permission = permission });
        }

        var devAdmin = await db.Users.SingleOrDefaultAsync(x => x.UserName == "devadmin");
        if (devAdmin is not null && !await db.UserRoles.AnyAsync(x => x.UserId == devAdmin.Id && x.RoleId == adminRole.Id))
            db.UserRoles.Add(new UserRole { User = devAdmin, Role = adminRole });
    }

    private static async Task SeedSettingsAsync(HeCashierDbContext db)
    {
        var existing = await db.SystemSettings.ToListAsync();
        var keys = existing.Select(x => x.Key).ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (var (key, value) in DefaultSettings)
        {
            if (keys.Add(key))
                db.SystemSettings.Add(new SystemSetting { Key = key, Value = value });
        }

        var template = await db.BarcodeLabelTemplates
            .Include(x => x.Elements)
            .SingleOrDefaultAsync(x => x.Name == "Default");
        if (template is null)
        {
            template = new BarcodeLabelTemplate
            {
                Name = "Default",
                Width = 50m,
                Height = 30m,
                IsDefault = true,
                Elements =
                [
                    new BarcodeLabelElement { FieldKey = "ProductName", IsVisible = true, X = 1m, Y = 1m, Width = 48m, Height = 5m, FontFamily = "Segoe UI", FontSize = 9m, Alignment = "Center", SortOrder = 0 },
                    new BarcodeLabelElement { FieldKey = "Barcode", IsVisible = true, X = 2m, Y = 7m, Width = 46m, Height = 13m, FontFamily = "Segoe UI", FontSize = 10m, Alignment = "Center", SortOrder = 1 },
                    new BarcodeLabelElement { FieldKey = "ProductCode", IsVisible = true, X = 1m, Y = 21m, Width = 24m, Height = 5m, FontFamily = "Segoe UI", FontSize = 7m, Alignment = "Left", SortOrder = 2 },
                    new BarcodeLabelElement { FieldKey = "SellingPrice", IsVisible = true, X = 25m, Y = 21m, Width = 24m, Height = 5m, FontFamily = "Segoe UI", FontSize = 8m, Alignment = "Right", SortOrder = 3 }
                ]
            };
            db.BarcodeLabelTemplates.Add(template);
        }
        else if (!template.IsDefault)
        {
            template.IsDefault = true;
        }
    }

    private static async Task SeedOperationalRecordsAsync(HeCashierDbContext db)
    {
        if (!await db.Categories.AnyAsync(x => x.Name == "عام"))
            db.Categories.Add(new Category { Name = "عام" });
        if (!await db.Units.AnyAsync(x => x.Name == "قطعة"))
            db.Units.Add(new Unit { Name = "قطعة" });

        if (!await db.Stores.AnyAsync(x => x.Name == "المخزن الرئيسي"))
            db.Stores.Add(new Store { Name = "المخزن الرئيسي", IsDefault = !await db.Stores.AnyAsync(x => x.IsDefault), IsActive = true });
        if (!await db.Safes.AnyAsync(x => x.Name == "الخزينة الرئيسية"))
            db.Safes.Add(new Safe { Name = "الخزينة الرئيسية", IsActive = true });
        if (!await db.Customers.AnyAsync(x => x.Name == "عميل نقدي"))
            db.Customers.Add(new Customer { Name = "عميل نقدي", IsCashCustomer = true, IsActive = true });
        if (!await db.Suppliers.AnyAsync(x => x.Name == "مورد نقدي"))
            db.Suppliers.Add(new Supplier { Name = "مورد نقدي", IsActive = true });

        var existingExpenseTypes = (await db.ExpenseTypes.Select(x => x.Name).ToListAsync()).ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (var name in DefaultExpenseTypes)
        {
            if (existingExpenseTypes.Add(name))
                db.ExpenseTypes.Add(new ExpenseType { Name = name });
        }
    }
}
