# Research: PetWalker Rating Aggregation

## Executive Summary

This document captures research findings for implementing a per-petwalker rating system, replacing the current per-booking system. All key technical decisions have been resolved through the feature specification.

---

## Decision 1: Rating Entity Design

**Decision**: Remove BookingId reference from Rating entity, add Status field (Active/Moderated/Removed)

**Rationale**: The new rating system is independent of specific bookings. Ratings are for the petwalker as a whole. The spec explicitly states "Rating entity has no Booking reference."

**Alternatives Considered**:
- Keep BookingId nullable - rejected because ratings are now per-petwalker, not per-booking
- Separate Rating and RatingHistory tables - deferred to v2

---

## Decision 2: PetWalker Denormalized Fields

**Decision**: Add AverageRating (double) and TotalRatingsCount (int) to PetWalker entity

**Rationale**: Performance requirements specify denormalized fields for fast reads. Average calculations must be efficient for <1000 ratings per petwalker.

**Alternatives Considered**:
- Calculate on-the-fly from Rating table - rejected for performance reasons
- Use SQL computed columns - possible future optimization

---

## Decision 3: Domain Events for Aggregate Updates

**Decision**: Use domain events (RatingAddedEvent, RatingUpdatedEvent, RatingRemovedEvent) to trigger PetWalker aggregate recalculation

**Rationale**: Constitution requires domain events for aggregate state changes. PetWalker is the aggregate root that maintains rating statistics.

**Implementation Pattern**:
```csharp
// In Rating entity or handler after save
await _domainEventDispatcher.DispatchAsync(new RatingAddedEvent(rating));
```

**Alternatives Considered**:
- Recalculate in handler directly - violates aggregate isolation principle
- Use database triggers - hidden logic, hard to test

---

## Decision 4: Eligibility Check Implementation

**Decision**: Check for at least one completed booking (BookingStatus.Completed) between client and petwalker before allowing rating submission

**Rationale**: Spec states "Client must have ≥1 completed booking with that petwalker to be eligible"

**Alternatives Considered**:
- Use any booking status - rejected, spec requires Completed only
- Check at booking completion time - adds complexity, defer to v2

---

## Decision 5: Ratings ≤ Bookings Constraint

**Decision**: Enforce # ratings ≤ # completed bookings per client-petwalker pair. New rating replaces previous active rating.

**Rationale**: Spec states "Client can have multiple ratings, but # ratings ≤ # completed bookings" and "Each client has one active rating per petwalker (new rating replaces previous)"

**Implementation**:
- Query completed bookings count for client-petwalker pair
- Query active ratings count for client-petwalker pair
- Fail if ratings >= bookings

---

## Decision 6: 24-Hour Edit Window

**Decision**: Ratings can be edited or deleted within 24 hours of submission. After 24 hours, ratings are locked.

**Rationale**: Spec requirement FR-010 explicitly states this behavior.

**Implementation**:
```csharp
public bool CanEdit() => CreatedDate.AddHours(24) > DateTime.UtcNow;
```

---

## Decision 7: Rating Status Transitions

**Decision**: Status enum: Active, Moderated, Removed. Active → Moderated (admin) → Removed.

**Rationale**: Clarified in spec session 2026-04-12.

**Note**: Moderation features deferred to v2 (FR-011).

---

## Technical Implementation Notes

### Entity Changes Required

1. **Rating.cs** - Remove BookingId, add Status property
2. **PetWalker.cs** - Add AverageRating, TotalRatingsCount fields with update methods
3. **New Events** - RatingAddedEvent, RatingUpdatedEvent, RatingRemovedEvent in PetWalkerAggregate/Events/

### Database

- New migration to remove BookingId from Ratings table (if not already done)
- New migration to add columns to PetWalker table (AverageRating, TotalRatingsCount)

### API Endpoints (FastEndpoints)

- POST /api/ratings - Create rating (requires eligibility check)
- PUT /api/ratings/{id} - Update rating (24-hour window)
- DELETE /api/ratings/{id} - Delete rating (24-hour window)  
- GET /api/ratings/petwalker/{petWalkerId} - List all ratings for petwalker
- GET /api/ratings/petwalker/{petWalkerId}/summary - Get average and count

### Blazor Components

- RatingSubmission.razor - Form for submitting/editing ratings
- PetWalkerRatings.razor - Display ratings on petwalker profile
- RatingDisplay.razor - Reusable star rating display component

---

## References

- Feature Spec: `specs/008-petwalker-ratings/spec.md`
- Constitution: `.specify/memory/constitution.md`
- Existing Implementation: `src/FurryFriends.Core/RatingAggregate/Rating.cs`

---

*Research completed: 2026-04-12*