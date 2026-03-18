# Contract: UpdateTimeslot

**Feature**: Petwalker Timeslots  
**Pattern**: FastEndpoints (Request/Response/Endpoint separation)

---

## Endpoint Definition

| Property | Value |
|----------|-------|
| Method | PUT |
| Route | `/api/timeslots/{TIMESLOTID}` |
| Authorization | PetWalker only (own timeslots) |
| Summary | Update an existing timeslot |
| Description | Allows a petwalker to modify the start time and/or duration of their timeslot |

---

## Request

```csharp
public class UpdateTimeslotRequest
{
    public const string Route = "/api/timeslots/{TIMESLOTID}";
    
    public Guid TimeslotId { get; set; }  // Route parameter
    public TimeOnly StartTime { get; set; }
    public int DurationInMinutes { get; set; }  // 30-45
}
```

### Validation Rules

| Field | Rule | Error Message |
|-------|------|---------------|
| TimeslotId | NotEmpty, NotNull | Timeslot ID is required |
| StartTime | NotEmpty | Start time is required |
| DurationInMinutes | InclusiveBetween(30, 45) | Duration must be between 30 and 45 minutes |
| StartTime + Duration | Must be within WorkingHours | Timeslot must fall within defined working hours |
| Overlap | Must not overlap existing slots | Timeslot overlaps with an existing slot |
| Status | Must be Available | Cannot modify a booked or unavailable slot |

---

## Response: Success (200 OK)

```csharp
public class UpdateTimeslotResponse
{
    public Guid TimeslotId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int DurationInMinutes { get; set; }
    public string Status { get; set; }
}
```

**Example**:
```json
{
  "timeslotId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "date": "2026-03-15",
  "startTime": "09:30",
  "endTime": "10:15",
  "durationInMinutes": 45,
  "status": "Available"
}
```

---

## Response: Validation Error (400 Bad Request)

```json
{
  "errors": {
    "DurationInMinutes": ["Duration must be between 30 and 45 minutes"]
  }
}
```

---

## Response: Not Found (404)

```json
{
  "error": "Timeslot not found"
}
```

---

## Response: Conflict (409)

```json
{
  "error": "Cannot modify timeslot - it is already booked"
}
```

---

## Response: Conflict - Overlap (409)

```json
{
  "error": "Cannot update timeslot to 10:00-10:45 - overlaps with existing slot 10:15-11:00"
}
```

---

## Command Handler Interface

```csharp
public class UpdateTimeslotCommand : ICommand<Result<Guid>>
{
    public Guid TimeslotId { get; set; }
    public TimeOnly StartTime { get; set; }
    public int DurationInMinutes { get; set; }
}
```

**Returns**: `Result<Guid>` - The ID of the updated timeslot

---

## Business Rules

1. Only timeslots with status `Available` can be modified
2. If the timeslot has an associated booking, modification should be rejected
3. The new timeslot must still fall within the petwalker's working hours
4. The new timeslot must not overlap with any other existing timeslots

---

## Integration Test Scenarios

| Scenario | Expected Status | Description |
|----------|-----------------|-------------|
| Update available slot | 200 | Successfully updates slot |
| Modify duration | 200 | Duration changed within valid range |
| Duration < 30 | 400 | Invalid duration |
| Duration > 45 | 400 | Invalid duration |
| Booked slot | 409 | Cannot modify booked slot |
| Overlapping slot | 409 | Conflict with existing slot |
| Outside working hours | 400 | Must be within working hours |
| Invalid timeslot ID | 404 | Timeslot doesn't exist |
