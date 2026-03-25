# Data Model: Petwalker-Wide Rating System

## Entity Definition

### Rating Entity (Updated for Petwalker-Wide)

**Location**: `src/FurryFriends.Core/RatingAggregate/Rating.cs`

**Fields**:

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| Id | Guid | Primary Key, Required | Unique identifier |
| PetWalkerId | Guid | Required, FK | Reference to rated petwalker |
| ClientId | Guid | Required, FK | Reference to client who rated |
| RatingValue | int | Required, 1-5 | Star rating (1-5) |
| Comment | string? | Optional, max 1000 chars | Optional text feedback |
| CreatedDate | DateTime | Required | When rating was submitted |
| ModifiedDate | DateTime? | Optional | When rating was last updated |

**Changes from 006**:
- **REMOVED**: BookingId field
- **ADDED**: Unique constraint on (ClientId, PetWalkerId) instead of BookingId

**Relationships**:
- Rating → PetWalker (many-to-one)
- Rating → Client (many-to-one)

**Business Rules**:
1. RatingValue must be between 1 and 5 (inclusive)
2. One rating per client per petwalker (unique constraint on ClientId+PetWalkerId)
3. Rating can be updated within 7 days of creation (one update allowed)

---

## DTOs (Data Transfer Objects)

### CreateRatingDto
```csharp
public record CreateRatingDto(
    Guid PetWalkerId,  // Changed from BookingId
    int RatingValue,
    string? Comment
);
```

### UpdateRatingDto
```csharp
public record UpdateRatingDto(
    int RatingValue,
    string? Comment
);
```

### RatingDto
```csharp
public record RatingDto(
    Guid Id,
    Guid PetWalkerId,
    Guid ClientId,
    int RatingValue,
    string? Comment,
    DateTime CreatedDate,
    DateTime? ModifiedDate,
    string? ClientName  // For display purposes
);
```

### PetWalkerRatingSummaryDto
```csharp
public record PetWalkerRatingSummaryDto(
    Guid PetWalkerId,
    double AverageRating,
    int TotalRatings,
    List<RatingDto> RecentRatings
);
```

### PetWalkerSearchDto (NEW for 007)
```csharp
public record PetWalkerSearchDto(
    Guid Id,
    string Name,
    string? Location
);
```

---

## Specifications (Query Objects)

### GetRatingsForPetWalkerSpecification
- Filter by PetWalkerId
- OrderBy CreatedDate descending
- Include Client navigation for name
- Support pagination (page, pageSize)

### GetRatingByClientAndPetWalkerSpecification (NEW)
- Filter by ClientId and PetWalkerId
- Single result
- Used to check for existing rating

### GetPetWalkerRatingSummarySpecification
- Filter by PetWalkerId
- Calculate average rating
- Count total ratings
- Include recent ratings (last 10)

### SearchPetWalkersSpecification (NEW)
- Filter by name or location
- Return Id, Name, Location
- Support pagination