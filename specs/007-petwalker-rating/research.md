# Research: Petwalker-Wide Rating (007)

## Key Decision: Per-Booking vs Petwalker-Wide Rating

### Context
The 006 implementation tied ratings to specific bookings, requiring clients to enter a booking GUID. This was identified as a UX problem - clients cannot find their booking ID.

### Decision Made
Switch to petwalker-wide rating:
- Rating is directly associated with a petwalker (not a booking)
- Client searches for petwalker by name/location
- One rating per client per petwalker

### Why This Approach
1. **Better UX**: Clients can easily find and rate a petwalker
2. **Industry Standard**: Similar to rating companies (Uber, Airbnb) not individual trips
3. **Simpler**: No booking lookup needed
4. **Encourages More Ratings**: Lower friction = more ratings

### Alternative Considered
- Option A: Keep per-booking but add email confirmation with booking ID link
- Rejected because: More complex, still ties to specific booking

### References
- Rating approach comparison: `plans/rating-approach-comparison.md`
- Reusability analysis: `plans/007-reusability-analysis.md`