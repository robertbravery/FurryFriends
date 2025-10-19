# Data Model: Booking Cancellation

## Entities

### Booking

-   **Id**: Unique identifier (GUID)
-   **ClientId**: ID of the client associated with the booking (GUID)
-   **PetWalkerId**: ID of the pet walker associated with the booking (GUID)
-   **BookingDate**: Date of the booking (DateTime)
-   **Status**: Current status of the booking (Enum: Confirmed, Cancelled, Completed)
-   **Cancellation**: Cancellation details (see Cancellation entity)

### Cancellation

-   **Id**: Unique identifier (GUID)
-   **BookingId**: ID of the booking being cancelled (GUID)
-   **CancellationDate**: Date of the cancellation (DateTime)
-   **Reason**: Reason for cancellation (Enum: ClientRequest, PetWalkerRequest, Other)
-   **CancelledBy**: User who initiated the cancellation (Enum: Client, PetWalker)

### AuditLog

-   **Id**: Unique identifier (GUID)
-   **BookingId**: ID of the booking being audited (GUID)
-   **Timestamp**: Timestamp of the event (DateTime)
-   **EventType**: Type of event (Enum: Created, Updated, Cancelled)
-   **User**: User associated with the event (Enum: Client, PetWalker, System)
-   **Details**: Additional details about the event (String)

## Relationships

-   A Booking can have one Cancellation.
-   A Booking can have multiple AuditLog entries.

## Validation Rules

-   A booking can only be cancelled if its status is "Confirmed".
-   The cancellation reason must be selected from a predefined list.
-   The user initiating the cancellation must be either the client or the pet walker associated with the booking.