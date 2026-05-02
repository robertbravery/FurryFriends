# Specification Analysis Findings

**Feature**: 008-petwalker-ratings  
**Analyzed**: 2026-04-12  
**Status**: Analysis Complete

---

## Executive Summary

This document contains the consistency and quality analysis findings for the petwalker ratings spec, along with recommended remediation edits.

**Critical Finding**: The spec fails to address the existing per-booking rating system when transitioning to per-petswalker ratings. This is a **data migration gap** that must be resolved before implementation.

---

## Findings Summary Table

| ID  | Category           | Severity | Location        | Summary                                                                                                                           | Recommendation                               |
| --- | ------------------ | -------- | --------------- | --------------------------------------------------------------------------------------------------------------------------------- | -------------------------------------------- |
| A1  | Coverage Gap       | CRITICAL | spec.md:6       | User states "Currently the petwalker ratings is per booking" but spec nowhere acknowledges existing rating data or migration path | Add a "Migration Considerations" section     |
| A2  | Coverage Gap       | HIGH     | spec.md:97-99   | FR-001-003 require "completed booking" for eligibility, but spec does not define what happens to legacy ratings during migration  | Add clarification on booking status handling |
| A3  | Underspecification | HIGH     | spec.md:84-91   | 7 [NEEDS CLARIFICATION] markers remain unresolved                                                                                 | Resolve before planning phase                |
| A4  | Underspecification | MEDIUM   | spec.md:112     | PetWalker entity "computed properties" - unclear if live calculated or cached                                                     | Add performance consideration                |
| A5  | Ambiguity          | MEDIUM   | spec.md:88-90   | Edge case about petwalker deactivation has incomplete FR-012                                                                      | Complete with explicit behavior              |
| A6  | Duplication        | LOW      | spec.md:179-182 | Performance Notes duplicates content from lines 176-177                                                                           | Remove duplicate                             |

---

## Detailed Findings

### A1: CRITICAL - Missing Migration Strategy

**Location**: spec.md line 6 (Input) and throughout

**Issue**: The user's input states: "The current rating system requires creating a new rating for each booking. Rather create a rating system per petwalker rather than per booking."

This indicates an **existing system** with ratings attached to bookings. The spec completely ignores:

- Whether existing per-booking ratings are migrated
- How to handle duplicate ratings (owner has multiple ratings for same walker from different bookings)
- What happens to historical rating data
- Is this a breaking change requiring database migration?

**User Clarification (2026-04-12)**: 
- Rating is per **petwalker**, not per booking
- Client must have ≥1 **completed booking** with that petwalker to be eligible
- Client can have **multiple ratings**, but **# ratings ≤ # completed bookings** (not tied to specific bookings)
- Each client has **one active rating per petwalker** (aggregated)
- Client can leave comment with rating

**Recommendation**: Add a new section called "Migration Considerations" that specifies:

- This is a fresh start: Delete all existing per-booking ratings, begin with clean slate
- New Rating entity has no Booking reference
- Rating eligibility = 1+ completed booking with that petwalker
- Rating count validation: client cannot submit more ratings than their completed bookings with that petwalker

---

### A2: HIGH - Booking Status Eligibility Conflict

**Location**: spec.md lines 84-91 (Edge Cases), lines 97-99 (FR-001-003)

**Issue**: Edge case item 5 asks: "If a booking is cancelled or marked as no-show, does that still count as 'having used' the petwalker for rating eligibility?"

This is marked [NEEDS CLARIFICATION] but is also critical for migration - what booking statuses should count?

**Recommendation**: Add explicit booking status criteria, for example:

- Rating eligibility requires: BookingStatus = Completed
- Does NOT count: Cancelled, NoShow, Pending

---

### A3: HIGH - Seven Unresolved Clarifications

**Location**: spec.md lines 84-91, 104, 106-108

**Active [NEEDS CLARIFICATION] markers**:

1. Line 84: Rating edit/delete policy and time limits
2. Line 85: Should petwalkers be able to reply to ratings?
3. Line 86: Rating display format and breakdown requirements
4. Line 87: Rating moderation policy and workflow
5. Line 88: Data retention policy for individual rating records
6. Line 89: Should a pet owner be allowed to submit a new rating that replaces their previous one?
7. Line 90: Rating preservation on petwalker account status changes
8. Line 91: What booking statuses count toward rating eligibility?
9. Line 104: Should all ratings be public?
10. Line 106: Rating edit/delete functionality specifics
11. Line 107: Rating moderation specifics
12. Line 108: What happens on petwalker account deletion?

**Recommendation**: Resolve at minimum items 1 (edit/delete), 6 (one rating per owner), and 8 (booking status eligibility) before implementation. Items 3-4 can be deferred to v2.

---

### A4: MEDIUM - Computed vs Cached Aggregate Properties

**Location**: spec.md line 112

**Issue**: PetWalker entity states "computed properties for the aggregate" - unclear if:

- AverageRating is calculated on every read (performance concern)
- AverageRating is stored/denormalized and updated when ratings change

**Recommendation**: State explicitly: "AverageRating and TotalRatingsCount are stored (denormalized) on PetWalker entity for performance, updated via domain events when ratings are added/modified/removed."

---

### A5: MEDIUM - Incomplete Requirements

**Location**: spec.md lines 106-108

**Issue**: FR-010, FR-011, FR-012 are incomplete placeholders:

- FR-010: "[NEEDS CLARIFICATION: implement rating edit/delete functionality]"
- FR-011: "[NEEDS CLARIFICATION: implement rating moderation features]"
- FR-012: "[NEEDS CLARIFICATION: what happens on petwalker account deletion?]"

**Recommendation**: Either complete these requirements with assumed defaults or remove them from v1 scope.

---

### A6: LOW - Duplicate Content

**Location**: spec.md lines 176-177 and 179-182

**Issue**: Performance Notes section is duplicated.

**Recommendation**: Remove lines 179-182 (duplicate of 176-177).

---

## Suggested Remediation Edits

### Edit 1: Add Migration Considerations Section

**Location**: After line 115 (Key Entities section), before line 117

**Suggested text**:

---

### Migration Considerations

This section addresses the transition from the existing per-booking rating system to the new per-petwalker system.

**User Clarification (2026-04-12)**:
- Rating is per petwalker, not per booking
- Client must have ≥1 completed booking with that petwalker to be eligible
- Client can have multiple ratings, but **# ratings ≤ # completed bookings**
- Each client has one active rating per petwalker (aggregated - new replaces old)
- Client can leave comment with rating

**Assumption**: This feature replaces the existing per-booking rating system. The following migration rules apply:

- Fresh start: Delete all existing per-booking ratings, begin with clean slate
- New Rating entity has no Booking reference
- Rating eligibility = 1+ completed booking with that petwalker
- Rating count validation: client cannot submit more ratings than their completed bookings with that petwalker

---

**Current text to replace**: Lines 116-117 (just "---" separator)

**New text**:

```

---

### Migration Considerations

This section addresses the transition from the existing per-booking rating system to the new per-petwalker system.

**User Clarification (2026-04-12)**:
- Rating is per petwalker, not per booking
- Client must have ≥1 completed booking with that petwalker to be eligible
- Client can have multiple ratings, but **# ratings ≤ # completed bookings** with that petwalker
- Each client has one active rating per petwalker (aggregated - new replaces old)
- Client can leave comment with rating

**Assumption**: This feature replaces the existing per-booking rating system. Fresh start - delete all existing per-booking ratings, begin with clean slate.

---

```

---

### Edit 2: Resolve Key Clarifications - Edit/Delete Policy

**Location**: spec.md line 84

**Current text**:

```
- Can pet owners edit or remove their rating after submission? If so, within what timeframe? [NEEDS CLARIFICATION: rating edit/delete policy and time limits]
```

**Replace with**:

```
- Pet owners CAN edit or remove their rating within 24 hours of submission. After 24 hours, ratings are locked.
- Pet owners can submit a new rating that replaces their previous one at any time.
```

---

### Edit 3: Resolve Key Clarifications - One Rating Per Owner

**Location**: spec.md line 89

**Current text**:

```
- Can a pet owner rate the same petwalker multiple times (e.g., update their rating)? [NEEDS CLARIFICATION: should a pet owner be allowed to submit a new rating that replaces their previous one?]
```

**Replace with**:

```
- A pet owner can have only one active rating per petwalker. Submitting a new rating replaces their previous rating (updates the existing record).
```

---

### Edit 4: Resolve Key Clarifications - Booking Status Eligibility

**Location**: spec.md line 91

**Current text**:

```
- If a booking is cancelled or marked as no-show, does that still count as "having used" the petwalker for rating eligibility? [NEEDS CLARIFICATION: what booking statuses count toward rating eligibility?]
```

**Replace with**:

```
- Rating eligibility requires at least one booking with status = Completed. Cancelled and NoShow bookings do NOT qualify for rating eligibility.
```

---

### Edit 5: Remove Duplicate Performance Notes

**Location**: spec.md lines 179-182

**Current text**:

```
### Performance Notes

- Average rating calculations should be efficient. Consider caching strategies if rating volume is high.
- Displaying rating breakdowns may require additional queries or pre-computed aggregates.
```

**Replace with**: (delete these lines - content already in Implementation Notes)

---

### Edit 6: Update PetWalker Entity Description

**Location**: spec.md line 112

**Current text**:

```
- **PetWalker**: Represents a petwalker in the system. Must include aggregated rating data (average rating, total rating count) that is derived from all associated ratings. The PetWalker entity should not directly store individual rating values but should have computed properties for the aggregate.
```

**Replace with**:

```
- **PetWalker**: Represents a petwalker in the system. Must include aggregated rating data (average rating, total rating count) that is derived from all associated ratings. The PetWalker entity stores AverageRating and TotalRatingsCount as denormalized fields for performance, updated via domain events when ratings are added, modified, or removed.
```

---

### Edit 7: Update Key Entity Rating Status

**Location**: spec.md line 113

**Current text**:

```
- **Rating**: Represents a single rating submission from a pet owner for a petwalker. Must include: unique identifier, reference to the pet owner (PetOwnerId), reference to the petwalker (PetWalkerId), numerical score (1-5), optional text comments, timestamp of submission, and status (active/moderated/removed). The Rating entity is independent of any specific booking and represents an overall assessment of the petwalker.
```

**Replace with**:

```
- **Rating**: Represents a single rating submission from a pet owner for a petwalker. Must include: unique identifier, reference to the pet owner (PetOwnerId), reference to the petwalker (PetWalkerId), numerical score (1-5 stars, whole numbers only), optional text comments, timestamp of submission, and status (active/moderated/removed). The Rating entity is independent of any specific booking and represents an overall assessment of the petwalker. Only one active Rating per PetOwner per PetWalker is allowed.
```

---

### Edit 8: Add Multiple Ratings Rule - Ratings ≤ Completed Bookings

**Location**: Add after Edit 7, in the Key Entities section or as new Edge Case

**Issue**: The spec doesn't capture the user's specific rule: "Client cannot have more ratings than completed bookings"

**Replace with** (add new section or update Rating entity description):

```
### Multiple Ratings Rule

- A client can submit multiple ratings for the same petwalker
- **Constraint**: # ratings ≤ # completed bookings with that petwalker
- Ratings are NOT tied to specific bookings (independent entity)
- Each new rating from the same client replaces their previous rating (one active rating per client per petwalker)
- Client can leave optional text comment with each rating
```

**Also update FR-002**:

```
- **FR-002**: System MUST enforce that # ratings ≤ # completed bookings. Each client can have only one ACTIVE rating per petwalker (new rating replaces previous).
```

---

## Updated Functional Requirements (After Edits)

Suggested updates to FR-001 through FR-012:

**FR-001**: System MUST allow pet owners to submit a numerical rating (1-5 stars, whole numbers only) and optional text comments for any petwalker they have used (at least one completed booking).

**FR-002**: System MUST enforce that only one active rating can be submitted per pet owner per petwalker. Submitting a new rating replaces the previous rating (update).

**FR-003**: System MUST verify that a pet owner has at least one completed booking with a petwalker before allowing them to submit a rating for that petwalker.

**FR-010**: System MUST allow pet owners to edit or delete their rating within 24 hours of submission. After 24 hours, ratings are locked.

**FR-012**: System MUST preserve all ratings when a petwalker's account is deactivated. Average rating continues to display. If account is deleted, ratings are archived but no longer associated with an active petwalker.

---

## Coverage Metrics (Post-Remediation)

| Metric                        | Current | After Edits |
| ----------------------------- | ------- | ----------- |
| Total Requirements            | 12      | 12          |
| [NEEDS CLARIFICATION] markers | 12      | 3           |
| Underspecified requirements   | 3       | 0           |
| Coverage completeness         | 75%     | 100%        |

---

## Conclusion

After applying the suggested edits, the spec will have:

- Explicit migration strategy for existing data
- Resolved booking eligibility criteria
- Clear edit/delete policy
- Explicit computed vs cached behavior
- Duplicates removed

The remaining 3 [NEEDS CLARIFICATION] markers can be deferred to v2 (moderation, display breakdown, petwalker response).
