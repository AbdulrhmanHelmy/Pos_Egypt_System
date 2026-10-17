Here is the comprehensive reverse-engineering analysis of the provided reference video for the DoCashier application.

# Video Analysis

## 1. Video Overview

The video demonstrates the core workflow of the "Purchases" (المشتريات) module within the DoCashier Egyptian desktop POS system. The system operates in Arabic (RTL layout) and features a dark-themed UI. The recorded workflow covers navigating from the main dashboard, creating a new supplier, handling an opening balance prompt, adding items to a purchase invoice via manual selection, applying a credit payment, viewing the purchases report, and assigning product expiration dates.

## 2. Screens Discovered

| ID | Screen | Purpose | Confidence |
| --- | --- | --- | --- |
| 01 | Main Dashboard (الرئيسية) | Central navigation hub with main modules. | High |
| 02 | Purchases Screen (شاشة المشتريات) | Creating and managing purchase invoices. | High |
| 03 | Add Supplier Dialog (إضافة مورد) | Creating a new supplier record. | High |
| 04 | Confirmation Dialog (تأكيد) | Prompting for opening balance. | High |
| 05 | Credit Payment Dialog (دفع آجل) | Handling deferred/partial payments. | High |
| 06 | Purchases Reports (تقارير المشتريات) | Viewing historical purchase invoices. | High |
| 07 | Product Expirations (صلاحية المنتجات) | Assigning production and expiration dates. | High |

## 3. Detailed Screen Analysis

### Screen 1: Main Dashboard (الرئيسية)

* **Purpose:** Application entry point and module routing.
* **Layout:** Dark background with a prominent central cluster of large circular buttons and a top navigation menu. Bottom area contains smaller icon-based navigation.
* **Header:** Standard window controls, system logo/name (DoCashier), top menu bar with dropdowns.
* **Buttons:** Large Red Circle ("شاشة المشتريات" - Purchases), Large Green Circle ("شاشة المبيعات" - Sales), POS button, User settings, etc.
* **Colors:** Dark grey/black background, vibrant red, green, and blue accents.
* **Fonts/typography:** Standard Arabic sans-serif (likely Segoe UI or Tahoma).

### Screen 2: Purchases Screen (شاشة المشتريات)

* **Purpose:** Core data entry for buying stock from suppliers.
* **Layout:** Split vertically. Right side: Supplier selection and barcode scanning. Left/Center: Main DataGrid for invoice items. Bottom: Financial totals and checkout actions.
* **Header:** Red strip containing column headers for the DataGrid.
* **Buttons:** Add Supplier (+ icon), Search, Cash (كاش), Credit (آجل), Visa (فيزا).
* **TextBoxes:** Barcode scan input, Quantity, Purchase Price, Discount, Shipping Cost.
* **ComboBoxes:** Supplier selection dropdown.
* **Tables/DataGrids:** Main table showing: Code, Item Name, Unit, Quantity, Purchase Price, Selling Price, Total, Edit/Delete icons.
* **Totals:** Bottom right area displaying subtotal, discount, shipping, and grand total.

### Screen 3: Add Supplier Dialog (إضافة مورد)

* **Purpose:** Registering a new supplier without leaving the invoice screen.
* **Layout:** Modal window, form-based grid.
* **Form fields:** Supplier ID (auto-generated), Supplier Name ("اسم المورد"), Company Name, Phone, Address.
* **Buttons:** Save ("تسجيل" / + icon).
* **Visible validation:** None explicitly triggered in the video, but fields are structured.

### Screen 4: Confirmation Dialog (تأكيد)

* **Purpose:** Asking the user if the new supplier has an existing financial balance.
* **Text:** "هل تريد اضافة رصيد أول المدة للمورد الجديد؟" (Do you want to add an opening balance for the new supplier?)
* **Buttons:** Yes (نعم), No (لا).

### Screen 5: Credit Payment Dialog (دفع آجل)

* **Purpose:** Finalizing an invoice where the full amount is not paid upfront.
* **Layout:** Small modal window.
* **TextBoxes:** Amount Paid.
* **Labels:** Total amount, Remaining amount.
* **Buttons:** Confirm/Enter (F12).

### Screen 6: Purchases Reports (تقارير المشتريات)

* **Purpose:** Auditing past purchases.
* **Layout:** Top filter bar, large central DataGrid.
* **Tables/DataGrids:** Shows Invoice Number, Date, Remaining Balance, Paid Amount, Discount, User, Supplier Name.
* **Search controls:** Text input for filtering.

### Screen 7: Product Expirations (تعيين تاريخ صلاحية)

* **Purpose:** Managing inventory for perishable goods (Supermarket context).
* **Form fields:** Product dropdown, Production Date (تاريخ الانتاج), Expiration Date (تاريخ الانتهاء).
* **Buttons:** Save Operation (حفظ العملية).
* **Status indicators:** A notification area indicating items approaching expiration.

## 4. UI Components

* **Modals:** Used heavily for data entry without losing context of the primary screen.
* **DataGrids:** Unified styling across the system (dark background, red headers, alternating row colors).
* **Dropdowns with Search:** The supplier and product dropdowns appear to support typing to filter.
* **Iconography:** Standardized flat icons used for saving (floppy disk/check), adding (+), and payment methods.

## 5. User Actions

* **User Action:** Clicks "شاشة المشتريات"
* **System Response:** Navigates to the Purchases screen.


* **User Action:** Clicks red "+" icon next to the supplier dropdown.
* **System Response:** Opens "إضافة مورد" (Add Supplier) modal.


* **User Action:** Types "مورد تجريبي" and clicks Save.
* **System Response:** Opens a confirmation dialog asking about opening balance.


* **User Action:** Clicks "لا" (No) on opening balance.
* **System Response:** Shows "تم الحفظ بنجاح" success checkmark dialog, closes modal, returns to invoice.


* **User Action:** Selects product "FFF" from dropdown.
* **System Response:** Adds product row to the main DataGrid with default quantity of 1.
* **UI change:** Grand total updates.


* **User Action:** Modifies Quantity and Purchase Price.
* **System Response:** Recalculates row total and invoice grand total.


* **User Action:** Clicks "آجل" (Credit).
* **System Response:** Opens Credit Payment modal.


* **User Action:** Leaves "Paid" at 0 and confirms.
* **System Response:** Calculates "Remaining" as 1, saves invoice, shows success message.
* **Data affected:** Supplier balance increases by 1, Inventory increases.


* **User Action:** Navigates to Expiration Dates, selects product, sets dates, clicks save.
* **System Response:** Saves record, evaluates expiration window, populates notification panel if within 1 month.



## 6. Business Rules

### Confirmed

* **Supplier Creation Workflow:** Creating a supplier prompts an optional "Opening Balance" entry.
* **Invoice Population:** Products can be added via barcode scanner or manual dropdown selection.
* **Pricing Overrides:** The purchase price can be manually overridden per invoice line item; it does not strictly lock to the product master file.
* **Cost Adjustments:** "Shipping Service" (خدمة شحن) costs are added to the invoice total.
* **Cash Drawer Integration:** Checking a specific box deducts the paid cash from the user's current shift drawer (الدرج).
* **Deferred Payments (آجل):** Invoices can be saved with zero or partial payment, shifting the remaining total to the supplier's payable balance.
* **Expiration Tracking:** Products can have explicit production and expiration dates.
* **Expiration Alerts:** The system generates a notification when a product is 1 month away from its expiration date.

### Inferred

* **Inventory Impact:** Saving a purchase invoice automatically increases the on-hand quantity of the purchased items.
* **Financial Impact:** Choosing 'Credit' creates a payable account entry for the selected supplier.
* **Product Master Sync:** If the purchase price is modified during the invoice, the system likely prompts or automatically updates the moving average cost or last purchase price on the product master.

### Unknown

* How are product returns (Purchase Returns) handled?
* Are there user permissions/roles that restrict who can edit purchase prices or issue credit payments?
* Does the system support multiple warehouses or locations?
* What happens to the inventory if an expiration date is passed? Does it hard-lock sales of that item?

## 7. Business Entities

* **Supplier (المورد):** Contains ID, Name, Company, Phone, Address, Balance.
* **Product (المنتج/الصنف):** Contains Barcode, Name, Purchase Price, Selling Price.
* **Purchase Invoice (فاتورة المشتريات):** Contains Invoice Number, Date, Supplier ID, Total, Discount, Shipping, Paid, Remaining, Payment Method.
* **Purchase Invoice Item (عنصر الفاتورة):** Contains Invoice ID, Product ID, Quantity, Unit, Unit Price, Line Total.
* **Drawer/Shift (الدرج/الوردية):** Tracks available cash for the current session.
* **Expiration Record (صلاحية):** Maps a Product ID to a Batch/Production Date and Expiration Date.

## 8. Workflow

1. User logs into the application (Main Dashboard).
2. User selects "Purchases Screen".
3. User adds a new Supplier (fills form -> skips opening balance -> saves).
4. User selects the new Supplier for the current invoice.
5. User adds items (via scan or dropdown).
6. User adjusts Quantities and Purchase Prices.
7. User applies any Discounts or Shipping costs.
8. User selects Payment Method (e.g., Credit).
9. User enters the amount paid (if partial/zero) and confirms.
10. System saves the invoice and updates supplier balance/inventory.
11. User navigates to Expiration module to tag incoming perishable stock with dates.

## 9. Calculations

* **Line Item Total** = Quantity × Purchase Price
* **Invoice Subtotal** = Sum of all Line Item Totals
* **Invoice Grand Total** = (Invoice Subtotal - Discount) + Shipping Cost
* **Remaining Balance (الباقي)** = Invoice Grand Total - Amount Paid

## 10. Validation

* **Supplier Opening Balance:** System intercepts supplier creation to ensure the opening balance is explicitly addressed (Yes/No).
* **Payment Amounts:** The Credit payment dialog calculates remaining balance in real-time, preventing users from entering a paid amount greater than the total without notice.

## 11. Error / Warning Messages

* **Success Message:** "تم الحفظ بنجاح" (Saved successfully) - appears as a center-screen modal with a large blue checkmark.
* **Expiration Warning:** "منتجات قاربت على انتهاء الصلاحية" (Products nearing expiration) - appears in a notification list.

## 12. Navigation

* **Hub and Spoke:** The Main Dashboard acts as the hub. Modules are opened in separate views.
* **Top Menu Bar:** Used for secondary navigation (e.g., Reports, Settings) without returning to the main dashboard.
* **Hotkeys:** Explicit mention/showing of F-keys (e.g., F12 to confirm payment).

## 13. Reusable UI Patterns

* **Standardized Modals:** Dialogs for adding suppliers, confirming actions, and making payments share the same dark-background, modal-overlay pattern.
* **Success Prompts:** The system relies on explicit, interrupting success dialogues (checkmarks) rather than passive toast notifications for primary actions.
* **Searchable Comboboxes:** Dropdowns for entities (Suppliers, Products) consistently feature a search/type-to-filter capability.

## 14. Questions / Unknowns

* What database backend is expected for this volume of transactions?
* How are units of measure (UOM) handled? (The video shows a "Unit" column, but it is not interacted with).
* Is there a barcode generation feature for the expiration dates, or are they purely tracked logically in the database?

## 15. Important Findings

* **Supermarket Specifics:** The system is explicitly tailored to handle supermarket workflows, as evidenced by the dedicated module for Production/Expiration dates and the 1-month warning threshold.
* **Drawer Deduction:** The checkbox to deduct payments from the "Drawer" (الدرج) indicates a tight integration between back-office purchasing and front-of-house cash management. This means the system uses a unified cash flow model per shift.
* **Frictionless Data Entry:** The system allows creating complex entities (like Suppliers) directly from the transaction screen via modals, ensuring the user does not lose their current invoice state.