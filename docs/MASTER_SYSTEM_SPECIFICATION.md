# MASTER SYSTEM SPECIFICATION

## 1. System Overview

The reference system is **DoCashier**, an Arabic-language Egyptian desktop POS and
business-management application. It is primarily a dark-themed, right-to-left
(RTL) Windows application with an optional light theme. The application combines
retail/wholesale sales, purchasing, inventory, customer and supplier accounts,
cash/bank operations, employees, reporting, and several specialized workflows.

This document describes the original reference system only. It is a requirements
consolidation, not a redesign or implementation plan. Evidence is classified as:

- **[CONFIRMED]**: directly shown or explicitly documented.
- **[INFERRED]**: strongly suggested but not directly verified.
- **[UNKNOWN]**: insufficient evidence.

The source analyses contain both high-confidence video observations and lower
confidence interpretations. Where they disagree, the disagreement is retained in
section 18.

## 2. Source Documents

| Document | Main topic | Screens/features covered | Key entities/rules | Dependencies |
|---|---|---|---|---|
| `Quick System Overview.md` | Startup and primary POS navigation | Login, dashboard, sales, fast POS, purchases, add product, top-selling report | Authentication, default sale quantity, trial dialog | Sales, purchases, inventory, reports |
| `Good System Explanation.md` | Startup, settings, and navigation | Login, dashboard, settings, menus | RTL/dark theme, settings, global navigation | All modules |
| `Program Interface Explanation.md` | General UI and specialized modules | Dashboard, cash/bank, repair, manufacturing, e-wallet, ledgers, shifts, item movement, report designer | Financial movement, repair, manufacturing cost, wallets | Inventory, treasury, reports |
| `System Sub-sections.md` | Product and secondary modules | Product data, manual price editing, groups, units | Product attributes, activation, bulk editing | Inventory, settings |
| `System Settings.md` | Configuration | Printer, store, invoice, operational settings | Printer and invoice configuration | Sales, purchases, printing |
| `Sales.md` | Standard sales workflow | Shift, sales, customer, payment, credit, printing | Stock, payment, customer debt, loss warning | Inventory, customers, treasury |
| `Customers.md` | Customer accounts | Customer CRUD, credit ledger, statements, external debts | Debt, payment, customer reports | Sales, installments |
| `Installment Sales.md` | Installment sales | Installment sale, schedules, payment, print preview | Down payment, installments, remaining balance | Sales, customers, treasury |
| `Purchases.md` | Supplier purchases | Purchase invoice, supplier selection, payment, reports | Stock receipt, supplier balance | Inventory, suppliers, treasury |
| `Suppliers.md` | Supplier accounts | Supplier CRUD, account ledger, payments, statements | Payables and supplier transactions | Purchases, treasury |
| `Inventory.md` | Stock control | Items, quantities, stock adjustment, movement, expiry | Quantity, reorder, expiry, costing | Products, purchases, sales |
| `Edit Price, Quantity, and Barcode.md` | Bulk item maintenance | Add products, manual price editing, group selection | Price, barcode, quantity, activation | Products, permissions |
| `Returns.md` | Returns | Sales return, item selection, refund/credit | Inventory reversal, customer/cash effect | Sales, inventory, customers, treasury |
| `Expenses.md` | Expenses and statistics | Expense types, voucher, expense reports, statistics | Expense, source safe, totals | Treasury, reports |
| `Bank.md` | Safes and bank | Safe CRUD, deposits, balances | Safe, bank, financial transaction, required depositor | Treasury, reports |
| `Statistics and Profits.md` | Performance reporting | Sales, purchases, expenses, profit statistics | Revenue, cost, profit, date filters | Sales, purchases, expenses |
| `Notifications.md` | Alerts | Low stock, supplier payables, expiry | Alert thresholds and acknowledgement | Inventory, suppliers |
| `Barcode Design and Printing.md` | Barcode printing | Barcode design, labels, print preview | Barcode, product label, printer | Products, settings |
| `User Permissions.md` | Access control | Users, roles, permission assignment | User, role, module/action permissions | All protected modules |
| `Attendance, Departure, and Employees.md` | HR | Employees, attendance, advances, salaries | Employee, attendance, break, salary, advance | Users, treasury, reports |

All 20 Markdown files in `docs` were read for this consolidation. The first 19
documents above are the explicitly enumerated source list; the employee/attendance
document is also included because it is present in the source directory and covers
additional system behavior.

## 3. Complete Module Inventory

1. Authentication, licensing, and session/shift startup.
2. Dashboard and global navigation.
3. Sales: standard POS, fast/touch POS, cash, credit, delivery, discounts, and printing.
4. Purchases and supplier invoices.
5. Products and inventory: item data, stock, units, groups, categories, costing, expiry.
6. Manual price, quantity, barcode, and activation maintenance.
7. Customers, customer accounts, payments, statements, checks, and external debts.
8. Suppliers, supplier accounts, payments, and statements.
9. Sales returns.
10. Installment sales and installment collection.
11. Expenses, expense types, payment vouchers, and expense reports.
12. Cash safes, bank, deposits, withdrawals, transfers, and financial movement.
13. Employees, attendance/departure, advances, and salaries.
14. Barcode design and label printing.
15. Notifications and alerts.
16. Reports, report preview, report designer, and Excel export.
17. Statistics and profit analysis.
18. Specialized operations: repairs/devices, manufacturing, and e-wallet services.
19. Users, roles, permissions, and administrative controls.
20. System settings, printers, invoice templates, backup, and theme settings.

## 4. Complete Screen Inventory

| ID | Screen/dialog/state | Module | Purpose | Evidence | Confidence |
|---|---|---|---|---|---|
| SCR-001 | تسجيل الدخول (Login) | Authentication | Authenticate user | Login videos | [CONFIRMED] |
| SCR-002 | Trial version dialog | Authentication | Show trial/licensing information after login | Quick overview | [CONFIRMED] |
| SCR-003 | Shift Management / بدء الوردية | Shifts | Start or continue a shift before sales | Sales/customer analyses | [CONFIRMED] |
| SCR-004 | الرئيسية (Dashboard) | Navigation | Central navigation and status | All UI analyses | [CONFIRMED] |
| SCR-005 | شاشة المبيعات (Standard Sales) | Sales | Build and pay an invoice | Sales analyses | [CONFIRMED] |
| SCR-006 | شاشة مبيعات سريعة / POS | Sales | Touch-oriented rapid sale | Quick overview | [CONFIRMED] |
| SCR-007 | Add Customer | Customers | Create a customer inline from sales | Customers | [CONFIRMED] |
| SCR-008 | Customer Management | Customers | CRUD and account actions | Customers | [CONFIRMED] |
| SCR-009 | Customer Balance | Customers | Adjust/open a customer balance | Customers | [CONFIRMED] |
| SCR-010 | Customer credit accounts | Customers | View unpaid invoices and settle them | Customers | [CONFIRMED] |
| SCR-011 | Customer reports/statements | Customers | Preview printable customer history | Customers | [CONFIRMED] |
| SCR-012 | External Debts | Customers | Track debts outside standard invoices | Customers | [CONFIRMED] |
| SCR-013 | Credit Payment / دفع أجل | Sales/customers | Enter paid and remaining amounts | Sales | [CONFIRMED] |
| SCR-014 | Installment Sale | Installments | Create a payment schedule | Installment Sales | [CONFIRMED] |
| SCR-015 | Pay Installments | Installments | Record installment payments | Installment Sales | [CONFIRMED] |
| SCR-016 | Installment print preview | Installments | Preview/print schedule or receipt | Installment Sales | [CONFIRMED] |
| SCR-017 | Purchases | Purchases | Create supplier invoice and receive stock | Purchases | [CONFIRMED] |
| SCR-018 | Supplier Management | Suppliers | CRUD suppliers and account actions | Suppliers | [CONFIRMED] |
| SCR-019 | Supplier Accounts | Suppliers | View/pay supplier balances | Suppliers | [CONFIRMED] |
| SCR-020 | Supplier reports | Suppliers | Print supplier history/statements | Suppliers | [CONFIRMED] |
| SCR-021 | Add Product / إضافة صنف | Products | Create item and initial prices/quantity | Product analyses | [CONFIRMED] |
| SCR-022 | Product Data / بيانات الصنف | Products | Maintain categories, units, and metadata | System sub-sections | [CONFIRMED] |
| SCR-023 | Item Details / Matrix | Products | Maintain detailed item attributes, variations, weights | System sub-sections | [MEDIUM confidence] |
| SCR-024 | Manual Price Editing | Products | Bulk edit product fields | Edit-price analysis | [CONFIRMED] |
| SCR-025 | Select Group | Products | Assign a product group code | Edit-price analysis | [CONFIRMED] |
| SCR-026 | Inventory/warehouse | Inventory | View and manage stock | Inventory analyses | [CONFIRMED] |
| SCR-027 | Item Movement / حركة الأصناف | Inventory | View item in/out ledger | Specialized analysis | [CONFIRMED] |
| SCR-028 | Stock count / جرد | Inventory | Perform inventory count/adjustment | Cross-document navigation | [MEDIUM confidence] |
| SCR-029 | Returns / المرتجعات | Returns | Reverse sold goods | Returns | [CONFIRMED] / [INFERRED] for some paths |
| SCR-030 | Expense Types | Expenses | Maintain expense categories | Expenses | [CONFIRMED] |
| SCR-031 | Expenses Reports | Expenses | Filter and report expenses | Expenses | [CONFIRMED] |
| SCR-032 | Payment Voucher / سند صرف | Expenses | Record an expense payment | Expenses | [CONFIRMED] |
| SCR-033 | Statistics | Statistics | Summarize sales, expenses, and profit | Expenses/statistics | [CONFIRMED] |
| SCR-034 | Safe Management / رقم خزنة | Treasury | CRUD cash safes | Bank | [CONFIRMED] |
| SCR-035 | Safe Deposit | Treasury | Deposit money into a safe | Bank | [CONFIRMED] |
| SCR-036 | Bank Deposit | Treasury | Deposit money into bank | Bank | [CONFIRMED] / similar behavior inferred |
| SCR-037 | Current Safe/Bank Balances | Treasury | View selected safe and bank balances | Bank | [CONFIRMED] |
| SCR-038 | Financial Movement / حركة مالية | Treasury | View all financial ins/outs | Specialized analysis | [CONFIRMED] |
| SCR-039 | Shift Reports | Shifts/reports | Summarize cashier shift activity | Specialized analysis | [CONFIRMED] |
| SCR-040 | Employee Management | HR | CRUD employee records | Attendance | [CONFIRMED] |
| SCR-041 | Salaries and Advances | HR | Record advances and pay salaries | Attendance | [CONFIRMED] |
| SCR-042 | Attendance and Departure | HR | Check in, break, and check out | Attendance | [CONFIRMED] |
| SCR-043 | Barcode Designer | Barcode | Design product labels | Barcode | [CONFIRMED] |
| SCR-044 | Barcode Print Preview | Barcode | Preview and print labels | Barcode | [CONFIRMED] |
| SCR-045 | Low Stock Alerts | Notifications | Show items at/below reorder level | Notifications | [CONFIRMED] |
| SCR-046 | Supplier Payables Alerts | Notifications | Show supplier amounts due | Notifications | [CONFIRMED] |
| SCR-047 | Expiry Alerts | Notifications | Show near-expiration items | Notifications | [CONFIRMED] |
| SCR-048 | Device Receipt / استلام جهاز | Repairs | Register repair intake | Specialized analysis | [CONFIRMED] |
| SCR-049 | Device Delivery / تسليم جهاز | Repairs | Return repaired device and bill | Specialized analysis | [CONFIRMED] |
| SCR-050 | Manufacturing / تصنيع | Manufacturing | Consume raw materials into a finished item | Specialized analysis | [CONFIRMED] |
| SCR-051 | E-wallet Services | Treasury | Track wallet cash-in/cash-out | Specialized analysis | [CONFIRMED] |
| SCR-052 | System Settings | Settings | Configure printer, store, invoice, behavior | Settings | [CONFIRMED] |
| SCR-053 | Report Designer | Reports | Edit report/print layouts | Specialized analysis | [CONFIRMED] |
| SCR-054 | Users and Permissions | Security | Manage users and access | User Permissions | [CONFIRMED] |
| SCR-055 | Backup/restore entry point | Administration | Access backup functionality | Navigation analyses | [CONFIRMED] as menu item; details unknown |

## 5. Complete Feature Inventory

### Authentication, navigation, and settings

- User selection/name and masked password entry.
- On-screen numeric keypad, Enter, Numlock, Backspace, and exit/power controls.
- Manager/system-manager entry point is visible in some analyses.
- Trial-version informational dialog after login.
- Dashboard bubble navigation and persistent RTL top menu.
- User, date, time, shift status, theme, copyright, and version in the footer.
- Dark/light theme switch and selectable accent colors.
- Store identity, invoice styling, printer selection, report/receipt printing, and
  general operational toggles.
- Backup menu entry; exact backup/restore behavior is not documented.

### Sales

- Standard invoice grid with item code/number, name, unit, price, quantity, discount,
  and line total.
- Barcode scan/search and manual product search/selection.
- Default quantity of one when a product is selected.
- Retail/wholesale sale type and retail price type.
- Cash, credit/deferred, and Visa payment choices.
- Customer selection, inline add customer, customer search, delivery fee, discount,
  line deletion, quantity adjustment, and invoice editing.
- Invoice number, date, time, and shift number.
- Pay (F12), print (shown as P-10 in one analysis), and loss-confirmation warning.
- Fast/touch POS with category buttons, product tiles, invoice grid, quantity controls,
  and payment controls.

### Purchases and suppliers

- Supplier selection and inline supplier management.
- Purchase invoice line items, quantities, unit prices, discounts/totals.
- Receive stock and record supplier payment or payable balance.
- Supplier ledger, settlement/payment, reports/statements, checks, and Excel export.

### Products and inventory

- Item code, name, category, sub-category, group code, unit, barcode, color, size,
  weight, cost center, packaging units, purchase price, cost price, selling price,
  reorder point, current quantity, and activation.
- Add product, edit product, delete/deactivate behavior, and group lookup.
- Manual bulk editing of price, quantity, barcode, group, activation, and related fields.
- Edited rows turn red as an unsaved/pending visual state.
- Warehouse stock view, item movement ledger, stock count, reorder monitoring, and
  expiry monitoring.

### Customers, credit, and installments

- Customer ID, name, phone, address, and debt balance.
- Customer CRUD, call action, checks, late-installment action, Excel export.
- Customer opening/current balance adjustment.
- Credit invoice ledger with invoice number, paid, due, invoice/payment dates, total,
  and user.
- Full or partial settlement.
- Customer statement/report printing and external debt records.
- Installment sale, down payment, schedule, due dates, installment amount, collection,
  remaining balance, late-payment view, and print preview.

### Treasury, expenses, and financial movement

- Safe CRUD with sequential generated ID.
- Safe and bank deposits; menu entries also indicate withdrawal and transfer operations.
- Selected safe balance, bank balance, depositor, reason, and transaction date.
- Expense types, expense voucher, source safe/payment source, notes, reports, and totals.
- Financial movement ledger with date, description, amount, transaction type, safe,
  invoice number, and user; totals for incoming, outgoing, and balance.
- E-wallet cash-in/cash-out tracking.

### Employees and specialized operations

- Employee ID, name, job, monthly salary, national ID, phone, and address.
- Employee CRUD and Excel export.
- Attendance check-in, break entry, departure, actual work time, and notes.
- Salary payment, advances, net salary, payment source, and operation history.
- Repair device intake/delivery by order and serial number, customer, problem, notes,
  and billing.
- Manufacturing raw-material consumption, finished item, quantities, costs, and total.

### Reporting, alerts, security, and printing

- Top-selling reports over supported time periods.
- Sales, purchases, customer, supplier, expense, shift, inventory, financial movement,
  and profit/statistics reports.
- Report preview, configurable report layouts, and a DevExpress-style report designer.
- Excel export appears on employee, customer, supplier, and management screens.
- Low stock, supplier payable, and near-expiry alerts.
- User, role, permission, and restricted-action management.
- Barcode label design, product data binding, print preview, and printer settings.

## 6. Business Rules

| Rule ID | Rule | Trigger/behavior | Evidence | Confidence |
|---|---|---|---|---|
| BR-AUTH-001 | Authentication precedes application access. | User must submit login credentials. | Login screens | [CONFIRMED] |
| BR-AUTH-002 | Trial information may appear after login. | Successful login displays trial dialog. | Quick overview | [CONFIRMED] |
| BR-SHIFT-001 | Sales requires an active shift. | Opening sales invokes shift management if no active shift exists. | Sales/customer analyses | [CONFIRMED] |
| BR-SHIFT-002 | A shift can be started or continued. | User chooses new shift or continuation of a closed shift. | Shift dialog | [CONFIRMED] |
| BR-SALES-001 | Selecting a product adds a sale line. | Product selection adds a default quantity of one. | Quick overview | [CONFIRMED] |
| BR-SALES-002 | A sale may be retail or wholesale. | Sale type controls available pricing. | Sales | [CONFIRMED] |
| BR-SALES-003 | Credit payment is customer-account activity. | Credit sale records paid/due values against a customer. | Sales/customers | [CONFIRMED] |
| BR-SALES-004 | Selling below cost requires confirmation. | Warning asks `تأكيد: هذه الفاتورة بها خسارة هل تريد استكمال البيع`. | Sales | [CONFIRMED] |
| BR-SALES-005 | Product stock and invoice totals update during editing. | Adding/removing/changing quantity recalculates visible totals. | Sales | [CONFIRMED] |
| BR-PUR-001 | A purchase receives inventory from a supplier invoice. | Purchase completion affects stock and supplier account. | Purchases | [CONFIRMED] |
| BR-INV-001 | Product records expose explicit stock and pricing fields. | Add/edit screens show quantity, prices, barcode, and activation. | Inventory/product analyses | [CONFIRMED] |
| BR-INV-002 | Activation can hide an item without deleting it. | Deactivated items are not intended for normal sales. | Edit-price analysis | [CONFIRMED] |
| BR-INV-003 | Bulk edits are visually marked. | Changed row becomes red. | Edit-price analysis | [CONFIRMED] |
| BR-CUST-001 | Customer IDs are generated. | Inline customer creation assigns an ID. | Customers | [CONFIRMED] |
| BR-CUST-002 | Customer debt is visible and settleable. | Account screen lists due invoices and supports settlement. | Customers | [CONFIRMED] |
| BR-RET-001 | A return reverses some sale effects. | Return workflow is intended to affect stock and financial/customer totals. | Returns | [CONFIRMED] for existence; exact reversal rules [UNKNOWN] |
| BR-TRE-001 | Safes must exist before deposits. | Deposit selects an existing safe. | Bank | [CONFIRMED] |
| BR-TRE-002 | Safe IDs are sequential and generated. | New safe receives an ID. | Bank | [CONFIRMED] |
| BR-TRE-003 | Depositor name is mandatory for safe deposit. | Empty name produces `خطأ: من فضلك ادخل اسم المودع`. | Bank | [CONFIRMED] |
| BR-TRE-004 | Multiple safes are tracked independently. | Balance is selected by safe. | Bank | [CONFIRMED] |
| BR-EXP-001 | Expenses are recorded as financial outflows. | Voucher records amount, type, source, and notes. | Expenses | [CONFIRMED] |
| BR-HR-001 | Attendance is stateful and row-driven. | Check-in, break, and departure mutate the attendance row. | Attendance | [CONFIRMED] |
| BR-HR-002 | Advances reduce salary payable. | Net salary is automatically calculated from salary and advances. | Attendance | [CONFIRMED] |
| BR-PRINT-001 | Reports and invoices are printable/configurable. | Print preview/settings/designer are available. | Settings, barcode, reports | [CONFIRMED] |
| BR-SEC-001 | Access is permission-dependent. | User permissions module and restricted actions exist. | Permissions/product analyses | [CONFIRMED] for existence; matrix [UNKNOWN] |

## 7. Business Entities

| Entity | Purpose and known attributes | Relationships | Confidence |
|---|---|---|---|
| User | Login identity, username, password, display name, role | Creates/records sales, payments, movements, reports | [CONFIRMED] |
| Role/Permission | Controls access to screens/actions | Assigned to users; gates modules and operations | [CONFIRMED] concept; exact model [UNKNOWN] |
| Shift | Cashier work session, number, start/end, totals, status | Required by sales; summarized by shift reports | [CONFIRMED] |
| Product/Item | Sellable or purchasable stock item | Belongs to category/unit/group; appears in sale/purchase/return | [CONFIRMED] |
| Category/Sub-category/Group | Product classification and group code | Classifies products and filters POS | [CONFIRMED] |
| Unit | Measurement such as piece or kilo | Used by product and transaction lines | [CONFIRMED] |
| Barcode | Product scan/label identifier | Belongs to product; used by sales and printing | [CONFIRMED] |
| Cost center | Product/operation cost classification | Associated with product or financial reporting | [INFERRED] |
| Customer | Buyer with identity/contact and debt balance | Owns sales, credit invoices, installments, returns | [CONFIRMED] |
| Supplier | Vendor with contact/account balance | Owns purchases, payables, payments, returns | [CONFIRMED] |
| Sale/Invoice | Outgoing transaction with date, user, shift, totals, payment | Has sale lines, customer, payments, stock effects | [CONFIRMED] |
| Sale line | Product, quantity, price, discount, total | Belongs to sale and product | [CONFIRMED] |
| Purchase | Incoming supplier transaction | Has purchase lines and supplier payment/payable | [CONFIRMED] |
| Return | Reversal transaction | References sale/product/customer and financial effect | [CONFIRMED] existence; links [UNKNOWN] |
| Payment | Cash, Visa, credit settlement, installment, supplier/customer payment | Applies to sale, debt, salary, expense, or treasury transaction | [CONFIRMED] |
| Installment plan/payment | Schedule, due dates, paid and remaining | Belongs to customer and installment sale | [CONFIRMED] |
| Expense/Expense type | Outgoing cost and classification | Posts to safe/bank and statistics | [CONFIRMED] |
| Safe | Cash drawer with generated ID, name, balance | Receives deposits and pays expenses/transactions | [CONFIRMED] |
| Bank | Bank balance/account | Receives deposits and financial movements | [CONFIRMED] existence; account cardinality [UNKNOWN] |
| Financial transaction | Deposit, withdrawal, transfer, expense, payment | References safe/bank, user, date, invoice where applicable | [CONFIRMED] |
| Employee | HR record and salary basis | Has attendance, advances, salary payments | [CONFIRMED] |
| Attendance record | Check-in, break, departure, actual work time | Belongs to employee | [CONFIRMED] |
| Repair order/device | Order, device type, brand, model, serial, problem, notes | Belongs to customer; may create charge | [CONFIRMED] |
| Manufacturing record | Finished item and raw materials with quantity/cost | Consumes inventory and creates/updates product cost | [CONFIRMED] |
| E-wallet transaction | Wallet cash-in/cash-out | Financial movement involving a wallet | [CONFIRMED] |
| Notification | Low-stock, payable, or expiry alert | Derived from inventory/supplier data | [CONFIRMED] |
| Report/template | Report data and printable layout | Uses sales, purchases, inventory, finance, and settings | [CONFIRMED] |

## 8. Workflows

### 8.1 Login/startup

**START** → application opens login → user selects/enters username and password →
user may use on-screen keypad → submit login → validate credentials/licensing →
show trial dialog if applicable → open dashboard. Failure states: invalid credentials,
cancel/exit, or licensing behavior not shown **[UNKNOWN]**.

### 8.2 Shift opening

**START** → user selects sales → system checks shift → if no active shift, show
shift dialog → choose new shift or continue closed shift → start/continue → open
sales. Cancellation leaves the user outside sales **[CONFIRMED]**.

### 8.3 Standard/fast sales

**START** → open standard or fast POS → scan/search/select product → add line with
quantity one → adjust quantity/price/discount/delivery → choose customer and sale
type → choose cash, credit, or Visa → calculate totals → if below-cost, ask the
loss confirmation → confirm payment → update invoice, stock, shift, and relevant
accounting records → print if requested → return to sale-ready state.

Failure states include missing/invalid product, insufficient stock, missing customer
for credit, invalid payment, permission denial, and cancellation. Exact behavior for
each is **[UNKNOWN]** unless stated elsewhere.

### 8.4 Credit sale

**START** → build sale → choose customer and `أجل` → press Pay/F12 → show total,
required, paid, and remaining → enter paid amount → confirm → record unpaid balance
against customer → update stock and financial/shift records → optionally print.
The exact rule for allowing overpayment or zero payment is **[UNKNOWN]**.

### 8.5 Installment sale and collection

**START** → select customer → select products → choose installment sale → enter
total/down payment and installment terms → calculate schedule → confirm → record
customer obligation → print schedule/receipt. Later: select customer/install­ment →
choose due installment → enter payment → update paid/remaining and status → print
receipt if requested. Interest, grace periods, rescheduling, and late-fee behavior
are **[UNKNOWN]**.

### 8.6 Purchases

**START** → open purchases → select/create supplier → add products and quantities →
enter purchase prices/discounts → calculate total → choose payment or payable →
confirm invoice → increase stock → update supplier account and treasury/shift
records → print/report. Partial-payment and stock-costing details are **[UNKNOWN]**.

### 8.7 Inventory changes

**START** → open product/inventory screen → add or select item → enter/edit item
metadata, prices, barcode, quantity, reorder point, and activation → save or use
bulk grid → changed rows are highlighted → close/commit according to the reference
behavior → item appears in inventory/POS. Whether bulk edits commit on focus loss,
explicit close, or another event is **[UNKNOWN]**.

### 8.8 Customer management

**START** → open customer management or use inline add → enter name/contact →
save → generated customer ID and success feedback → select customer for sales or
accounts → view debt/transactions → settle, print statement, or adjust balance.
Duplicate handling and deletion semantics are **[UNKNOWN]**.

### 8.9 Supplier management

**START** → open supplier management → create/edit supplier → save → use supplier
in purchase → view account/payable ledger → record settlement/payment → print/export
statement. Exact supplier fields beyond those shown are **[UNKNOWN]**.

### 8.10 Returns

**START** → open returns → identify original sale/item or enter return details →
select quantity/reason → validate return → confirm → reverse or adjust stock →
refund/credit customer or cash → update totals/reports → print if available.
Whether original invoice linkage and partial returns are mandatory is **UNKNOWN**.

### 8.11 Expenses

**START** → maintain/select expense type → open payment voucher → enter amount,
date, source safe/payment source, and notes → validate → confirm → post financial
outflow → update safe balance and statistics → show success feedback/report.

### 8.12 Treasury/bank

**START** → create/select safe → open deposit/withdrawal/transfer → choose safe or
bank → view current balance → enter amount, date, depositor, reason → validate
required depositor → confirm → post financial transaction → refresh balance and
ledger. Safe deposits explicitly reject an empty depositor with
`خطأ: من فضلك ادخل اسم المودع`.

### 8.13 Barcode

**START** → select product(s) → open barcode designer → choose label layout/data →
preview → select printer/quantity → print. Barcode format, duplicate policy, and
printer failure behavior are **[UNKNOWN]**.

### 8.14 Employees and attendance

**START** → create employee → record check-in → optionally enter break duration →
record departure → calculate actual work time → later record advances/pay salary →
derive net salary → post payment source and history. Break input uses the prompt
`الرجاء ادخال وقت الاستراحة`; supported units are **[UNKNOWN]**.

### 8.15 Users and permissions

**START** → authorized administrator opens users/permissions → create/select user →
assign role or permissions → save → user logs in → system shows or blocks screens/
actions according to access. The exact permission list and administrator override
rules are **[UNKNOWN]**.

### 8.16 Reports/statistics

**START** → choose report → select date/range/entity filters → load data → view
totals/grid → preview, print, export, or open designer where supported. Report
security warning before designer access is **[CONFIRMED]**; exact report formulas
are consolidated in section 10.

### 8.17 Settings

**START** → open settings → configure printer/store/invoice/general options →
save → subsequent screens use configuration. Which settings require restart and
which users may change them are **[UNKNOWN]**.

## 9. Cross-Module Dependencies

| Dependency | Classification | Evidence/impact |
|---|---|---|
| Login → user/session | [CONFIRMED] | All actions display or record a user. |
| Login → permissions | [CONFIRMED] concept | Permission-management module exists; exact enforcement is unknown. |
| Sales → active shift | [CONFIRMED] | Sales is blocked/opened through shift dialog. |
| Sales → inventory | [CONFIRMED] | Sale lines use products and quantity; exact stock timing unknown. |
| Purchases → inventory | [CONFIRMED] | Purchase receipts are incoming inventory. |
| Sales/purchases → product cost | [INFERRED] | Cost and profit screens depend on product cost; costing algorithm unknown. |
| Sales → customer debt | [CONFIRMED] for credit sales | Credit account lists unpaid invoices. |
| Installments → customer debt | [CONFIRMED] | Installment schedules and payments are customer-linked. |
| Returns → inventory | [CONFIRMED] at intent level | Return workflow reverses stock; exact rules unknown. |
| Returns → customer/cash | [INFERRED] | Refund or credit is expected but not fully demonstrated. |
| Sales/purchases/expenses → treasury | [CONFIRMED] concept | Financial movement and safe/bank records exist; posting timing varies by workflow. |
| Expenses → statistics/profit | [CONFIRMED] | Expense reports/statistics exist. |
| Employees → treasury | [CONFIRMED] concept | Salary/advance payment has a payment source. |
| Inventory → notifications | [CONFIRMED] | Low-stock and expiry alerts. |
| Suppliers → notifications | [CONFIRMED] | Payables alerts. |
| Settings → printing | [CONFIRMED] | Printer and invoice settings. |
| Products → barcode printing | [CONFIRMED] | Product barcodes feed labels. |
| Manufacturing → inventory/cost | [CONFIRMED] | Raw materials and finished-product cost are shown. |
| Repairs → customer/financials | [CONFIRMED] | Device delivery includes customer and billing fields. |

## 10. Calculations

| Calculation | Formula/inputs documented | Used in | Evidence | Confidence |
|---|---|---|---|---|
| Line total | `quantity × unit price` (discount handling not fully specified) | Sales/purchases | Visible totals and grids | [CONFIRMED] basic formula |
| Invoice total | Sum of line totals, with discount and delivery where applicable | Sales/purchases | Sales UI | [CONFIRMED] components; order of operations [UNKNOWN] |
| Required/due | Total minus paid (and possibly discount) | Cash/credit/payment dialog | Sales/customers | [CONFIRMED] concept |
| Remaining | Required/due minus paid in payment dialog | Credit/installments | Sales/installments | [CONFIRMED] concept |
| Customer debt | Prior balance plus credit due minus settlements/credits | Customer accounts | Customers | [CONFIRMED] concept; opening-balance semantics [UNKNOWN] |
| Installment balance | Installment obligation minus recorded payments | Installments | Installment Sales | [CONFIRMED] concept |
| Net salary | Monthly salary minus advances/deductions | Salaries | Attendance | [CONFIRMED] |
| Actual work time | Departure time minus attendance time minus break | Attendance | Attendance | [CONFIRMED] concept; exact precision [UNKNOWN] |
| Safe/bank balance | Prior balance plus incoming minus outgoing | Treasury | Bank/financial movement | [CONFIRMED] concept |
| Financial balance | Total incoming minus total outgoing | Financial movement | Specialized analysis | [CONFIRMED] |
| Manufacturing cost | Sum of raw-material quantity × raw-material cost | Manufacturing | Specialized analysis | [CONFIRMED] |
| Profit | Sales revenue minus product cost and possibly expenses | Statistics/profit | Statistics | [CONFIRMED] concept; cost basis and expense inclusion [UNKNOWN] |
| Reorder condition | Current quantity at/below reorder point | Low-stock notification | Inventory/notifications | [INFERRED] exact comparison unknown |
| Expiry alert | Item expiry within configured threshold | Expiry notification | Notifications | [INFERRED] threshold unknown |
| Top-selling ranking | Aggregated sold quantity/value by product and time period | Top-selling report | Quick overview | [CONFIRMED] existence; metric [UNKNOWN] |

No source provides a verified costing method (FIFO, weighted average, or another
method). Purchase price, cost price, and selling price must not be treated as
interchangeable until the original system is verified.

## 11. Validation Rules

- Login credentials are required to authenticate **[CONFIRMED]**.
- Sales requires an active shift **[CONFIRMED]**.
- A credit sale requires a customer or customer-account context **[CONFIRMED] in
  workflow; exact validation message [UNKNOWN]**.
- A sale below cost requires an explicit confirmation **[CONFIRMED]**.
- Safe deposit requires an existing safe **[CONFIRMED]**.
- Safe deposit requires depositor name **[CONFIRMED]**.
- Amount fields must accept valid monetary values **[INFERRED]**; negative/decimal
  rules are **[UNKNOWN]**.
- Product, customer, supplier, employee, and safe identifiers are generated or
  selected in the shown screens **[CONFIRMED]**.
- Product activation controls whether it is available for normal sale
  **[CONFIRMED]**.
- Required fields for other forms are not consistently demonstrated and remain
  **[UNKNOWN]**.
- Permission checks can restrict operations **[CONFIRMED] as a system capability;
  exact rules [UNKNOWN]**.
- Stock restrictions, overselling, negative stock, and return quantity validation
  are **[UNKNOWN]**.

## 12. Error / Warning / Confirmation States

| State/message | Context | Classification |
|---|---|---|
| `نسخة تجريبية` / Trial version | After login | [CONFIRMED] informational |
| `تأكيد: هذه الفاتورة بها خسارة هل تريد استكمال البيع` | Sale below cost | [CONFIRMED] confirmation |
| `خطأ: من فضلك ادخل اسم المودع` | Safe deposit without depositor | [CONFIRMED] error |
| `تم` / `تم الحفظ بنجاح` | Successful save | [CONFIRMED] |
| `تم الاضافة بنجاح` | Safe/entity creation | [CONFIRMED] |
| `تم الايداع بنجاح` | Safe deposit | [CONFIRMED] |
| Low-stock/reorder dialog | Dashboard load | [CONFIRMED] |
| Near-expiry alert | Notification area | [CONFIRMED] |
| Supplier-payable alert | Notification area | [CONFIRMED] |
| Report-designer security warning | Opening designer | [CONFIRMED] |
| `Change Settings` / `الرجاء ادخال وقت الاستراحة` | Attendance break entry | [CONFIRMED] |
| Invalid login, insufficient stock, invalid payment, duplicate records | Various | [UNKNOWN] exact wording and behavior |

Success feedback is generally a dark modal/toast with a blue checkmark; errors are
described as dark notifications with a red X in the treasury analysis
**[CONFIRMED] for the visual pattern**.

## 13. Users, Roles & Permissions

- The system has at least cashier and manager/system-manager identities
  **[CONFIRMED]**.
- Login uses a username/user number and masked password **[CONFIRMED]**.
- The dashboard/footer displays the current user **[CONFIRMED]**.
- A dedicated user-permissions/roles module exists **[CONFIRMED]**.
- Permissions can affect module/action visibility or execution **[INFERRED]**;
  the source does not provide a complete permission matrix.
- Administrative functions visibly include settings, backup, report design,
  product maintenance, employee management, and permissions **[CONFIRMED as
  navigation entries]**.
- Exact roles, inheritance, password policy, audit requirements, and manager
  override behavior are **[UNKNOWN]**.

## 14. UI Structure

- Full-screen desktop main window with persistent RTL top navigation.
- Dashboard uses a dark background, colored circular/bubble quick actions, and a
  bottom status bar.
- Standard child screens use a dark custom header, close button, dense forms, and
  DataGrids with alternating rows.
- List/CRUD screens commonly use a right-side action sidebar: New, Add, Edit,
  Delete, Excel.
- Inline row actions commonly include edit and red trash/delete icons.
- Transaction screens use dense data-entry panels beside a line-item DataGrid.
- Modal dialogs dim the parent and are used for shift opening, payments, balances,
  deposits, group selection, warnings, and confirmations.
- RTL is used for labels, forms, menus, and data presentation; keypad and some
  numeric controls may remain visually LTR.
- Dark/light theme changes backgrounds, foregrounds, grid rows, and accents.
- Buttons use recurring semantic colors: green add/confirm/pay, red delete/close/
  danger, blue save/search/edit, with some module-specific red/green headers.
- Searchable ComboBoxes, barcode fields, date filters, and DataGrid editing are
  recurring controls.
- Report and barcode preview/designer surfaces may use a DevExpress-like interface.
- Exact window hierarchy (MDI versus modal child), resizing rules, accessibility,
  localization coverage, and responsive behavior are **[UNKNOWN]**.

## 15. Keyboard Shortcuts

| Shortcut/control | Action | Evidence | Confidence |
|---|---|---|---|
| F12 | Pay/complete sale | Sales and POS screens | [CONFIRMED] |
| P-10 | Print sale/invoice | Sales analysis | [CONFIRMED] as displayed label; actual key mapping [UNKNOWN] |
| Enter | Login/confirm payment | Login/payment screens | [CONFIRMED] |
| Numlock, numeric keypad | Enter login/password numbers | Login screens | [CONFIRMED] |
| Backspace | Keypad editing | Login analysis | [CONFIRMED] |
| Other function keys | Not documented | — | [UNKNOWN] |

## 16. Reports

Known reports and report-like views include:

- Top-selling products by a selectable period.
- Sales report, sales-profit report, purchase report, and shift report.
- Customer account statements and customer transaction reports.
- Supplier account statements and supplier transaction reports.
- Expense reports and expense statistics.
- Inventory/item movement and stock reports.
- Financial movement ledger with incoming, outgoing, and balance totals.
- Most-sold/top-selling analysis.
- Installment schedule/payment print preview.
- Barcode label print preview.
- Employee, customer, supplier, and other grid exports to Excel.
- Configurable report templates through a report designer.

The exact filter set, grouping, pagination, export formats, and formulas of each
report are **[UNKNOWN]** unless stated in section 10.

## 17. Notifications

- Low-stock/reorder notification may appear when the dashboard loads **[CONFIRMED]**.
- Supplier payable notification exists **[CONFIRMED]**.
- Near-expiration notification exists **[CONFIRMED]**.
- Success/error toasts and confirmation dialogs provide operation feedback
  **[CONFIRMED]**.
- The alert threshold, persistence, dismissal/read state, notification history,
  refresh timing, and user-specific filtering are **[UNKNOWN]**.

## 18. Document Conflicts

1. **Sales and purchases color conventions.** One analysis describes Sales as green
   and Purchases as red; another describes the reverse for dashboard bubbles.
   Known: both modules use distinct colored bubbles/headers. Exact color assignment
   is **[UNKNOWN]** and must be verified from the reference UI.
2. **Customer/sale layout proportions.** Sales analyses place the customer/control
   panel on the right but report different percentages and grid orientation.
   Known: RTL split layout and a line-item grid exist. Exact proportions are
   **[UNKNOWN]**.
3. **Login user control.** One document describes a username ComboBox with
   Manager/Cashier; another describes a username text field/user number and a
   cashier/register selector. Known: user identity and password are required.
   Exact controls and station selection are **[UNKNOWN]**.
4. **Sales print shortcut.** The sales analysis labels print as `P-10`; other
   documents only show a print action. Known: printing exists. Actual shortcut is
   **[UNKNOWN]**.
5. **Product terminology.** Sources alternate among item, product, item code,
   group, category, sub-category, and cost center. Known: the fields are visible;
   whether each is a separate maintained entity is **[UNKNOWN]** for cost center
   and some group relationships.
6. **Bulk-edit persistence.** The edit-price analysis suggests changes save when
   focus is lost or the window closes, but no explicit save action is shown.
   Known: rows turn red and the narrator says changes are saved. Exact commit
   timing is **[UNKNOWN]**.
7. **Returns evidence level.** Returns are clearly a menu/screen in some analyses,
   while one source labels the observed behavior inferred. Known: a returns
   module exists; original-invoice linkage, stock, and refund rules are
   **[UNKNOWN]**.
8. **Bank cardinality.** The system displays a current bank balance, while safes
   are explicitly multiple. Known: bank operations exist; whether multiple bank
   accounts are supported is **[UNKNOWN]**.
9. **Profit formula.** Sources show sales profit/statistics and product cost, but
   do not identify costing method or whether expenses are included. Any precise
   profit formula beyond section 10 would be an unsupported assumption.

## 19. Unknown / Needs Verification

### High priority

- Product costing method and the relationship between purchase price and cost price.
- Exact stock rules: overselling, negative stock, reservation, timing of stock changes,
  and stock behavior on returns.
- Complete payment rules: zero/partial/overpayment, Visa posting, cash drawer
  selection, and credit-sale requirements.
- Exact return model, original invoice linkage, refund method, and partial returns.
- Complete user/role/permission matrix and administrator overrides.
- Transaction atomicity across inventory, customer debt, shift, and treasury.
- Exact database-level identifiers and uniqueness rules for products/barcodes,
  customers, suppliers, and invoices.
- Whether financial postings are reversible/audited and how edits/deletes affect
  historical reports.

### Medium priority

- Installment interest, due-date, grace-period, late-fee, and rescheduling rules.
- Reorder and expiry thresholds and whether users can configure them.
- Safe deletion restrictions and behavior for non-zero balances.
- Bank account cardinality and transfer/withdrawal details.
- Bulk-edit commit timing and close-with-unsaved-changes behavior.
- Exact report formulas, filters, date inclusivity, and export formats.
- Printer failure handling, barcode symbologies, label dimensions, and quantities.
- Backup/restore scope, location, scheduling, and permission requirements.
- Employee salary period, advance rollover, historical filtering, and break units.
- Repair billing, manufacturing posting, and e-wallet settlement details.

### Low priority

- Exact colors, dimensions, fonts, animation, hover behavior, and icon library.
- Whether the application is MDI or uses custom modal child windows in every module.
- Dashboard bubble placement and exact navigation duplication.
- Theme accent persistence and per-user versus global theme scope.
- Notification dismissal, history, and refresh behavior.
- Full keyboard shortcut list and accessibility behavior.

## 20. Master System Map

```text
User / Role / Permission
          |
          v
      Login
          |
          v
 Dashboard ---- Settings ---- Printers / Invoice Templates / Theme / Backup
    |
    +--> Shift Management --> Shift Reports
    |
    +--> Sales / Fast POS
    |       |
    |       +--> Sale + Sale Lines --> Inventory quantities
    |       |          |
    |       |          +--> Customer account / credit / installments
    |       |          +--> Cash / Visa / Treasury / Shift totals
    |       |          +--> Sales reports / top-selling / profit
    |       |
    |       +--> Returns --> Inventory reversal + customer/cash adjustment
    |
    +--> Purchases --> Supplier invoice --> Inventory receipt
    |                         |
    |                         +--> Supplier payable / treasury
    |
    +--> Products / Units / Categories / Groups / Barcodes
    |       |
    |       +--> Manual price/quantity/barcode editing
    |       +--> Barcode design and printing
    |       +--> Low-stock / expiry notifications
    |
    +--> Customers --> Credit accounts / settlements / statements / external debts
    |
    +--> Suppliers --> Supplier accounts / payments / statements / payable alerts
    |
    +--> Treasury
    |       |
    |       +--> Safes / Bank / Deposits / Withdrawals / Transfers
    |       +--> Expenses / payment vouchers
    |       +--> Financial movement / balances
    |       +--> E-wallet transactions
    |
    +--> Employees
    |       |
    |       +--> Attendance / breaks / departure
    |       +--> Advances / salaries --> Treasury
    |
    +--> Specialized Operations
            +--> Repair devices --> Customer / billing
            +--> Manufacturing --> Raw-material inventory / finished-item cost
            +--> Reports / report designer
```

## 21. Implementation-Relevant Requirements

### Database architecture

- Model transactional history separately from mutable master data; sales,
  purchases, returns, payments, treasury movements, attendance, and salary
  payments must remain distinguishable.
- Preserve user, shift, date/time, invoice number, safe/bank, and source-document
  references needed by the shown ledgers and reports.
- Support multiple safes and at least one bank balance, while leaving room to verify
  bank-account cardinality.
- Represent customer and supplier balances as auditable transaction-derived values
  or an equivalently traceable mechanism; do not rely on an opaque mutable total.
- Preserve product price types, purchase/cost/selling prices, quantity, unit,
  barcode, activation, reorder point, expiry, and classification fields.
- Define a verified costing strategy before implementing profit or stock valuation.
- Ensure returns, payments, and corrections can be traced to their originating
  transactions once the original-system rules are verified.

### Application architecture

- Enforce authentication and permission checks at the operation boundary, not only
  by hiding buttons.
- Enforce active-shift requirements before entering or committing sales.
- Keep inventory, customer/supplier accounts, shift totals, and treasury postings
  consistent as one business operation where the reference system does so.
- Centralize monetary calculation, rounding, discount, delivery, payment, debt,
  installment, and profit logic so reports and screens cannot diverge.
- Treat warnings, confirmations, validation failures, and success feedback as
  explicit outcomes; do not silently continue or discard errors.
- Preserve user/audit context for financial and inventory changes.

### UI architecture

- Support Arabic RTL layout, Arabic labels/messages, dense DataGrids, modal dialogs,
  recurring action sidebars, and the standard dark/light theme behavior.
- Provide reusable transaction-grid, CRUD-sidebar, toast, confirmation, and
  payment-dialog patterns.
- Support both standard sales and touch/fast POS flows over the same business
  transaction rules.
- Keep keyboard interactions, especially F12 payment and Enter/keypad login, aligned
  with verified behavior.
- Make unsaved/pending bulk edits explicit until the real commit behavior is verified.

### Security

- Protect authentication data, permissions, report designer, settings, backup,
  financial operations, and destructive actions.
- Record the acting user for sales, purchases, payments, treasury movements,
  expenses, salary actions, and administrative edits.
- Validate all monetary, identifier, date, quantity, and barcode inputs.
- Do not expose report designer or backup operations without the demonstrated
  security warning/permission behavior.

### Performance

- Support dense product and transaction DataGrids with search/filtering without
  loading unbounded history into the UI.
- Keep dashboard alerts and balance summaries responsive while deriving data from
  authoritative transactions.
- Design reports and movement ledgers for date-range filtering and export.

### Printing and reporting

- Support configurable printer/store/invoice settings and printable invoices,
  statements, schedules, reports, and barcode labels.
- Preserve report-preview and designer workflows, including the security warning.
- Keep report formulas tied to the verified costing, payment, return, expense, and
  date-range rules rather than duplicating screen-specific calculations.

