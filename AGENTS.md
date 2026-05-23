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

<!-- gitnexus:start -->
# GitNexus — Code Intelligence

This project is indexed by GitNexus as **FurryFriends** (7267 symbols, 13673 relationships, 161 execution flows). Use the GitNexus MCP tools to understand code, assess impact, and navigate safely.

> If any GitNexus tool warns the index is stale, run `npx gitnexus analyze` in terminal first.

## Always Do

- **MUST run impact analysis before editing any symbol.** Before modifying a function, class, or method, run `gitnexus_impact({target: "symbolName", direction: "upstream"})` and report the blast radius (direct callers, affected processes, risk level) to the user.
- **MUST run `gitnexus_detect_changes()` before committing** to verify your changes only affect expected symbols and execution flows.
- **MUST warn the user** if impact analysis returns HIGH or CRITICAL risk before proceeding with edits.
- When exploring unfamiliar code, use `gitnexus_query({query: "concept"})` to find execution flows instead of grepping. It returns process-grouped results ranked by relevance.
- When you need full context on a specific symbol — callers, callees, which execution flows it participates in — use `gitnexus_context({name: "symbolName"})`.

## Never Do

- NEVER edit a function, class, or method without first running `gitnexus_impact` on it.
- NEVER ignore HIGH or CRITICAL risk warnings from impact analysis.
- NEVER rename symbols with find-and-replace — use `gitnexus_rename` which understands the call graph.
- NEVER commit changes without running `gitnexus_detect_changes()` to check affected scope.

## Resources

| Resource | Use for |
|----------|---------|
| `gitnexus://repo/FurryFriends/context` | Codebase overview, check index freshness |
| `gitnexus://repo/FurryFriends/clusters` | All functional areas |
| `gitnexus://repo/FurryFriends/processes` | All execution flows |
| `gitnexus://repo/FurryFriends/process/{name}` | Step-by-step execution trace |

## CLI

| Task | Read this skill file |
|------|---------------------|
| Understand architecture / "How does X work?" | `.claude/skills/gitnexus/gitnexus-exploring/SKILL.md` |
| Blast radius / "What breaks if I change X?" | `.claude/skills/gitnexus/gitnexus-impact-analysis/SKILL.md` |
| Trace bugs / "Why is X failing?" | `.claude/skills/gitnexus/gitnexus-debugging/SKILL.md` |
| Rename / extract / split / refactor | `.claude/skills/gitnexus/gitnexus-refactoring/SKILL.md` |
| Tools, resources, schema reference | `.claude/skills/gitnexus/gitnexus-guide/SKILL.md` |
| Index, status, clean, wiki CLI commands | `.claude/skills/gitnexus/gitnexus-cli/SKILL.md` |

<!-- gitnexus:end -->
