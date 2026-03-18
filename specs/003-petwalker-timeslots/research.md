# Research: Petwalker Timeslots Feature

**Date**: 2026-03-14  
**Feature**: 003-petwalker-timeslots  
**Status**: Complete

---

## Research Topics

### 1. Timeslot/Scheduling Systems Best Practices

**Decision**: Use discrete time slot model with explicit duration tracking

**Rationale**: 
- Discrete slots (30-45 min) are simpler to manage than continuous availability
- Explicit duration allows flexible slot sizing per service type
- Easier to display in calendar UIs and prevents partial slot confusion

**Alternatives Considered**:
- **Continuous availability**: Too complex for this use case; requires complex blocking algorithms
- **Fixed 60-minute slots**: Less flexible for varying service durations

**Implementation Approach**:
- Store explicit StartTime and EndTime for each Timeslot
- Use DurationInMinutes property (30-45 range) for validation
- Calculate EndTime automatically from StartTime + Duration
- Query slots by Date for client availability display

---

### 2. Travel Time Buffer Handling

**Decision**: Store travel buffers as separate entities linked to bookings, calculated dynamically

**Rationale**:
- Travel time depends on actual addresses from bookings
- Pre-calculating all possible buffers is computationally expensive
- Dynamic calculation allows for real-time address changes
- Buffers only needed when bookings exist at different addresses

**Alternatives Considered**:
- **Static travel time matrix**: Too rigid; doesn't account for traffic or address changes
- **Pre-calculated buffer per location pair**: Storage-intensive; many unused combinations

**Implementation Approach**:
- Store TravelBuffer entity with: BufferDurationMinutes, OriginAddress, DestinationAddress
- Calculate buffer on-the-fly when checking slot availability
- When a new booking is created at a different address than previous booking:
  - Query previous booking's destination address
  - Calculate travel time between addresses
  - Insert TravelBuffer record
  - Mark subsequent timeslots as unavailable during buffer period
- Use Haversine formula for distance calculation between coordinates

**Buffer Duration Defaults**:
- Same suburb: 15 minutes
- Different suburb (same city): 25 minutes
- Different city: 45 minutes (or next day)

---

### 3. Preventing Overlapping Time Slots

**Decision**: Database-level constraint + application-level validation

**Rationale**:
- Database constraints provide ultimate protection against race conditions
- Application validation provides better error messages for users
- Dual approach ensures data integrity

**Implementation Approach**:

**Database Level**:
- Use filtered unique index to prevent overlapping slots for same petwalker on same date
- Partial index: `WHERE Status = 'Available'`
- Check: No slot where (New.StartTime < Existing.EndTime AND New.EndTime > Existing.StartTime)

**Application Level (CQRS Handler)**:
```csharp
// In CreateTimeslotHandler
var overlappingSpec = new OverlappingTimeslotsSpecification(
    request.PetWalkerId,
    request.Date,
    request.StartTime,
    request.EndTime);

var existingSlots = await _repository.ListAsync(overlappingSpec, ct);
if (existingSlots.Any())
{
    return Result.Error("Time slot overlaps with an existing timeslot");
}
```

**Specification Pattern**:
```csharp
public class OverlappingTimeslotsSpecification : Specification<Timeslot>
{
    public OverlappingTimeslotsSpecification(
        Guid petWalkerId,
        DateTime date,
        DateTime startTime,
        DateTime endTime)
    {
        Query.Where(t => t.PetWalkerId == petWalkerId)
             .Where(t => t.Date.Date == date.Date)
             .Where(t => t.StartTime < endTime && t.EndTime > startTime);
    }
}
```

---

## Technical Implementation Notes

### Entity Relationships

```
PetWalker (existing)
    │
    ├── 1:N Timeslot
    │       └── Status (Available/Booked/Unavailable)
    │
    ├── 1:N WorkingHours
    │
    └── 1:N Booking (existing)
            └── 1:N TravelBuffer
```

### Validation Rules (from FR requirements)

1. **FR-001**: Duration must be 30-45 minutes
   - Validator: `RuleFor(x => x.DurationInMinutes).InclusiveBetween(30, 45)`

2. **FR-002**: No overlapping slots
   - Specification + DB unique constraint

3. **FR-008**: Timeslot within working hours
   - Validate: StartTime >= WorkingHours.StartTime AND EndTime <= WorkingHours.EndTime

4. **FR-010**: Clear error messages
   - Use Result.Error() with descriptive messages
   - Example: "Cannot create timeslot from 10:00-10:45 - overlaps with existing slot 10:15-11:00"

---

## Summary

| Research Topic | Decision | Key Benefit |
|---------------|----------|-------------|
| Slot Model | Discrete explicit slots | Simpler UI, clearer booking |
| Travel Buffers | Dynamic calculation | Flexibility, real-time accuracy |
| Overlap Prevention | Dual validation | Data integrity + UX |

All decisions align with FurryFriends Constitution:
- Uses CQRS with MediatR
- Uses Specification pattern for queries
- Uses Result pattern for operation outcomes
- All handlers return Result types
