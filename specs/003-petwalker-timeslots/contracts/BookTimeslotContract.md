# Contract: BookTimeslot

**Feature**: Petwalker Timeslots  
**Pattern**: FastEndpoints (Request/Response/Endpoint separation)

---

## Endpoint Definition

| Property | Value |
|----------|-------|
| Method | POST |
| Route | `/api/timeslots/{TIMESLOTID}/book` |
| Authorization | Client authenticated |
| Summary | Book an available timeslot |
| Description | Allows a client to book an available timeslot for a pet walking service |

---

## Request

```csharp
public class BookTimeslotRequest
{
    public const string Route = "/api/timeslots/{TIMESLOTID}/book";
    
    public Guid TimeslotId { get; set; }  // Route parameter
    public Guid ClientId { get; set; }    // From auth context
    public string ClientAddress { get; set; }  // Service location
    public List<Guid> PetIds { get; set; }  // Pets to be walked
}
```

### Validation Rules

| Field | Rule | Error Message |
|-------|------|---------------|
| TimeslotId | NotEmpty | Timeslot ID is required |
| ClientId | NotEmpty | Client ID is required |
| ClientAddress | NotEmpty, MaxLength(500) | Service address is required |
| PetIds | NotEmpty | At least one pet is required |
| PetIds | Count <= 5 | Maximum 5 pets per booking |

---

## Response: Success (201 Created)

```csharp
public class BookTimeslotResponse
{
    public Guid BookingId { get; set; }
    public Guid TimeslotId { get; set; }
    public Guid PetWalkerId { get; set; }
    public Guid ClientId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string ClientAddress { get; set; }
    public string Status { get; set; }  // Confirmed
    public bool HasTravelBuffer { get; set; }
    public int? TravelBufferMinutes { get; set; }
}
```

**Example**:
```json
{
  "bookingId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "timeslotId": "b2c3d4e5-f6a7-8901-bcde-f23456789012",
  "petWalkerId": "c3d4e5f6-a7b8-9012-cdef-345678901234",
  "clientId": "d4e5f6a7-b8c9-0123-def0-456789012345",
  "date": "2026-03-15",
  "startTime": "09:00",
  "endTime": "09:45",
  "clientAddress": "123 Main Street, Suburb, City, 1234",
  "status": "Confirmed",
  "hasTravelBuffer": true,
  "travelBufferMinutes": 15
}
```

---

## Response: Validation Error (400 Bad Request)

```json
{
  "errors": {
    "PetIds": ["At least one pet is required"],
    "ClientAddress": ["Service address is required"]
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

## Response: Conflict - Already Booked (409)

```json
{
  "error": "This timeslot is no longer available"
}
```

---

## Command Handler Interface

```csharp
public class BookTimeslotCommand : ICommand<Result<Guid>>
{
    public Guid TimeslotId { get; set; }
    public Guid ClientId { get; set; }
    public string ClientAddress { get; set; }
    public List<Guid> PetIds { get; set; }
}
```

**Returns**: `Result<Guid>` - The ID of the created booking

---

## Business Logic

1. **Availability Check**: Verify timeslot status is `Available` before booking
2. **Atomic Update**: Update timeslot status to `Booked` in same transaction as booking creation
3. **Travel Buffer Calculation**:
   - Query previous booking for this petwalker on the same day
   - If different address, calculate travel buffer
   - Create TravelBuffer entity
   - Mark subsequent timeslots as `Unavailable` during buffer period

---

## Travel Buffer Implementation

```csharp
// When booking is created
var previousBooking = await _bookingRepository.GetPreviousBookingForPetWalkerToday(
    request.PetWalkerId, 
    timeslot.Date);

if (previousBooking != null && 
    previousBooking.ClientAddress != request.ClientAddress)
{
    // Calculate buffer
    var bufferMinutes = CalculateTravelBuffer(
        previousBooking.ClientAddress, 
        request.ClientAddress);
    
    // Create travel buffer
    var buffer = TravelBuffer.Create(
        bookingId,
        previousBooking.Id,
        previousBooking.ClientAddress,
        request.ClientAddress,
        bufferMinutes,
        timeslot.EndTimeDateTime,
        timeslot.EndTimeDateTime.AddMinutes(bufferMinutes));
    
    await _repository.AddAsync(buffer, ct);
    
    // Block subsequent timeslots during buffer
    await _timeslotRepository.BlockTimeslotsDuringBuffer(
        request.PetWalkerId,
        timeslot.Date,
        timeslot.EndTime,
        bufferMinutes);
}
```

---

## Integration Test Scenarios

| Scenario | Expected Status | Description |
|----------|-----------------|-------------|
| Book available slot | 201 | Creates booking, updates slot status |
| Book already booked slot | 409 | Slot no longer available |
| Book unavailable slot | 409 | Slot is blocked |
| Different address - creates buffer | 201 | Travel buffer created |
| Same address - no buffer | 201 | No travel buffer needed |
| Invalid pet IDs | 400 | Pets not found or don't belong to client |
| Client owns timeslot | 400 | Cannot book own timeslot |
