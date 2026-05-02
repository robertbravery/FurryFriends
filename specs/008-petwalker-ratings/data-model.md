# Data Model: PetWalker Rating Aggregation

## Entities

### Rating

**Purpose**: Represents a single rating submission from a pet owner for a petwalker.

| Property | Type | Constraints | Description |
|----------|------|-------------|-------------|
| Id | Guid | PK, required | Unique identifier |
| PetWalkerId | Guid | Required, FK | Reference to petwalker |
| ClientId | Guid | Required, FK | Reference to client (pet owner) |
| RatingValue | int | Required, 1-5 | Numerical score (whole numbers only) |
| Comment | string? | Max 1000 chars | Optional text comments |
| Status | RatingStatus | Required | Active, Moderated, or Removed |
| CreatedDate | DateTime | Required | Timestamp of submission |
| ModifiedDate | DateTime? | Nullable | Timestamp of last edit |

**Relationships**:
- Many Ratings → One PetWalker
- Many Ratings → One Client

**Validation Rules**:
- RatingValue must be between 1 and 5
- Comment maximum 1000 characters
- Only one ACTIVE rating per Client per PetWalker allowed

**State Transitions**:
- Active → Moderated (admin action)
- Moderated → Removed (admin action)

---

### PetWalker (Modified)

**Purpose**: Represents a petwalker in the system. Extended with denormalized rating aggregates.

| Property | Type | Constraints | Description |
|----------|------|-------------|-------------|
| Id | Guid | PK, required | Unique identifier |
| AverageRating | double? | Nullable | Computed average (rounded to 1 decimal) |
| TotalRatingsCount | int | Default 0 | Total number of ratings received |

**Relationships**:
- One PetWalker → Many Ratings

**Update Behavior**:
- AverageRating and TotalRatingsCount updated via domain events when ratings are added, modified, or removed

---

### Client (Existing)

**Purpose**: Represents pet owner who books petwalkers.

| Property | Type | Constraints | Description |
|----------|------|-------------|-------------|
| Id | Guid | PK, required | Unique identifier |

**Relationships**:
- One Client → Many Ratings (for different petwalkers)

---

### Booking (Reference Only)

**Purpose**: Used for eligibility verification only. Not referenced by Rating entity.

**Eligibility Check**:
- Client must have at least one booking with PetWalker where Status = Completed
- Cancelled and NoShow bookings do NOT qualify

---

## Enumerations

### RatingStatus

```csharp
public enum RatingStatus
{
    Active = 0,    // Visible to public
    Moderated = 1, // Under review by admin
    Removed = 2    // Hidden from public
}
```

---

## Database Schema Changes

### New Columns on PetWalker Table

```sql
ALTER TABLE PetWalkers
ADD AverageRating DECIMAL(3,1) NULL,
ADD TotalRatingsCount INT NOT NULL DEFAULT 0;
```

### Rating Table Changes (if not already done)

```sql
-- Remove BookingId reference
ALTER TABLE Ratings
DROP COLUMN BookingId;

-- Add Status column
ALTER TABLE Ratings
ADD Status INT NOT NULL DEFAULT 0;
```

---

## Index Recommendations

```sql
-- Rating lookups by PetWalker
CREATE INDEX IX_Ratings_PetWalkerId ON Ratings(PetWalkerId);

-- Rating lookups by Client
CREATE INDEX IX_Ratings_ClientId ON Ratings(ClientId);

-- Active ratings only queries
CREATE INDEX IX_Ratings_PetWalkerId_Status 
ON Ratings(PetWalkerId, Status) 
WHERE Status = 0;
```

---

## Domain Events

| Event | Trigger | Handler Action |
|-------|---------|-----------------|
| RatingAddedEvent | New rating created with Active status | Recalculate PetWalker aggregate |
| RatingUpdatedEvent | Rating modified | Recalculate PetWalker aggregate |
| RatingRemovedEvent | Rating deleted or status changed to Removed | Recalculate PetWalker aggregate |

---

## Aggregate Methods (PetWalker)

```csharp
public void UpdateRatingAggregate(double averageRating, int totalCount)
{
    AverageRating = averageRating;
    TotalRatingsCount = totalCount;
}
```

---

*Data model created: 2026-04-12*