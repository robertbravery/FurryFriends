# Update Rating Contract

## Endpoint
`PUT /api/ratings/{id}`

## Description
Allows a client to update their rating within 7 days of submission.

## FR Reference
- FR-003: Allow clients to update rating within 7 days

## Request

### URL Parameters
| Parameter | Type | Description |
|-----------|------|-------------|
| id | Guid | Rating ID to update |

### Fields
| Field | Type | Required | Constraints |
|-------|------|----------|-------------|
| RatingValue | int | Yes | 1-5 (inclusive) |
| Comment | string | No | Max 1000 characters |

### Example
```json
{
  "ratingValue": 4,
  "comment": "Updated rating - initial excitement wore off a bit"
}
```

## Response

### Success (200 OK)
```json
{
  "id": "b2c3d4e5-f6a7-8901-bcde-f23456789012",
  "petWalkerId": "c3d4e5f6-a7b8-9012-cdef-345678901234",
  "clientId": "d4e5f6a7-b8c9-0123-defg-456789012345",
  "bookingId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "ratingValue": 4,
  "comment": "Updated rating - initial excitement wore off a bit",
  "createdDate": "2026-03-19T10:30:00Z",
  "modifiedDate": "2026-03-20T14:45:00Z"
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

#### 403 - Forbidden (Update not allowed)
```json
{
  "errorType": "Forbidden",
  "message": "Rating can no longer be updated. Updates are only allowed within 7 days of submission and can only be made once."
}
```

#### 404 - Not Found
```json
{
  "errorType": "NotFound",
  "message": "Rating not found"
}
```

## Business Rules
1. Rating must exist
2. Client must be the original creator of the rating
3. Rating must not have been previously modified (ModifiedDate must be null)
4. Rating must be within 7 days of creation (CreatedDate + 7 days > Now)
5. RatingValue must be 1, 2, 3, 4, or 5
