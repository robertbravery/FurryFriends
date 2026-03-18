# Quickstart Guide: Petwalker Timeslots

**Feature**: 003-petwalker-timeslots  
**Date**: 2026-03-14

---

## Overview

This guide helps developers get started with implementing the Petwalker Timeslots feature. Follow these steps in order.

---

## Prerequisites

- .NET 9 SDK
- SQL Server (local or container)
- Visual Studio 2022 or VS Code
- Git

---

## Step 1: Understand the Architecture

Read these files first:

1. **[spec.md](spec.md)** - Feature requirements and acceptance criteria
2. **[data-model.md](data-model.md)** - Entity definitions
3. **[contracts/index.md](contracts/index.md)** - API endpoints overview

---

## Step 2: Set Up Development Environment

```bash
# Clone the repository
git clone <repository-url>
cd FurryFriends

# Switch to feature branch
git checkout 003-petwalker-timeslots

# Restore packages
dotnet restore

# Verify build
dotnet build
```

---

## Step 3: Run Existing Tests

Before making changes, ensure existing tests pass:

```bash
# Run all tests
dotnet test

# Or run specific test project
dotnet test tests/FurryFriends.UnitTests
```

---

## Step 4: Create Database Migration

Create a new EF Core migration for the new entities:

```bash
dotnet ef migrations add PetwalkerTimeslots_V1 `
  --project src/FurryFriends.Infrastructure `
  --startup-project src/FurryFriends.Web `
  --output-dir "Data/Migrations"
```

---

## Step 5: Implement Entities (Core Layer)

Create entity classes in `src/FurryFriends.Core/TimeslotAggregate/`:

1. **Timeslot.cs** - Main timeslot entity
2. **TravelBuffer.cs** - Travel time between appointments
3. **WorkingHours.cs** - Petwalker's schedule
4. **TimeslotStatus.cs** - Enum (Available, Booked, Unavailable, Cancelled)

---

## Step 6: Implement Specifications (Core Layer)

Create in `src/FurryFriends.Core/TimeslotAggregate/Specifications/`:

- `AvailableTimeslotsByPetWalkerAndDateSpecification.cs`
- `OverlappingTimeslotsSpecification.cs`
- `TimeslotsWithinWorkingHoursSpecification.cs`

---

## Step 7: Implement Commands & Queries (UseCases Layer)

Create in `src/FurryFriends.UseCases/Timeslots/`:

### Commands (writes):
- `CreateTimeslot/` - Create new timeslot
- `UpdateTimeslot/` - Modify timeslot
- `DeleteTimeslot/` - Remove timeslot
- `BookTimeslot/` - Client books a slot

### Queries (reads):
- `GetTimeslots/` - Petwalker views their slots
- `GetAvailableTimeslots/` - Client views available slots

### WorkingHours:
- `CreateWorkingHours/`
- `GetWorkingHours/`
- `UpdateWorkingHours/`
- `DeleteWorkingHours/`

---

## Step 8: Implement API Endpoints (Web Layer)

Create in `src/FurryFriends.Web/Endpoints/TimeslotEndpoints/`:

For each operation:
- `{Operation}Request.cs` - Request DTO
- `{Operation}Response.cs` - Response DTO
- `{Operation}Validator.cs` - FluentValidation validator
- `{Operation}.cs` - FastEndpoints endpoint

---

## Step 9: Update Blazor UI

### Petwalker Side (`src/FurryFriends.BlazorUI/`):

- **Services/Implementation/TimeslotService.cs** - HTTP calls to API
- **Components/Pages/Timeslots/** - Timeslot management pages

### Client Side (`src/FurryFriends.BlazorUI.Client/`):

- **Services/ITimeslotService.cs** - Service interface
- **Components/Pages/Booking/DateTimeSelectionComponent.razor** - Updated to show timeslots

---

## Step 10: Run Tests

```bash
# Unit tests
dotnet test tests/FurryFriends.UnitTests

# Integration tests
dotnet test tests/FurryFriends.IntegrationTests
```

---

## Quick Reference: API Endpoints

| Operation | Method | Route |
|-----------|--------|-------|
| Create Timeslot | POST | `/api/timeslots` |
| Get Timeslots | GET | `/api/petwalkers/{id}/timeslots` |
| Get Available | GET | `/api/petwalkers/{id}/timeslots/available` |
| Update Timeslot | PUT | `/api/timeslots/{id}` |
| Delete Timeslot | DELETE | `/api/timeslots/{id}` |
| Book Timeslot | POST | `/api/timeslots/{id}/book` |
| Create WorkingHours | POST | `/api/petwalkers/{id}/workinghours` |
| Get WorkingHours | GET | `/api/petwalkers/{id}/workinghours` |

---

## Common Implementation Patterns

### Result Pattern
```csharp
// Handler returns Result<T>
public async Task<Result<Guid>> Handle(CreateTimeslotCommand cmd, CancellationToken ct)
{
    Guard.Against.Null(cmd, nameof(cmd));
    
    if (overlappingSlots.Any())
    {
        return Result.Error("Timeslot overlaps with existing slot");
    }
    
    var timeslot = Timeslot.Create(cmd.PetWalkerId, cmd.Date, cmd.StartTime, cmd.Duration);
    await _repository.AddAsync(timeslot, ct);
    
    return Result<Guid>.Success(timeslot.Id);
}
```

### Specification Pattern
```csharp
// Use for database queries
public class AvailableTimeslotsSpecification : Specification<Timeslot>
{
    public AvailableTimeslotsSpecification(Guid petWalkerId, DateOnly date)
    {
        Query.Where(t => t.PetWalkerId == petWalkerId)
             .Where(t => t.Date == date)
             .Where(t => t.Status == TimeslotStatus.Available)
             .OrderBy(t => t.StartTime);
    }
}
```

---

## Troubleshooting

### Migration Issues
If migration fails:
```bash
# Remove pending migrations
dotnet ef migrations remove --project src/FurryFriends.Infrastructure

# Re-add migration
dotnet ef migrations add PetwalkerTimeslots_V2
```

### Test Failures
Check:
1. Database is seeded with test data
2. EF Core in-memory provider used for unit tests
3. Authorization policies configured

---

## Next Steps

After implementation:

1. Run all tests
2. Test via Swagger UI at `/swagger`
3. Verify Blazor UI integration
4. Check Serilog logs for any errors

---

## References

- [Constitution](../.specify/memory/constitution.md)
- [Technical Guide](../../docs/FurryFriends_Technical_Guide.md)
- [Feature Spec](spec.md)
- [Data Model](data-model.md)
