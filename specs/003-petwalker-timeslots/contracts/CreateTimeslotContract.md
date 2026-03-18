# Contract: CreateTimeslot

**Feature**: Petwalker Timeslots  
**Pattern**: FastEndpoints (Request/Response/Endpoint separation)

---

## Endpoint Definition

| Property | Value |
|----------|-------|
| Method | POST |
| Route | `/api/timeslots` |
| Authorization | PetWalker only |
| Summary | Create a new available timeslot |
| Description | Allows a petwalker to create a new time slot for bookings |

---

## Request

```csharp
public class CreateTimeslotRequest
{
    public const string Route = "/api/timeslots";
    
    public Guid PetWalkerId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public int DurationInMinutes { get; set; }  // 30-45
}
```

### Validation Rules

| Field | Rule | Error Message |
|-------|------|---------------|
| PetWalkerId | NotEmpty, NotNull | PetWalker ID is required |
| Date | NotEmpty | Date is required |
| Date | Must be today or future | Cannot create timeslots for past dates |
| StartTime | NotEmpty | Start time is required |
| DurationInMinutes | InclusiveBetween(30, 45) | Duration must be between 30 and 45 minutes |
| StartTime + Duration | Must be within WorkingHours | Timeslot must fall within defined working hours |
| Overlap | Must not overlap existing slots | Timeslot overlaps with an existing slot |

---

## Response: Success (201 Created)

```csharp
public class CreateTimeslotResponse
{
    public Guid TimeslotId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int DurationInMinutes { get; set; }
    public string Status { get; set; }  // "Available"
}
```

**Example**:
```json
{
  "timeslotId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "date": "2026-03-15",
  "startTime": "09:00",
  "endTime": "09:45",
  "durationInMinutes": 45,
  "status": "Available"
}
```

---

## Response: Validation Error (400 Bad Request)

```json
{
  "errors": {
    "DurationInMinutes": ["Duration must be between 30 and 45 minutes"],
    "StartTime": ["Timeslot overlaps with an existing slot"]
  }
}
```

---

## Response: Not Found (404)

```json
{
  "error": "PetWalker not found"
}
```

---

## Response: Conflict (409)

```json
{
  "error": "Cannot create timeslot from 10:00-10:45 - overlaps with existing slot 10:15-11:00"
}
```

---

## Command Handler Interface

```csharp
public class CreateTimeslotCommand : ICommand<Result<Guid>>
{
    public Guid PetWalkerId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public int DurationInMinutes { get; set; }
}
```

**Returns**: `Result<Guid>` - The ID of the created timeslot

---

## Integration Test Scenarios

| Scenario | Expected Status | Description |
|----------|-----------------|-------------|
| Valid timeslot creation | 201 | Creates slot within working hours |
| Duration < 30 | 400 | Invalid duration |
| Duration > 45 | 400 | Invalid duration |
| Overlapping slot | 409 | Conflict error |
| Past date | 400 | Cannot create for past |
| Outside working hours | 400 | Must be within working hours |
