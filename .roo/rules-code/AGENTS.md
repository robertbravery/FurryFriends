# AGENTS.md (Code Mode)

## Coding Conventions

- **Private fields**: Use `_camelCase` prefix (enforced by `.editorconfig` rule `private_members_with_underscore` at line 69-75)
- **Indentation**: 2 spaces for all code files (`.editorconfig` line 19)
- **Nullable/ImplicitUsings**: Enabled globally via `Directory.Build.props`
- **Target Framework**: `net9.0` (all projects)

## Project-Specific Patterns

- **FastEndpoints**: Endpoints inherit from `Endpoint<TRequest, TResult>`, configure routes in `Configure()`, handle in `HandleAsync()`. See [`CreateTimeslot.cs`](src/FurryFriends.Web/Endpoints/TimeslotEndpoints/Timeslot/CreateTimeslot.cs:7)
- **MediatR CQRS**: Commands/Queries in `FurryFriends.UseCases`, handlers implement `IRequestHandler` or `IQueryHandler`. See [`GetPetWalkerHandler.cs`](src/FurryFriends.UseCases/Domain/PetWalkers/Query/GetPetWalker/GetPetWalkerHandler.cs:7)
- **Ardalis.Result<T>**: Return type for all handlers. Check `result.IsSuccess`, iterate `result.Errors` for failures
- **Domain Events**: Aggregates inherit from `HasDomainEventsBase`, register events via `RegisterDomainEvent()`, dispatched in `AppDbContext.SaveChangesAsync`
- **Guard Clauses**: Custom extensions in [`GuardClauseExtensions.cs`](src/FurryFriends.Core/BookingAggregate/Validation/GuardClauseExtensions.cs) - use `Guard.Against.BookingSchedule()`, `Guard.Against.DailyBookingLimit()`
- **Specification Pattern**: Queries use `Ardalis.Specification` classes in each aggregate's `Specifications` folder
- **EF Configurations**: Entity configurations in `FurryFriends.Infrastructure/Data/Config/` using `IEntityTypeConfiguration<T>`

## Key Dependencies

- FastEndpoints 5.35.0, MediatR 12.4.1, Ardalis.Result 10.1.0, Ardalis.GuardClauses 5.0.0
- FluentValidation 11.11.0, Serilog 4.2.0, EF Core 9.0.0
- Central package management in `Directory.Packages.props` - do NOT add versions in `.csproj`

## Blazor Component Structure

- Server host: `FurryFriends.BlazorUI` registers services, serves WASM client
- Client project: `FurryFriends.BlazorUI.Client` contains components
- Code-behind: `.razor.cs` files with `partial` class; `.razor` files are markup only (no `@code` blocks)
- Services registered in server but injected into client components via DI
