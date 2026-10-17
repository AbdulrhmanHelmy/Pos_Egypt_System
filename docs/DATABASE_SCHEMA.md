# Database Schema

## Provider and initialization

Infrastructure uses SQLite through Entity Framework Core. `DatabaseInitializer` creates the schema with `EnsureCreated` and performs idempotent startup seeding. Foreign keys use `DeleteBehavior.Restrict`, preventing deletion of referenced master and transaction records.

## Tables by domain

| Domain | Tables |
| --- | --- |
| Security and shifts | `Users`, `Roles`, `Permissions`, `UserRoles`, `RolePermissions`, `Shifts` |
| Product catalog | `Categories`, `Units`, `ProductGroups`, `Products`, `Barcodes` |
| Inventory | `Stores`, `ProductStoreStocks`, `InventoryMovements`, `InventoryTransfers`, `Stocktakes`, `DamagedStock`, `ProductBatches` |
| Sales and purchasing | `Customers`, `CustomerTransactions`, `ExternalDebts`, `CustomerChecks`, `Suppliers`, `SupplierTransactions`, `Sales`, `SaleLines`, `Purchases`, `PurchaseLines`, `SaleReturns`, `SaleReturnLines`, `PurchaseReturns`, `PurchaseReturnLines` |
| Installments | `InstallmentPlans`, `InstallmentContracts`, `InstallmentSchedules`, `InstallmentPayments` |
| Finance and expenses | `Safes`, `BankAccounts`, `FinancialTransactions`, `ExpenseTypes`, `Expenses` |
| Employees | `Employees`, `EmployeeAttendances`, `EmployeeFinancialTransactions`, `SalaryPayments` |
| Application support | `AuditEntries`, `SystemSettings`, `Notifications`, `BarcodeLabelTemplates`, `BarcodeLabelElements` |
| Additional operations | `EWallets`, `EWalletMovements`, `RepairOrders`, `ManufacturingBatches`, `ManufacturingMaterials`, `BillsOfMaterials`, `BillOfMaterialsLines` |

`WarehouseStock` maps to `ProductStoreStocks`, `StocktakeRecord` to `Stocktakes`, and `DamagedStockRecord` to `DamagedStock`. The alternate DbSet properties for stock and stocktakes address those same tables, not duplicates.

## Keys and relationships

- `UserRoles` uses composite primary key `(UserId, RoleId)`; `RolePermissions` uses `(RoleId, PermissionId)`; `ProductStoreStocks` uses `(ProductId, StoreId)`. Other entities use an `Id` primary key.
- Products optionally reference categories, units, and product groups. Barcodes, batches, store stock, and inventory movements reference products; inventory records also reference stores and, where applicable, users. Transfers reference a product and source/destination stores.
- Shifts reference users. Sales and purchases reference their party and may reference a shift, store, and user; their line items reference the invoice, product, and store as applicable. Returns reference the original invoice and their lines reference original invoice lines.
- Customer transactions reference a customer and may reference a sale, external debt, financial transaction, and user. External debts and checks reference a customer. Supplier transactions reference a supplier and may reference a purchase, financial transaction, and user.
- Financial transactions may reference a safe, bank account, shift, and user. Expenses reference an expense type and may reference a safe, bank account, shift, user, and financial transaction.
- Installment contracts reference customers and may reference a sale, plan, and user; schedules belong to contracts and payments reference schedules and financial transactions. Employee attendance and financial/payroll records reference their employee and applicable safe, shift, user, or financial transaction.
- Remaining operational relationships connect notifications to their optional subjects, label elements to templates, e-wallet movements to wallets/users, repairs to customers/users, manufacturing records to products/batches, and audit entries to users.
- All configured foreign keys use restrictive delete behavior.

## Unique indexes

Unique values are enforced for usernames, role names, permission codes, system-setting keys, product codes, barcode values, category names, unit names, product-group codes, store names, safe names, expense-type names, sales and purchase invoice numbers, sales and purchase return numbers, installment contract numbers, expense voucher numbers, e-wallet names, repair order numbers, and barcode-label template names. Composite unique indexes cover `(CustomerId, CheckNumber)`, `(BarcodeLabelTemplateId, FieldKey)`, and `(InstallmentContractId, SequenceNumber)`. Customer and supplier names have non-unique lookup indexes.

SQLite `NOCASE` collation is configured for usernames, role names, permission codes, setting keys, product codes, barcode values, category names, unit names, store names, safe names, expense-type names, and sales/purchase invoice numbers.

## Numeric precision

Decimal properties use precision `(18, 2)`, except quantity and measurement fields which use `(18, 3)`: `Quantity`, `QuantityReceived`, `QuantityRemaining`, `ReorderPoint`, `Weight`, `PackagingQuantity`, `BeforeQuantity`, `AfterQuantity`, `SystemQuantity`, `ActualQuantity`, `Difference`, `BreakHours`, `HoursWorked`, and `OutputQuantity`.

## Startup seed data

Initialization adds missing canonical permissions and grants them to the `Admin` role. It adds missing default system settings without overwriting existing values, and creates the default barcode-label template and elements when absent. It also ensures the general category (`عام`), piece unit (`قطعة`), main store (`المخزن الرئيسي`), main safe (`الخزينة الرئيسية`), cash customer (`عميل نقدي`), cash supplier (`مورد نقدي`), and expense types (`إيجار`, `مرتبات`, `سلفيات`, `خسائر`, `أخرى`) exist. Seeding is idempotent and preserves existing setting values and master records.
