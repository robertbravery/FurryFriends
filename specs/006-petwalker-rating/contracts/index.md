# Rating API Contracts

This folder contains the API contract definitions for the Petwalker Rating System feature.

## Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/ratings` | Create a new rating |
| PUT | `/api/ratings/{id}` | Update an existing rating |
| GET | `/api/petwalkers/{id}/ratings` | Get all ratings for a petwalker |
| GET | `/api/petwalkers/{id}/ratings/summary` | Get rating summary for a petwalker |

## Contract Files

1. [CreateRatingContract.md](CreateRatingContract.md) - Submit a rating for a completed booking
2. [UpdateRatingContract.md](UpdateRatingContract.md) - Update rating within 7 days
3. [GetRatingsForPetWalkerContract.md](GetRatingsForPetWalkerContract.md) - List all ratings with pagination
4. [GetPetWalkerRatingSummaryContract.md](GetPetWalkerRatingSummaryContract.md) - Get average rating and statistics

## Authentication

All endpoints require authentication. The client must be authenticated to:
- Create or update a rating (must own the booking)
- View ratings for a petwalker (must be the petwalker)

## Common Error Response Format

```json
{
  "errorType": "ErrorType",
  "message": "Human readable message",
  "errors": [  // Optional, for validation errors
    {
      "field": "fieldName",
      "message": "Validation message"
    }
  ]
}
```

## Error Types

| Type | HTTP Status | Description |
|------|-------------|-------------|
| ValidationError | 400 | Invalid input data |
| NotFound | 404 | Resource not found |
| Conflict | 409 | Business rule violation |
| Forbidden | 403 | Access denied |
| Unauthorized | 401 | Authentication required |

---

**Date**: 2026-03-19
