# Deferred Requirements - PetWalker Ratings v2

**Feature**: 008-petwalker-ratings  
**Deferred**: 2026-04-12  
**Status**: Pending (for future PR)

---

## Purpose

These requirements were not included in v1 to keep the PR focused. They are captured here to ensure they are not forgotten for future implementation.

---

## Deferred Items

### D1: Rating Display Format & Breakdown
- **Current question**: What is the minimum number of ratings before an average is displayed? Should we show individual rating breakdowns (e.g., count of 1-star, 2-star, etc.)?
- **Priority**: Low (v2)
- **Impact**: UI/UX enhancement

### D2: Rating Moderation Policy
- **Current question**: How should the system handle fraudulent, abusive, or inappropriate ratings? Who can moderate them and what is the process?
- **FR-011 placeholder**: System MUST implement rating moderation features - who can moderate, what actions can be taken
- **Priority**: Medium (requires policy decision)
- **Impact**: Backend + Admin UI

### D3: PetWalker Response to Ratings
- **Current question**: Can petwalkers respond to ratings publicly? Should responses be displayed alongside the rating?
- **Priority**: Low (v2)
- **Impact**: UI + new "Response" entity

### D4: Rating Preservation on PetWalker Account Deletion
- **Current question**: What happens when a petwalker's account is deactivated or deleted? Should their ratings be preserved?
- **FR-012 placeholder**: System MUST maintain rating history even if a petwalker's account is deactivated
- **Priority**: Medium (requires policy decision)
- **Impact**: Data retention strategy

### D5: Data Retention Policy
- **Current question**: Should ratings expire or be removed after a certain period?
- **Priority**: Low (v2)
- **Impact**: Database maintenance

---

## Summary for Future Spec

When ready to implement v2, create a new spec file (e.g., `009-petwalker-ratings-v2.md`) that builds on `008-petwalker-ratings` and includes:

1. Rating breakdown/histogram display
2. Rating moderation workflow (admin features)
3. PetWalker response capability
4. Account deletion handling for ratings
5. Data retention/expiration rules

---

## User Requirements Context (from v1)

For reference, the v1 requirements that ARE implemented:

- Rating per petwalker (not per booking)
- Eligibility: ≥1 completed booking with that petwalker
- Multiple ratings allowed, but **# ratings ≤ # completed bookings**
- One active rating per client per petwalker (new replaces old)
- Comment optional with rating
- Edit/delete within 24 hours

See `spec.md` for complete v1 specification.
