# Research: Petwalker Rating System

## Overview
This document captures the research findings for implementing the Petwalker Rating System feature, resolving all unknowns from the technical context.

## Key Findings

### 1. Existing Rating Entity Analysis

**Decision**: Extend existing Rating entity in `Core/RatingAggregate/Rating.cs`

**Rationale**: 
- Rating entity already exists with core fields: PetWalkerId, ClientId, RatingValue (1-5), Comment, CreatedDate
- Pattern follows Constitution: uses Guard Clauses, static factory method, private constructor
- Entity inherits from `BaseEntity<Guid>`

**Alternatives considered**:
- Create new Rating entity: Rejected - existing entity provides foundation
- Use value object: Rejected - ratings need identity for updates

### 2. Rating Enhancement Requirements

**Decision**: Add BookingId field to Rating entity

**Rationale**:
- FR-002 requires association with specific booking
- FR-006 requires one rating per booking (enforced via unique constraint)
- Links rating to completed service (not just client/petwalker pair)

**Implementation**:
- Add `public Guid BookingId { get; private set; }` to Rating entity
- Update `Create()` factory method to accept BookingId
- Add EF Core configuration for foreign key

### 3. Rating Update Window

**Decision**: Implement 7-day update window in Rating entity

**Rationale**:
- FR-003 allows clients to modify rating within 7 days
- Need method to check if update is allowed: `CanUpdate()` 
- Need to track if rating was ever updated: add `DateTime? ModifiedDate`

**Implementation**:
- Add `public DateTime? ModifiedDate { get; private set; }`
- Add method: `public bool CanUpdate() => CreatedDate.AddDays(7) > DateTime.UtcNow && ModifiedDate == null`
- Update `UpdateRatingValue()` and `UpdateComment()` to set ModifiedDate

### 4. PetWalker Rating Summary

**Decision**: Create query that aggregates ratings for petwalker display

**Rationale**:
- FR-003 requires average rating score
- FR-007 requires total number of ratings
- FR-009 requires descending order (most recent first)

**Implementation**:
- Create `PetWalkerRatingSummaryDto` with AverageRating, TotalRatings, RecentRatings list
- Use Specifications to query with aggregations
- Order by CreatedDate descending

### 5. API Endpoint Design

**Decision**: Create 4 FastEndpoints for rating functionality

**Rationale**:
- Submit rating (POST /ratings)
- Update rating (PUT /ratings/{id})
- Get ratings for petwalker (GET /petwalkers/{id}/ratings)
- Get rating summary for petwalker (GET /petwalkers/{id}/ratings/summary)

**Endpoints**:
```
POST   /api/ratings              - Create a rating
PUT    /api/ratings/{id}         - Update a rating (within 7 days)
GET    /api/petwalkers/{id}/ratings           - List ratings for petwalker
GET    /api/petwalkers/{id}/ratings/summary   - Get rating summary
```

### 6. Blazor UI Components

**Decision**: Create two main components following existing patterns

**Components**:
1. **RatingSubmissionComponent** - Modal/popup for clients to submit rating after booking completion
2. **PetWalkerRatingsDashboard** - Page for petwalkers to view their ratings

**Patterns to follow**:
- Use existing BookingService for checking booking completion status
- Follow popup-based workflow (Constitution standard)
- Server-side HTTP communication (no direct HttpClient in client)

### 7. Database Considerations

**Decision**: Add migration for Rating table changes

**Changes needed**:
1. Add BookingId column (nullable for backward compatibility, then NOT NULL after data migration)
2. Add ModifiedDate column
3. Add unique index on (BookingId) where BookingId is not null

## Summary of Technical Decisions

| Area | Decision | Rationale |
|------|----------|-----------|
| Rating Entity | Extend existing | Avoid duplication, leverage existing patterns |
| Booking Link | Add BookingId field | FR-002, FR-006 requirements |
| Update Window | 7-day check in entity | FR-003 requirement |
| API Design | 4 FastEndpoints | RESTful, follows existing patterns |
| UI Components | Rating submission + Dashboard | Client and petwalker use cases |
| Data Access | Specification pattern | Constitution requirement |

## Dependencies Identified

1. **Booking Aggregate** - Need to reference completed bookings for rating eligibility
2. **PetWalker Aggregate** - Need to query ratings per petwalker
3. **Client Aggregate** - Need to identify client submitting rating
4. **Existing TimeslotService** - May need to check booking completion status

## Risks and Mitigations

| Risk | Mitigation |
|------|------------|
| Existing ratings without BookingId | Make BookingId nullable, migrate data |
| Race condition on rating submission | Database unique constraint on BookingId |
| Performance with many ratings | Add pagination, caching for summary |

---

**Research Status**: COMPLETE - All unknowns resolved
**Date**: 2026-03-19
