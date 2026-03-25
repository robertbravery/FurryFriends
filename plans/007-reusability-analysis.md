# Code Reusability Analysis: 006 → 007 (Petwalker-Wide Rating)

## Branch 006 Status
✅ Committed and pushed to `006-petwalker-rating`
✅ PR created: https://github.com/robertbravery/FurryFriends/pull/new/006-petwalker-rating

---

## Reusability Matrix

### ✅ Fully Reusable (Keep As-Is)

| Component | Location | Notes |
|-----------|----------|-------|
| `PetWalkerId` | Rating.cs | Core field unchanged |
| `ClientId` | Rating.cs | Core field unchanged |
| `RatingValue` (1-5) | Rating.cs | Same validation |
| `Comment` | Rating.cs | Optional text |
| `CreatedDate/ModifiedDate` | Rating.cs | Audit fields |
| `StarRating.razor` | BlazorUI.Client | Visual component works |
| `PetWalkerRatingsDashboard.razor` | BlazorUI.Client | Petwalker view works |
| `RatingConfiguration.cs` | Infrastructure | Base config, modify constraint |
| Specifications (most) | Core/RatingAggregate/Specifications | Filter by PetWalker still works |

### ⚠️ Needs Modification

| Component | Current (006) | Required (007) | Effort |
|----------|--------------|----------------|--------|
| `Rating.cs` | Has `BookingId` | Remove `BookingId` | Low |
| `CreateRatingRequest` | `BookingId` param | Replace with `PetWalkerId` | Low |
| `CreateRatingCommand/Handler` | Requires `BookingId` | Remove booking dependency | Medium |
| `RatingSubmission.razor` | Enter BookingId textbox | Search/select petwalker UI | Medium |
| `RatingConfiguration` | Unique on `BookingId` | Unique on `ClientId+PetWalkerId` | Low |
| `GetRatingByBookingIdSpecification` | Exists | Can remove or repurpose | Low |
| `UpdateRating` logic | 7-day + one-update | May need different rules | Medium |

### 🗑️ Not Needed for 007

- Booking lookup functionality
- One-rating-per-booking constraint
- Booking context in rating

---

## Recommended Approach

### Step 1: Create new branch 007
```bash
git checkout -b 007-petwalker-rating
```

### Step 2: Modify Rating entity
- Remove `BookingId` property
- Remove from constructor and `Create()` method
- Update unique constraint to `(ClientId, PetWalkerId)`

### Step 3: Modify CreateRating flow
- Replace `BookingId` → `PetWalkerId` in request/command
- Add validation: one rating per client per petwalker (instead of per booking)

### Step 4: New UI for RatingSubmission
- Client searches for petwalker by name/location
- Selects petwalker → submits rating
- No more booking GUID input needed

### Step 5: Keep reusable components
- StarRating component ✓
- PetWalkerRatingsDashboard ✓
- Rating DTOs (modify fields) ✓
- Use case handlers (modify logic) ✓
- Endpoints (modify input) ✓

---

## Summary

**Reusable**: ~60% of code (entity base, UI components, endpoints structure)
**Modified**: ~30% (remove booking, add petwalker selection)
**New**: ~10% (new validation logic, new UI flow)