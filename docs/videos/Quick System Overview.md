# Video Analysis

## 1. Video Overview

The video provides a roughly 4-minute demonstration of an Egyptian desktop Point of Sale (POS) and business management application named "DoCashier". The system features user authentication, a central dashboard, sales interfaces, purchase management, product entry, and various reporting menus. The application primarily operates in a dark mode theme, with a light mode toggle available.

## 2. Screens Discovered

| ID | Screen | Purpose | Confidence |
| --- | --- | --- | --- |
| 1 | Login (تسجيل الدخول) | Authenticate users (Manager/Cashier).

 | High |
| 2 | Main Dashboard (الرئيسية) | Central navigation and quick access to core modules.

 | High |
| 3 | Sales (شاشة المبيعات) | Process customer transactions.

 | High |
| 4 | Fast POS (POS) | Quick-touch interface for rapid sales.

 | High |
| 5 | Purchases (شاشة المشتريات) | Process incoming inventory from suppliers.

 | High |
| 6 | Add Product (إضافة صنف) | Create new inventory items.

 | High |
| 7 | Top Selling (الأكثر مبيعا) | Display top-selling product reports.

 | High |

## 3. Detailed Screen Analysis

### Screen 1: Login

* **Purpose:** User authentication.


* **Layout:** Centered modal window over the desktop background.


* **Header:** Logo and "DoCashier" branding.


* **Buttons:** On-screen numeric keypad (0-9, Numlock, Enter), login button (تسجيل الدخول), and a power/exit button.


* **Form fields:** User selection dropdown (Manager/Cashier), Password input field (الرقم السري).


* **Dialogs:** "Trial version" (نسخة تجريبية) informational dialog appears after login.


* **Colors:** Dark grey/black window with a blue header and accents.



### Screen 2: Main Dashboard

* **Purpose:** System navigation.


* **Layout:** Top horizontal menu bar with dropdowns, central area with large interactive circular nodes (bubble layout).


* **Buttons:** Circular shortcut buttons for Sales, Purchases, Add Product, POS, Users, Reports, etc..


* **Dialogs:** Low stock/reorder notification dialog appears upon loading the dashboard.


* **Colors:** Dark background, top navigation bar has a blue accent block on the right. The Sales node is green, Purchases is red, others are blue.


* **Toggle:** A "Dark Mode" / "Light Mode" toggle is located at the bottom center.



### Screen 3: Sales Screen

* **Purpose:** Processing outgoing customer sales.


* **Layout:** Left side contains the invoice data grid; right side contains product search, barcode input, and total summaries.


* **Header:** Standard blue top navigation bar remains visible.


* **TextBoxes:** Barcode scanner field, manual product search field.


* **Tables/DataGrids:** Main grid displaying Item Code, Name, Unit, Quantity, Price, and Total.


* **Buttons:** Discount (خصم), Delivery (خدمة توصيل), Payment options (Cash/كاش, Credit/آجل, Visa/فيزا).


* **Totals:** A large green total amount indicator is displayed at the bottom right.



### Screen 4: Fast POS Screen

* **Purpose:** Rapid checkout using touch-friendly product buttons.


* **Layout:** Left side contains the current invoice data grid (similar to the standard Sales screen); right side is populated with large square product tiles.


* **Buttons:** Grid of product buttons displaying item names and prices.


* **Tables/DataGrids:** Invoice grid showing added items, quantities, and prices.



### Screen 5: Purchases Screen

* **Purpose:** Processing incoming supplier invoices.


* **Layout:** Identical structural layout to the Sales Screen.


* **Colors:** The header strip above the application area is red to distinguish it from the blue Sales screen.



### Screen 6: Add Product Screen

* **Purpose:** Data entry for new inventory.


* **Layout:** Form-based layout with categorized horizontal fields.


* **Form fields:** Item Name (اسم المنتج), Category (المجموعة), Barcode (رقم الباركود), Quantity (الكمية الحالية), Buy Price, Sell Price.


* **Buttons:** Add/Save Product (إضافة الصنف) at the bottom left.


* **Dialogs:** A success checkmark dialog appears in the bottom right corner after saving.



## 4. UI Components

* **Top Navigation Menu:** Persistent horizontal bar with dropdown categories (Settings, Sales, Purchases, Inventory, HR, etc.).


* **On-Screen Keypad:** Used in the login screen for password entry.


* **DataGrids:** Used consistently across transaction screens (Sales, Purchases) to display line items.


* **Toast Notifications/Modals:** Used for alerts (trial version, low stock) and success confirmations.


* **Theme Toggle:** A switch in the footer to change between dark and light UI themes.



## 5. User Actions

**User Action:** User clicks numbers on the on-screen keypad and clicks "Enter".

* **System Response:** An informational dialog stating "Trial version" appears.


* **Data affected:** None.
* **UI change:** Modal popup over the login screen.


* **Possible Business Rule:** [CONFIRMED] The system validates licenses upon authentication.

**User Action:** User clicks the "Sales" (شاشة المبيعات) node on the dashboard.

* **System Response:** Navigates to the Sales screen.


* **Data affected:** None.
* **UI change:** Replaces dashboard view with the Sales data grid and input fields.


* **Possible Business Rule:** [CONFIRMED] Quick-access nodes route to primary functional modules.

**User Action:** User selects a product from the dropdown in the Sales screen.

* **System Response:** The product is added as a line item in the left-side data grid.


* **Data affected:** Current invoice state.
* **UI change:** A new row appears in the grid, and the bottom right Total updates.


* **Possible Business Rule:** [CONFIRMED] Selecting an item automatically adds a default quantity (1) to the active invoice.

**User Action:** User clicks "Add Product" (إضافة صنف) node, fills out the product form (Name: "الف الف", Qty: 1, Buy: $1.00, Sell: $1.00), and clicks Add.

* **System Response:** A checkmark success notification appears.


* **Data affected:** New product record is created in the database.
* **UI change:** A green checkmark popup shows in the bottom right.


* **Possible Business Rule:** [CONFIRMED] Products require a Name, Barcode, Quantity, Buy Price, and Sell Price to be saved.

**User Action:** User clicks the "Dark Mode" toggle at the bottom of the dashboard.

* **System Response:** The application theme changes.


* **Data affected:** Local user preference/UI state.
* **UI change:** The dark background turns white/light gray.


* **Possible Business Rule:** [CONFIRMED] The application supports dynamic visual themes.

## 6. Business Rules

### Confirmed

* Access control distinguishes between at least two roles: Manager and Cashier.


* The system generates notifications for low inventory stock upon login.


* Products can be entered into sales via barcode scanning or manual selection.


* Sales can be finalized using multiple payment methods: Cash, Credit (اجل), and Visa.


* The system supports adding discounts and delivery fees to a sales invoice.



### Inferred

* The total amount on an invoice is automatically calculated as the sum of (Quantity × Sell Price) for all items in the grid.
* Completing a sale or purchase automatically adjusts the inventory quantities of the associated products.
* The Fast POS screen items are likely mapped to specific categories or top-selling items for quick access.

### Unknown

* How tax (VAT) is calculated and applied to invoices.
* The exact procedure for handling customer returns or partial refunds.
* How cashier shifts (opening/closing balances) are managed.
* Maximum limits on invoice totals or discount amounts.

## 7. Business Entities

* User (Manager, Cashier)


* Product (Item)


* Customer


* Supplier


* Sales Invoice


* Purchase Invoice


* Category


* Employee (referenced in HR menus)



## 8. Workflow

1. User opens the application and inputs credentials on the Login screen.


2. System validates credentials and displays a trial warning.


3. User dismisses the warning and views the Main Dashboard.


4. User dismisses a low-stock alert dialog.


5. User navigates to the Sales screen to process transactions by selecting items.


6. User navigates to the Fast POS screen to demonstrate rapid item selection via large buttons.


7. User navigates to the Purchases screen to view supplier invoice capabilities.


8. User opens the Add Product screen, inputs product details, and saves the new record.


9. User explores the top navigation menu, revealing reporting, HR, and inventory features.


10. User toggles the UI theme to Light Mode before the video concludes.



## 9. Calculations

* **Invoice Total:** Visibly updates when items are added to the sales or purchase grid. [INFERRED: Sum of individual item totals].


* **Item Total:** [INFERRED: Quantity multiplied by unit price].

## 10. Validation

* [UNKNOWN] It is not explicitly demonstrated what happens if the user attempts to save a product with blank fields or letters in numeric fields, though standard behavior implies validation exists.

## 11. Error / Warning Messages

* "Trial version" (نسخة تجريبية) - Appears after successful login.


* Low stock alert - Appears on the dashboard notifying the user of items needing reorder.



## 12. Navigation

* **Primary:** Large circular nodes on the main dashboard for high-frequency tasks (Sales, Purchases, Add Product).


* **Secondary:** Persistent top menu bar with cascading dropdown menus for administrative, reporting, and settings modules.



## 13. Reusable UI Patterns

* **Transaction Screens:** The layout for Sales and Purchases is identical in structure (grid on the left, inputs/totals on the right), using a colored header strip (blue for sales, red for purchases) to denote context.


* **DataGrids:** Consistent tabular presentation for line items across different modules.


* **Numeric Input:** The on-screen keypad pattern is used for authentication.



## 14. Questions / Unknowns

* What database system is utilized for storage?
* How does the application interface with peripheral hardware like receipt printers and barcode scanners?
* What specific permissions are restricted from the "Cashier" role compared to the "Manager" role?

## 15. Important Findings

* The application prioritizes speed with two distinct sales interfaces: a standard detailed invoice screen and a fast touch-optimized POS screen.


* Color coding is heavily utilized as a contextual indicator (Blue = Sales, Red = Purchases, Green = Totals/Success).


* The system includes comprehensive business management features beyond basic POS, including HR, supplier accounts, and manufacturing/assembly modules indicated in the dropdown menus.