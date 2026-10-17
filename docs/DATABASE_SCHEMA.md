# HeCashier Database Schema

The initial schema is created by EF Core `EnsureCreated` in the per-user
directory `%LOCALAPPDATA%\HeCashier\hecashier.db`. The initializer never deletes
or overwrites an existing database. Future production schema changes should use
reviewed EF migrations rather than destructive startup actions.

Foundational entities include users, roles, permissions and join tables; shifts;
products, categories, units, barcodes and inventory movements; customers and
suppliers; sale/purchase headers and lines; safes, bank accounts, financial
transactions, expenses and expense types; and audit entries.

Money and fractional quantities use `decimal`. Customer/supplier balances and
safe/bank balances are not implemented as authoritative mutable totals. The
current schema does not yet implement posting rules, returns, installments,
costing, reconciliation, or permission enforcement at every operation boundary.
Those rules remain to be verified before later phases.
