# AGENTS.md

This file provides guidance to agents when working with code in this repository.

## Critical Information

- **Primary entry point**: Run `dotnet run --project src/FurryFriends.AspireHost` (not the Web API directly). The AspireHost orchestrates both the API and Blazor UI.
- **FastEndpoints**: API uses FastEndpoints library, not MVC Controllers. Endpoint classes inherit from `Endpoint<TRequest, TResult>` and configure routes in `Configure()` method.
- **Blazor Hybrid Architecture**: `FurryFriends.BlazorUI` is a server-side host that serves WebAssembly components from `FurryFriends.BlazorUI.Client`. Services are registered in the server project but injected into client components.
- **Service Discovery**: BlazorUI manually configures `AddServiceDiscovery()` and `AddStandardResilienceHandler()` instead of using `AddServiceDefaults()`. BaseAddress fallback is `http://api/api` if `ApiBaseUrl` not configured.
- **Testing Environment**: The `"Testing"` environment (set by `CustomWebApplicationFactory`) switches email sender to `MimeKitEmailSender` and uses in-memory database with seeded test data.
- **Domain Events**: `AppDbContext.SaveChangesAsync` dispatches domain events via `IDomainEventDispatcher` after successful save. Events are registered with `RegisterDomainEvent()` in aggregate methods.
- **Guard Clauses**: Custom validation extensions like `Guard.Against.BookingSchedule()` and `Guard.Against.DailyBookingLimit()` are project-specific and must be used for aggregate invariants.
- **Log Files**: Separate log files: `FurryFriends.Web/Logs/web-log-.txt` and `FurryFriends.BlazorUI/Logs/blazorui-log-.txt` (rolling daily, 10MB max, 31 days retention).
- **CORS Policy**: Must be configured before `UseRouting()`; policy name `"AllowBlazorClient"` allows origins `https://localhost:7214` and `http://localhost:5185`.
- **Migrations Location**: All EF Core migrations are in `src/FurryFriends.Infrastructure/Migrations/`. Apply with `dotnet ef database update --project src/FurryFriends.Infrastructure --startup-project src/FurryFriends.Web`.
- **Central Package Management**: Package versions are managed in `Directory.Packages.props`. Do not add `<PackageReference>` versions in individual `.csproj` files.
- **Test Database Seeding**: `SeedData.PopulateTestDataAsync()` is called by `CustomWebApplicationFactory` for functional tests. It creates in-memory DB with specific GUIDs (e.g., `locality1.Id = Guid.Parse("929ccaf2-8c74-49bb-b9a0-ce26db0611ab")`).
- **Specification Pattern**: Query logic is encapsulated in specifications under each aggregate's `Specifications` folder (e.g., `BookingConflictSpec.cs`, `ActiveBookingsByPetWalkerOnDateSpec.cs`).
- **Result Pattern**: Use `Ardalis.Result<T>` for return types. In endpoints, check `result.IsSuccess` and iterate `result.Errors` for validation failures.
- **Private Field Naming**: Private fields use `_camelCase` prefix (enforced by `.editorconfig` rule `private_members_with_underscore`).
- **Blazor Component Code-Behind**: Component logic resides in `.razor.cs` files with `partial` class. The `.razor` file contains markup only; do not add `@code` blocks.
