# Research Findings: Booking Cancellation

## Performance Goals

- Decision: Aim for <200ms API response time and 60fps UI.
- Rationale: This aligns with the existing performance goals for the FurryFriends application.
- Alternatives considered: None.

## Constraints

- Decision: The booking cancellation functionality must work with the existing Client and PetWalker aggregates.
- Rationale: This ensures that the new functionality integrates seamlessly with the existing system.
- Alternatives considered: Creating new aggregates was considered, but rejected due to the potential for data duplication and inconsistency.

## Scale/Scope

- Decision: The booking cancellation functionality will affect the Booking aggregate, introduce 2 new endpoints, and require updates to 1 Blazor page.
- Rationale: This scope is manageable and allows for focused development efforts.
- Alternatives considered: Expanding the scope to include other aggregates was considered, but rejected due to the increased complexity and potential for delays.

## Concurrent Cancellation Handling

- Decision: Implement a pessimistic locking mechanism using EF Core to prevent race conditions during concurrent cancellation requests.
- Rationale: Pessimistic locking ensures that only one request can modify the booking at a time, preventing data corruption.
- Alternatives considered: Optimistic locking was considered, but rejected due to the potential for frequent conflicts and retries.

## Cancellation Reason

- Decision: Provide a predefined list of reasons for cancellation.
- Rationale: This ensures data consistency and simplifies reporting.
- Alternatives considered: Allowing free text was considered, but rejected due to the potential for inconsistent data and difficulties in analysis.