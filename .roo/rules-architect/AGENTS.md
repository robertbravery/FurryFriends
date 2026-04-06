# AGENTS.md (Architect Mode)

## Architectural Constraints

- **Clean Architecture**: Strict layer dependencies: Core → UseCases → Infrastructure → Web. Web depends on UseCases and Infrastructure, but UseCases depends only on Core interfaces.
- **Domain Events**: Aggregates inherit from `HasDomainEventsBase` and events are dispatched in `AppDbContext.SaveChangesAsync`. This creates implicit coupling between persistence and domain layer via dispatcher injection.
- **Blazor Hybrid**: Server project (`FurryFriends.BlazorUI`) acts as both Blazor Server host and API gateway for WebAssembly client. Services registered in server are injected into client components via DI, creating tight coupling between server and client.
- **Service Discovery**: BlazorUI manually configures `AddServiceDiscovery()` and `AddStandardResilienceHandler()` instead of using `AddServiceDefaults()`, indicating custom service mesh integration.

## Hidden Coupling

- **Email sender**: Switched based on environment (`Testing` uses `MimeKitEmailSender`). See [`ServiceConfigs.cs`](src/FurryFriends.Web/Configurations/ServiceConfigs.cs:19-32)
- **Database**: Functional tests use in-memory DB with seeded data that includes hardcoded GUIDs, creating test data dependencies.
- **CORS**: Blazor client origins hardcoded in Web API (`https://localhost:7214`, `http://localhost:5185`). Changing Blazor port requires updating Web API CORS policy.
- **Specification pattern**: Each aggregate has its own `Specifications` folder, but some specifications are used across aggregates (e.g., booking conflict checks depend on PetWalker schedules).

## Performance Bottlenecks

- **Logging**: Serilog file rolling configured with 10MB limit and 31 days retention. High-volume logging could impact I/O.
- **EF Core queries**: Specifications are used but not all queries may be optimized. Check for `Include` statements in configurations.
- **Domain event dispatch**: Occurs after every `SaveChangesAsync` and iterates all entities with events. Could become slow with many tracked entities.

## Entry Points

- **API**: `FurryFriends.Web/Program.cs` - configures FastEndpoints, CORS, OpenTelemetry, Serilog
- **Blazor Server**: `FurryFriends.BlazorUI/Program.cs` - registers services, configures HTTP clients with service discovery
- **Blazor Client**: `FurryFriends.BlazorUI.Client/Program.cs` - configures HttpClient with `ApiBaseUrl` fallback
- **Orchestration**: `FurryFriends.AspireHost/FurryFriends.AspireHost.csproj` - primary entry point for running entire application
