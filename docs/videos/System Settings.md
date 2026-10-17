# Video Analysis

## 1. Video Overview

The provided video is a 2-minute demonstration of a desktop POS/Business application named "DoCashier" (as seen on the desktop icon). The video primarily focuses on the "System Settings" (إعدادات النظام) module, showcasing how a user configures printers, invoice details, general system behaviors, bulk price updates, and receipt/interface layout coordinates. The entire application is in Arabic and operates in a Right-to-Left (RTL) layout.

## 2. Screens Discovered

| ID | Screen | Purpose | Confidence |
| --- | --- | --- | --- |
| 1 | Main Dashboard | Entry point of the application showing main modules as large circular buttons. | High |
| 2 | System Settings (إعدادات النظام) | Main configuration hub for hardware, store info, and system rules. | High |
| 3 | Bulk Price Update Dialog | A modal to increase/decrease prices across categories. | High |
| 4 | Layout & Font Editor Dialog | A modal to adjust fonts, sizes, and X/Y coordinates for printed elements. | High |

## 3. Detailed Screen Analysis

### Screen 1: Main Dashboard

* **Screen purpose:** Main navigation menu.
* **Layout:** Dark background with large circular nodes (like a mind map) for primary functions. Top horizontal navigation bar.
* **Header:** Features a top menu bar with dropdowns: "الرئيسية" (Home), "إعدادات" (Settings), "المنتجات" (Products), "العملاء" (Customers), "الموردين" (Suppliers), "المبيعات" (Sales), "المشتريات" (Purchases), "المرتجعات" (Returns), etc.
* **Buttons:** Large colored circles: "شاشة المبيعات" (Sales Screen - Green), "شاشة المشتريات" (Purchases Screen - Red), "POS", "إضافة منتجات" (Add Products). Small blue circles at the bottom for quick links (e.g., Reports, Users).
* **Colors:** Dark gray background, red, green, and blue primary interactive elements.
* **Alignment:** Right-to-Left (RTL).

### Screen 2: System Settings (إعدادات النظام)

* **Screen purpose:** Configure printers, store details, and operational toggles.
* **Layout:** Tabbed interface with three main tabs. Bottom fixed action bar containing save buttons and dangerous actions.
* **Tabs:**
1. **إعدادات الطابعات (Printer Settings):**
* *ComboBoxes:* "اختر طابعة الفواتير" (Invoice Printer), "اختر طابعة التقارير A4" (A4 Reports Printer), "اختر طابعة الباركود" (Barcode Printer).
* *TextBoxes:* "عدد نسخ الفاتورة" (Number of Invoice Copies).
* *Radio Buttons/Toggles:* "طباعة وحفظ" (Print & Save), "طباعة مباشرة" (Direct Print), "معاينة وطباعة" (Preview & Print), "طباعة ملصق الباركود" (Print Barcode Label), "حفظ فقط" (Save Only).


2. **بيانات الفاتورة (Invoice Data):**
* *TextBoxes:* "اسم المحل" (Store Name), "العنوان" (Address), "عنوان المحل" (Store Address), "التليفون" (Telephone), "الهاتف" (Mobile), "جملة في اسفل الفاتورة" (Sentence at bottom of invoice).
* *Buttons:* "اختر صورة" (Choose Image for Logo), "بدون صورة" (Without Image).


3. **اعدادات عامة (General Settings):**
* *TextBoxes:* "تعديل ضريبة القيمة المضافة" (Edit VAT - currently 0.00), "مبلغ عامل التوصيل" (Delivery worker amount), "مسار النسخ الاحتياطي" (Backup path), "مسار النسخ الاحتياطي الاوتوماتيكي" (Automatic backup path).
* *Toggles/Checkboxes (Right-aligned text):* "تفعيل شاشة البيع بدون باركود" (Enable sales without barcode), "تفعيل البيع بدون رصيد في المخزن" (Enable negative inventory selling), "تفعيل درج الكاشير بعد الشراء" (Enable cash drawer after purchase).
* *Buttons:* "ربط واتساب API WhatsApp" (Connect WhatsApp API).




* **Bottom Action Bar (Visible across all tabs):**
* *Buttons:* "حفظ الاعدادات" (Save Settings - Green checkmark icon), "تعديل اسعار المنتجات" (Edit Product Prices), "تصفير المبيعات" (Zero Sales - Red X icon), "تصفير المشتريات" (Zero Purchases - Red X icon), Paint Palette Icon (Layout Editor).


* **Colors:** Dark theme (black/dark gray). Input fields are slightly lighter dark gray. Blue highlights for active tabs.

### Screen 3: Bulk Price Update Dialog

* **Screen purpose:** Modify prices for all products or specific categories at once.
* **Layout:** Centered modal window over the settings screen.
* **Header:** "تعديل الاسعار" (Edit Prices).
* **Radio Buttons:** "قيمة من المال" (Fixed value), "نسبة مئوية" (Percentage).
* **ComboBox:** "كل التصنيفات" (All Categories).
* **TextBoxes:** "زيادة أو نقصان سعر الشراء لكل المنتجات" (Increase/Decrease purchase price), "زيادة أو نقصان سعر البيع لكل المنتجات" (Increase/Decrease sales price) - Defaulted to $0.00.
* **Buttons:** "حفظ التعديل" (Save Edit - Green), "الغاء" (Cancel - Red).

### Screen 4: Layout & Font Editor Dialog

* **Screen purpose:** Fine-tune the printed receipt layout and fonts.
* **Layout:** Multi-column grid interface within a modal.
* **Components:** Lists receipt elements (e.g., "اسم الكاشير" Cashier Name, "الرقم الضريبي" Tax Number, "الباركود" Barcode).
* **ComboBoxes:** Font selection ("Regular"), Font Size ("8", "10").
* **TextBoxes:** X and Y coordinate inputs for precise printing placement (e.g., 85, 180, 190).
* **Buttons:** "معاينة أو تحديث" (Preview or Update).

## 4. UI Components

* **Top Navigation Menu:** Standard Windows-style dropdown menus, adapted for RTL.
* **Tabs:** Horizontal text-based tabs used to divide logical sections within a single screen.
* **Data Entry:** Standard TextBoxes for text/numbers, ComboBoxes for hardware selection and categories.
* **Action Buttons:** Usually accompanied by icons (Checkmarks for save, Red 'X' for destructive actions).
* **Modals/Dialogs:** Used for secondary workflows (Price Editing, Layout Editing) to prevent leaving the current context.
* **Theme:** Strict Dark Mode. Input fields have subtle borders. Text is primarily white or light gray.

## 5. User Actions

* **User Action:** Clicks "إعدادات" -> "إعدادات النظام" from the top menu.
* **System Response:** Opens the System Settings screen, defaulting to the Printer Settings tab.


* **User Action:** Clicks the "بيانات الفاتورة" tab.
* **System Response:** Switches view to show store information and logo upload fields.


* **User Action:** Clicks the "اعدادات عامة" tab.
* **System Response:** Switches view to show VAT, backups, and system toggles.


* **User Action:** Clicks "تعديل اسعار المنتجات" button at the bottom.
* **System Response:** Opens the Bulk Price Update modal dialog.


* **User Action:** Clicks the Paint Palette icon.
* **System Response:** Opens the Receipt Layout & Font Editor dialog.


* **User Action:** Clicks "خروج" (Exit) from the top menu.
* **System Response:** Closes the current screen/application.



## 6. Business Rules

### Confirmed

* The system supports routing print jobs to at least three distinct printer types: Invoice (Receipt), A4 (Reports), and Barcode labels.
* Invoices can be configured to print automatically, preview first, or save without printing.
* The system allows selling items even if they are out of stock (Negative Inventory Selling), but this must be explicitly enabled by the administrator.
* Sales can be processed with or without a barcode scanner.
* The cash drawer can be configured to open automatically after a purchase.
* A fixed "Delivery Worker" fee can be set globally.
* The system supports automated backups to a user-defined local path.
* Product prices (both purchase and sales prices) can be mass-updated. This update can be applied as a flat currency amount or as a percentage, and can be filtered by Category.
* The system allows the administrator to completely wipe ("Zero out") all historical Sales or Purchases data via dedicated buttons.
* Receipt layouts are not hardcoded; X/Y coordinates and font sizes for individual receipt elements are customizable by the user.

### Inferred

* VAT (Value Added Tax) is calculated as a percentage and added to totals globally, based on the general setting field.
* The application integrates with WhatsApp for sending invoices or notifications, inferred from the "WhatsApp API" button.
* Because there are settings for A4 printers vs. Invoice printers, the system handles different types of outputs (e.g., standard POS receipts vs. large format reports or formal B2B invoices).

### Unknown

* How individual products are added to the system.
* How the actual Point of Sale (Checkout) screen operates.
* Whether the system supports multiple branches or warehouses.
* How user permissions and roles are managed to protect these sensitive settings.

## 7. Business Entities

* Store/Company (Details, Logo)
* Printer Configuration
* Product
* Category
* Invoice
* Sales Record
* Purchase Record
* Delivery Worker

## 8. Workflow

*(Based strictly on the recorded settings session)*

1. User launches application from Windows desktop.
2. User navigates to Settings -> System Settings.
3. User selects designated printers from installed Windows printers via dropdowns.
4. User selects print behavior (e.g., Print & Save).
5. User navigates to Invoice Data tab to enter Store Name, Phone, and footer text.
6. User navigates to General Settings tab.
7. User configures VAT percentage and sets backup directory paths.
8. User toggles specific operational rules (e.g., allowing negative stock).
9. User opens the Price Modification tool to apply a bulk percentage increase to a specific category.
10. User opens the Layout tool to adjust the printed position of the Tax ID on the receipt.
11. User saves settings.

## 9. Calculations

* **Bulk Price Adjustment:** System can calculate a new price by adding/subtracting a fixed numeric value (`Price ± Value`) or by applying a percentage (`Price * (1 ± Percentage)`).
* **VAT:** System calculates VAT based on the percentage entered in General Settings.

## 10. Validation

* No active validation errors were shown in the video. However, numeric fields like "VAT", "Price Increase", and "X/Y Coordinates" are strictly formatted to accept numbers/decimals (e.g., 0.00 is shown by default).

## 11. Error / Warning Messages

* The Bulk Price Update dialog contains instructional warning text: "هل تريد أن يكون التعديل على سعر المنتج بنسبه مئويه أم من قيمه أم الغاء؟" prompting the user to be careful about how mass adjustments are calculated.
* Destructive actions ("تصفير المبيعات" / Zero Sales) are visually warned against by using stark red backgrounds and 'X' icons.

## 12. Navigation

* **Primary:** Top horizontal menu bar with classic dropdown lists.
* **Secondary:** Visual dashboard with large circular clickable nodes.
* **Tertiary:** Horizontal tabs within specific modules (like Settings).

## 13. Reusable UI Patterns

* **Bottom Action Bars:** Critical actions (Save, Reset, Tools) are pinned to a distinct bar at the bottom of the screen.
* **Settings Grouping:** Related settings are grouped into horizontal tabs rather than long scrolling pages.
* **Destructive Button Styling:** Buttons that delete data are colored deep red with 'X' icons to distinguish them from standard actions.

## 14. Questions / Unknowns

* What happens if a user clicks the "Zero Sales" button? Is there a secondary confirmation dialog, or does it execute immediately?
* How does the "Connect WhatsApp API" function work? Does it require a third-party gateway provider, or is it a direct local connection?
* Are the layout coordinates (X/Y) measured in pixels, millimeters, or standard print points?

## 15. Important Findings

* **High Print Customization:** The system relies heavily on Windows Print Spooler integration and allows granular control over receipt printing, including manual manipulation of X/Y coordinates for text elements. This implies a highly customizable printing engine rather than static receipt templates.
* **Bulk Manipulation:** The inclusion of a global price modifier suggests the system is designed for retail environments with highly volatile pricing or large inventories where manual price updating is unfeasible.
* **Data Reset Capabilities:** The prominent "Zero Data" buttons indicate this software might be used frequently by businesses that clear their books periodically, or it is an artifact of easy testing environments.