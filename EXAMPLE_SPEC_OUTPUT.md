# Example: What /spec Command Generates

## Example 1: Booking Cancellation Feature

**Command**:
```bash
speckai spec "Add booking cancellation and rescheduling functionality"
```

**Generated Output** (`specs/001-booking-cancellation/spec.md`):

```markdown
# Feature Specification: Booking Cancellation and Rescheduling

**Feature Branch**: `001-booking-cancellation`
**Created**: 2025-10-04
**Status**: Draft
**Input**: "Add booking cancellation and rescheduling functionality"

## User Scenarios & Testing

### Primary User Story
As a client, I want to cancel or reschedule my pet walking booking so that 
I can adjust my plans without losing my entire payment.

### Acceptance Scenarios
1. **Given** a confirmed booking 48 hours in the future, **When** client 
   cancels, **Then** full refund is issued and pet walker is notified
2. **Given** a confirmed booking 12 hours in the future, **When** client 
   cancels, **Then** 50% cancellation fee is charged and pet walker is notified
3. **Given** a confirmed booking, **When** client requests reschedule and 
   pet walker has availability, **Then** booking is moved to new time slot
4. **Given** a confirmed booking, **When** client requests reschedule but 
   pet walker has no availability, **Then** error message is shown with 
   alternative dates

### Edge Cases
- What happens when client cancels after booking has started?
- How does system handle rescheduling to a date when pet walker is unavailable?
- What if pet walker cancels the booking?
- How are partial refunds calculated for multi-day bookings?

## Requirements

### Functional Requirements
- **FR-001**: System MUST allow clients to cancel confirmed bookings
- **FR-002**: System MUST calculate cancellation fees based on time remaining 
  before booking start time
- **FR-003**: System MUST issue refunds according to cancellation policy 
  [NEEDS CLARIFICATION: refund method - original payment method, account credit, or both?]
- **FR-004**: System MUST notify pet walker when booking is cancelled
- **FR-005**: System MUST allow clients to request rescheduling
- **FR-006**: System MUST check pet walker availability for rescheduled dates
- **FR-007**: System MUST update booking status (Confirmed → Cancelled or Rescheduled)
- **FR-008**: System MUST log all cancellation and rescheduling events for audit
- **FR-009**: System MUST prevent cancellation of bookings that have already started
- **FR-010**: System MUST [NEEDS CLARIFICATION: who can initiate cancellation - 
  client only, or both client and pet walker?]

### Key Entities
- **Booking**: Existing entity, needs CancellationStatus, CancellationFee, 
  CancellationReason, RescheduledFromBookingId
- **CancellationPolicy**: [NEEDS CLARIFICATION: stored as configuration or 
  database entity?] - defines time thresholds and fee percentages
- **RefundTransaction**: [NEEDS CLARIFICATION: new entity or part of existing 
  Payment system?]

## Review & Acceptance Checklist

### Content Quality
- [x] No implementation details (languages, frameworks, APIs)
- [x] Focused on user value and business needs
- [x] Written for non-technical stakeholders
- [x] All mandatory sections completed

### Requirement Completeness
- [ ] No [NEEDS CLARIFICATION] markers remain ← **ACTION REQUIRED**
- [x] Requirements are testable and unambiguous
- [x] Success criteria are measurable
- [x] Scope is clearly bounded
- [ ] Dependencies and assumptions identified ← **ACTION REQUIRED**

## Execution Status
- [x] User description parsed
- [x] Key concepts extracted
- [x] Ambiguities marked
- [x] User scenarios defined
- [x] Requirements generated
- [x] Entities identified
- [ ] Review checklist passed ← **Blocked by NEEDS CLARIFICATION**
```

---

## Example 2: Review & Rating System

**Command**:
```bash
speckai spec "Add review and rating system for pet walkers"
```

**Generated Output** (`specs/002-review-rating/spec.md`):

```markdown
# Feature Specification: Review and Rating System

**Feature Branch**: `002-review-rating`
**Created**: 2025-10-04
**Status**: Draft

## User Scenarios & Testing

### Primary User Story
As a client, I want to rate and review pet walkers after completed bookings 
so that other clients can make informed decisions.

### Acceptance Scenarios
1. **Given** a completed booking, **When** client submits 5-star rating with 
   comment, **Then** review is saved and displayed on pet walker profile
2. **Given** a review on their profile, **When** pet walker views it, **Then** 
   they can respond with a comment
3. **Given** multiple reviews, **When** viewing pet walker profile, **Then** 
   average rating is displayed with review count
4. **Given** an incomplete booking, **When** client attempts to leave review, 
   **Then** error message indicates only completed bookings can be reviewed

### Edge Cases
- Can clients edit or delete reviews after submission?
- Can pet walkers flag inappropriate reviews?
- How are reviews handled if booking is cancelled?
- What happens to reviews if pet walker account is deleted?

## Requirements

### Functional Requirements
- **FR-001**: System MUST allow clients to rate pet walkers on 1-5 star scale
- **FR-002**: System MUST allow clients to add optional text comments to reviews
- **FR-003**: System MUST only allow reviews for completed bookings
- **FR-004**: System MUST prevent duplicate reviews for same booking
- **FR-005**: System MUST calculate and display average rating for each pet walker
- **FR-006**: System MUST display review count on pet walker profiles
- **FR-007**: System MUST allow pet walkers to respond to reviews
- **FR-008**: System MUST display reviews in chronological order (newest first)
- **FR-009**: System MUST [NEEDS CLARIFICATION: can clients edit/delete reviews 
  after submission?]
- **FR-010**: System MUST [NEEDS CLARIFICATION: moderation policy for 
  inappropriate reviews?]
- **FR-011**: System MUST [NEEDS CLARIFICATION: time limit for leaving reviews 
  after booking completion?]

### Key Entities
- **Review**: New entity with Rating (1-5), Comment, ReviewDate, ClientId, 
  PetWalkerId, BookingId
- **ReviewResponse**: New entity with ResponseText, ResponseDate, PetWalkerId, 
  ReviewId
- **Booking**: Existing entity, needs HasReview flag to prevent duplicates

## Review & Acceptance Checklist
- [ ] No [NEEDS CLARIFICATION] markers remain ← **ACTION REQUIRED**
- [x] Requirements are testable
- [x] User scenarios defined
```

---

## What You Need to Do Next

### Step 1: Review the Generated Spec

```bash
# Open the generated spec
code specs/001-booking-cancellation/spec.md
```

### Step 2: Resolve [NEEDS CLARIFICATION] Markers

Look for lines like:
```markdown
[NEEDS CLARIFICATION: refund method - original payment method, account credit, or both?]
```

Replace with your decision:
```markdown
Refunds will be issued to original payment method within 5-7 business days
```

### Step 3: Add Missing Details

Based on your BRD, add:
- Cancellation policy specifics (24 hours = full refund, etc.)
- Who can cancel (client only or both parties)
- Notification requirements
- Any business rules from your domain

### Step 4: Remove [NEEDS CLARIFICATION] Markers

Once resolved, delete the marker text so the spec is clean.

### Step 5: Mark Spec as Ready

Update the status:
```markdown
**Status**: Ready for Planning
```

---

## 🎯 Recommended First Feature from Your BRD

Based on your BRD and current implementation, I recommend starting with:

### **Option A: Booking Cancellation** (Easiest)
- ✅ Small, well-defined scope
- ✅ Builds on existing Booking aggregate
- ✅ Clear business rules
- ✅ No new aggregates needed

**Command**:
```bash
speckai spec "Add booking cancellation functionality. Clients can cancel bookings with cancellation fees: 24+ hours before = full refund, less than 24 hours = 50% fee. Pet walker receives notification. Cancelled bookings cannot be reactivated. Cancellation reason is optional."
```

### **Option B: Review & Rating System** (Medium)
- ✅ Well-defined in BRD (sections 2.1, 2.2, 3.6)
- ✅ New aggregate (good learning experience)
- ✅ Clear user value
- ⚠️ Requires new entities

**Command**:
```bash
speckai spec "Add review and rating system. Clients rate pet walkers 1-5 stars after completed bookings with optional comments. Pet walkers can respond to reviews. Average rating displayed on profiles. Only completed bookings can be reviewed, one review per booking."
```

---

## 📝 Quick Start Guide

```bash
# 1. Navigate to your project
cd c:\Users\rbrav\source\repos\FurryFriends

# 2. Run spec command (choose one feature from above)
speckai spec "your feature description here"

# 3. Review the generated spec
code specs/001-feature-name/spec.md

# 4. Fill in [NEEDS CLARIFICATION] markers

# 5. Update status to "Ready for Planning"

# 6. Run plan command
speckai plan specs/001-feature-name/spec.md

# 7. Review the plan (Constitution Check runs automatically!)
code specs/001-feature-name/plan.md

# 8. Generate tasks
speckai tasks specs/001-feature-name/plan.md

# 9. Execute tasks
code specs/001-feature-name/tasks.md
```

---

## 💡 Pro Tips

1. **Start Small**: Pick one feature from BRD, not multiple
2. **Be Specific**: More detail in spec command = better output
3. **Use BRD Language**: Copy exact requirements from your BRD
4. **Resolve Clarifications**: Don't skip [NEEDS CLARIFICATION] markers
5. **Constitution Check**: The /plan command will validate against your v2.0.0 rules

---

## 🎯 My Recommendation

**Try this right now**:

```bash
cd c:\Users\rbrav\source\repos\FurryFriends

speckai spec "Add booking cancellation functionality for FurryFriends. Clients can cancel confirmed bookings through the BookingManagement page. Cancellation policy: bookings cancelled 24+ hours before start time receive full refund, bookings cancelled less than 24 hours before start time incur 50% cancellation fee. Pet walker receives email notification when booking is cancelled. Booking status changes from Confirmed to Cancelled. Clients can optionally provide cancellation reason. Cancelled bookings cannot be reactivated."
```

This will create your first spec file, and you can see exactly how Spec-Kit works with your solution!

**Want me to help you run this command and review the output?** 🚀

