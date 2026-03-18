# Feature Specification: Petwalker Timeslots

**Feature Branch**: `003-petwalker-timeslots`  
**Created**: 2026-03-14  
**Status**: Draft  
**Input**: User description: "Create Timeslots for Petwalkers. So that when a Client chooses a date and time, time slots are availble. Petwalkers should create 30 - 45 min timeslots. Timeslots should not override each other. Petwalker needs time to move from one client address to another."

---

## User Scenarios & Testing *(mandatory)*

### Primary User Story
As a Petwalker, I want to define my available time slots so that Clients can book appointments during times when I am available. As a Client, I want to see available time slots for a specific date so that I can choose a convenient time for my pet walking service.

### Petwalker Workflow
1. Define WorkingHours (weekly schedule - e.g., Mon-Fri 9am-5pm)
2. Create Timeslots within working hours (e.g., Mon 9:00, 9:30, 10:00...)
3. View/manage their created timeslots
4. Receive notifications for custom time requests
5. Accept/Decline/Counter-offer custom requests

### Client Workflow
1. View available timeslots for a petwalker on a specific date
2. Select an available timeslot → Book immediately
3. OR Request custom time → Petwalker approval required

### User Scenarios

#### Scenario 1: Petwalker defines weekly working hours
**Given** a Petwalker wants to set their weekly availability, **When** they define their working hours for each day of the week, **Then** the system stores the weekly schedule (day, start time, end time) that will constrain which timeslots can be created.

#### Scenario 2: Petwalker creates timeslots within their working hours
**Given** a Petwalker has defined their working hours, **When** they create timeslots on specific dates within those working hours, **Then** the system validates that timeslot times fall within the defined working hours and creates the timeslots.

#### Scenario 3: Client books an available timeslot
**Given** a Client views available timeslots for a Petwalker on a specific date, **When** they select an available timeslot and confirm the booking, **Then** the system creates a booking and marks the timeslot as unavailable.

#### Scenario 4: Client requests custom time when exact slot unavailable
**Given** a Client cannot find a suitable available timeslot, **When** they submit a custom time request with their preferred date, time, and duration, **Then** the system creates a pending CustomTimeRequest that the Petwalker can review and respond to.

### Acceptance Scenarios
1. **Given** a Petwalker has defined their working hours for a week, **When** they create time slots within those hours, **Then** the system stores the time slots with start time, end time, and duration (30-45 minutes).

2. **Given** a Petwalker has already created time slots for a specific date, **When** they attempt to create overlapping time slots, **Then** the system rejects the overlapping time slot and displays an error message.

3. **Given** a Client is viewing available time slots for a specific Petwalker on a specific date, **When** the Petwalker has created available time slots, **Then** the system displays only the unbooked time slots.

4. **Given** a Petwalker has scheduled appointments at different client addresses, **When** calculating available time slots, **Then** the system accounts for travel time between appointments.

5. **Given** a Client selects a time slot and confirms a booking, **When** the booking is created, **Then** that time slot becomes unavailable for other Clients.

6. **Given** a Petwalker has defined their working hours, **When** they create a timeslot outside those hours, **Then** the system rejects the timeslot and displays an error message indicating it falls outside working hours.

7. **Given** a Client requests a custom time with preferred date, time, and duration, **When** the request is submitted, **Then** the system creates a CustomTimeRequest with status Pending.

8. **Given** a Petwalker receives a CustomTimeRequest, **When** they accept the request, **Then** the system creates a confirmed booking and updates the request status to Accepted.

9. **Given** a Petwalker receives a CustomTimeRequest, **When** they decline the request, **Then** the system updates the request status to Declined and notifies the Client.

### Edge Cases
- What happens when a Petwalker tries to create a time slot that spans beyond their defined working hours?
- What happens when a Petwalker modifies or deletes an existing time slot that has a pending booking?
- How does the system handle time zone differences for Clients and Petwalkers in different locations?
- What happens when a Petwalker creates back-to-back time slots at different addresses without adequate travel buffer?
- What is the minimum advance notice time for Clients to book a time slot?

---

## Requirements *(mandatory)*

### Functional Requirements
- **FR-001**: System MUST allow Petwalkers to create time slots with a duration between 30 and 45 minutes.
- **FR-002**: System MUST prevent Petwalkers from creating overlapping time slots on the same date.
- **FR-003**: System MUST display available time slots to Clients when they select a specific date and Petwalker.
- **FR-004**: System MUST mark time slots as unavailable when a Client books them.
- **FR-005**: System MUST require a travel time buffer between appointments at different client addresses.
- **FR-006**: System MUST allow Petwalkers to view their created time slots organized by date.
- **FR-007**: System MUST allow Petwalkers to edit or delete their available time slots.
- **FR-008**: System MUST validate that time slot start times occur within the Petwalker's defined working hours.
- **FR-009**: System MUST calculate available time slots considering existing bookings and travel buffers.
- **FR-010**: System MUST display clear error messages when time slot creation fails due to conflicts.
- **FR-011**: System MUST allow Petwalkers to define their weekly working hours.
- **FR-012**: System MUST prevent creation of timeslots that fall outside the Petwalker's working hours.
- **FR-013**: System MUST allow Clients to submit custom time requests when no suitable slot is available.
- **FR-014**: System MUST allow Petwalkers to Accept, Decline, or Counter-offer custom time requests.
- **FR-015**: System MUST notify Clients when their custom time request status changes.
- **FR-016**: System MUST create a confirmed booking when a custom time request is accepted.

---

## Key Entities *(include if feature involves data)*

- **Timeslot**: Represents a period during which a Petwalker is available for booking. Key attributes include: start datetime, end datetime, duration (30-45 minutes), status (available/booked), associated Petwalker identifier.
- **TravelBuffer**: Represents the time required for a Petwalker to move between client addresses. Key attributes include: buffer duration, origin address, destination address.
- **Booking**: Represents a confirmed appointment. Key attributes include: booked timeslot reference, client identifier, Petwalker identifier, client address, status.
- **WorkingHours**: Represents the Petwalker's defined schedule. Key attributes include: day of week, start time, end time.
- **CustomTimeRequest**: Represents a client's request for a custom time when preset slots are unavailable. Key attributes include: client identifier, petwalker identifier, requested date, preferred start time, preferred duration, status (Pending/Accepted/Declined/CounterOffered).

---

## Review & Acceptance Checklist

### Content Quality
- [x] No implementation details (languages, frameworks, APIs)
- [x] Focused on user value and business needs
- [x] Written for non-technical stakeholders
- [x] All mandatory sections completed

### Requirement Completeness
- [x] No [NEEDS CLARIFICATION] markers remain
- [x] Requirements are testable and unambiguous
- [x] Success criteria are measurable
- [x] Scope is clearly bounded
- [x] Dependencies and assumptions identified

---

## Execution Status
*Updated by main() during processing*

- [x] User description parsed
- [x] Key concepts extracted
- [x] Ambiguities marked
- [x] User scenarios defined
- [x] Requirements generated
- [x] Entities identified
- [x] Review checklist passed

---

## Implementation Decisions

### Technology Stack
- **Framework**: .NET 9
- **API Framework**: FastEndpoints
- **Validation**: FluentValidation
- **Logging**: Serilog
- **Result Handling**: Ardalis.Result
- **Database**: SQLite (for development)

### Architecture
- Clean Architecture with separated layers:
  - Core: Domain entities and business logic
  - UseCases: Application-specific use cases with CQRS pattern using MediatR
  - Infrastructure: Database and external service implementations
  - Web: API endpoints and Blazor UI

### Key Implementation Details

1. **Timeslot Duration**: 30-45 minutes (validated in domain)
2. **Travel Buffer**: 15 minutes between bookings at different addresses
3. **Working Hours**: Per-day configuration with start/end times
4. **Custom Time Requests**: Support for Accept, Decline, and Counter-offer
5. **Error Handling**: Global exception middleware with structured error responses
6. **Conflict Detection**: 409 Conflict status for booking conflicts

### API Design
- RESTful endpoints using FastEndpoints
- Endpoint prefix: `/api`
- Anonymous access allowed for development
- Structured logging with Serilog
- Input validation via FluentValidation validators

### UI Components (Blazor)
- PetwalkerAvailability: Manage working hours and timeslots
- ClientBooking: View and book available timeslots

---
