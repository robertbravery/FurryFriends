# Data Model

## Entities

### PetWalker
- ID (Guid)
- Name
- Availability (Timeslot schedule)
- Ratings (list of numeric reviews)
- Photos (list of image URLs)
- Status (Active/Inactive/Verified)

### Client
- ID (Guid)
- Name
- Pets (list of pet details)
- BookingHistory (list of booking IDs)
- Subscription (active/deleted)

### Booking
- ID (Guid)
- ClientID
- PetWalkerID
- DateTime
- Status (Pending/Confirmed/Completed/Cancelled)
- Notes

## Relationships
- Many-to-Many: Clients <-> Pets
- Many-to-One: Bookings <-> Clients, Bookings <-> PetWalkers

## Validation Rules
- All required fields must be non-null and non-empty
- Ratings must be between 1-5
- Status must be one of defined enum

## State Transitions
- Pending -> Confirmed (when accepted)
- Confirmed -> Completed (when delivered)
- Active -> Inactive (if cancellation or deactivation)