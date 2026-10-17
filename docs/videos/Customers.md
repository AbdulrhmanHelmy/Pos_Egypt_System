# Video Analysis

## 1. Video Overview

The video demonstrates a desktop Point of Sale (POS) and business management application designed for the Egyptian market. The application features a dark-themed user interface with Arabic localization. The recording specifically focuses on the "Sales" and "Customer Management" modules, showcasing how to open a shift, create a new customer, process a credit (أجل) sale that triggers a below-cost warning, view customer balances, and settle outstanding debts.

## 2. Screens Discovered

| ID | Screen | Purpose | Confidence |
| --- | --- | --- | --- |
| 1 | Main Dashboard (الرئيسية) | Central navigation hub with quick access to primary modules. | [CONFIRMED] |
| 2 | Shift Management Dialog | To open a new shift or continue an existing one before accessing sales. | [CONFIRMED] |
| 3 | Sales Screen (شاشة المبيعات) | Core POS screen for processing transactions and selecting customers. | [CONFIRMED] |
| 4 | Add Customer Form (إضافة عميل) | Data entry form to register a new customer from within the Sales screen. | [CONFIRMED] |
| 5 | Payment Dialog (دفع أجل) | Finalizes the transaction, specifying total, paid, and remaining amounts. | [CONFIRMED] |
| 6 | Customer Management (إدارة العملاء) | DataGrid view of all customers, showing details and current debt balances. | [CONFIRMED] |
| 7 | Customer Balance Dialog (رصيد العميل) | Small modal to manually adjust a customer's opening/current balance. | [CONFIRMED] |
| 8 | Customer Accounts - Credit (حسابات العملاء - أجل) | Ledger view to track unpaid invoices per customer and process payments. | [CONFIRMED] |
| 9 | Customer Reports (تقارير العملاء) | Generates and previews printable statements of a customer's transaction history. | [CONFIRMED] |
| 10 | External Debts (ديون خارجية) | Dedicated screen to manage debts not tied to standard system invoices. | [CONFIRMED] |

## 3. Detailed Screen Analysis

### Screen 1: Main Dashboard

* **Screen purpose:** Primary navigation.
* **Layout:** Top horizontal navigation bar. Central area with large circular quick-action buttons. Bottom row with smaller circular utility buttons.
* **Header:** Features application name/logo on the right, user info on the bottom status bar.
* **Sidebar/navigation:** Top horizontal menu with dropdowns (الرئيسية, إدارة المبيعات, الموردين, المشتريات, المرتجعات, الرصيد, المصروفات والسلفيات, الخزينة, الحسابات, التقارير, إدارة الموظفين, القسم الصيدليه, الإعدادات).
* **Buttons:**
* Large circles: شاشة المشتريات (Red), شاشة المبيعات (Green).
* Medium circles (Blue): حسابات العملاء, فاتورة مشتريات, POS, ديون خارجية, صلاحيات المستخدمين.
* Bottom row (Dark Gray): تقرير يوميات, حركة الاصناف, المصروفات, تحويل, استلام بضاعة, جرد, مرتجعات.


* **Colors:** Dark gray/black background (`#202020` approx). Top bar is dark gray with bright blue active states. Buttons use solid, flat colors (Red, Green, Blue).
* **Status indicators:** Bottom status bar shows current date, time, user name, and shift status.

### Screen 2: Shift Management Dialog

* **Screen purpose:** Enforce shift tracking before sales operations.
* **Layout:** Modal window centered on screen.
* **Buttons:** بدء الوردية (Start Shift - Green checkmark), Cancel/Close.
* **Radio Buttons:** بناء وردية جديدة (Build new shift), إستكمال على وردية مغلقة (Continue on closed shift).

### Screen 3: Sales Screen

* **Screen purpose:** Processing customer purchases.
* **Layout:** Split view. Right side (approx 25% width) for customer info. Left side (75%) for invoice items and totals.
* **Search controls:** Barcode text field at the top left.
* **Tables/DataGrids:** Main invoice grid (Columns: م, اسم الصنف, سعر البيع, الكمية, الإجمالي, خصم, حذف).
* **Buttons:**
* Right side: `+` (Add customer), Search icon.
* Bottom left: دفع (Pay - F12), طباعة (Print - P-10).


* **ComboBoxes:** Payment method selection (نقدي - Cash, أجل - Credit).
* **Totals:** Bottom left displays الإجمالي (Total), الخصم (Discount), المطلوب (Required).
* **Colors:** Dark theme continues. Grid headers are slightly lighter gray. Buttons are bright blue.

### Screen 4: Add Customer Form

* **Screen purpose:** Quick data entry for new customers.
* **Layout:** Replaces the main invoice grid temporarily.
* **Form fields:** رقم العميل (Auto-generated), اسم العميل (Text), رقم التليفون (Text), العنوان (Text).
* **Buttons:** Vertical toolbar on the right side of the form containing: حفظ (Save - Green), تحديث (Update - Blue), مسح (Clear - Red).

### Screen 5: Payment Dialog (دفع أجل)

* **Screen purpose:** Finalize transaction financials.
* **Layout:** Centered modal with a dark background.
* **Form fields:** المدفوع (Paid amount - editable).
* **Labels (Read-only):** الإجمالي (Total), المطلوب (Required), المتبقي (Remaining).
* **Buttons:** Enter (Confirm), F12 (Save).

### Screen 6: Customer Management

* **Screen purpose:** View and manage the customer database.
* **Layout:** Main DataGrid on the left (85% width). Vertical action sidebar on the right (15% width).
* **Tables/DataGrids:** Columns for ID, Name, Address, Phone, Debt (المديونية), Edit icon, Delete icon.
* **Sidebar Buttons:** بحث (Search), إضافة (Add), تعديل (Edit), حذف (Delete), إتصال (Call), شيكات (Checks), تأخير الأقساط (Late Installments), إكسيل (Excel).

### Screen 7: Customer Accounts - Credit

* **Screen purpose:** View unpaid invoices and process debt settlement.
* **Layout:** Top search bar. Center DataGrid showing invoice history. Bottom action bar for settlement.
* **Search controls:** Dropdown to select Customer Name (اسم العميل).
* **Tables/DataGrids:** Columns for Invoice No, Paid Amount, Due Amount, Invoice Date, Payment Date, Total, User.
* **Buttons:** تسديد (Settle - Green button at the bottom). A small checkbox/button next to it for partial payments.
* **Totals:** الإجمالي المبالغ المستحقة (Total Due Amounts) displayed at the bottom right.

## 4. UI Components

* **Top Navigation Menu:** Persistent across main modules, featuring dropdown menus on hover/click.
* **Action Sidebars:** A recurring pattern where list/grid screens have a vertical column of action buttons (Add, Edit, Delete, Export) anchored to the right side of the screen.
* **DataGrids:** Dark-themed tables with alternating row colors (subtle dark gray differences) and distinct header backgrounds. Includes inline icon buttons (edit/delete) in specific columns.
* **Modals:** Pop-up dialogs used for confirmations, warnings, and small data entry tasks (like shift opening or payment). They dim the background.
* **Flat Icon Buttons:** Buttons consist of a solid bright color block with a white outline icon and white text below the icon.

## 5. User Actions

**User Action:** Clicks "شاشة المبيعات" (Sales Screen).

* **System Response:** Opens Shift Management dialog.
* **Data affected:** None yet.
* **UI change:** Modal appears over the dashboard.
* **Possible Business Rule:** A user cannot access the POS interface without an active, declared shift.

**User Action:** Clicks "+" in the customer pane of the Sales screen.

* **System Response:** Opens the Add Customer form inline.
* **Data affected:** Generates a new Customer ID (2).
* **UI change:** The central invoice grid is hidden and replaced by the customer data entry form.

**User Action:** Enters customer name "محمد" and clicks "حفظ" (Save).

* **System Response:** Saves the customer and returns to the sales view.
* **Data affected:** New customer record created in the database.
* **UI change:** Success toast/popup appears briefly. The customer pane now displays "محمد".

**User Action:** Selects item, changes payment type to "أجل" (Credit), and clicks Pay (F12).

* **System Response:** Validates the sale against item cost.
* **Data affected:** None yet, blocked by validation.
* **UI change:** A warning modal appears: "تأكيد: هذه الفاتورة بها خسارة هل تريد استكمال البيع".

**User Action:** Clicks "نعم" (Yes) on the loss warning, then leaves "المدفوع" (Paid) as 0 in the payment dialog and presses Enter.

* **System Response:** Completes the sale and registers the debt.
* **Data affected:** Inventory reduced, Invoice created, Customer "محمد" debt increased by 1 EGP.
* **UI change:** Screen resets for the next sale.

**User Action:** Navigates to "حسابات العملاء - أجل", selects customer "محمد", and clicks "تسديد" without selecting an amount/invoice.

* **System Response:** Validation fails.
* **Data affected:** None.
* **UI change:** Error popup appears: "خطأ: لا يوجد بيانات في هذه الفتره".

**User Action:** Activates partial payment, enters 1 EGP, and clicks "تسديد".

* **System Response:** Processes the payment and clears the debt.
* **Data affected:** Customer debt reduced by 1 EGP. Ledger updated with payment date.
* **UI change:** Success modal appears ("تم تسديد المبلغ بنجاح"). Total Due Amount at the bottom updates to 0.

## 6. Business Rules

### Confirmed

* Sales operations require an active, explicitly opened shift.
* Customers can be created dynamically directly from the POS/Sales screen.
* The system allows "Credit" (أجل) sales where the customer pays less than the total required amount.
* Unpaid amounts on credit sales are automatically added to the customer's overall debt balance (المديونية).
* The system actively tracks the profit margin of invoices and triggers a hard warning if the selling price results in a loss (selling below cost). The user can override this warning.
* Debts can be settled in full or partially through the Customer Accounts screen.
* Attempting to settle a debt without specifying the target amount or selecting the invoice yields a validation error.

### Inferred

* Items are likely added to the invoice via a barcode scanner, given the focused search box behavior.
* Because there is a button for "ديون خارجية" (External Debts), the system differentiates between debts generated automatically via POS invoices and legacy/external debts entered manually.
* Exporting and importing customer lists via Excel is supported (narrator mentions it, buttons are visible).

### Unknown

* How shifts are closed and reconciled at the end of the day.
* The exact permission matrix (who is allowed to override the "loss" warning).
* How products are added to the system and how their cost price is defined.
* Whether the system supports multiple payment methods on a single invoice (e.g., part cash, part card).

## 7. Business Entities

* **User/Employee:** Represents the person logged in and operating the shift.
* **Shift (وردية):** A tracking entity for a session of sales.
* **Customer (عميل):** Contains ID, Name, Phone, Address, and Debt Balance.
* **Item/Product (صنف):** Contains Name, Selling Price, Cost Price (inferred via loss warning), Quantity.
* **Invoice (فاتورة):** Represents a single sales transaction. Has a Type (Cash/Credit), Total, Discount, Paid, and Remaining amounts.
* **Invoice Item:** Line items linking Products to an Invoice with specific quantities and prices.
* **Payment/Settlement:** A ledger entry recording the payoff of a customer's debt.
* **External Debt:** A specific type of ledger entry for manual debt tracking.

## 8. Workflow

1. **System Initialization:** User opens application, lands on Main Dashboard.
2. **Shift Start:** User clicks Sales, prompted to open a new shift, confirms.
3. **Customer Creation:** In the Sales screen, user opens Add Customer form, inputs data, saves, and customer is automatically selected for the current transaction.
4. **Transaction Entry:** User scans/enters an item. System calculates line total and invoice total.
5. **Credit Sale Processing:** User changes payment method to Credit (أجل). User attempts checkout.
6. **Margin Validation:** System detects a negative margin (loss), warns user. User overrides.
7. **Payment Allocation:** Payment dialog opens. User leaves paid amount at zero. Invoice completes. Customer debt increases.
8. **Debt Review:** User navigates to Customer Management to verify the debt reflects on the customer's profile.
9. **Debt Settlement:** User navigates to Customer Credit Accounts, searches for the customer, views the outstanding invoice.
10. **Payment Execution:** User enters the settlement amount, confirms payment. System validates, updates ledger, and reduces customer debt to zero.

## 9. Calculations

* **Line Item Total:** `سعر البيع (Selling Price) * الكمية (Quantity)`
* **Invoice Total:** `Sum of Line Item Totals`
* **Required Amount (المطلوب):** `Invoice Total - خصم (Discount)`
* **Remaining Amount (المتبقي):** `Required Amount - المدفوع (Paid Amount)`
* **Customer Debt Balance:** `Sum of all Remaining Amounts from Credit Invoices + External Debts - Settlements`

## 10. Validation

* **Sales Margin:** The system checks if `Selling Price < Cost Price`. If true, halts flow and demands confirmation.
* **Settlement Execution:** The system requires a selected row or an explicitly typed amount in the settlement field before processing a payment. Fails with an error if triggered while empty.

## 11. Error / Warning Messages

* **Warning:** `تأكيد: هذه الفاتورة بها خسارة هل تريد استكمال البيع` (Confirmation: This invoice has a loss, do you want to continue selling?). Triggered on F12 checkout. Requires Yes/No.
* **Error:** `خطأ: لا يوجد بيانات في هذه الفتره` (Error: No data in this period / No selection). Triggered when attempting to settle a debt without valid selection/input.
* **Success:** `تم تسديد المبلغ بنجاح` (Amount settled successfully). Triggered after successful debt payment.

## 12. Navigation

* **Primary:** A persistent horizontal top menu bar with traditional dropdown lists.
* **Dashboard:** Visual shortcuts (circles) on the main screen for high-frequency tasks.
* **Internal/Contextual:** Right-aligned vertical toolbars inside specific list screens (like Customer Management) for CRUD operations.

## 13. Reusable UI Patterns

* **Right-Side Action Bars:** Every data table view (Customer Management, External Debts) uses a vertical column of buttons on the far right for actions (Add, Edit, Delete, Export, Search).
* **Modal Dialogs for Critical Actions:** Opening shifts, confirming losses, and making payments all use centered, screen-dimming modals to force user focus.
* **Split Screen POS:** The Sales screen consistently uses the right 25% for entity selection (Customer) and the left 75% for transactional data (Items).
* **Color-Coded Status/Actions:** Green is universally used for positive/save actions (Add, Settle, Start Shift). Red is used for destructive/negative actions (Delete, Cancel).

## 14. Questions / Unknowns

* What is the exact logic determining the "loss" in the warning dialog? Is it strictly `Selling Price < Average Cost` or `Selling Price < Last Purchase Price`?
* Are the custom report layouts (seen briefly in Customer Reports) hardcoded or generated via a tool like Crystal Reports/DevExpress?
* How does the user close a shift, and what reports are generated upon closure?
* Can a single invoice be split between multiple payment methods (e.g., 50% Cash, 50% Credit)? The UI implies a single dropdown selection for the whole invoice.

## 15. Important Findings

* **Strict Margin Control:** The system is heavily focused on profitability tracking, aggressively interrupting the user if a sale is about to result in a loss.
* **Flexible Customer Workflow:** The POS allows on-the-fly customer creation without abandoning the current transaction, which is critical for fast-paced retail.
* **Two-Tier Debt System:** The application explicitly separates auto-generated system debts (from credit invoices) and "External Debts", suggesting it was designed to accommodate businesses migrating from paper ledgers or managing external loans/advances.
