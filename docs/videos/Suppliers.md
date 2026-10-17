# Video Analysis

## 1. Video Overview

The video demonstrates a segment of an Egyptian point-of-sale (POS) and business management desktop application named "DoCashier" (operating in a dark theme UI). The recorded workflow specifically focuses on the purchasing module and supplier management. It covers creating a new purchase invoice, adding a new supplier on the fly, optionally setting an opening balance, executing a split payment (part cash, part deferred/credit), and subsequently managing and paying off the supplier's outstanding debt across two different supplier report screens.

## 2. Screens Discovered

| ID | Screen | Purpose | Confidence |
| --- | --- | --- | --- |
| 1 | Main Dashboard (الرئيسية) | Navigation hub to access main modules via circular buttons and a top menu bar. | High |
| 2 | Purchases (شاشة المشتريات) | Creating purchase invoices, selecting items, and processing supplier payments. | High |
| 3 | Add Supplier Dialog (إضافة مورد) | Form to input new supplier details. | High |
| 4 | Opening Balance Dialog | Setting initial credit or debit for a supplier. | High |
| 5 | Deferred Payment Dialog (دفع آجل) | Splitting an invoice total into paid and remaining (credit) amounts. | High |
| 6 | Supplier Management (إدارة الموردين) | Managing the list of suppliers and quick-adjusting balances. | High |
| 7 | Supplier Credit Accounts (حسابات الموردين آجل) | Viewing and partially or fully settling outstanding supplier debts. | High |
| 8 | Supplier Account Statement (كشف حساب المورد) | Detailed transaction history and ledger for a specific supplier. | High |

## 3. Detailed Screen Analysis

### Screen 1: Main Dashboard (الرئيسية)

* **Screen purpose**: Main entry point and quick navigation.
* **Layout**: Top navigation bar with a large dark canvas area containing circular quick-access nodes.
* **Header**: Standard window header with "DoCashier".
* **Sidebar/navigation**: No sidebar; relies on a top horizontal menu with dropdowns (الرئيسية, إدارة المشتريات, الموردين, المنتجات, المبيعات, etc.).
* **Buttons**: Large circular nodes for main functions (e.g., green for Sales "شاشة المبيعات", red for Purchases "شاشة المشتريات").
* **Colors**: Dark grey/black background, vibrant colored nodes (Red, Green, Blue, Orange).
* **Visible validation**: None.

### Screen 2: Purchases (شاشة المشتريات)

* **Screen purpose**: Processing incoming stock and creating purchase invoices.
* **Layout**: Two-pane layout. Left side: DataGrid for invoice items. Right side: Controls for supplier selection, barcode scanning, and payment totals. A red sub-header contains tabular actions.
* **Buttons**: Add Supplier (+ icon), Edit Supplier (pencil icon), Payment options (كاش, آجل, فيزا).
* **TextBoxes**: Barcode input, Discount, Shipping fee, Paid amount.
* **ComboBoxes**: Supplier selection dropdown.
* **Tables/DataGrids**: Invoice items table with columns (Code, Name, Quantity, Price, Total, Delete icon).
* **Totals**: "إجمالي الفاتورة" (Total Invoice) displayed prominently in the bottom right corner.
* **Colors**: Dark theme with red and blue accents.

### Screen 3: Add Supplier Dialog (إضافة مورد)

* **Screen purpose**: Creating a new supplier record.
* **Layout**: Modal window with a form.
* **Buttons**: Save/Add (+ icon), Clear/New (paper icon).
* **Form fields**: Supplier Name (اسم المورد), Company Name (اسم الشركة), Phone 1, Phone 2, Address (العنوان), Notes (ملاحظات).
* **Reusable UI patterns**: Standard centered modal dialog.

### Screen 4: Deferred Payment Dialog (دفع آجل)

* **Screen purpose**: Specifying how much is paid now vs. left as debt for the supplier.
* **Layout**: Small modal window.
* **TextBoxes**: Paid amount (المدفوع), Remaining (المتبقي).
* **Buttons**: Save (حفظ / F12), Enter.
* **Calculations**: Automatically updates Remaining based on Paid input.

### Screen 5: Supplier Credit Accounts (حسابات الموردين آجل)

* **Screen purpose**: Managing and paying off supplier debts.
* **Layout**: Top search/filter bar, main DataGrid displaying debts.
* **Buttons**: Pay part of amount (تسديد جزء من المبلغ - icon), Pay full amount (تسديد - green text button).
* **Tables/DataGrids**: Columns include Date, Paid, Remaining, Invoice Total, Supplier Name, Invoice Number.
* **Dialogs**: Prompts for partial payment amount and notes (e.g., typing "instapay").

## 4. UI Components

* **Top Navigation Bar**: Persistent across all module screens (except modals). Features dropdown menus for categorizing functions.
* **DataGrids**: Uniform dark-themed grids with right-to-left alignment, frequently featuring an "Actions" column at the far left (e.g., delete row, pay debt).
* **Modal Dialogs**: Used for data entry (Add Supplier) and confirmation steps. They block interaction with the main screen.
* **Quick Action Icons**: Small square buttons with icons next to dropdowns (e.g., the "+" next to the supplier dropdown to open the add dialog).
* **Confirmation Prompts**: Standard Yes/No message boxes for critical actions (saving invoice, opening balance).

## 5. User Actions

* **User Action**: User clicks the red "شاشة المشتريات" (Purchases) circle.
* **System Response**: Opens the Purchases screen.


* **User Action**: User clicks the "+" icon next to the Supplier dropdown.
* **System Response**: Opens the "إضافة مورد" (Add Supplier) dialog.


* **User Action**: User enters "مورد تجريبي" and clicks the Add button.
* **System Response**: Saves the supplier and opens a confirmation dialog asking if they want to add an opening balance.


* **User Action**: User clicks "نعم" (Yes) for opening balance.
* **System Response**: Opens the Opening Balance dialog with options for "له" (Owed to him) and "عليه" (He owes us).


* **User Action**: User closes the Opening Balance dialog without saving.
* **System Response**: Dialog closes, returns to Purchases screen.


* **User Action**: User adds an item to the invoice (via barcode or manual selection, resulting in a 5 EGP total). User clicks "آجل" (Deferred/Credit) button.
* **System Response**: Opens the Deferred Payment modal.


* **User Action**: User types "2" in the Paid field.
* **System Response**: System calculates Remaining as "3".


* **User Action**: User clicks Save (حفظ) and confirms the dialog.
* **System Response**: Invoice is saved, drawer cash is reduced by 2 EGP, supplier debt increases by 3 EGP.


* **User Action**: User navigates to Supplier Credit Accounts, searches for the supplier, clicks partial payment, enters "1" and note "instapay", confirms.
* **System Response**: The debt row updates; remaining balance drops from 3 to 2.


* **User Action**: User clicks the "تسديد" (Pay Full) button on the row and confirms.
* **System Response**: The remaining 2 EGP debt is cleared.



## 6. Business Rules

### Confirmed

* [CONFIRMED] Creating a new supplier triggers an optional workflow to define an opening balance (initial debt/credit).
* [CONFIRMED] Opening balances can be either "له" (The business owes the supplier) or "عليه" (The supplier owes the business).
* [CONFIRMED] Purchase invoices support split payments (partially paid from the cash drawer, partially recorded as supplier debt/آجل).
* [CONFIRMED] Supplier debts can be paid off in partial installments.
* [CONFIRMED] Notes/Payment methods (like "instapay") can be attached to partial debt settlements.
* [CONFIRMED] Invoice totals dynamically calculate remaining balances when a user inputs a paid amount.

### Inferred

* [INFERRED] Adding items to a purchase invoice increases the inventory/stock levels of those items.
* [INFERRED] Paying a supplier from the POS screen decreases the current shift's cash drawer balance (the system explicitly warns "المبلغ المدفوع من الدرج" - The paid amount is from the drawer).
* [INFERRED] "Cash" (كاش) payments require full payment of the invoice amount, whereas "Credit" (آجل) allows partial.

### Unknown

* [UNKNOWN] Whether a single invoice can be split across multiple payment methods (e.g., Part Cash, Part Visa, Part Credit) simultaneously.
* [UNKNOWN] What happens if the cash drawer does not have enough funds to cover the cash portion of a purchase invoice.
* [UNKNOWN] How tax is applied or if it exists in this specific workflow.
* [UNKNOWN] If suppliers can have their debts strictly capped (Credit Limits).

## 7. Business Entities

* **Supplier (المورد)**: Has Name, Company, Phones, Address, Notes, and a Balance.
* **Product/Item (الصنف)**: Has Code, Name, Price, Quantity.
* **Purchase Invoice (فاتورة مشتريات)**: Has Supplier, Items, Discount, Shipping, Total, Paid Amount, Remaining Amount, Payment Type.
* **Supplier Transaction/Ledger (حركة حساب المورد)**: Tracks debts, payments, dates, and associated notes.
* **Drawer/Shift (الدرج/الوردية)**: Manages available cash for the active session.

## 8. Workflow

1. User navigates to Main Dashboard.
2. User opens the Purchases Screen.
3. User initiates new Supplier creation.
4. User fills supplier details and saves.
5. System prompts for Opening Balance; User skips.
6. User adds an item to the purchase invoice.
7. User selects Deferred/Credit (آجل) payment.
8. User inputs partial payment amount; System calculates remaining.
9. User confirms and saves the invoice.
10. User navigates to Supplier Credit Accounts report.
11. User searches for the newly created supplier.
12. User executes a partial payment against the outstanding debt and adds a note.
13. User executes a full payment to clear the remaining debt.
14. User navigates to Supplier Account Statement to review the full transaction history.

## 9. Calculations

* **Invoice Total**: (Sum of Item Quantities * Item Prices) + Shipping (خدمة الشحن) - Discount (خصم).
* **Deferred Payment Remaining**: Invoice Total - Paid Amount (المدفوع).
* **Supplier Debt Settlement**: Previous Outstanding Balance - Partial Payment Amount = New Outstanding Balance.

## 10. Validation

* [CONFIRMED] The system requires explicit user confirmation (Yes/No dialogs) before finalizing a purchase invoice that draws cash from the drawer.
* [CONFIRMED] The system requires confirmation before processing a debt payment.
* [INFERRED] The system likely prevents entering a "Paid" amount greater than the "Total" in the Deferred Payment dialog.

## 11. Error / Warning Messages

* **Warning/Confirmation**: "هل تريد إضافة رصيد أول المدة للمورد الجديد؟" (Do you want to add an opening balance for the new supplier?)
* **Warning/Confirmation**: "المبلغ المدفوع من الدرج هل أنت متأكد من تسجيل العملية هل تريد إتمام العملية طباعة وإغلاق؟" (The paid amount is from the drawer, are you sure you want to record the transaction, complete, print and close?)

## 12. Navigation

* Primary navigation relies on a persistent top menu bar containing hierarchical dropdowns.
* Secondary quick-access navigation is provided via large circular nodes on the main dashboard screen.
* Navigation within transactions uses modal dialogs to prevent users from leaving the screen mid-process.

## 13. Reusable UI Patterns

* **Inline Additions**: Adding master data (like a Supplier) directly from the transactional screen (Purchases) via a "+" icon next to a dropdown, without needing to navigate to a separate module.
* **Action Columns**: DataGrids heavily utilize the far-left or far-right columns for row-specific actions (Delete icon in invoice items, Payment icons in supplier accounts).
* **Standardized Confirmations**: Consistent black/dark-themed confirmation dialogs with checkmark (Yes) and X (No) icons for all major state changes.

## 14. Questions / Unknowns

* Not determinable from this video: Can an invoice item's price be edited directly in the datagrid, or does it inherit strictly from the master product record?
* Not determinable from this video: Are there user permission levels restricting who can pay off supplier debts or add opening balances?
* Not determinable from this video: Does deleting an invoice automatically reverse the supplier's debt and the drawer's cash?

## 15. Important Findings

* The application explicitly separates the concepts of "Supplier Management" (editing details), "Supplier Credit Accounts" (viewing and settling open debts), and "Supplier Account Statements" (viewing historical ledgers) into completely different screens accessible via the top menu.
* The payment flow prioritizes drawer tracking, specifically warning the user that cash is leaving the register during a purchase, which indicates tight cash-drawer control is a core design principle of this POS.