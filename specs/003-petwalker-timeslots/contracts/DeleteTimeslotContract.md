# Contract: DeleteTimeslot

**Feature**: Petwalker Timeslots  
**Pattern**: FastEndpoints (Request/Response/Endpoint separation)

---

## Endpoint Definition

| Property | Value |
|----------|-------|
| Method | DELETE |
| Route | `/api/timeslots/{TIMESLOTID}` |
| Authorization | PetWalker only (own timeslots) |
| Summary | Delete a timeslot |
| Description | Allows a petwalker to delete one of their timeslots. Booked timeslots cannot be deleted. |

---

## Request

```csharp
public class DeleteTimeslotRequest
{
    public const string Route = "/api/timeslots/{TIMESLOTID}";
    
    public Guid TimeslotId { get; set; }  // Route parameter
}
```

### Validation Rules

| Field | Rule | Error Message |
|-------|------|---------------|
| TimeslotId | NotEmpty, NotNull | Timeslot ID is required |

---

## Response: Success (204 No Content)

No content returned on successful deletion.

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
  "error": "Cannot delete a timeslot that has an active booking"
}
```

---

## Command Handler Interface

```csharp
public class DeleteTimeslotCommand : ICommand<Result>
{
    public Guid TimeslotId { get; set; }
}
```

**Returns**: `Result` - Success or error

---

## Business Rules

1. Only timeslots with status `Available` or `Cancelled` can be deleted
2. If the timeslot has an associated active booking (status `Booked`), deletion should be rejected
3. Deletion should set status to `Cancelled` rather than hard delete for audit purposes

---

## Integration Test Scenarios

| Scenario | Expected Status | Description |
|----------|-----------------|-------------|
| Delete available slot | 204 | Successfully deletes slot |
| Delete booked slot | 409 | Cannot delete - has booking |
| Delete already cancelled | 204 | Can delete (idempotent) |
| Invalid timeslot ID | 404 | Timeslot doesn't exist |
| Wrong petwalker | 403 | Not authorized to delete |
