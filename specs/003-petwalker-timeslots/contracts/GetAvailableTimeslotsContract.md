# Contract: GetAvailableTimeslots

**Feature**: Petwalker Timeslots  
**Pattern**: FastEndpoints (Request/Response/Endpoint separation)

---

## Endpoint Definition

| Property | Value |
|----------|-------|
| Method | GET |
| Route | `/api/petwalkers/{PETWALKERID}/timeslots/available?date={DATE}` |
| Authorization | Client or Public |
| Summary | Get available timeslots for a petwalker |
| Description | Retrieves only available (bookable) timeslots for a specific petwalker on a specific date. Excludes booked, unavailable, and cancelled slots. |

---

## Request

```csharp
public class GetAvailableTimeslotsRequest
{
    public const string Route = "/api/petwalkers/{PETWALKERID}/timeslots/available";
    
    public Guid PetWalkerId { get; set; }  // Route parameter
    public DateOnly Date { get; set; }     // Query: required, specific date
}
```

### Validation Rules

| Field | Rule | Error Message |
|-------|------|---------------|
| PetWalkerId | NotEmpty, NotNull | PetWalker ID is required |
| Date | NotEmpty | Date is required |

---

## Response: Success (200 OK)

```csharp
public class GetAvailableTimeslotsResponse
{
    public Guid PetWalkerId { get; set; }
    public DateOnly Date { get; set; }
    public List<AvailableTimeslotDto> Timeslots { get; set; } = new();
    public bool HasTravelBufferWarning { get; set; }
    public string? TravelBufferMessage { get; set; }
}

public class AvailableTimeslotDto
{
    public Guid TimeslotId { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int DurationInMinutes { get; set; }
}
```

**Example**:
```json
{
  "petWalkerId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "date": "2026-03-15",
  "timeslots": [
    {
      "timeslotId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
      "startTime": "09:00",
      "endTime": "09:45",
      "durationInMinutes": 45
    },
    {
      "timeslotId": "b2c3d4e5-f6a7-8901-bcde-f23456789012",
      "startTime": "11:00",
      "endTime": "11:30",
      "durationInMinutes": 30
    }
  ],
  "hasTravelBufferWarning": true,
  "travelBufferMessage": "Note: Travel buffers between appointments may affect availability"
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

## Response: No Available Slots (200 OK)

```json
{
  "petWalkerId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "date": "2026-03-15",
  "timeslots": [],
  "hasTravelBufferWarning": false,
  "travelBufferMessage": null
}
```

---

## Query Handler Interface

```csharp
public class GetAvailableTimeslotsQuery : IQuery<Result<GetAvailableTimeslotsResponse>>
{
    public Guid PetWalkerId { get; set; }
    public DateOnly Date { get; set; }
}
```

**Returns**: `Result<GetAvailableTimeslotsResponse>` - Available timeslots for booking

---

## Specification Used

```csharp
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
```

---

## Integration Test Scenarios

| Scenario | Expected Status | Description |
|----------|-----------------|-------------|
| Get available slots | 200 | Returns available slots only |
| No available slots | 200 | Returns empty array |
| Invalid petwalker | 404 | Petwalker doesn't exist |
| Past date | 200 | Returns empty (no past bookings) |
| Date not specified | 400 | Date is required |
