# Quickstart: Petwalker-Wide Rating (007)

## Prerequisites
- .NET 9
- Existing PetWalker data from 003-petwalker-timeslots

## Key Difference from 006
- **006**: Client enters Booking ID to rate
- **007**: Client searches for petwalker by name/location to rate

## Quick Start

### 1. Modify Rating Entity
```csharp
// Remove BookingId from Rating.cs
public class Rating : BaseEntity<Guid>, IAggregateRoot
{
    public Guid PetWalkerId { get; private set; }
    public Guid ClientId { get; private set; }
    // REMOVE: public Guid BookingId { get; private set; }
    public int RatingValue { get; private set; }
    // ... rest unchanged
}
```

### 2. Update Rating Configuration
```csharp
// Change unique constraint
builder.HasIndex(r => new { r.ClientId, r.PetWalkerId }).IsUnique();
```

### 3. Update CreateRating Endpoint
```csharp
// Change input from BookingId to PetWalkerId
public class CreateRatingRequest
{
    public Guid PetWalkerId { get; set; }  // Was BookingId
    public int RatingValue { get; set; }
    public string? Comment { get; set; }
}
```

### 4. Add Petwalker Search
- Add endpoint to search petwalkers by name/location
- Return list of PetWalkerSearchDto

### 5. Update RatingSubmission UI
- Replace BookingId textbox with petwalker search
- Client searches → selects petwalker → rates

## Run and Test
1. Build solution
2. Search for a petwalker
3. Submit rating (1-5 stars)
4. Try to rate same petwalker again (should fail)
5. View petwalker rating summary