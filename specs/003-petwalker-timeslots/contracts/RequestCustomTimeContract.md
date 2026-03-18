# Contract: RequestCustomTime

**Feature**: Petwalker Timeslots  
**Pattern**: FastEndpoints (Request/Response/Endpoint separation)

---

## Endpoint Definition

| Property | Value |
|----------|-------|
| Method | POST |
| Route | `/api/timeslots/request-custom` |
| Authorization | Client authenticated |
| Summary | Request a custom time when preset slots are unavailable |
| Description | Allows a client to submit a request for a custom time slot when they cannot find a suitable available timeslot |

---

## Request

```csharp
public class RequestCustomTimeRequest
{
    public const string Route = "/api/timeslots/request-custom";
    
    public Guid PetWalkerId { get; set; }  // Route parameter
    public Guid ClientId { get; set; }     // From auth context
    public DateOnly RequestedDate { get; set; }  // Date client wants
    public TimeOnly PreferredStartTime { get; set; }  // Preferred start time
    public int PreferredDurationMinutes { get; set; }  // Duration (30-45)
    public string ClientAddress { get; set; }  // Service location
    public List<Guid> PetIds { get; set; }  // Pets to be walked
}
```

### Validation Rules

| Field | Rule | Error Message |
|-------|------|---------------|
| PetWalkerId | NotEmpty | Petwalker ID is required |
| ClientId | NotEmpty | Client ID is required |
| RequestedDate | NotEmpty, NotInPast | Requested date is required and must be today or future |
| PreferredStartTime | NotEmpty | Preferred start time is required |
| PreferredDurationMinutes | Range(30, 45) | Duration must be between 30 and 45 minutes |
| ClientAddress | NotEmpty, MaxLength(500) | Service address is required |
| PetIds | NotEmpty | At least one pet is required |
| PetIds | Count <= 5 | Maximum 5 pets per booking |

---

## Response: Success (201 Created)

```csharp
public class RequestCustomTimeResponse
{
    public Guid RequestId { get; set; }
    public Guid PetWalkerId { get; set; }
    public Guid ClientId { get; set; }
    public DateOnly RequestedDate { get; set; }
    public TimeOnly PreferredStartTime { get; set; }
    public TimeOnly PreferredEndTime { get; set; }
    public int PreferredDurationMinutes { get; set; }
    public string ClientAddress { get; set; }
    public string Status { get; set; }  // Pending
    public DateTime CreatedAt { get; set; }
}
```

**Example**:
```json
{
  "requestId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "petWalkerId": "c3d4e5f6-a7b8-9012-cdef-345678901234",
  "clientId": "d4e5f6a7-b8c9-0123-def0-456789012345",
  "requestedDate": "2026-03-15",
  "preferredStartTime": "10:30",
  "preferredEndTime": "11:15",
  "preferredDurationMinutes": 45,
  "clientAddress": "123 Main Street, Suburb, City, 1234",
  "status": "Pending",
  "createdAt": "2026-03-14T14:30:00Z"
}
```

---

## Response: Validation Error (400 Bad Request)

```json
{
  "errors": {
    "RequestedDate": ["Requested date is required and must be today or future"],
    "PreferredDurationMinutes": ["Duration must be between 30 and 45 minutes"],
    "PetIds": ["At least one pet is required"]
  }
}
```

---

## Response: Not Found (404)

```json
{
  "error": "Petwalker not found"
}
```

---

## Response: Conflict - Existing Pending Request (409)

```json
{
  "error": "You already have a pending custom time request for this petwalker"
}
```

---

## Command Handler Interface

```csharp
public class RequestCustomTimeCommand : ICommand<Result<Guid>>
{
    public Guid PetWalkerId { get; set; }
    public Guid ClientId { get; set; }
    public DateOnly RequestedDate { get; set; }
    public TimeOnly PreferredStartTime { get; set; }
    public int PreferredDurationMinutes { get; set; }
    public string ClientAddress { get; set; }
    public List<Guid> PetIds { get; set; }
}
```

**Returns**: `Result<Guid>` - The ID of the created CustomTimeRequest

---

## Business Logic

1. **Validation**: Verify petwalker exists and is active
2. **No Duplicate Requests**: Check for existing pending requests from this client for this petwalker
3. **Create Request**: Create CustomTimeRequest entity with status `Pending`
4. **Notification**: Send notification to petwalker about new custom time request
5. **Expiration**: Set expiration to 48 hours from creation (background job)

---

## Integration Test Scenarios

| Scenario | Expected Status | Description |
|----------|-----------------|-------------|
| Submit valid request | 201 | Creates pending request |
| Request for past date | 400 | Cannot request past dates |
| Invalid duration | 400 | Duration must be 30-45 minutes |
| Petwalker not found | 404 | Petwalker doesn't exist |
| Duplicate pending request | 409 | Already has pending request |
| Invalid pet IDs | 400 | Pets not found or don't belong to client |
| No available slots exists | 201 | Request created even if slots available (client preference) |
