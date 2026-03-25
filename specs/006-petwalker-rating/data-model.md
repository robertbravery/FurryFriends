# Data Model: Petwalker Rating System

## Entity Definition

### Rating Entity (Extended)

**Location**: `src/FurryFriends.Core/RatingAggregate/Rating.cs`

**Fields**:

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| Id | Guid | Primary Key, Required | Unique identifier |
| PetWalkerId | Guid | Required, FK | Reference to rated petwalker |
| ClientId | Guid | Required, FK | Reference to client who rated |
| BookingId | Guid | Required, FK | Reference to completed booking |
| RatingValue | int | Required, 1-5 | Star rating (1-5) |
| Comment | string? | Optional, max 1000 chars | Optional text feedback |
| CreatedDate | DateTime | Required | When rating was submitted |
| ModifiedDate | DateTime? | Optional | When rating was last updated |

**Relationships**:
- Rating → PetWalker (many-to-one)
- Rating → Client (many-to-one)  
- Rating → Booking (many-to-one, unique constraint on BookingId)

**Business Rules**:
1. RatingValue must be between 1 and 5 (inclusive)
2. One rating per booking (unique constraint on BookingId)
3. Rating can only be updated within 7 days of creation
4. Rating can only be updated once

---

## DTOs (Data Transfer Objects)

### CreateRatingDto
```csharp
public record CreateRatingDto(
    Guid BookingId,
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
    Guid BookingId,
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

---

## Specifications (Query Objects)

### GetRatingsForPetWalkerSpecification
- Filter by PetWalkerId
- OrderBy CreatedDate descending
- Include Client navigation for name
- Support pagination (page, pageSize)

### GetRatingByBookingIdSpecification
- Filter by BookingId
- Single result

### GetPetWalkerRatingSummarySpecification
- Filter by PetWalkerId
- Calculate average rating
- Count total ratings
- Include recent ratings (last 10)

---

## API Contracts

### POST /api/ratings
**Create a new rating**

**Request**:
```json
{
  "bookingId": "guid",
  "ratingValue": 5,
  "Comment": "Great service!"
}
```

**Response** (201 Created):
```json
{
  "id": "guid",
  "petWalkerId": "guid",
  "clientId": "guid",
  "bookingId": "guid",
  "ratingValue": 5,
  "comment": "Great service!",
  "createdDate": "2026-03-19T10:00:00Z",
  "modifiedDate": null
}
```

**Error Responses**:
- 400: Validation error (invalid rating value, missing fields)
- 404: Booking not found
- 409: Rating already exists for this booking

---

### PUT /api/ratings/{id}
**Update an existing rating**

**Request**:
```json
{
  "ratingValue": 4,
  "comment": "Updated comment"
}
```

**Response** (200 OK):
```json
{
  "id": "guid",
  "petWalkerId": "guid",
  "clientId": "guid",
  "bookingId": "guid",
  "ratingValue": 4,
  "comment": "Updated comment",
  "createdDate": "2026-03-19T10:00:00Z",
  "modifiedDate": "2026-03-20T10:00:00Z"
}
```

**Error Responses**:
- 400: Validation error
- 403: Update not allowed (rating is older than 7 days or has already been updated)
- 404: Rating not found

---

### GET /api/petwalkers/{id}/ratings
**Get all ratings for a petwalker**

**Query Parameters**:
- `page` (int, default: 1)
- `pageSize` (int, default: 20)

**Response** (200 OK):
```json
{
  "items": [
    {
      "id": "guid",
      "petWalkerId": "guid",
      "clientId": "guid",
      "bookingId": "guid",
      "ratingValue": 5,
      "comment": "Great service!",
      "createdDate": "2026-03-19T10:00:00Z",
      "modifiedDate": null,
      "clientName": "John Doe"
    }
  ],
  "page": 1,
  "pageSize": 20,
  "totalCount": 50,
  "totalPages": 3
}
```

---

### GET /api/petwalkers/{id}/ratings/summary
**Get rating summary for a petwalker**

**Response** (200 OK):
```json
{
  "petWalkerId": "guid",
  "averageRating": 4.5,
  "totalRatings": 50,
  "recentRatings": [
    {
      "id": "guid",
      "ratingValue": 5,
      "comment": "Great!",
      "createdDate": "2026-03-19T10:00:00Z",
      "clientName": "John Doe"
    }
  ]
}
```

---

## Validation Rules

### CreateRatingCommand
| Field | Rules |
|-------|-------|
| BookingId | Required, must be valid GUID |
| RatingValue | Required, integer 1-5 |
| Comment | Optional, max 1000 characters |

### UpdateRatingCommand
| Field | Rules |
|-------|-------|
| RatingValue | Required, integer 1-5 |
| Comment | Optional, max 1000 characters |

---

## Database Schema Changes

### New Columns (Migration)
```sql
ALTER TABLE Ratings ADD BookingId uniqueidentifier NOT NULL;
ALTER TABLE Ratings ADD ModifiedDate datetime2 NULL;

CREATE UNIQUE INDEX IX_Ratings_BookingId ON Ratings(BookingId) WHERE BookingId IS NOT NULL;
```

---

## File Structure

```
src/FurryFriends.Core/
└── RatingAggregate/
    ├── Rating.cs              # Extended entity
    └── Specifications/
        ├── GetRatingsForPetWalkerSpecification.cs
        ├── GetRatingByBookingIdSpecification.cs
        └── GetPetWalkerRatingSummarySpecification.cs

src/FurryFriends.UseCases/
└── Rating/
    ├── CreateRating/
    │   ├── CreateRatingCommand.cs
    │   ├── CreateRatingHandler.cs
    │   ├── CreateRatingValidator.cs
    │   └── CreateRatingDto.cs
    ├── UpdateRating/
    │   ├── UpdateRatingCommand.cs
    │   ├── UpdateRatingHandler.cs
    │   └── UpdateRatingValidator.cs
    ├── GetRatingsForPetWalker/
    │   ├── GetRatingsForPetWalkerQuery.cs
    │   ├── GetRatingsForPetWalkerHandler.cs
    │   └── RatingDto.cs
    └── GetPetWalkerRatingSummary/
        ├── GetPetWalkerRatingSummaryQuery.cs
        ├── GetPetWalkerRatingSummaryHandler.cs
        └── PetWalkerRatingSummaryDto.cs

src/FurryFriends.Web/
└── Endpoints/
    └── RatingEndpoints/
        ├── CreateRating/
        │   ├── CreateRatingRequest.cs
        │   ├── CreateRatingResponse.cs
        │   ├── CreateRatingValidator.cs
        │   └── CreateRating.cs
        ├── UpdateRating/
        ├── GetRatingsForPetWalker/
        └── GetPetWalkerRatingSummary/
```

---

**Date**: 2026-03-19
