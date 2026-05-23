# API Contracts: PetWalker Ratings

This document defines the API contracts for the rating feature.

---

## POST /api/ratings

Create a new rating for a petwalker.

### Request

```json
{
  "petWalkerId": "guid",
  "ratingValue": 5,
  "comment": "Great service!"
}
```

| Field | Type | Required | Constraints |
|-------|------|----------|-------------|
| petWalkerId | Guid | Yes | Must be valid petwalker |
| ratingValue | int | Yes | 1-5 |
| comment | string | No | Max 1000 chars |

### Response: 201 Created

```json
{
  "ratingId": "guid"
}
```

### Response: 400 Bad Request

```json
{
  "errors": {
    "petWalkerId": ["Pet Walker ID is required"],
    "ratingValue": ["Rating must be between 1 and 5"]
  }
}
```

### Response: 400 Error (Business Rule)

```json
{
  "error": "You must have at least one completed booking with this petwalker to submit a rating."
}
```

```json
{
  "error": "You have reached the maximum number of ratings for this petwalker."
}
```

---

## PUT /api/ratings/{id}

Update an existing rating (within 24-hour window).

### Request

```json
{
  "ratingValue": 4,
  "comment": "Updated comment"
}
```

| Field | Type | Required | Constraints |
|-------|------|----------|-------------|
| ratingValue | int | No | 1-5, required if comment not provided |
| comment | string | No | Max 1000 chars |

### Response: 204 No Content

### Response: 404 Not Found

```json
{
  "error": "Rating not found."
}
```

### Response: 400 Error

```json
{
  "error": "Rating can only be edited within 24 hours of submission."
}
```

---

## DELETE /api/ratings/{id}

Delete a rating (within 24-hour window).

### Response: 204 No Content

### Response: 404 Not Found

```json
{
  "error": "Rating not found."
}
```

### Response: 400 Error

```json
{
  "error": "Rating can only be deleted within 24 hours of submission."
}
```

---

## GET /api/ratings/petwalker/{petWalkerId}

Get all ratings for a petwalker (public endpoint).

### Query Parameters

| Parameter | Type | Default | Constraints |
|-----------|------|---------|-------------|
| page | int | 1 | Min 1 |
| pageSize | int | 10 | 1-100 |

### Response: 200 OK

```json
{
  "items": [
    {
      "id": "guid",
      "clientId": "guid",
      "ratingValue": 5,
      "comment": "Great service!",
      "createdDate": "2026-04-12T10:00:00Z",
      "status": "Active"
    }
  ],
  "pageNumber": 1,
  "pageSize": 10,
  "totalCount": 25,
  "totalPages": 3,
  "hasPreviousPage": false,
  "hasNextPage": true
}
```

### Response: 404 Not Found

```json
{
  "error": "Pet walker not found."
}
```

---

## GET /api/ratings/petwalker/{petWalkerId}/summary

Get rating summary for a petwalker (public endpoint).

### Response: 200 OK

```json
{
  "petWalkerId": "guid",
  "averageRating": 4.5,
  "totalRatings": 25
}
```

### Response: 404 Not Found

```json
{
  "error": "Pet walker not found."
}
```

---

## Eligibility Check Endpoint (Internal)

Used by CreateRatingHandler to verify client eligibility.

### Specification

A client is eligible to rate a petwalker if:
1. Client has at least one Booking with Status = Completed
2. Booking.PetWalkerId matches the target petwalker
3. Client has # ratings < # completed bookings with that petwalker

---

*Contracts created: 2026-04-12*