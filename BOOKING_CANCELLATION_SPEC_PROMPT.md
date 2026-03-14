# Spec-Kit Prompt: Booking Cancellation Feature

## 📋 Use This Prompt with Spec-Kit

Run this command in your terminal:
```bash
speckai spec "Add booking cancellation functionality that allows clients and pet walkers to cancel existing bookings with appropriate status transitions, validation rules, and audit logging. The system should prevent cancellation of completed or already-cancelled bookings, maintain booking history for audit purposes, and provide clear feedback to users about cancellation success or failure. Cancellations should be logged for business analytics and potential future refund processing."
```

---

## 📝 Detailed Specification Prompt

**Feature Name**: Booking Cancellation

**Feature Description**:
Add comprehensive booking cancellation functionality to the FurryFriends platform that enables both clients and pet walkers to cancel existing bookings. The system must enforce business rules around when bookings can be cancelled, maintain an audit trail of all cancellations, and provide appropriate user feedback.

**Key Requirements to Address**:

1. **Cancellation Eligibility**
   - Determine which booking statuses allow cancellation (e.g., Pending, Confirmed but not Started)
   - Prevent cancellation of completed bookings
   - Prevent cancellation of already-cancelled bookings
   - Define any time-based restrictions (e.g., must cancel X hours before booking start)

2. **User Permissions**
   - Who can cancel a booking? (Client only, Pet Walker only, or both?)
   - Can an admin/system cancel bookings?
   - Should there be different cancellation workflows for different user types?

3. **Cancellation Workflow**
   - What happens when a booking is cancelled? (Status change, notifications, etc.)
   - Should cancellation reasons be captured?
   - Should there be a confirmation step before cancellation?

4. **Data & Audit Trail**
   - Track who cancelled the booking and when
   - Store cancellation reason (if applicable)
   - Maintain booking history for analytics
   - Should cancelled bookings be soft-deleted or hard-deleted?

5. **Business Rules & Constraints**
   - Are there any penalties or restrictions for frequent cancellations?
   - Should cancellation be allowed within X hours of booking start?
   - Any refund-related considerations for future payment integration?

6. **User Experience**
   - How should users initiate cancellation? (Button, menu option, etc.)
   - What confirmation/warning messages should be shown?
   - Should users receive confirmation of successful cancellation?

7. **Error Handling**
   - What error messages for invalid cancellation attempts?
   - How to handle race conditions (simultaneous cancellation attempts)?
   - What if booking is deleted while cancellation is in progress?

---

## 🎯 Expected Output

When you run the `/spec` command with the above prompt, Spec-Kit will generate:

1. **spec.md** - Complete feature specification with:
   - User scenarios and acceptance criteria
   - Functional requirements (FR-001, FR-002, etc.)
   - Key entities (Booking, CancellationRecord, etc.)
   - Edge cases and error scenarios
   - Review checklist

2. **Clarification Points** - Marked with [NEEDS CLARIFICATION] for:
   - Specific business rules you need to define
   - Permission models
   - Time-based constraints
   - Refund/payment integration considerations

---

## 💡 Tips for Best Results

1. **Be Specific About Your Business Rules**
   - If you have specific cancellation windows (e.g., "must cancel 24 hours before"), include them
   - If certain user types have different permissions, clarify that

2. **Consider Your Current System**
   - You already have Booking, Client, and PetWalker entities
   - Think about how cancellation fits into your existing booking lifecycle

3. **Think About Future Features**
   - Cancellation will eventually tie into payment/refunds
   - Consider how cancellation reasons might be used for analytics

4. **Review the Generated Spec**
   - Look for [NEEDS CLARIFICATION] markers
   - Clarify any ambiguous requirements before moving to planning phase

---

## 📚 Reference: Your Current Booking System

**Existing Booking Endpoints**:
- POST /Bookings - Create new booking
- GET /bookings/client/{id} - Get client bookings
- GET /bookings/petwalker/{id} - Get walker bookings
- GET /petwalker/{id}/available-slots - Check availability

**Current Booking Statuses** (inferred from codebase):
- Pending
- Confirmed
- Completed
- [Cancelled - to be added]

**Existing Entities**:
- Booking (with Id, StartDate, EndDate, Status, Price)
- Client (with Id, Email, Name)
- PetWalker (with Id, Email, Name)

---

## ✅ Next Steps

1. Run the spec command with the prompt above
2. Review the generated `specs/###-booking-cancellation/spec.md`
3. Address any [NEEDS CLARIFICATION] markers
4. Once spec is approved, run `/plan` command to generate the plan
5. Then run `/tasks` command to generate implementation tasks

