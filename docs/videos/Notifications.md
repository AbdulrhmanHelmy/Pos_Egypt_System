# Video Analysis

## 1. Video Overview

The 1-minute and 56-second video demonstrates the "Alerts/Notifications" (التنبيهات) module of an Egyptian desktop POS and business management application named "Docashier" (as noted in the bottom right copyright footer). The video acts as a guided tour of five specific alert screens: Low Stock, Supplier Payables, Customer Receivables, Nearing Expiration, and Due Installments. Additionally, it briefly demonstrates the "Add Item" dialog to explain how low stock triggers are configured. The application uses a dark-themed user interface with a top navigation bar and a central visual dashboard.

## 2. Screens Discovered

| ID | Screen | Purpose | Confidence |
| --- | --- | --- | --- |
| 1 | Main Dashboard (الرئيسية) | Central navigation hub containing menus, visual nodes, and quick action buttons. | High |
| 2 | Low Stock Alerts (منتجات اوشكت على النفاذ) | Displays products whose current quantity has reached or fallen below the configured reorder limit. | High |
| 3 | Add Item Dialog (إضافة صنف) | A multi-step form to create a new product and define its inventory properties. | High |
| 4 | Supplier Payables (مبالغ مستحقة الدفع - موردين) | Tracks outstanding invoices and amounts owed to suppliers with their due dates. | High |
| 5 | Customer Receivables (مبالغ مستحقة الدفع - عملاء) | Tracks outstanding invoices and amounts owed by customers with their due dates. | High |
| 6 | Nearing Expiration Alerts (منتجات اقتربت على إنتهاء الصلاحية) | Displays products approaching their expiration dates with time-based filters. | High |
| 7 | Due Installments (أقساط مستحقة) | Manages and tracks customer installment payments, separating overdue from current-day dues. | High |

## 3. Detailed Screen Analysis

### Screen 1: Main Dashboard

* **Layout**: Full window. Top horizontal menu bar. Central canvas area with interconnected circular nodes. Bottom row of smaller circular quick-action buttons. Bottom status bar.
* **Header**: Standard window controls (not clearly visible but implied), Top Menu containing: الرئيسية (Main), المبيعات (Sales), المشتريات (Purchases), المرتجعات (Returns), الحسابات (Accounts), المصروفات والسلفيات (Expenses & Advances), الخزنة والبنك (Safe & Bank), إدارة الموظفين (Employee Management), الاعدادات (Settings).
* **Buttons**: Large circular nodes (e.g., شاشة المبيعات in green, شاشة المشتريات in red, شاشة المرتجعات). Small circular buttons at the bottom (e.g., إضافة صنف, مصروفات, تقارير, مبيعات مالية).
* **Colors**: Dark grey/charcoal background (`#202020` approx). Menu bar is black/dark blue. Accents in bright colors (Green, Red, Blue) for the dashboard nodes.
* **Typography**: Arabic UI, sans-serif font (likely Segoe UI or Tahoma).

### Screen 2: Low Stock Alerts

* **Layout**: Top menu remains. The main body is replaced by a full-width DataGrid. A footer section contains a search bar and totals.
* **Tables/DataGrids**: One main table.
* *Columns*: كود الصنف (Item Code), اسم الصنف (Item Name), القسم (Category), المورد (Supplier), رقم التليفون (Phone Number), الكمية الموجودة (Available Quantity), حد الطلب (Reorder Limit).


* **Search Controls**: A text box in the bottom left labeled "بحث بإسم المورد" (Search by Supplier Name).
* **Totals**: A label in the bottom right corner showing "إجمالي العدد المتبقي فقط: 0" (Total remaining count only: 0).

### Screen 3: Add Item Dialog

* **Layout**: A floating modal dialog overlaying the main dashboard. Dark background. Appears to have two visible sections/tabs during the video.
* **Form Fields**:
* *Section 1 (بيانات الصنف)*: اسم الصنف (Item Name), المجموعة (Group), التصنيف الفرعي (Subcategory), نوع المنتج (Product Type), الوحدة (Unit). These appear to be ComboBoxes or TextBoxes with "Add" (+) and "Edit" (pencil) icons next to them.
* *Section 2 (خصائص المخزون)*: رقم الباركود (Barcode Number), مقاس (Size), الكمية الحالية (Current Quantity), حد الطلب (Reorder Limit).


* **Checkboxes**: لا يؤثر على المخزون (Does not affect inventory), يدعم ميزان باركود (Supports barcode scale), يدعم بيع بطول ميزان باركود (Supports sale by length on scale).
* **Buttons**: "التالي >" (Next) in the bottom left, indicating a wizard-style flow, and "Cancel".
* **Visible Validation**: Quantity fields display decimal formatting (e.g., `0.000`).

### Screen 4: Supplier Payables

* **Layout**: Full-width DataGrid below the top menu.
* **Tables/DataGrids**: One main table.
* *Columns*: رقم الفاتورة (Invoice Number), اسم المورد (Supplier Name), اسم الشركة (Company Name), المبلغ المستحق (Amount Due), تاريخ الدفع (Payment Date).



### Screen 5: Customer Receivables

* **Layout**: Full-width DataGrid below the top menu.
* **Tables/DataGrids**: One main table.
* *Columns*: رقم الفاتورة (Invoice Number), اسم العميل (Customer Name), المبلغ المستحق (Amount Due), تاريخ الدفع (Payment Date).



### Screen 6: Nearing Expiration Alerts

* **Layout**: Full-width DataGrid with three distinct toggle filter buttons anchored at the top.
* **Filters**: Three large horizontal buttons acting as time filters: "هذا الشهر" (This Month) in blue, "هذا الاسبوع" (This Week) in light blue, "هذا اليوم" (Today) in cyan.
* **Tables/DataGrids**:
* *Columns*: رقم الفاتورة (Invoice No.), الصنف (Item), اللون (Color), الوحدة (Unit), المقاس (Size), الباركود (Barcode), المورد (Supplier), اسم العميل (Customer Name), تاريخ الانتاج (Production Date), تاريخ الانتهاء (Expiration Date), الصلاحية (Validity), الكمية (Quantity), الخصم (Discount), الاجمالي (Total), تم الدفع للمورد (Paid to Supplier), تم إرجاع للمورد (Returned to Supplier).



### Screen 7: Due Installments

* **Layout**: Full-width DataGrid with two large toggle filter buttons at the top. Bottom footer with totals.
* **Filters**: "باقي الأقساط المتأخرة" (Remaining Overdue Installments) in red, "باقي أقساط اليوم" (Remaining Today's Installments) in green.
* **Tables/DataGrids**:
* *Columns*: مسلسل (Serial), رقم العملية (Operation Number), اسم المنتج (Product Name), اسم العميل (Customer Name), سعر القسط (Installment Price), تاريخ السداد (Payment Date).


* **Totals**: Bottom footer contains labels: "إجمالي مبالغ الأقساط" (Total Installment Amounts), "إجمالي مبالغ اليوم" (Total Today's Amounts), "إجمالي السداد اليوم" (Total Paid Today).

## 4. UI Components

* **Top Navigation Menu**: Dropdown-based menu system for primary navigation.
* **Dashboard Nodes**: Interactive graphical elements acting as shortcuts.
* **DataGrids**: Used extensively for listing alerts. Includes column drag-and-drop grouping functionality (indicated by the header text "Drag a column header here to group by that column").
* **Modal Dialogs**: Floating windows with dimmed background for data entry (Add Item).
* **Wizard Navigation**: "Next" / "Cancel" buttons for multi-step forms.
* **Contextual Footer**: A persistent bar at the bottom of datagrid screens used for context-specific search (Supplier name) or aggregated totals.
* **Time-based Toggle Buttons**: Large, colored rectangular buttons used to filter grid data (e.g., Today, Week, Month).

## 5. User Actions

* **User Action**: User hovers and clicks "التنبيهات" (Alerts) on the top menu.
* **System Response**: Opens a dropdown menu with 5 options.
* **UI Change**: Dropdown list becomes visible.


* **User Action**: User clicks "منتجات اوشكت على النفاذ" (Low Stock).
* **System Response**: Navigates to the Low Stock screen.
* **UI Change**: Main dashboard canvas is replaced by a DataGrid.


* **User Action**: User clicks the "إضافة صنف" (Add Item) circular button on the dashboard.
* **System Response**: Opens the item creation wizard.
* **UI Change**: A modal dialog appears over the screen.


* **User Action**: User clicks "مبالغ مستحقة الدفع (موردين)" (Supplier Payables) from the menu.
* **System Response**: Navigates to the Supplier Payables screen.
* **UI Change**: DataGrid updates to show supplier debts.


* **User Action**: User clicks "مبالغ مستحقة الدفع (عملاء)" (Customer Receivables) from the menu.
* **System Response**: Navigates to the Customer Receivables screen.
* **UI Change**: DataGrid updates to show customer debts.


* **User Action**: User clicks "منتجات اقتربت على إنتهاء الصلاحية" (Nearing Expiration) from the menu.
* **System Response**: Navigates to the Expiration alerts screen.
* **UI Change**: DataGrid loads with time-filter buttons at the top.


* **User Action**: User clicks "أقساط مستحقة" (Due Installments) from the menu.
* **System Response**: Navigates to the Due Installments screen.
* **UI Change**: DataGrid loads with "Overdue" and "Today" filter buttons at the top.



## 6. Business Rules

### Confirmed

* Products have a configurable "Reorder Limit" (حد الطلب).
* An alert is generated when a product's current quantity reaches or falls below the "Reorder Limit".
* The system tracks due dates for financial obligations to suppliers (Payables) and from customers (Receivables).
* Products support production dates and expiration dates.
* Expiration alerts can be filtered by timeframe (Today, This Week, This Month).
* The system manages installment payments (أقساط) and categorizes them by current day's due vs. overdue.
* Products can be explicitly flagged to *not* affect inventory tracking via a checkbox.
* Products can be configured to integrate with barcode scales, including selling by length.

### Inferred

* Expiration dates are captured at the time of purchase/stock entry (narrator mentions this verbally).
* Installments are linked to specific sales operations/invoices, tracked by "رقم العملية" (Operation Number).
* The system dynamically aggregates total amounts (quantities and money) based on the currently filtered view in the datagrids.
* Alerts are dynamically populated based on real-time transaction data (sales reducing inventory, payments reducing debt).

### Unknown

* How a notification is dismissed or acknowledged (e.g., does it disappear automatically once the threshold is crossed back, or does it require manual dismissal?).
* How partial payments affect the Payables/Receivables lists.
* What happens when a product is completely expired.
* How the user physically navigates from the Alert screen to the actual document (Invoice/Product) to take action.

## 7. Business Entities

* Product / Item (صنف / منتج)
* Category / Subcategory (مجموعة / تصنيف فرعي)
* Supplier (مورد)
* Customer (عميل)
* Company (شركة)
* Purchase / Sale Invoice (فاتورة)
* Installment (قسط)
* Operation (عملية)

## 8. Workflow

1. User logs into the system and lands on the Main Dashboard.
2. When configuring a new item, the user opens "إضافة صنف" (Add Item).
3. The user enters standard item details and navigates to "خصائص المخزون" (Inventory Properties).
4. The user sets a specific numeric value in "حد الطلب" (Reorder Limit).
5. As sales occur, inventory drops. When inventory <= Reorder Limit, the system flags the item.
6. The user clicks "التنبيهات" -> "منتجات اوشكت على النفاذ" to view all flagged items to make purchasing decisions.
7. Similarly, as purchase invoices are entered with due dates, the system flags them.
8. The user checks "مبالغ مستحقة الدفع (موردين)" to see which suppliers need to be paid today.
9. The user checks "أقساط مستحقة" to see which customers need to be contacted for installment collections.

## 9. Calculations

* [CONFIRMED] Total remaining count (إجمالي العدد المتبقي فقط) on the low stock screen.
* [CONFIRMED] Total installment amounts (إجمالي مبالغ الأقساط) and Total today's amounts (إجمالي مبالغ اليوم).
* [INFERRED] Current Quantity calculation: Total Purchased - Total Sold - Total Returned.
* [INFERRED] Time remaining until expiration (comparing System Date vs. Expiration Date) to populate Today/Week/Month buckets.

## 10. Validation

* [CONFIRMED] Quantity limits and current quantities support three decimal places (`0.000`).
* [INFERRED] Reorder limit must be a valid numeric type.
* [INFERRED] Payment dates and expiration dates must be valid Date types.
* [UNKNOWN] Which fields are strictly mandatory when creating a new product.

## 11. Error / Warning Messages

* [CONFIRMED] No explicit popup error messages or validation warnings were triggered during the video. The entire module itself acts as a passive warning system.

## 12. Navigation

* [CONFIRMED] Primary navigation occurs via the top dropdown menu.
* [CONFIRMED] Multi-step forms use sequential wizard navigation (Next / Cancel buttons).
* [UNKNOWN] It is not determinable from this video if double-clicking a row in a DataGrid navigates to the detailed record.

## 13. Reusable UI Patterns

* **DataGrid + Footer Pattern**: Almost all list screens use a full-width DataGrid paired with a dark footer bar containing either a contextual search box or aggregated calculation labels.
* **Top-Aligned Filter Toggles**: Reports/Lists that require time-based segmentation (Expiration, Installments) use large, full-width colored toggle buttons directly above the DataGrid to switch views instantly.
* **Dark Theme Contrast**: Dark grey backgrounds (`#202020`) with bright, distinct colors for primary interaction points (Blue for Next/Save, Red for Overdue, Green for Today).

## 14. Questions / Unknowns

* Not determinable from this video: Can the alerts be exported to Excel or printed directly from these screens? (Though the narrator mentions "printing a list to buy", no print button is visibly clicked).
* Not determinable from this video: Does the system support email or SMS notifications for these alerts, or are they strictly in-app UI alerts?
* Not determinable from this video: How are the visual dashboard nodes customized or linked to these alerts?

## 15. Important Findings

* The system is designed to handle complex retail environments, explicitly supporting installment-based sales, barcode scales, and precise expiration date tracking.
* The Alerts module acts as a proactive daily task list for the business owner, aggregating debts, collections, purchasing needs, and loss-prevention (expiring items) into a single menu.
* The presence of three decimal places on quantities suggests the system supports fractional units (e.g., weight in kilograms or length in meters), which aligns with the "barcode scale" and "sell by length" checkboxes found in the Add Item dialog.