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

## Verification

- `dotnet build Pos_Egypt_System.slnx` succeeded with zero warnings and zero
  errors.
- `dotnet test tests/HeCashier.Tests/HeCashier.Tests.csproj` passed: 7 tests,
  0 failed, 0 skipped.
- `dotnet build Pos_Egypt_System.slnx -c Release` succeeded.
- `dotnet test tests/HeCashier.Tests/HeCashier.Tests.csproj -c Release` passed:
  7 tests, 0 failed, 0 skipped.
