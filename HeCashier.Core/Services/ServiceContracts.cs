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
        Task<Shift> StartShiftAsync(int userId, decimal openingCash = 0m, int? safeId = null);
        Task<ShiftSummary> GetShiftSummaryAsync(int shiftId);
        Task<Shift> CloseShiftAsync(int shiftId, decimal actualCash, string? notes = null, int? userId = null);
    }

    public sealed record ShiftSummary(
        int ShiftId,
        decimal OpeningCash,
        decimal CashSales,
        decimal CashReturns,
        decimal CustomerCashCollections,
        decimal SupplierCashPayments,
        decimal ShiftExpenses,
        decimal ExpectedCash);

    public sealed record SafeDraft(string Name, decimal OpeningBalance = 0m, bool IsActive = true);

    public sealed record BankAccountDraft(
        string Name,
        string? AccountNumber = null,
        string? BankName = null,
        decimal OpeningBalance = 0m,
        bool IsActive = true);

    public sealed record FinancialTransactionFilter(
        DateTime? FromUtc = null,
        DateTime? ToUtc = null,
        int? SafeId = null,
        int? BankAccountId = null,
        FinancialTransactionType? Type = null);

    public sealed record ExpenseTypeDraft(string Name);

    public sealed record ExpenseDraft(
        int ExpenseTypeId,
        int SafeId,
        decimal Amount,
        string Description,
        string? Notes = null,
        int? ShiftId = null,
        DateTime? OccurredAtUtc = null,
        string? VoucherNumber = null);

    public sealed record CustomerDraft(
        string Name,
        string? CompanyName = null,
        string? Phone = null,
        string? SecondaryPhone = null,
        string? Address = null,
        string? NationalId = null,
        decimal OpeningBalance = 0m,
        decimal? CreditLimit = null,
        bool IsCashCustomer = false,
        bool IsActive = true,
        string? Notes = null);

    public sealed record SupplierDraft(
        string Name,
        string? CompanyName = null,
        string? Phone = null,
        string? SecondaryPhone = null,
        string? Address = null,
        decimal OpeningBalance = 0m,
        decimal? CreditLimit = null,
        bool IsActive = true,
        string? Notes = null);

    public sealed record AccountStatementEntry(
        DateTime OccurredAtUtc,
        PartyTransactionType Type,
        decimal Amount,
        decimal BalanceBefore,
        decimal BalanceAfter,
        string? ReferenceNumber,
        string Notes);

    public sealed record ExternalDebtDraft(
        int CustomerId,
        string Description,
        decimal Amount,
        DateTime? DueAtUtc = null,
        string? Notes = null);

    public sealed record CustomerCheckDraft(
        int CustomerId,
        string CheckNumber,
        decimal Amount,
        DateTime IssuedAtUtc,
        DateTime? DueAtUtc = null,
        string? BankName = null,
        bool IsPromissoryNote = false,
        string? Notes = null);

    public interface ITreasuryService
    {
        Task<IReadOnlyList<Safe>> GetSafesAsync(bool includeInactive = false);
        Task<Safe> CreateSafeAsync(SafeDraft draft, int userId);
        Task<Safe> UpdateSafeAsync(int safeId, SafeDraft draft, int userId);
        Task DeleteSafeAsync(int safeId, int userId);
        Task<IReadOnlyList<BankAccount>> GetBankAccountsAsync(bool includeInactive = false);
        Task<BankAccount> CreateBankAccountAsync(BankAccountDraft draft, int userId);
        Task<BankAccount> UpdateBankAccountAsync(int bankAccountId, BankAccountDraft draft, int userId);
        Task DeleteBankAccountAsync(int bankAccountId, int userId);
        Task<FinancialTransaction> DepositToSafeAsync(int safeId, decimal amount, string depositorName, int userId, int? shiftId = null, string? description = null);
        Task<FinancialTransaction> WithdrawFromSafeAsync(int safeId, decimal amount, string beneficiaryName, int userId, int? shiftId = null, string? description = null);
        Task<FinancialTransaction> DepositToBankAsync(int bankAccountId, decimal amount, string depositorName, int userId, int? shiftId = null, string? description = null);
        Task<FinancialTransaction> WithdrawFromBankAsync(int bankAccountId, decimal amount, string beneficiaryName, int userId, int? shiftId = null, string? description = null);
        Task<IReadOnlyList<FinancialTransaction>> TransferBetweenSafeAndBankAsync(int safeId, int bankAccountId, decimal amount, bool safeToBank, string beneficiaryName, int userId, int? shiftId = null, string? description = null);
        Task<IReadOnlyList<FinancialTransaction>> GetFinancialTransactionsAsync(FinancialTransactionFilter filter);
    }

    public interface IExpenseService
    {
        Task<IReadOnlyList<ExpenseType>> GetExpenseTypesAsync();
        Task<ExpenseType> CreateExpenseTypeAsync(ExpenseTypeDraft draft, int userId);
        Task<ExpenseType> UpdateExpenseTypeAsync(int expenseTypeId, ExpenseTypeDraft draft, int userId);
        Task DeleteExpenseTypeAsync(int expenseTypeId, int userId);
        Task<Expense> RecordExpenseAsync(ExpenseDraft draft, int userId);
        Task<IReadOnlyList<Expense>> GetExpensesAsync(DateTime? fromUtc = null, DateTime? toUtc = null, int? expenseTypeId = null, int? shiftId = null, int? userId = null);
    }

    public interface ICustomerSupplierService
    {
        Task<IReadOnlyList<Customer>> SearchCustomersAsync(string? searchText = null, bool includeInactive = false);
        Task<Customer> CreateCustomerAsync(CustomerDraft draft, int userId);
        Task<Customer> UpdateCustomerAsync(int customerId, CustomerDraft draft, int userId);
        Task DeleteCustomerAsync(int customerId, int userId);
        Task<IReadOnlyList<Supplier>> SearchSuppliersAsync(string? searchText = null, bool includeInactive = false);
        Task<Supplier> CreateSupplierAsync(SupplierDraft draft, int userId);
        Task<Supplier> UpdateSupplierAsync(int supplierId, SupplierDraft draft, int userId);
        Task DeleteSupplierAsync(int supplierId, int userId);
        Task<CustomerTransaction> CollectFromCustomerAsync(int customerId, decimal amount, int? safeId, int? bankAccountId, string depositorName, int userId, int? shiftId = null, string? notes = null);
        Task<SupplierTransaction> PaySupplierAsync(int supplierId, decimal amount, int? safeId, int? bankAccountId, string beneficiaryName, int userId, int? shiftId = null, string? notes = null);
        Task<IReadOnlyList<AccountStatementEntry>> GetCustomerStatementAsync(int customerId, DateTime? fromUtc = null, DateTime? toUtc = null);
        Task<IReadOnlyList<AccountStatementEntry>> GetSupplierStatementAsync(int supplierId, DateTime? fromUtc = null, DateTime? toUtc = null);
        Task<IReadOnlyList<ExternalDebt>> GetExternalDebtsAsync(int? customerId = null, bool? isSettled = null);
        Task<ExternalDebt> RecordExternalDebtAsync(ExternalDebtDraft draft, int userId);
        Task<CustomerTransaction> CollectExternalDebtAsync(int externalDebtId, decimal amount, int? safeId, int? bankAccountId, string depositorName, int userId, int? shiftId = null, string? notes = null);
        Task<IReadOnlyList<CustomerCheck>> GetCustomerChecksAsync(int? customerId = null, CustomerCheckStatus? status = null);
        Task<CustomerCheck> CreateCustomerCheckAsync(CustomerCheckDraft draft, int userId);
        Task<CustomerCheck> CollectCustomerCheckAsync(int checkId, int? safeId, int? bankAccountId, int userId, int? shiftId = null);
        Task<CustomerCheck> RejectCustomerCheckAsync(int checkId, string reason, int userId);
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
