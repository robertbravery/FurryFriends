# Quickstart: Booking Cancellation

This quickstart guide outlines the steps to cancel a booking in the FurryFriends system.

## Prerequisites

-   A confirmed booking exists in the system.
-   The user initiating the cancellation is either the client or the pet walker associated with the booking.

## Steps

1.  **Client cancels a confirmed booking:**
    -   The client initiates the cancellation process through the user interface.
    -   The system prompts the client to select a cancellation reason.
    -   The system displays a confirmation message to the client.
    -   The booking status changes to "Cancelled".
    -   The client and pet walker receive a cancellation confirmation notification.

2.  **Pet walker cancels a confirmed booking:**
    -   The pet walker initiates the cancellation process through the user interface.
    -   The system prompts the pet walker to select a cancellation reason.
    -   The system displays a confirmation message to the pet walker.
    -   The booking status changes to "Cancelled".
    -   The client and pet walker receive a cancellation confirmation notification.

3.  **Attempt to cancel a completed booking:**
    -   The client or pet walker attempts to cancel a booking with status "Completed".
    -   The system prevents the cancellation and displays an error message indicating that completed bookings cannot be cancelled.

4.  **Attempt to cancel a cancelled booking:**
    -   The client or pet walker attempts to cancel a booking with status "Cancelled".
    -   The system prevents the cancellation and displays an error message indicating that cancelled bookings cannot be cancelled.