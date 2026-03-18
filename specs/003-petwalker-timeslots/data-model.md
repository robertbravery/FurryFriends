# Data Model: Petwalker Timeslots

**Date**: 2026-03-14  
**Feature**: 003-petwalker-timeslots

---

## Entities

### 1. Timeslot

Represents a period during which a Petwalker is available for booking.

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| Id | Guid | PK, Required | Unique identifier |
| PetWalkerId | Guid | FK, Required | Reference to PetWalker |
| Date | DateOnly | Required | Date of the timeslot |
| StartTime | TimeOnly | Required | Start time (e.g., 09:00) |
| EndTime | TimeOnly | Required | End time (e.g., 09:45) |
| DurationInMinutes | int | Required, 30-45 | Slot duration |
| Status | TimeslotStatus | Required | Available/Booked/Unavailable |
| CreatedAt | DateTime | Required | Creation timestamp |
| UpdatedAt | DateTime | Nullable | Last update timestamp |

**Status Enum**:
```csharp
public enum TimeslotStatus
{
    Available = 0,     // Can be booked by clients
    Booked = 1,       // Has an active booking
    Unavailable = 2,   // Blocked (e.g., travel buffer, petwalker unavailable)
    Cancelled = 3     // Was cancelled
}
```

**Relationships**:
- Many-to-One with PetWalker (existing aggregate)
- One-to-Many with Booking (via TimeslotId)

**Validation Rules**:
- DurationInMinutes must be between 30 and 45 (FR-001)
- StartTime + DurationInMinutes must equal EndTime
- Date must be today or future (no past slots)
- StartTime must be within PetWalker's WorkingHours (FR-008)

---

### 2. TravelBuffer

Represents the time required for a Petwalker to travel between client addresses.

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| Id | Guid | PK, Required | Unique identifier |
| BookingId | Guid | FK, Required | The booking this buffer follows |
| PreviousBookingId | Guid | FK, Nullable | The booking before this one |
| OriginAddress | string | Required, max 500 | Starting address |
| DestinationAddress | string | Required, max 500 | Ending address |
| BufferDurationMinutes | int | Required, > 0 | Time needed to travel |
| StartTime | DateTime | Required | When buffer starts |
| EndTime | DateTime | Required | When buffer ends |
| CreatedAt | DateTime | Required | Creation timestamp |

**Relationships**:
- Many-to-One with Booking (the booking this buffer is for)
- Many-to-One with Booking (PreviousBooking - nullable)

**Validation Rules**:
- BufferDurationMinutes must be > 0
- EndTime must be > StartTime
- If PreviousBooking exists, OriginAddress should match PreviousBooking.ClientAddress

---

### 3. WorkingHours

Represents the Petwalker's defined schedule for each day of the week.

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| Id | Guid | PK, Required | Unique identifier |
| PetWalkerId | Guid | FK, Required | Reference to PetWalker |
| DayOfWeek | DayOfWeek | Required | Day (0=Sunday, 6=Saturday) |
| StartTime | TimeOnly | Required | Daily start time (e.g., 08:00) |
| EndTime | TimeOnly | Required | Daily end time (e.g., 18:00) |
| IsActive | bool | Required, default true | Whether schedule is active |

**Relationships**:
- Many-to-One with PetWalker

**Validation Rules**:
- EndTime must be > StartTime
- Maximum 2 WorkingHours per day (e.g., morning shift + evening shift)
- Cannot overlap with existing WorkingHours on same day

---

### 4. Booking (Existing - Extended)

The existing Booking entity will be extended with timeslot reference.

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| TimeslotId | Guid | FK, Nullable | Reference to booked timeslot |
| ClientAddress | string | Required, max 500 | Service location |

**New Validation**:
- If TimeslotId is set, Timeslot.Status must be Available
- ClientAddress is required for calculating travel buffers

---

### 5. CustomTimeRequest

Represents a client's request for a custom time when preset timeslots are unavailable.

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| Id | Guid | PK, Required | Unique identifier |
| ClientId | Guid | FK, Required | Reference to Client |
| PetWalkerId | Guid | FK, Required | Reference to PetWalker |
| RequestedDate | DateOnly | Required | Date the client wants |
| PreferredStartTime | TimeOnly | Required | Preferred start time |
| PreferredEndTime | TimeOnly | Required | Preferred end time (calculated from duration) |
| PreferredDurationMinutes | int | Required, 30-45 | Requested duration |
| Status | CustomTimeRequestStatus | Required | Current status |
| ClientAddress | string | Required, max 500 | Service location for this booking |
| CounterOfferedTime | TimeOnly | Nullable | Petwalker's counter-offer time |
| CounterOfferedDate | DateOnly | Nullable | Petwalker's counter-offer date |
| ResponseReason | string | Nullable, max 500 | Reason for decline/counter-offer |
| CreatedAt | DateTime | Required | Creation timestamp |
| UpdatedAt | DateTime | Nullable | Last update timestamp |

**Status Enum**:
```csharp
public enum CustomTimeRequestStatus
{
    Pending = 0,         // Awaiting petwalker response
    Accepted = 1,       // Request accepted, booking created
    Declined = 2,       // Request declined by petwalker
    CounterOffered = 3,  // Petwalker proposed alternative time
    Expired = 4         // Request timed out (no response within 48 hours)
}
```

**Relationships**:
- Many-to-One with Client
- Many-to-One with PetWalker
- One-to-One with Booking (when accepted)

**Validation Rules**:
- RequestedDate must be today or future (no past dates)
- PreferredDurationMinutes must be between 30 and 45
- PreferredStartTime + PreferredDurationMinutes must equal PreferredEndTime
- Status can only change in order: Pending → Accepted/Declined/CounterOffered

---

## Entity Relationships Diagram

```
┌─────────────┐       ┌─────────────┐       ┌─────────────┐
│  PetWalker  │       │  Timeslot   │       │   Booking   │
│  (existing) │       │  (new)      │       │  (extended) │
└──────┬──────┘       └──────┬──────┘       └──────┬──────┘
       │                     │                     │
       │ 1:N                 │ N:1                 │ N:1
       │                     │                     │
       ▼                     ▼                     ▼
┌─────────────┐       ┌─────────────┐       ┌─────────────┐
│ WorkingHours│       │   Status    │       │TravelBuffer│
│   (new)     │       │  Enum       │       │   (new)    │
└─────────────┘       └─────────────┘       └─────────────┘
                                                      │
                                                      │
                                             ┌────────┴────────┐
                                             │CustomTimeRequest│
                                             │    (new)        │
                                             └─────────────────┘
```

---

## Database Constraints

### Timeslot Table
- Unique index: (PetWalkerId, Date, StartTime) - prevents exact duplicate slots
- Filtered unique index: WHERE Status = Available (PetWalkerId, Date, StartTime, EndTime) - prevents overlapping available slots
- FK: PetWalkerId references PetWalkers(Id)

### TravelBuffer Table
- FK: BookingId references Bookings(Id)
- FK: PreviousBookingId references Bookings(Id)

### WorkingHours Table
- Unique index: (PetWalkerId, DayOfWeek, StartTime)
- FK: PetWalkerId references PetWalkers(Id)

### CustomTimeRequest Table
- FK: ClientId references Clients(Id)
- FK: PetWalkerId references PetWalkers(Id)
- Index: (PetWalkerId, Status) for querying pending requests
- Index: (ClientId, CreatedAt) for client's request history
- Filtered index: WHERE Status = Pending (PetWalkerId, RequestedDate) - for petwalker's pending requests by date

---

## Specifications (Ardalis.Specification)

### TimeslotSpecifications.cs
```csharp
// Get available timeslots for a petwalker on a specific date
public class AvailableTimeslotsByPetWalkerAndDateSpecification : Specification<Timeslot>
{
    public AvailableTimeslotsByPetWalkerAndDateSpecification(Guid petWalkerId, DateOnly date)
    {
        Query.Where(t => t.PetWalkerId == petWalkerId)
             .Where(t => t.Date == date)
             .Where(t => t.Status == TimeslotStatus.Available)
             .OrderBy(t => t.StartTime);
    }
}

// Check for overlapping timeslots
public class OverlappingTimeslotsSpecification : Specification<Timeslot>
{
    public OverlappingTimeslotsSpecification(
        Guid petWalkerId, 
        DateOnly date, 
        TimeOnly startTime, 
        TimeOnly endTime)
    {
        Query.Where(t => t.PetWalkerId == petWalkerId)
             .Where(t => t.Date == date)
             .Where(t => t.StartTime < endTime && t.EndTime > startTime);
    }
}

// Get timeslots within working hours
public class TimeslotsWithinWorkingHoursSpecification : Specification<Timeslot>
{
    public TimeslotsWithinWorkingHoursSpecification(
        Guid petWalkerId,
        DateOnly date,
        TimeOnly workingStart,
        TimeOnly workingEnd)
    {
        Query.Where(t => t.PetWalkerId == petWalkerId)
             .Where(t => t.Date == date)
             .Where(t => t.StartTime >= workingStart)
             .Where(t => t.EndTime <= workingEnd);
    }
}
```

### WorkingHoursSpecifications.cs
```csharp
public class WorkingHoursByPetWalkerAndDaySpecification : Specification<WorkingHours>
{
    public WorkingHoursByPetWalkerAndDaySpecification(Guid petWalkerId, DayOfWeek day)
    {
        Query.Where(w => w.PetWalkerId == petWalkerId)
             .Where(w => w.DayOfWeek == day)
             .Where(w => w.IsActive);
    }
}
```

### CustomTimeRequestSpecifications.cs
```csharp
// Get pending custom time requests for a petwalker
public class PendingCustomTimeRequestsByPetWalkerSpecification : Specification<CustomTimeRequest>
{
    public PendingCustomTimeRequestsByPetWalkerSpecification(Guid petWalkerId)
    {
        Query.Where(r => r.PetWalkerId == petWalkerId)
             .Where(r => r.Status == CustomTimeRequestStatus.Pending)
             .OrderBy(r => r.RequestedDate)
             .ThenBy(r => r.PreferredStartTime);
    }
}

// Get custom time requests by client
public class CustomTimeRequestsByClientSpecification : Specification<CustomTimeRequest>
{
    public CustomTimeRequestsByClientSpecification(Guid clientId)
    {
        Query.Where(r => r.ClientId == clientId)
             .OrderByDescending(r => r.CreatedAt);
    }
}
```

---

## State Transitions

### Timeslot Status Flow
```
Available ──[Client books]──▶ Booked
    │                            │
    │                            │
    │──[Petwalker blocks]──▶ Unavailable
    │                            │
    │                            │
    └──[Petwalker cancels]──▶ Cancelled
```

### Booking Status (Existing)
No changes to existing Booking status flow - timeslot status updates are a side effect.

### CustomTimeRequest Status Flow
```
Pending ──[Petwalker accepts]──▶ Accepted
    │                              │
    │                              │
    ├──[Petwalker declines]──▶ Declined
    │
    │
    ├──[Petwalker counter-offers]──▶ CounterOffered
    │
    │
    └──[48 hours pass]──▶ Expired
```
