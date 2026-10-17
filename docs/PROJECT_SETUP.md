# HeCashier Project Setup

## Requirements

- Windows with the .NET 8 SDK and Windows Desktop Runtime.
- NuGet access for the first restore; normal application operation is offline.

## Build and run

From the repository root:

```powershell
dotnet restore Pos_Egypt_System.slnx
dotnet build Pos_Egypt_System.slnx
dotnet test tests\HeCashier.Tests\HeCashier.Tests.csproj
dotnet run --project Pos_Egypt.UI\Pos_Egypt.UI.csproj
```

The database is created at `%LOCALAPPDATA%\HeCashier\hecashier.db`. Existing
databases are not deleted automatically.

## Development login

Debug builds seed an empty database with the development-only account
`devadmin` / `HeCashierDev2026!`. Only a salted password hash is stored. Release
builds do not seed an account and require an administrator to provision users.

## Future work

Phase 4: add a first-run administrator setup flow. No first-run wizard is
implemented in this phase.

## Troubleshooting

If initialization fails, check that the user has write access to
`%LOCALAPPDATA%` and inspect debugger output for the technical exception.
Normal users receive an Arabic error message without stack traces. If package
restore fails, restore NuGet connectivity once and retry; the application itself
does not require a server or cloud database after restore.
