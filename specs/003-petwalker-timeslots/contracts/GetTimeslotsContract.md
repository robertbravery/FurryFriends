# Contract: GetTimeslots

**Feature**: Petwalker Timeslots  
**Pattern**: FastEndpoints (Request/Response/Endpoint separation)

---

## Endpoint Definition

| Property | Value |
|----------|-------|
| Method | GET |
| Route | `/api/petwalkers/{PETWALKERID}/timeslots?date={DATE}&status={STATUS}` |
| Authorization | PetWalker only (own data) |
| Summary | Get timeslots for a petwalker |
| Description | Retrieves all timeslots for a petwalker, optionally filtered by date and status |

---

## Request

```csharp
public class GetTimeslotsRequest
{
    public const string Route = "/api/petwalkers/{PETWALKERID}/timeslots";
    
    public Guid PetWalkerId { get; set; }  // Route parameter
    public DateOnly? Date { get; set; }   // Query: optional, filter by specific date
    public string? Status { get; set; }   // Query: optional, filter by status (Available/Booked/Unavailable/Cancelled)
}
```

### Validation Rules

| Field | Rule | Error Message |
|-------|------|---------------|
| PetWalkerId | NotEmpty, NotNull | PetWalker ID is required |

---

## Response: Success (200 OK)

```csharp
public class GetTimeslotsResponse
{
    public List<TimeslotDto> Timeslots { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

public class TimeslotDto
{
    public Guid TimeslotId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int DurationInMinutes { get; set; }
    public string Status { get; set; }  // Available, Booked, Unavailable, Cancelled
    public Guid? BookingId { get; set; }  // If booked, the booking ID
}
```

**Example**:
```json
{
  "timeslots": [
    {
      "timeslotId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
      "date": "2026-03-15",
      "startTime": "09:00",
      "endTime": "09:45",
      "durationInMinutes": 45,
      "status": "Available",
      "bookingId": null
    },
    {
      "timeslotId": "b2c3d4e5-f6a7-8901-bcde-f23456789012",
      "date": "2026-03-15",
      "startTime": "10:00",
      "endTime": "10:30",
      "durationInMinutes": 30,
      "status": "Booked",
      "bookingId": "c3d4e5f6-a7b8-9012-cdef-345678901234"
    }
  ],
  "totalCount": 2,
  "pageNumber": 1,
  "pageSize": 20
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

## Query Handler Interface

```csharp
public class GetTimeslotsQuery : IQuery<Result<PaginatedList<TimeslotDto>>>
{
    public Guid PetWalkerId { get; set; }
    public DateOnly? Date { get; set; }
    public string? Status { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
```

**Returns**: `Result<PaginatedList<TimeslotDto>>` - Paginated list of timeslots

---

## Specification Used

```csharp
public class TimeslotsByPetWalkerAndDateSpecification : Specification<Timeslot>
{
    public TimeslotsByPetWalkerAndDateSpecification(
        Guid petWalkerId, 
        DateOnly? date,
        string? status,
        int pageNumber,
        int pageSize)
    {
        Query.Where(t => t.PetWalkerId == petWalkerId);

        if (date.HasValue)
        {
            Query.Where(t => t.Date == date.Value);
        }

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<TimeslotStatus>(status, true, out var statusEnum))
        {
            Query.Where(t => t.Status == statusEnum);
        }

        Query.OrderBy(t => t.Date).ThenBy(t => t.StartTime);
        Query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    }
}
```

---

## Integration Test Scenarios

| Scenario | Expected Status | Description |
|----------|-----------------|-------------|
| Get all timeslots | 200 | Returns all slots for petwalker |
| Filter by date | 200 | Returns only slots on specified date |
| Filter by status | 200 | Returns only slots with specified status |
| No timeslots found | 200 | Returns empty list (not 404) |
| Invalid petwalker | 404 | Petwalker doesn't exist |
| Invalid status | 400 | Invalid status enum value |
