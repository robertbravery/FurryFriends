# Get Ratings For PetWalker Contract

## Endpoint
`GET /api/petwalkers/{id}/ratings`

## Description
Retrieves all ratings for a specific petwalker, ordered by most recent first.

## FR Reference
- FR-004: System MUST allow petwalkers to view individual ratings
- FR-005: System MUST display the date of each rating
- FR-009: System MUST display ratings in descending order (most recent first)

## Request

### URL Parameters
| Parameter | Type | Description |
|-----------|------|-------------|
| id | Guid | PetWalker ID |
| page | int | Page number (default: 1) |
| pageSize | int | Items per page (default: 20, max: 100) |

### Example
```
GET /api/petwalkers/c3d4e5f6-a7b8-9012-cdef-345678901234/ratings?page=1&pageSize=20
```

## Response

### Success (200 OK)
```json
{
  "items": [
    {
      "id": "b2c3d4e5-f6a7-8901-bcde-f23456789012",
      "petWalkerId": "c3d4e5f6-a7b8-9012-cdef-345678901234",
      "clientId": "d4e5f6a7-b8c9-0123-defg-456789012345",
      "bookingId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
      "ratingValue": 5,
      "comment": "Excellent service! My dog loved the walk.",
      "createdDate": "2026-03-19T10:30:00Z",
      "modifiedDate": null,
      "clientName": "John Doe"
    },
    {
      "id": "c3d4e5f6-a7b8-9012-cdef-345678901234",
      "petWalkerId": "c3d4e5f6-a7b8-9012-cdef-345678901234",
      "clientId": "e5f6a7b8-c9d0-1234-efgh-567890123456",
      "bookingId": "b2c3d4e5-f6a7-8901-bcde-f12345678901",
      "ratingValue": 4,
      "comment": "Very good, would recommend",
      "createdDate": "2026-03-18T15:00:00Z",
      "modifiedDate": null,
      "clientName": "Jane Smith"
    }
  ],
  "page": 1,
  "pageSize": 20,
  "totalCount": 50,
  "totalPages": 3
}
```

### Empty Response (No ratings)
```json
{
  "items": [],
  "page": 1,
  "pageSize": 20,
  "totalCount": 0,
  "totalPages": 0
}
```

### Error Responses

#### 404 - Not Found (PetWalker not found)
```json
{
  "errorType": "NotFound",
  "message": "Petwalker not found"
}
```

## Pagination
- Default page size: 20
- Maximum page size: 100
- Results ordered by CreatedDate descending (most recent first)

## Response Fields
| Field | Type | Description |
|-------|------|-------------|
| items | array | List of ratings |
| page | int | Current page number |
| pageSize | int | Items per page |
| totalCount | int | Total number of ratings |
| totalPages | int | Total number of pages |
