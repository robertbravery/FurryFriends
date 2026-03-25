# Quickstart Guide: Petwalker Rating System

This document provides steps to validate the Petwalker Rating System implementation.

## Prerequisites

- .NET 9 SDK
- SQL Server (local or containerized)
- FurryFriends solution builds successfully

## Database Setup

1. Ensure database is created:
```bash
dotnet ef database update
```

2. Verify new columns exist in Ratings table:
- `BookingId` (uniqueidentifier, NOT NULL)
- `ModifiedDate` (datetime2, NULL)

## Build Verification

1. Build the solution:
```bash
dotnet build
```

2. Verify no compilation errors in:
- `FurryFriends.Core` (Rating entity)
- `FurryFriends.UseCases` (Commands/Queries/Handlers)
- `FurryFriends.Web` (API Endpoints)
- `FurryFriends.BlazorUI` (UI Components)

## API Testing

### 1. Create a Rating

**Request:**
```bash
POST /api/ratings
Content-Type: application/json

{
  "bookingId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "ratingValue": 5,
  "comment": "Excellent service!"
}
```

**Expected Response:** 201 Created with rating details

### 2. Update a Rating

**Request:**
```bash
PUT /api/ratings/{ratingId}
Content-Type: application/json

{
  "ratingValue": 4,
  "comment": "Updated comment"
}
```

**Expected Response:** 200 OK with updated rating

**Error Case (after 7 days):** 403 Forbidden

### 3. Get Ratings for PetWalker

**Request:**
```bash
GET /api/petwalkers/{petWalkerId}/ratings?page=1&pageSize=20
```

**Expected Response:** 200 OK with paginated list of ratings

### 4. Get Rating Summary

**Request:**
```bash
GET /api/petwalkers/{petWalkerId}/ratings/summary
```

**Expected Response:**
```json
{
  "petWalkerId": "...",
  "averageRating": 4.5,
  "totalRatings": 50,
  "recentRatings": [...]
}
```

## Functional Test Scenarios

### Scenario 1: Client rates completed booking
1. Create a completed booking
2. Submit rating via POST /api/ratings
3. Verify rating created with correct BookingId

**Success Criteria:**
- Rating exists in database
- BookingId is linked correctly
- One rating per booking enforced

### Scenario 2: Petwalker views ratings
1. Login as petwalker
2. Navigate to ratings dashboard
3. Verify average rating displayed
4. Verify list of ratings visible

**Success Criteria:**
- Average rating calculated correctly
- Ratings ordered by date descending
- Client names visible

### Scenario 3: Client updates rating within 7 days
1. Create rating
2. Update within 7 days
3. Verify ModifiedDate set

**Success Criteria:**
- RatingValue updated
- ModifiedDate is not null
- Can only update once

### Scenario 4: Client cannot update after 7 days
1. Create rating (or mock created date > 7 days ago)
2. Attempt update

**Expected:** 403 Forbidden

### Scenario 5: Cannot rate same booking twice
1. Submit rating for booking
2. Submit another rating for same booking

**Expected:** 409 Conflict

## User Interface Validation

### Rating Submission (Client)
1. Complete a booking
2. See rating prompt
3. Select star rating (1-5)
4. Optionally add comment
5. Submit

**Success Criteria:**
- Stars are clickable
- Comment is optional
- Confirmation shown after submission

### Rating Dashboard (Petwalker)
1. Login as petwalker
2. Navigate to ratings page
3. View average rating
4. View list of ratings

**Success Criteria:**
- Average displayed prominently
- Total count visible
- Each rating shows date and client name

## Edge Cases to Test

| Scenario | Expected |
|----------|----------|
| No ratings yet | "No ratings yet" message displayed |
| Invalid rating value | Validation error (400) |
| Rating for non-existent booking | 404 Not Found |
| Rating for cancelled booking | 400 Validation Error |
| Empty comment | Allowed (comment is optional) |
| Very long comment | 400 Validation Error (>1000 chars) |

## Performance Targets

- API response time: < 200ms
- Rating submission: < 500ms (including DB write)
- Summary calculation: < 300ms

---

**Date**: 2026-03-19
