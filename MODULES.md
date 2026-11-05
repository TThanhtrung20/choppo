# Project modules

The project follows the familiar Visual Studio / ASP.NET Core separation of concerns, with feature-specific code grouped under `Modules` as features are introduced.

## Existing application areas

- `Pages/Help` — user-facing fraud guidance flows (choose a scenario, payment, information, post-incident).
- `Pages/Baseline` — protection plan pages (rules, trusted contacts, usual payments).
- `Pages/Account` — sign-in, registration, sign-out, account management.
- `Models` — existing persistence models shared by the current baseline and Identity implementation.
- `Data` and `Migrations` — EF Core context and schema history.
- `Services` — current safety-guidance content service.

## New feature modules

- `Modules/Chat` — isolated foundation for AI chat. It currently contains only conversation/message domain models and this module note. No provider/API integration, persistence, endpoints, or chat UI has been added.

## Adding a complete module

Keep future chat files together under `Modules/Chat`, for example `Models`, `Services`, `Pages` or `Endpoints`, and `Configuration`. Register services in `Program.cs` only when implementation is ready. Keep provider credentials in user-secrets or environment variables, never source control. Add database entities and an EF migration only when persistence is requested.
