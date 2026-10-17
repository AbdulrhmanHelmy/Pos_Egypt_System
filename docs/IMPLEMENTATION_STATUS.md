# Phase 0: Baseline Stabilization & Schema Hardening

## Status

Implementation and verification are complete.

## Completed

- Removed legacy configuration API usage from Infrastructure and added
  LocalAppData fallback when the configured data drive is unavailable.
- Restricted development-user seeding and login credential prefill to DEBUG
  builds; login and shift failures are written using `FileLogger`.
- Added unique indexes for usernames, barcode values, and role names.
- Applied explicit precision to every decimal property present in the EF model:
  `(18, 3)` for quantities and `(18, 2)` for currency.
- Added regression tests for unique constraints, decimal precision, data-root
  fallback, and database startup initialization.
- Removed the legacy .NET Framework startup declaration from WPF configuration.

The current domain model does not define Sale/Purchase total, Shift amount, or
Safe/BankAccount balance properties. No new financial fields were added during
baseline stabilization.

## Verification

- `dotnet build Pos_Egypt_System.slnx` succeeded with zero warnings and zero
  errors.
- `dotnet test tests/HeCashier.Tests/HeCashier.Tests.csproj` passed: 7 tests,
  0 failed, 0 skipped.
- `dotnet build Pos_Egypt_System.slnx -c Release` succeeded.
- `dotnet test tests/HeCashier.Tests/HeCashier.Tests.csproj -c Release` passed:
  7 tests, 0 failed, 0 skipped.

## Phase 1: Products, Inventory, Multi-Barcode & Fast Maintenance

### Completed

- Expanded product data with product code, Arabic name, category, unit, purchase,
  cost, wholesale and selling prices, stock/reorder values, expiry, classification,
  activation and notes; added multiple uniquely indexed product barcodes.
- Added store-level stock, inventory movement, stocktake, transfer, and damaged-stock
  records. Product opening stock, movement history and audit entries are saved
  atomically with the generated product ID.
- Implemented product search, barcode lookup/generation, product maintenance,
  quick price/quantity/barcode/activation updates, category/unit CRUD, and atomic
  adjustments, stocktakes, damaged-stock removal and transfers.
- Added the configurable negative-stock setting. It defaults to disabled and
  rejects transactions that would make store or product stock negative.
- Seeded default lookups for `عام`, `قطعة`, and `محل` without overwriting existing
  settings or lookup data.
- Added RTL dark-theme product/inventory, fast-edit, and category/unit/settings
  screens, registered their services, and wired shell navigation.
- Added SQLite integration coverage for opening stock, multi-barcode uniqueness and
  secondary-barcode search, atomic inventory/audit updates, negative-stock rejection,
  quick edits, and category/unit deletion protection.

### Verification

- `dotnet build Pos_Egypt_System.slnx` succeeded with 0 warnings and 0 errors.
- `dotnet test tests/HeCashier.Tests/HeCashier.Tests.csproj` passed: 13 tests,
  0 failed, 0 skipped.

## Phase 1.5: Complete Master Schema and Seed Data

### Completed

- Added the remaining master and operational entities to the EF Core model and
  registered their tables, relationships, indexes, composite keys, restrictive
  delete behavior, and decimal precision in `HeCashierDbContext`.
- Added idempotent startup seeding for canonical Admin permissions, missing default
  system settings, the default barcode-label template, and required catalog and
  operational master records. Existing setting values and existing records are
  preserved.
- Added SQLite integration tests for catalog and permission seeding, repeat
  initialization, physical schema creation, persistence, unique indexes, and
  restrictive deletes.
- Restored the schema reference with table groups, key and relationship behavior,
  uniqueness rules, precision conventions, and startup seed definitions.

### Verification

- `dotnet build Pos_Egypt_System.slnx` succeeded with 0 warnings and 0 errors.
- `dotnet test tests/HeCashier.Tests/HeCashier.Tests.csproj` passed:
  17 tests, 0 failed, 0 skipped.
- `git status --short` confirmed that no `Pos_Egypt.UI` files were modified.
