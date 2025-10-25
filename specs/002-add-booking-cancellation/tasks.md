# Feature Tasks: Booking Cancellation

This document outlines the tasks required to implement the Booking Cancellation feature, ordered by dependencies and indicating opportunities for parallel execution.

## Setup Tasks
*   [ ] No explicit setup tasks are required as the project and solution are already initialized.

## Phase 1: Core Backend Implementation

### Data Model & Persistence
*   [ ] **T001**: Update `Booking` entity in `src/FurryFriends.Core/Bookings/Booking.cs` to include a `Status` property and a reference to `Cancellation`.
*   [ ] **T002**: Create `Cancellation` entity in `src/FurryFriends.Core/Bookings/Cancellation.cs` with properties: `Id`, `BookingId`, `CancellationDate`, `Reason`, `CancelledBy`.
*   [ ] **T003**: Update `AuditLog` entity in `src/FurryFriends.Core/Audits/AuditLog.cs` to ensure it can capture cancellation-specific details.
*   [ ] **T004**: Create EF Core configuration for `Cancellation` in `src/FurryFriends.Infrastructure/Data/Config/CancellationConfiguration.cs`.
*   [ ] **T005**: Update EF Core configuration for `Booking` in `src/FurryFriends.Infrastructure/Data/Config/BookingConfiguration.cs` to include the `Cancellation` relationship.
*   [ ] **T006**: Create a new database migration to apply the changes to the `Booking`, `Cancellation`, and `AuditLog` entities.

### Use Cases (CQRS)
*   [ ] **T007**: Create `CancelBookingCommand` in `src/FurryFriends.UseCases/Domain/Bookings/Command/CancelBookingCommand.cs` with `BookingId` and `Reason`.
*   [ ] **T008**: Create `CancelBookingHandler` in `src/FurryFriends.UseCases/Domain/Bookings/Command/CancelBookingHandler.cs` to process the cancellation logic, update booking status, create audit log, and handle refund logic.
*   [ ] **T009**: Create `CancelBookingValidator` in `src/FurryFriends.UseCases/Domain/Bookings/Command/CancelBookingValidator.cs` to validate cancellation requests (e.g., booking status, user authorization, time window).

## Phase 2: API Endpoint Implementation

*   [ ] **T010**: Create `CancelBookingEndpoint` in `src/FurryFriends.Web/Endpoints/BookingEndpoints/Cancel/CancelBooking.cs` to expose the cancellation functionality via an API.

## Phase 3: Testing

### Contract Tests
*   [ ] **T011 [P]**: Verify `CancelBookingContractTests.cs` in `specs/002-add-booking-cancellation/contracts/CancelBookingContractTests.cs` passes against the implemented contract.

### Unit Tests
*   [ ] **T012 [P]**: Write unit tests for `CancelBookingCommand` and `CancelBookingHandler` in `tests/FurryFriends.UnitTests/FurrFriends.UnitTests/UseCase/Bookings/CancelBookingTests.cs`.
*   [ ] **T013 [P]**: Write unit tests for `CancelBookingValidator` in `tests/FurryFriends.UnitTests/FurrFriends.UnitTests/UseCase/Bookings/CancelBookingValidatorTests.cs`.

### Integration Tests
*   [ ] **T014**: Write integration tests for the `CancelBookingEndpoint` in `tests/FurryFriends.FunctionalTests/ApiEndpoints/Bookings/CancelBookingTests.cs` to cover various scenarios (successful cancellation, invalid status, unauthorized user, concurrent requests).

## Phase 4: UI Implementation (Blazor)

*   [ ] **T015**: Update the relevant Blazor page (e.g., `src/FurryFriends.BlazorUI/Components/Pages/Bookings/BookingDetails.razor`) to include a cancellation button and handle user interaction.
*   [ ] **T016**: Implement client-side logic in `src/FurryFriends.BlazorUI/Services/Implementation/BookingService.cs` to call the `CancelBookingEndpoint`.
*   [ ] **T017**: Implement UI feedback mechanisms (toast messages, confirmation dialogs) for cancellation success or failure.

## Phase 5: Polish & Documentation

*   [ ] **T018**: Update `docs/FurryFriends_Technical_Guide.md` with details on the new cancellation functionality.
*   [ ] **T019**: Review and update `specs/002-add-booking-cancellation/spec.md` to reflect any final decisions or changes during implementation.

## Parallel Execution Opportunities

The following tasks can be executed in parallel:
*   **T011, T012, T013** (Contract and Unit Tests)