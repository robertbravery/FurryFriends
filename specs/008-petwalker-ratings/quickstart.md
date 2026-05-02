# Quickstart: PetWalker Rating Feature

This document provides a quick start guide for implementing and testing the PetWalker rating feature.

---

## Prerequisites

- .NET 9 SDK
- SQL Server (local or container)
- Entity Framework Core CLI tools
- xUnit test runner

---

## Implementation Order

### Step 1: Database Migration

```bash
# Navigate to infrastructure project
cd src/FurryFriends.Infrastructure

# Add migration for Rating changes and PetWalker denormalized fields
dotnet ef migrations add AddRatingStatusAndPetWalkerAggregates --startup-project ../FurryFriends.Web

# Apply migration
dotnet ef database update --startup-project ../FurryFriends.Web
```

### Step 2: Domain Layer (Core)

1. Update `Rating.cs`:
   - Remove `BookingId` property
   - Add `Status` property (RatingStatus enum)
   - Update `Create()` factory method

2. Update `PetWalker.cs`:
   - Add `AverageRating` property (double?)
   - Add `TotalRatingsCount` property (int)
   - Add `UpdateRatingAggregate()` method

3. Create domain events:
   - `RatingAddedEvent.cs`
   - `RatingUpdatedEvent.cs`
   - `RatingRemovedEvent.cs`

### Step 3: Use Cases Layer

1. Update/Create handlers in `FurryFriends.UseCases/Rating/`:
   - `CreateRatingCommand.cs` - Remove BookingId, add eligibility check
   - `CreateRatingHandler.cs` - Implement eligibility logic
   - `CreateRatingValidator.cs` - Update for new model
   - `UpdateRatingHandler.cs` - Add 24-hour window check
   - `DeleteRatingHandler.cs` - Add 24-hour window check

2. Create specifications:
   - `GetActiveRatingsForPetWalkerSpecification.cs`
   - `GetClientRatingsForPetWalkerSpecification.cs`
   - `CountCompletedBookingsForClientPetWalkerSpecification.cs`

### Step 4: Infrastructure Layer

1. Update `RatingConfiguration.cs`:
   - Remove BookingId mapping
   - Add Status mapping

2. Update `PetWalkerConfiguration.cs`:
   - Add AverageRating column configuration
   - Add TotalRatingsCount column configuration

### Step 5: API Layer (Web)

Update endpoints in `FurryFriends.Web/Endpoints/RatingEndpoints/`:
- `CreateRatingRequest.cs` - Remove BookingId
- `CreateRatingEndpoint.cs` - Update for new model
- `UpdateRatingEndpoint.cs` - Add 24-hour check response
- `DeleteRatingEndpoint.cs` - Add 24-hour check response

### Step 6: Blazor UI

1. Update `RatingService.cs`:
   - Update method signatures for new API

2. Update `IRatingService.cs`:
   - Add new methods if needed

3. Update/create components:
   - `RatingSubmission.razor` - Submit/edit ratings
   - `PetWalkerRatings.razor` - Display all ratings
   - `RatingDisplay.razor` - Star rating component

---

## Testing Quickstart

### Unit Tests

```bash
# Run unit tests
dotnet test tests/FurryFriends.UnitTests
```

**Key test scenarios**:
1. Rating entity creation (1-5 valid range)
2. 24-hour edit window enforcement
3. Eligibility check logic
4. Rating aggregate recalculation

### Integration Tests

```bash
# Run integration tests
dotnet test tests/FurryFriends.FunctionalTests
```

**Key test scenarios**:
1. Create rating with valid eligibility
2. Create rating without completed booking (should fail)
3. Create rating exceeding booking limit (should fail)
4. Update rating within 24 hours (should succeed)
5. Update rating after 24 hours (should fail)
6. Delete rating within 24 hours (should succeed)
7. Delete rating after 24 hours (should fail)

### Manual Testing

1. **Create test data**:
   - Create a Client
   - Create a PetWalker
   - Create a Completed Booking between them

2. **Submit rating**:
   ```http
   POST /api/ratings
   {
     "petWalkerId": "<petWalkerId>",
     "ratingValue": 5,
     "comment": "Great service!"
   }
   ```

3. **View ratings**:
   ```http
   GET /api/ratings/petwalker/<petWalkerId>
   ```

4. **View summary**:
   ```http
   GET /api/ratings/petwalker/<petWalkerId>/summary
   ```

---

## Verification Checklist

- [ ] Migration applied successfully
- [ ] Rating entity created without BookingId
- [ ] PetWalker has AverageRating and TotalRatingsCount
- [ ] Rating eligibility check works
- [ ] 24-hour edit window enforced
- [ ] API endpoints return correct status codes
- [ ] Blazor UI displays ratings correctly
- [ ] All unit tests pass
- [ ] All integration tests pass

---

*Quickstart created: 2026-04-12*