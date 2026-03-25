# Rating System: Two Approaches Comparison

## Option A: Current Approach (Per-Booking Rating)
### How It Works
- Each rating is tied to a specific booking (`BookingId` required)
- Client must enter a booking GUID to submit a rating
- One rating allowed per booking

### Pros
- ✅ Can track which specific service was rated
- ✅ Can prevent duplicate ratings per booking (enforced in current spec)
- ✅ Allows updating rating within 7 days per booking

### Cons
- ❌ **User cannot find their booking ID** - it's a database GUID
- ❌ Requires complex UI flow to look up bookings
- ❌ Rating submission flow is broken until this is fixed

---

## Option B: Petwalker-Wide Rating
### How It Works
- Rating is associated directly with a `PetWalkerId`, not a booking
- Client searches/selects a petwalker to rate (by name or location)
- One rating per client per petwalker (not per booking)

### Pros
- ✅ **Simple, user-friendly UI** - just search for petwalker
- ✅ No booking lookup needed - solves the GUID problem
- ✅ Like rating a company (Uber, Airbnb) rather than each transaction
- ✅ Encourages more ratings since it's easier

### Cons
- ❌ Cannot attribute rating to a specific booking/service
- ❌ Cannot enforce "one rating per booking" rule
- ❌ Cannot allow 7-day update window per booking (no booking context)
- ❌ May need different logic to prevent client from flooding ratings

---

## Data Model Changes Required

### Option A (Current - Fix Booking ID)
```
No schema changes needed - fix the UI flow
```

### Option B (Petwalker-Wide)
```csharp
// Rating entity - remove BookingId
public class Rating : BaseEntity<Guid>, IAggregateRoot
{
    public Guid PetWalkerId { get; private set; }
    public Guid ClientId { get; private set; }
    // REMOVED: public Guid BookingId { get; private set; }
    public int RatingValue { get; private set; }
    public string? Comment { get; private set; }
    public DateTime CreatedDate { get; private set; }
}
```

---

## Implementation Effort

### Option A: Fix Booking ID Input
| Component | Effort | Notes |
|-----------|--------|-------|
| RatingSubmission.razor | Low | Add route param for booking ID from email |
| Backend API | Low | May need new endpoint to lookup by client |
| **Total** | **Low** | Quick fix, maintains current spec |

### Option B: Petwalker-Wide Rating
| Component | Effort | Notes |
|-----------|--------|-------|
| Rating entity (Core) | Medium | Remove BookingId, add unique constraint |
| Database migration | Medium | Drop FK, add unique on ClientId+PetWalkerId |
| CreateRating endpoint | Medium | Change from BookingId to PetWalkerId |
| RatingSubmission.razor | Medium | New UI: search/select petwalker |
| Validation logic | Medium | One rating per client per petwalker |
| **Total** | **Medium** | More comprehensive change |

---

## Recommendation

**If you want a quick fix**: Go with Option A (fix booking ID input) - add confirmation email with link containing booking ID in URL parameter.

**If you want better UX**: Go with Option B (petwalker-wide) - more aligned with how people rate services in real life.

> **Note**: The current spec (FR-006) requires "one rating per booking" which mandates Option A. If switching to Option B, update the spec to say "one rating per client per petwalker".