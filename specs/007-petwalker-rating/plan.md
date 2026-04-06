# Plan: Petwalker-Wide Rating System (007)

## Overview
This plan implements Option B from the rating approach analysis - rating the petwalker as a whole rather than per booking. Clients search for a petwalker and submit a single rating per petwalker.

## Dependencies
- Requires PetWalker entity to exist (from 003-petwalker-timeslots)
- Reuses Rating aggregate infrastructure from 006-petwalker-rating (modified)
- Requires PetWalker search/list functionality

## Key Changes from 006

### Data Layer
1. **Rating Entity** - Remove BookingId, update unique constraint
2. **Database Migration** - Change unique constraint from BookingId to (ClientId, PetWalkerId)
3. **Specifications** - Add GetRatingByClientAndPetWalkerSpecification

### API Layer
4. **CreateRating Endpoint** - Change from BookingId to PetWalkerId
5. **UpdateRating Endpoint** - Same 7-day logic (reusable)
6. **New Endpoint** - Search petwalkers by name/location

### Frontend Layer
7. **RatingSubmission.razor** - New UI: search/select petwalker
8. **StarRating component** - Reusable from 006

## Implementation Order

### Phase 1: Backend Core
1. Modify Rating entity (remove BookingId, update constructor)
2. Update RatingConfiguration (unique constraint)
3. Add new specification (GetRatingByClientAndPetWalkerSpecification)
4. Create database migration

### Phase 2: API
5. Update CreateRating endpoint/handler
6. Add petwalker search endpoint
7. Update validation (one rating per client per petwalker)

### Phase 3: Frontend
8. Update RatingSubmission.razor (search & select UI)
9. Add petwalker search service
10. Update RatingService

## Reusable Components (from 006)
- StarRating.razor component
- PetWalkerRatingsDashboard.razor
- Rating entity base (modified)
- GetRatingsForPetWalkerSpecification
- GetPetWalkerRatingSummarySpecification
- UpdateRating logic (7-day window)

## Not Reused (006-specific)
- GetRatingByBookingIdSpecification
- BookingId in any request/response
- One-rating-per-booking validation

## Estimated Effort
- Backend: 2-3 hours
- API: 2 hours  
- Frontend: 3-4 hours
- Testing: 2 hours
- **Total: ~10 hours**

## Success Criteria
- Client can search and find any petwalker
- Client can submit rating with 1-5 stars
- One rating per client per petwalker enforced
- Petwalker sees average rating and individual reviews
- Rating update works within 7 days