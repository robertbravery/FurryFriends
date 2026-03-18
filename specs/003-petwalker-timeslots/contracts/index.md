# API Contracts: Petwalker Timeslots

**Feature**: 003-petwalker-timeslots  
**Date**: 2026-03-14

---

## Overview

This directory contains API contract definitions for the Petwalker Timeslots feature. All contracts follow the FastEndpoints pattern with separate Request, Response, and Endpoint files.

---

## Contracts

### 1. [CreateTimeslotContract.md](CreateTimeslotContract.md)

| Property | Value |
|----------|-------|
| Method | POST |
| Route | `/api/timeslots` |
| Purpose | Petwalker creates a new available timeslot |

**Key Features**:
- Validates duration (30-45 minutes)
- Checks working hours alignment
- Prevents overlapping slots

---

### 2. [GetTimeslotsContract.md](GetTimeslotsContract.md)

| Property | Value |
|----------|-------|
| Method | GET |
| Route | `/api/petwalkers/{PETWALKERID}/timeslots` |
| Purpose | Petwalker views their own timeslots |

**Key Features**:
- Filter by date
- Filter by status
- Paginated results

---

### 3. [GetAvailableTimeslotsContract.md](GetAvailableTimeslotsContract.md)

| Property | Value |
|----------|-------|
| Method | GET |
| Route | `/api/petwalkers/{PETWALKERID}/timeslots/available` |
| Purpose | Client views available timeslots for booking |

**Key Features**:
- Returns only Available slots
- Includes travel buffer warnings
- No authentication required

---

### 4. [UpdateTimeslotContract.md](UpdateTimeslotContract.md)

| Property | Value |
|----------|-------|
| Method | PUT |
| Route | `/api/timeslots/{TIMESLOTID}` |
| Purpose | Petwalker modifies an existing timeslot |

**Key Features**:
- Only Available slots can be modified
- Validates new time doesn't overlap
- Checks working hours alignment

---

### 5. [DeleteTimeslotContract.md](DeleteTimeslotContract.md)

| Property | Value |
|----------|-------|
| Method | DELETE |
| Route | `/api/timeslots/{TIMESLOTID}` |
| Purpose | Petwalker deletes a timeslot |

**Key Features**:
- Only Available/Cancelled slots can be deleted
- Booked slots cannot be deleted (would break booking)

---

### 6. [WorkingHoursContract.md](WorkingHoursContract.md)

| Property | Value |
|----------|-------|
| Methods | GET, POST, PUT, DELETE |
| Routes | `/api/petwalkers/{PETWALKERID}/workinghours`, `/api/workinghours/{WORKINGHOURSID}` |
| Purpose | Manage petwalker's working schedule |

**Key Features**:
- Multiple shifts per day supported
- Validates no overlapping shifts
- Timeslots must fall within working hours

---

### 7. [BookTimeslotContract.md](BookTimeslotContract.md)

| Property | Value |
|----------|-------|
| Method | POST |
| Route | `/api/timeslots/{TIMESLOTID}/book` |
| Purpose | Client books an available timeslot |

**Key Features**:
- Atomic booking with status update
- Travel buffer calculation for different addresses
- Creates TravelBuffer entity when needed

---

## Endpoint Summary

| # | Endpoint | Method | Auth | Description |
|---|----------|--------|------|-------------|
| 1 | `/api/timeslots` | POST | PetWalker | Create timeslot |
| 2 | `/api/petwalkers/{id}/timeslots` | GET | PetWalker | Get own timeslots |
| 3 | `/api/petwalkers/{id}/timeslots/available` | GET | Public | Get available slots |
| 4 | `/api/timeslots/{id}` | PUT | PetWalker | Update timeslot |
| 5 | `/api/timeslots/{id}` | DELETE | PetWalker | Delete timeslot |
| 6 | `/api/petwalkers/{id}/workinghours` | GET/POST | PetWalker | Manage working hours |
| 7 | `/api/workinghours/{id}` | PUT/DELETE | PetWalker | Update/delete hours |
| 8 | `/api/timeslots/{id}/book` | POST | Client | Book a timeslot |

---

## Common Patterns

All contracts follow these patterns per the FurryFriends Constitution:

1. **Request/Response Separation**: Each operation has dedicated request and response DTOs
2. **Result Pattern**: Handlers return `Result<T>` or `Result`
3. **Validation**: FluentValidation validators for all requests
4. **Specifications**: Ardalis.Specification for database queries
5. **Serilog**: Structured logging in all handlers

---

## Testing

Each contract should have corresponding tests:

- **Contract Tests**: Validate request/response schemas
- **Unit Tests**: Test handlers, validators, specifications
- **Integration Tests**: Test API endpoints end-to-end
