# Get PetWalker Rating Summary Contract

## Endpoint
`GET /api/petwalkers/{id}/ratings/summary`

## Description
Retrieves the rating summary for a specific petwalker, including average rating, total count, and recent ratings.

## FR Reference
- FR-003: System MUST display the average rating score to petwalkers
- FR-007: System MUST show the total number of ratings received to petwalkers

## Request

### URL Parameters
| Parameter | Type | Description |
|-----------|------|-------------|
| id | Guid | PetWalker ID |

### Example
```
GET /api/petwalkers/c3d4e5f6-a7b8-9012-cdef-345678901234/ratings/summary
```

## Response

### Success (200 OK)
```json
{
  "petWalkerId": "c3d4e5f6-a7b8-9012-cdef-345678901234",
  "averageRating": 4.5,
  "totalRatings": 50,
  "recentRatings": [
    {
      "id": "b2c3d4e5-f6a7-8901-bcde-f23456789012",
      "ratingValue": 5,
      "comment": "Excellent service! My dog loved the walk.",
      "createdDate": "2026-03-19T10:30:00Z",
      "clientName": "John Doe"
    },
    {
      "id": "c3d4e5f6-a7b8-9012-cdef-345678901234",
      "ratingValue": 4,
      "comment": "Very good, would recommend",
      "createdDate": "2026-03-18T15:00:00Z",
      "clientName": "Jane Smith"
    },
    {
      "id": "d4e5f6a7-b8c9-0123-defg-456789012345",
      "ratingValue": 5,
      "comment": "Great walker!",
      "createdDate": "2026-03-17T09:00:00Z",
      "clientName": "Bob Wilson"
    }
  ]
}
```

### Empty Response (No ratings yet)
```json
{
  "petWalkerId": "c3d4e5f6-a7b8-9012-cdef-345678901234",
  "averageRating": 0,
  "totalRatings": 0,
  "recentRatings": []
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

## Response Fields
| Field | Type | Description |
|-------|------|-------------|
| petWalkerId | Guid | The petwalker's ID |
| averageRating | double | Average rating (0-5, 2 decimal places) |
| totalRatings | int | Total number of ratings |
| recentRatings | array | Last 10 ratings (most recent first) |

## Calculation Rules
- Average rating: Sum of all RatingValue / TotalRatings
- Rounded to 2 decimal places
- If no ratings, averageRating = 0
- Recent ratings: Last 10 ratings ordered by CreatedDate descending
