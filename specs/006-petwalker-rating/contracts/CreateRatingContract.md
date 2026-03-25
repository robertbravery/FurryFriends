# Create Rating Contract

## Endpoint
`POST /api/ratings`

## Description
Allows a client to submit a rating for a completed petwalking service.

## FR Reference
- FR-001: System MUST allow clients to submit a rating from 1 to 5 stars
- FR-002: System MUST associate each rating with specific client, petwalker, and booking
- FR-006: System MUST prevent clients from submitting multiple ratings for same booking
- FR-008: System MUST allow clients to include an optional text comment

## Request

### Fields
| Field | Type | Required | Constraints |
|-------|------|----------|-------------|
| BookingId | Guid | Yes | Must be valid booking ID |
| RatingValue | int | Yes | 1-5 (inclusive) |
| Comment | string | No | Max 1000 characters |

### Example
```json
{
  "bookingId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "ratingValue": 5,
  "comment": "Excellent service! My dog loved the walk."
}
```

## Response

### Success (201 Created)
```json
{
  "id": "b2c3d4e5-f6a7-8901-bcde-f23456789012",
  "petWalkerId": "c3d4e5f6-a7b8-9012-cdef-345678901234",
  "clientId": "d4e5f6a7-b8c9-0123-defg-456789012345",
  "bookingId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "ratingValue": 5,
  "comment": "Excellent service! My dog loved the walk.",
  "createdDate": "2026-03-19T10:30:00Z",
  "modifiedDate": null
}
```

### Error Responses

#### 400 - Validation Error
```json
{
  "errorType": "ValidationError",
  "errors": [
    {
      "field": "ratingValue",
      "message": "Rating value must be between 1 and 5"
    }
  ]
}
```

#### 404 - Not Found (Booking not found)
```json
{
  "errorType": "NotFound",
  "message": "Booking not found"
}
```

#### 409 - Conflict (Rating already exists)
```json
{
  "errorType": "Conflict",
  "message": "A rating has already been submitted for this booking"
}
```

#### 400 - Invalid Booking Status
```json
{
  "errorType": "ValidationError",
  "message": "Only completed bookings can be rated"
}
```

## Business Rules
1. Booking must exist and be in "Completed" status
2. Client must be the owner of the booking
3. No existing rating for the specified BookingId
4. RatingValue must be 1, 2, 3, 4, or 5
