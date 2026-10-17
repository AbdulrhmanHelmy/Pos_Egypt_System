# HeCashier Architecture

Phase 1 keeps the existing `Pos_Egypt.UI` WPF project and uses an SDK-style
`net8.0-windows` target. The application is offline-first and has no web
backend.

## Structure

- `Models`: foundational domain entities.
- `Data`: EF Core SQLite context and safe initialization.
- `Security`: salted PBKDF2 password hashing.
- `Services`: authentication and shift operations.
- `ViewModels`: login and shell state/commands.
- `Views`: login, shell content, and honest placeholder screens.
- `Core`: small MVVM notification and command primitives.
- `Resources`: centralized theme resources.

Dependencies point from Views/ViewModels to Services and Data abstractions;
domain models do not depend on WPF.

## UI

Arabic is the primary language and windows use `RightToLeft`. The dark theme
is loaded from `Resources/Themes.xaml`; resource keys are centralized so a
future light dictionary can replace it. Navigation changes real WPF content
views. Modules outside Phase 1 show an explicit not-yet-implemented message.

## Boundaries

Authentication verifies a stored hash and returns an authenticated user.
`ShiftService` owns active-shift lookup and start behavior. Financial posting,
inventory posting, permissions enforcement, and complete module workflows are
intentionally deferred until their rules are verified.
