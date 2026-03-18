# Contract: WorkingHours Management

**Feature**: Petwalker Timeslots  
**Pattern**: FastEndpoints (Request/Response/Endpoint separation)

---

## Endpoint: Create WorkingHours

### Definition

| Property | Value |
|----------|-------|
| Method | POST |
| Route | `/api/petwalkers/{PETWALKERID}/workinghours` |
| Authorization | PetWalker only |
| Summary | Create working hours for a petwalker |
| Description | Define when a petwalker is available for creating timeslots |

### Request

```csharp
public class CreateWorkingHoursRequest
{
    public const string Route = "/api/petwalkers/{PETWALKERID}/workinghours";
    
    public Guid PetWalkerId { get; set; }  // Route parameter
    public DayOfWeek DayOfWeek { get; set; }  // 0=Sunday, 6=Saturday
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
}
```

### Validation Rules

| Field | Rule | Error Message |
|-------|------|---------------|
| PetWalkerId | NotEmpty | PetWalker ID is required |
| DayOfWeek | IsInEnum | Valid day of week required |
| StartTime | NotEmpty | Start time is required |
| EndTime | NotEmpty | End time is required |
| EndTime | GreaterThan(StartTime) | End time must be after start time |
| Overlap | No overlap with existing | Overlaps with existing working hours |

### Response: Success (201 Created)

```json
{
  "workingHoursId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "petWalkerId": "b2c3d4e5-f6a7-8901-bcde-f23456789012",
  "dayOfWeek": 1,
  "dayName": "Monday",
  "startTime": "08:00",
  "endTime": "12:00",
  "isActive": true
}
```

---

## Endpoint: Get WorkingHours

### Definition

| Property | Value |
|----------|-------|
| Method | GET |
| Route | `/api/petwalkers/{PETWALKERID}/workinghours` |
| Authorization | PetWalker or Client |

### Request

```csharp
public class GetWorkingHoursRequest
{
    public const string Route = "/api/petwalkers/{PETWALKERID}/workinghours";
    
    public Guid PetWalkerId { get; set; }
    public DayOfWeek? DayOfWeek { get; set; }  // Optional: filter by specific day
}
```

### Response: Success (200 OK)

```json
{
  "petWalkerId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "workingHours": [
    {
      "workingHoursId": "c3d4e5f6-a7b8-9012-cdef-345678901234",
      "dayOfWeek": 1,
      "dayName": "Monday",
      "startTime": "08:00",
      "endTime": "12:00",
      "isActive": true
    },
    {
      "workingHoursId": "d4e5f6a7-b8c9-0123-def0-456789012345",
      "dayOfWeek": 1,
      "dayName": "Monday",
      "startTime": "14:00",
      "endTime": "18:00",
      "isActive": true
    }
  ]
}
```

---

## Endpoint: Update WorkingHours

### Definition

| Property | Value |
|----------|-------|
| Method | PUT |
| Route | `/api/workinghours/{WORKINGHOURSID}` |
| Authorization | PetWalker only |

### Request

```csharp
public class UpdateWorkingHoursRequest
{
    public const string Route = "/api/workinghours/{WORKINGHOURSID}";
    
    public Guid WorkingHoursId { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public bool IsActive { get; set; }
}
```

---

## Endpoint: Delete WorkingHours

### Definition

| Property | Value |
|----------|-------|
| Method | DELETE |
| Route | `/api/workinghours/{WORKINGHOURSID}` |
| Authorization | PetWalker only |

### Request

```csharp
public class DeleteWorkingHoursRequest
{
    public const string Route = "/api/workinghours/{WORKINGHOURSID}";
    
    public Guid WorkingHoursId { get; set; }
}
```

### Response: Success (204 No Content)

---

## Business Rules

1. A petwalker can have multiple working hours per day (e.g., morning and evening shifts)
2. Working hours cannot overlap on the same day
3. When creating a timeslot, it must fall within at least one active working hours period
4. Deleting working hours does not automatically delete associated timeslots

---

## Integration Test Scenarios

| Scenario | Expected Status | Description |
|----------|-----------------|-------------|
| Create valid working hours | 201 | Morning shift created |
| Create second shift same day | 201 | Evening shift created |
| Overlapping working hours | 409 | Conflict - shifts overlap |
| Get petwalker working hours | 200 | Returns all defined hours |
| Update working hours | 200 | Successfully updates times |
| Delete working hours | 204 | Successfully deletes |
