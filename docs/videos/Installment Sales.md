# Video Analysis

## 1. Video Overview

The video demonstrates an Arabic-language desktop Point of Sale (POS) and business management application (DoCoshier) operating in a dark theme. The specific workflow recorded focuses entirely on the "Installments" (التقسيط) module. The user demonstrates how to create a new installment sale, generate an installment payment schedule, search for a customer's pending installments, register a payment, view installments due on the current day, and print an installment contract.

## 2. Screens Discovered

| ID | Screen | Purpose | Confidence |
| --- | --- | --- | --- |
| 1 | Main Dashboard (الرئيسية) | Main navigation and quick access to core system modules. | High |
| 2 | Installment Sale (بيع بالتقسيط) | Creating new installment contracts and calculating payment schedules. | High |
| 3 | Pay Installments (سداد الاقساط) | Finding customer contracts and marking specific installments as paid. | High |
| 4 | Installments Due Today (الاقساط المستحقة الدفع اليوم) | Listing customers who have payments due on the current system date. | High |
| 5 | Print Preview (طباعة) | Viewing and printing the generated installment contract. | High |

## 3. Detailed Screen Analysis

### Screen 1: Main Dashboard (الرئيسية)

* **Purpose**: Central hub for the application.
* **Layout**: Top navigation bar with dropdown menus. Main central area contains a network-like arrangement of large circular buttons, with smaller circular buttons below.
* **Header**: Dark blue bar containing dropdown menus.
* **Sidebar/navigation**: None (uses top menu).
* **Buttons**: Large circular buttons for "Sales Screen" (شاشة المبيعات), "Purchases Screen" (شاشة المشتريات), etc.
* **Colors**: Dark gray background, blue header, green/red/blue/gray circular buttons.

### Screen 2: Installment Sale (بيع بالتقسيط)

* **Purpose**: Entry form for creating an installment sale.
* **Layout**: Right-to-Left (RTL) layout. A vertical data entry form on the right panel. A wide DataGrid occupies the center/left area to display the generated schedule.
* **Header**: Breadcrumb style: "الرئيسية / التقسيط / بيع بالتقسيط".
* **Buttons**: "اتمام العملية" (Complete Process) with a checkmark icon, "جديد" (New) with a plus icon, "طباعة" (Print) with a printer icon. Navigation arrows for records.
* **ComboBoxes**: "إسم المنتج" (Product Name), "العميل" (Customer).
* **TextBoxes**: "الكمية" (Quantity), "رقم قومي" (National ID), "سعر المنتج" (Product Price), "نسبة القسط" (Installment Percentage), "إجمالي القسط" (Total Installment - read-only), "عدد الاقساط" (Number of Installments), "مبلغ القسط" (Installment Amount - auto-calculated).
* **Tables/DataGrids**: 6 columns: مسلسل (Serial), إسم المنتج (Product Name), إسم العميل (Customer Name), إسم البائع (Seller Name), مبلغ القسط (Installment Amount), تاريخ القسط (Installment Date).
* **System Time**: Visible at the bottom of the form: "07:42:02 2020-02-02".

### Screen 3: Pay Installments (سداد الاقساط)

* **Purpose**: Registering payments against existing installment plans.
* **Layout**: Search bar at the top right. Main DataGrid taking up the rest of the screen.
* **Search controls**: A ComboBox/Search box for customer name with a magnifying glass icon.
* **Tables/DataGrids**: Shows pending installments for the selected customer.
* **Dialogs**: A "تأكيد" (Confirm) dialog appears when paying, asking "هل انت متأكد من تسديد القسط ؟" (Are you sure you want to pay the installment?) with Yes/No buttons.

### Screen 4: Installments Due Today (الاقساط المستحقة الدفع اليوم)

* **Purpose**: Daily tracking of due payments.
* **Layout**: Simple top bar with a search icon, large DataGrid for results.
* **Buttons**: Search (Magnifying glass).

### Screen 5: Print Preview (طباعة)

* **Purpose**: Generating a physical contract for the installment.
* **Layout**: Standard A4 document view against a dark background.
* **Document Details**:
* Title: "عقد بيع بالتقسيط" (Installment Sale Contract).
* Header Data: Customer Name, National ID, Address, Phone, Date.
* Financial Totals Grid: Product Price, Paid Amount, Interest Percentage, Total Installment.
* Schedule Grid: Installment Date, Amount, Status.
* Footer: Signature lines for Buyer (المشتري) and Guarantor (الضامن).



## 4. UI Components

* **Top Navigation Menu**: Dropdown style, pure text, expands on hover or click.
* **Toast Notifications**: Appear in the bottom right corner. Green checkmark for success, Red 'X' for error. They slide up and disappear after a few seconds.
* **DataGrids**: Standard tabular display with alternating row shading (subtle), highlighted selection row.
* **Action Buttons**: Flat design, usually paired with an icon on the right side of the text (due to RTL).
* **Form Fields**: Label on the right, input field on the left.

## 5. User Actions

* **User Action**: Clicks Top Menu -> "التقسيط" -> "بيع بالتقسيط".
* **System Response**: Opens the Installment Sale screen.


* **User Action**: Enters "10" in Product Price, "5" in Number of Installments.
* **System Response**: Auto-calculates Installment Amount field to "2".


* **User Action**: Clicks "اتمام العملية" (Complete Process).
* **System Response**: Populates the DataGrid with 5 scheduled payment rows spaced exactly one month apart.
* **Data affected**: A new installment contract and 5 installment records are generated.


* **User Action**: Opens "سداد الاقساط" (Pay Installments), types customer name "محمد" and selects it.
* **System Response**: Populates the DataGrid with the 5 unpaid installments for "محمد".


* **User Action**: Double-clicks/Selects the first installment row.
* **System Response**: Opens confirmation dialog: "Are you sure you want to pay?".


* **User Action**: Clicks "نعم" (Yes).
* **System Response**: Shows green success toast "تم سداد القسط بنجاح". Removes the paid row from the UI grid.
* **Data affected**: The selected installment status is updated to Paid.


* **User Action**: Opens "الاقساط المستحقة الدفع اليوم" (Due Today) and clicks Search.
* **System Response**: Shows red error toast "لا يوجد اقساط في هذا اليوم" (No installments on this day). Grid remains empty.


* **User Action**: Clicks "طباعة" (Print) on the Installment Sale screen.
* **System Response**: Opens the print preview window with the formatted contract.



## 6. Business Rules

### Confirmed

* Installment schedules are generated monthly. The first payment is due exactly one month from the creation date.
* `Installment Amount = Total Price / Number of Installments`.
* A customer can be identified by Name and National ID (رقم قومي).
* Installments must be individually confirmed to be marked as paid.
* Paying an installment removes it from the "Pending Installments" grid.
* The system records the name of the user/seller who created the installment (visible as "شغف").
* The system uses the current system date to determine "Due Today" installments.

### Inferred

* The "Total Installment" field (إجمالي القسط) is likely calculated as `Price + (Price * Interest Percentage)`. The video showed 0% interest, so Total equaled Price.
* Products can be selected from a predefined inventory list via the ComboBox.
* Customers can either be searched from existing records or typed in as new entries directly in the form.
* The system prevents paying an installment twice (since it disappears from the pending list).

### Unknown

* Whether the system enforces sequential payment of installments (e.g., preventing payment of month 3 if month 2 is unpaid).
* How inventory levels are updated when an installment sale is completed.
* Whether a down payment (مقدم) feature exists, as it was not utilized in the video.
* What happens to overdue installments (are there penalties, or do they just roll over?).

## 7. Business Entities

* **Product** (المنتج)
* **Customer** (العميل)
* **User / Seller** (البائع / المستخدم)
* **Installment Contract / Sale** (عملية تقسيط)
* **Installment Payment** (قسط)

## 8. Workflow

1. User navigates to the Installment Sales module.
2. User selects a product, sets quantity and price.
3. User selects or enters a customer and their National ID.
4. User defines the interest rate and the number of months (installments).
5. System calculates the monthly payment.
6. User finalizes the transaction, generating a multi-month schedule.
7. User can optionally print a physical contract for signatures.
8. On a later date, the user navigates to the Payment screen.
9. User searches for the specific customer.
10. User selects a pending installment and confirms payment.
11. System marks it as paid and removes it from the pending view.

## 9. Calculations

* **Monthly Installment Amount** = `إجمالي القسط` (Total Installment) / `عدد الاقساط` (Number of Installments). (Demonstrated: 10 / 5 = 2).
* **First Installment Date** = `Current System Date` + 1 Month. (Demonstrated: Creation date 2020-02-02 -> First payment 2020-03-02).
* **Subsequent Installment Dates** = `Previous Installment Date` + 1 Month.

## 10. Validation

* [INFERRED] Auto-calculated fields (Total Installment, Installment Amount) are read-only and cannot be manually overridden.
* System validates if there are actually any records to show before populating the "Due Today" grid, using a notification to handle the "empty state".

## 11. Error / Warning Messages

* **Success Toast (Green)**: "تم سداد القسط بنجاح" (Installment paid successfully) - Shown after confirming a payment.
* **Error/Info Toast (Red)**: "خطأ - لا يوجد اقساط في هذا اليوم" (Error - No installments on this day) - Shown when searching for today's due installments and the query returns 0 results.
* **Confirmation Dialog**: "تأكيد - هل انت متأكد من تسديد القسط ؟" (Confirm - Are you sure you want to pay the installment?) - Requires explicit User opt-in to modify financial data.

## 12. Navigation

* Global navigation is handled exclusively through a persistent top menu bar containing hierarchical dropdown lists.
* No tabbed browsing is visible; navigating to a new screen replaces the current screen content.
* Screens include breadcrumb-style headers (e.g., الرئيسية > التقسيط > سداد الاقساط) indicating the current location in the module hierarchy.

## 13. Reusable UI Patterns

* **RTL Layout**: All forms and tables are strictly Right-to-Left. Labels are on the right, inputs on the left. Table columns start from the right.
* **Toast Notifications**: Used globally for non-blocking feedback (Success, Empty Search Results).
* **Dual-Pane Entry Screens**: Data entry on the right pane, real-time tabular data visualization on the left/center pane.
* **Search & Execute**: Screens designed for action (like payments) feature a prominent top search bar that must be utilized to populate the actionable grid below.

## 14. Questions / Unknowns

* How are Guarantor (الضامن) details entered? The print preview has a signature line for a Guarantor, but the UI form did not show fields for a Guarantor's name or ID.
* Is it possible to edit an installment sale after clicking "اتمام العملية" (Complete Process), or is it locked?
* How does the user handle partial payments for an installment?
* Can a single installment schedule contain multiple different products, or is it strictly 1 Contract = 1 Product Line?

## 15. Important Findings

* The system date during the recording is strictly set to **2020-02-02**, which is crucial for understanding the generated dates in the video.
* The system uses empty search results as an "Error" state (red toast message) rather than simply displaying an empty grid.
* Calculations are strictly live on the UI. The moment the user changes the "Number of Installments" from 1 to 5, the "Installment Amount" instantly updates without needing to click a calculation button.