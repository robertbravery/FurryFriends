# FurryFriends Constitution v2.0.0 - Summary

**Last Updated**: 2025-10-04

## Overview

The FurryFriends Constitution has been completely restructured from a generic library/CLI template to a comprehensive .NET API + Blazor Hybrid architecture guide.

## Version History

- **v0.1.0** (2025-06-15): Initial generic template
- **v2.0.0** (2025-10-04): MAJOR restructuring for .NET API + Blazor architecture

## Core Principles (15 Total)

### Architecture & Design (Principles I-V)
1. **Clean Architecture Layers** (NON-NEGOTIABLE)
2. **API Design Standards** (FastEndpoints)
3. **Blazor UI Architecture** (Server-side HTTP)
4. **CQRS & MediatR Usage**
5. **Data Access Patterns** (Repository + Specification)

### Testing (Principles VI-VII)
6. **Test-First Development** (NON-NEGOTIABLE)
7. **Integration Testing Standards**

### Operations (Principles VIII-X)
8. **Observability & Logging** (Serilog)
9. **Versioning & Breaking Changes**
10. **Simplicity & YAGNI**

### Detailed Pattern Implementations (Principles XI-XV)

#### XI. Result Pattern (Ardalis.Result) - NON-NEGOTIABLE
- **Purpose**: Replace exception throwing for expected failures
- **Key Types**: `Result.Success()`, `Result.NotFound()`, `Result.Invalid()`, `Result.Error()`
- **Usage**: All handlers return `Result<T>` or `Result`
- **Benefits**: Explicit error handling, better API responses, no expensive exceptions

**Example**:
```csharp
public async Task<Result<Guid>> Handle(CreateBookingCommand command, CancellationToken ct)
{
    if (petWalker.DailyPetWalkLimit <= currentBookings)
    {
        return Result.Error("Pet walker has reached daily booking limit");
    }
    
    var booking = Booking.Create(command.ClientId, command.PetWalkerId, command.Date);
    await _repository.AddAsync(booking, ct);
    
    return Result<Guid>.Success(booking.Id);
}
```

#### XII. FluentValidation - NON-NEGOTIABLE
- **Purpose**: Declarative, testable input validation
- **Scope**: Commands, queries, API requests
- **Integration**: Automatic with FastEndpoints
- **Key Rules**: `NotEmpty()`, `MaximumLength()`, `EmailAddress()`, `Must()`

**Example**:
```csharp
public class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
{
    public CreateBookingCommandValidator()
    {
        RuleFor(x => x.ClientId).NotEmpty();
        RuleFor(x => x.BookingDate).GreaterThan(DateTime.UtcNow);
        RuleFor(x => x.Duration).GreaterThan(0).LessThanOrEqualTo(480);
        RuleFor(x => x).Must(HaveValidTimeSlot).WithMessage("Time slot not available");
    }
}
```

#### XIII. Specification Pattern (Ardalis.Specification) - NON-NEGOTIABLE
- **Purpose**: Encapsulate complex query logic
- **Location**: Core project under `{Aggregate}/Specifications/`
- **Components**: `Where()`, `Include()`, `OrderBy()`, `Skip()`, `Take()`, `AsNoTracking()`
- **Best Practice**: Separate specifications for count queries

**Example**:
```csharp
public class ListPetWalkerByLocationSpecification : Specification<PetWalker>
{
    public ListPetWalkerByLocationSpecification(string? searchTerm, int? localityId, int page, int pageSize)
    {
        Query.Where(pw => pw.IsActive);
        
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            Query.Where(pw => pw.Name.FirstName.Contains(searchTerm));
        }
        
        if (localityId.HasValue)
        {
            Query.Where(pw => pw.ServiceAreas.Any(sa => sa.Locality.Id == localityId.Value));
        }
        
        Query.Include(pw => pw.Photos).Include(pw => pw.ServiceAreas);
        Query.OrderBy(pw => pw.Name.FirstName);
        Query.Skip((page - 1) * pageSize).Take(pageSize);
        Query.AsNoTracking();
    }
}
```

#### XIV. Serilog Structured Logging - NON-NEGOTIABLE
- **Purpose**: Structured, queryable logging
- **Configuration**: File + Console sinks, 30-day retention
- **Log Levels**: Trace, Debug, Information, Warning, Error, Critical
- **Pattern**: Structured properties, not string interpolation

**Example**:
```csharp
_logger.LogInformation(
    "Creating booking for Client {ClientId} with PetWalker {PetWalkerId} on {BookingDate}",
    command.ClientId,
    command.PetWalkerId,
    command.BookingDate);

// ❌ WRONG - String interpolation
_logger.LogInformation($"Creating booking for {command.ClientId}");
```

**Configuration**:
```csharp
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30)
    .CreateLogger();
```

#### XV. Guard Clauses (Ardalis.GuardClauses) - NON-NEGOTIABLE
- **Purpose**: Fail fast with clear precondition validation
- **Scope**: Method parameters, constructor parameters, dependencies
- **Common Guards**: `Null()`, `NullOrEmpty()`, `NegativeOrZero()`, `OutOfRange()`, `Default()`
- **Complement**: Works with FluentValidation (guards for methods, validation for commands)

**Example**:
```csharp
public class Booking : EntityBase<Guid>
{
    private Booking(Guid clientId, Guid petWalkerId, DateTime bookingDate, int durationMinutes)
    {
        Guard.Against.Default(clientId, nameof(clientId));
        Guard.Against.Default(petWalkerId, nameof(petWalkerId));
        Guard.Against.OutOfRange(durationMinutes, nameof(durationMinutes), 15, 480);
        Guard.Against.InvalidInput(
            bookingDate,
            nameof(bookingDate),
            date => date > DateTime.UtcNow,
            "Booking date must be in the future");
        
        ClientId = clientId;
        PetWalkerId = petWalkerId;
        BookingDate = bookingDate;
        DurationMinutes = durationMinutes;
    }
}
```

## Technology Stack

### Required
- .NET 9
- Entity Framework Core + SQL Server
- FastEndpoints
- MediatR
- FluentValidation
- Ardalis.Specification
- Ardalis.Result
- Ardalis.GuardClauses
- Serilog
- Blazor Server + WebAssembly (InteractiveServer)
- Bootstrap 5 + FontAwesome 6.4.0
- xUnit + bUnit

### Prohibited
- Direct SQL queries
- `Console.WriteLine` for logging
- Throwing exceptions for expected failures
- Manual null checks
- LINQ queries in handlers
- Magic strings

## Naming Conventions

- Private fields: `_camelCase`
- Public properties: `PascalCase`
- Async methods: `MethodNameAsync`
- DTOs: `EntityNameDto`
- Specifications: `EntityNameSpecification`
- Handlers: `CommandOrQueryNameHandler`
- Validators: `CommandNameValidator`

## File Organization

- **Web**: Feature folders (`/BookingEndpoints/GetAvailablePetWalkers/`)
- **Core**: Aggregate folders (`/PetWalkerAggregate/`)
- **BlazorUI.Client**: `/Components/Common/` for shared components
- **Documentation**: `/docs/` with Mermaid diagrams

## Pattern Decision Matrix

| Scenario | Use This Pattern | Example |
|----------|------------------|---------|
| Method preconditions | Guard Clauses | `Guard.Against.Null(parameter, nameof(parameter))` |
| Command validation | FluentValidation | `RuleFor(x => x.Email).EmailAddress()` |
| Expected failures | Result Pattern | `return Result.NotFound()` |
| Complex queries | Specification Pattern | `new ListPetWalkerByLocationSpecification(...)` |
| Logging | Serilog | `_logger.LogInformation("User {UserId} created", userId)` |

## Quick Reference

### When to Use Each Pattern

**Guard Clauses vs FluentValidation**:
- Guard Clauses: Constructor parameters, method preconditions, dependencies
- FluentValidation: Commands, queries, API requests, complex business rules

**Result Pattern vs Exceptions**:
- Result Pattern: Expected failures (not found, validation errors, business rules)
- Exceptions: Unexpected failures (database down, out of memory, programming errors)

**Specifications vs LINQ**:
- Specifications: All database queries (reusable, testable, encapsulated)
- LINQ: In-memory collections only (after data retrieved from database)

## Next Steps

1. ✅ Constitution updated to v2.0.0
2. ⚠️ Update `.specify/templates/plan-template.md`
3. ⚠️ Update `.specify/templates/spec-template.md`
4. ⚠️ Update `.specify/templates/tasks-template.md`
5. 📝 Share with team for review
6. 🔄 Commit changes to repository

## Commit Message

```
docs: update constitution to v2.0.0 (comprehensive .NET API + Blazor patterns)

BREAKING CHANGE: Complete restructuring from generic library/CLI to .NET API + Blazor architecture

Added detailed principles for:
- Result Pattern (Ardalis.Result)
- FluentValidation
- Specification Pattern (Ardalis.Specification)
- Serilog Structured Logging
- Guard Clauses (Ardalis.GuardClauses)

Removed:
- Library-First principle
- CLI Interface principle
```

