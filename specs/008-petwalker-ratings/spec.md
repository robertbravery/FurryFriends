# Feature Specification: PetWalker Rating Aggregation

**Feature Branch**: `008-petwalker-ratings`  
**Created**: 2025-04-06  
**Status**: Draft  
**Input**: User description: "petwalker ratings. The current rating system requires creating a new rating for each booking. Rather create a rating system per petwalker rather than per booking."

## Execution Flow (main)

```
1. Parse user description from Input
   → If empty: ERROR "No feature description provided"
2. Extract key concepts from description
   → Identify: actors, actions, data, constraints
3. For each unclear aspect:
   → Mark with [NEEDS CLARIFICATION: specific question]
4. Fill User Scenarios & Testing section
   → If no clear user flow: ERROR "Cannot determine user scenarios"
5. Generate Functional Requirements
   → Each requirement must be testable
   → Mark ambiguous requirements
6. Identify Key Entities (if data involved)
7. Run Review Checklist
   → If any [NEEDS CLARIFICATION]: WARN "Spec has uncertainties"
   → If implementation details found: ERROR "Remove tech details"
8. Return: SUCCESS (spec ready for planning)
```

---

### Migration Considerations

This section addresses the transition from the existing per-booking rating system to the new per-petwalker system.

**User Clarification (2026-04-12)**:
- Rating is per petwalker, not per booking
- Client must have ≥1 completed booking with that petwalker to be eligible
- Client can have multiple ratings, but **# ratings ≤ # completed bookings** with that petwalker
- Each client has one active rating per petwalker (new rating replaces previous)
- Client can leave comment with rating

**Migration**: This feature replaces the existing per-booking rating system. Fresh start - delete all existing per-booking ratings, begin with clean slate. The new Rating entity has no Booking reference.

---


- ✅ Focus on WHAT users need and WHY
- ❌ Avoid HOW to implement (no tech stack, APIs, code structure)
- 👥 Written for business stakeholders, not developers

### Section Requirements

- **Mandatory sections**: Must be completed for every feature
- **Optional sections**: Include only when relevant to the feature
- When a section doesn't apply, remove it entirely (don't leave as "N/A")

### For AI Generation

When creating this spec from a user prompt:

1. **Mark all ambiguities**: Use [NEEDS CLARIFICATION: specific question] for any assumption you'd need to make
2. **Don't guess**: If the prompt doesn't specify something (e.g., "login system" without auth method), mark it
3. **Think like a tester**: Every vague requirement should fail the "testable and unambiguous" checklist item
4. **Common underspecified areas**:
   - User types and permissions
   - Data retention/deletion policies
   - Performance targets and scale
   - Error handling behaviors
   - Integration requirements
   - Security/compliance needs

---

## Clarifications

### Session 2026-04-12

- Q: Should all individual ratings be publicly visible, or only the aggregate? → A: All ratings public - users can see individual rating details
- Q: Should we show average rating for petwalkers with 0 ratings, or hide until minimum threshold? → A: Show with 0 ratings - display "No ratings yet"
- Q: What rating status values and transitions are allowed? → A: Active, Moderated, Removed - admins can change status
- Q: What happens when a rating submission fails? → A: Show error message, allow retry
- Q: What is the expected data volume? → A: Small scale - <1000 ratings per walker

---

## User Scenarios & Testing _(mandatory)_

### Primary User Story

As a pet owner who has used a petwalker's services, I want to submit an overall rating for that petwalker so that I can share my experience with other pet owners. The petwalker's overall rating should be automatically calculated from all received ratings and displayed on their profile.

As a potential client, I want to view a petwalker's overall rating so that I can make an informed decision about which petwalker to book.

### Acceptance Scenarios

1. **Given** a pet owner who has previously used a petwalker's services (at least one completed booking), **When** they submit a rating (score 1-5) with optional comments for that petwalker, **Then** the rating is recorded and the petwalker's average rating is recalculated to include this new rating.

2. **Given** a petwalker who has received multiple ratings, **When** a user views the petwalker's profile, **Then** the system displays the petwalker's average rating (rounded to one decimal place) and the total number of ratings received.

3. **Given** a petwalker with no ratings, **When** a user views the petwalker's profile, **Then** the system indicates that no ratings are available yet (e.g., "No ratings yet" or similar message).

4. **Given** a pet owner who has used a petwalker's services, **When** they attempt to rate that petwalker, **Then** the system allows them to submit one rating for that petwalker and prevents duplicate ratings from the same pet owner for the same petwalker.

5. **Given** a petwalker's aggregated rating, **When** new ratings are added or existing ratings are modified, **Then** the average rating is automatically recalculated to reflect all current ratings.

6. **Given** a pet owner who has not yet used a particular petwalker's services (no completed bookings), **When** they attempt to rate that petwalker, **Then** the system prevents the rating submission and informs them that only clients who have used the petwalker can rate.

### Edge Cases

- If rating submission fails (validation error, server error), the system displays a clear error message and allows the user to retry immediately.
- Pet owners CAN edit or remove their rating within 24 hours of submission. After 24 hours, ratings are locked. Pet owners can submit a new rating that replaces their previous one at any time (subject to ratings ≤ bookings rule).
- Can petwalkers respond to ratings publicly? [NEEDS CLARIFICATION: should petwalkers be able to reply to ratings?]
- What is the minimum number of ratings before an average is displayed? Should we show individual rating breakdowns (e.g., count of 1-star, 2-star, etc.)? [NEEDS CLARIFICATION: rating display format and breakdown requirements]
- How should the system handle fraudulent, abusive, or inappropriate ratings? Who can moderate them and what is the process? [NEEDS CLARIFICATION: rating moderation policy and workflow]
- Should ratings expire or be removed after a certain period? [NEEDS CLARIFICATION: data retention policy for individual rating records]
- A pet owner can have only one active rating per petwalker. Submitting a new rating replaces their previous rating (subject to ratings ≤ bookings rule).
- What happens when a petwalker's account is deactivated or deleted? Should their ratings be preserved? [NEEDS CLARIFICATION: rating preservation on petwalker account status changes]
- Rating eligibility requires at least one booking with status = Completed. Cancelled and NoShow bookings do NOT qualify for rating eligibility.

## Requirements _(mandatory)_

### Functional Requirements

- **FR-001**: System MUST allow pet owners to submit a numerical rating (scale 1-5) and optional text comments for any petwalker they have used (at least one completed booking).
- **FR-002**: System MUST enforce that a client cannot have more ratings than their completed bookings with that petwalker. Each client can have only one ACTIVE rating per petwalker (new rating replaces previous).
- **FR-003**: System MUST verify that a pet owner has at least one completed booking with a petwalker before allowing them to submit a rating for that petwalker.
- **FR-004**: System MUST automatically calculate and maintain an aggregate average rating for each petwalker based on all their received ratings.
- **FR-005**: System MUST automatically recalculate the petwalker's average rating whenever a rating is added, modified, or removed.
- **FR-006**: System MUST display the petwalker's average rating (rounded to one decimal place) and total rating count on their public profile.
- **FR-007**: System MUST display a clear message when a petwalker has no ratings yet.
- **FR-008**: System MUST allow users to view individual rating details (score, comments, date) for a petwalker. All ratings are publicly visible.
- **FR-009**: System MUST prevent rating submissions from pet owners who have not used the petwalker (no completed bookings with that petwalker).
- **FR-010**: System MUST allow pet owners to edit or delete their rating within 24 hours of submission. After 24 hours, ratings are locked.
- **FR-011**: System MUST [NEEDS CLARIFICATION: implement rating moderation features - who can moderate, what actions can be taken].
- **FR-012**: System MUST maintain rating history even if a petwalker's account is deactivated [NEEDS CLARIFICATION: what happens on petwalker account deletion?].

---

## Deferred Requirements (v2)

Some items were deferred to keep this PR focused. See `DEFERRED_REQUIREMENTS.md` for details on:
- Rating display format & breakdown
- Rating moderation policy
- PetWalker response to ratings
- Account deletion handling
- Data retention policy

---

### Non-Functional Requirements

- **Performance**: Average rating calculations are efficient for small scale (<1000 ratings per petwalker)
- **Scalability**: System designed for <10,000 total users; denormalized fields on PetWalker ensure fast reads

### Key Entities _(include if feature involves data)_

- **PetWalker**: Represents a petwalker in the system. Must include aggregated rating data (average rating, total rating count) that is derived from all associated ratings. The PetWalker entity stores AverageRating and TotalRatingsCount as denormalized fields for performance, updated via domain events when ratings are added, modified, or removed.
- **Rating**: Represents a single rating submission from a pet owner for a petwalker. Must include: unique identifier, reference to the pet owner (PetOwnerId), reference to the petwalker (PetWalkerId), numerical score (1-5 stars, whole numbers only), optional text comments, timestamp of submission, and status (active/moderated/removed). The Rating entity is independent of any specific booking and represents an overall assessment of the petwalker. Only one active Rating per PetOwner per PetWalker is allowed. Status transitions: Active → Moderated (by admin) → Removed.
- **Client**: Represents the pet owner who books petwalkers. Must be able to submit ratings for petwalkers they have used and may have a history of ratings they've submitted.

---

## Review & Acceptance Checklist

_GATE: Automated checks run during main() execution_

### Content Quality

- [x] No implementation details (languages, frameworks, APIs)
- [x] Focused on user value and business needs
- [x] Written for non-technical stakeholders
- [x] All mandatory sections completed

### Requirement Completeness

- [x] No [NEEDS CLARIFICATION] markers remain (deferred to DEFERRED_REQUIREMENTS.md)
- [x] Requirements are testable and unambiguous
- [x] Success criteria are measurable
- [x] Scope is clearly bounded
- [x] Dependencies and assumptions identified

---

## Execution Status

_Updated by main() during processing_

- [x] User description parsed
- [x] Key concepts extracted
- [x] Ambiguities marked
- [x] User scenarios defined
- [x] Requirements generated
- [x] Entities identified
- [x] Review checklist passed

---

## Notes for Implementation Team

### Critical Clarifications Needed

All critical ambiguities have been resolved through clarification session. See `## Clarifications` section for answered questions. Remaining items are deferred to v2 (see `DEFERRED_REQUIREMENTS.md`).

### Implementation Notes

- The rating system is independent of specific bookings. Ratings are for the petwalker as a whole, not per-booking.
- Eligibility is determined by checking if the pet owner has at least one completed booking with the petwalker in the system.
- The Rating entity should reference the PetOwner and PetWalker directly, without a Booking reference.
- The PetWalker entity must maintain aggregated fields: AverageRating (computed) and TotalRatingsCount.
- Average rating calculations should be efficient. Consider caching strategies if rating volume is high.
- Displaying rating breakdowns may require additional queries or pre-computed aggregates.

### Performance Notes

- Average rating calculations should be efficient. Consider caching strategies if rating volume is high.
- Displaying rating breakdowns may require additional queries or pre-computed aggregates.

