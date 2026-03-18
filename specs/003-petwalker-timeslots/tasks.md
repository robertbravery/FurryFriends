# Feature Tasks: Petwalker Timeslots

**Feature Branch**: `003-petwalker-timeslots`
**Date**: 2026-03-14
**Spec**: [specs/003-petwalker-timeslots/spec.md](specs/003-petwalker-timeslots/spec.md)

This document outlines the tasks required to implement the Petwalker Timeslots feature, ordered by dependencies and following TDD principles (tests before implementation). Tasks are organized by user story and then by layer (test → domain → use case → infrastructure → API → UI).

---

## Phase 1: Setup (Project Initialization)

- [x] **T001** `[P]` Verify solution builds correctly with `dotnet build` and all existing tests pass

---

## Phase 2: Foundational (Blocking Prerequisites)

### Core Domain Entities (Test-First)

- [x] **T002** `[P]` Write unit tests for `TimeslotStatus` enum in `tests/FurryFriends.UnitTests/FurrFriends.UnitTests/Core/TimeslotAggregate/TimeslotStatusTests.cs`
- [x] **T003** `[P]` Write unit tests for `CustomTimeRequestStatus` enum in `tests/FurryFriends.UnitTests/FurrFriends.UnitTests/Core/TimeslotAggregate/CustomTimeRequestStatusTests.cs`
- [x] **T004** `[P]` Write unit tests for `Timeslot` entity creation and validation in `tests/FurryFriends.UnitTests/FurrFriends.UnitTests/Core/TimeslotAggregate/TimeslotTests.cs`
- [x] **T005** `[P]` Write unit tests for `WorkingHours` entity creation and validation in `tests/FurryFriends.UnitTests/FurrFriends.UnitTests/Core/TimeslotAggregate/WorkingHoursTests.cs`
- [x] **T006** `[P]` Write unit tests for `TravelBuffer` entity creation in `tests/FurryFriends.UnitTests/FurrFriends.UnitTests/Core/TimeslotAggregate/TravelBufferTests.cs`
- [x] **T007** `[P]` Write unit tests for `CustomTimeRequest` entity creation in `tests/FurryFriends.UnitTests/FurrFriends.UnitTests/Core/TimeslotAggregate/CustomTimeRequestTests.cs`

### Domain Implementation

- [x] **T008** `[P]` Create `Timeslot` entity in `src/FurryFriends.Core/TimeslotAggregate/Timeslot.cs` with properties: Id, PetWalkerId, Date, StartTime, EndTime, DurationInMinutes, Status, CreatedAt, UpdatedAt
- [x] **T009** `[P]` Create `TimeslotStatus` enum in `src/FurryFriends.Core/TimeslotAggregate/Enums/TimeslotStatus.cs`
- [x] **T010** `[P]` Create `Timeslot` Specifications in `src/FurryFriends.Core/TimeslotAggregate/Specifications/TimeslotSpecifications.cs`:
  - `AvailableTimeslotsByPetWalkerAndDateSpecification`
  - `OverlappingTimeslotsSpecification`
  - `TimeslotsByPetWalkerAndDateSpecification`
- [x] **T011** `[P]` Create `WorkingHours` entity in `src/FurryFriends.Core/TimeslotAggregate/WorkingHours.cs` with properties: Id, PetWalkerId, DayOfWeek, StartTime, EndTime, IsActive
- [x] **T012** `[P]` Create `WorkingHours` Specifications in `src/FurryFriends.Core/TimeslotAggregate/Specifications/WorkingHoursSpecifications.cs`:
  - `WorkingHoursByPetWalkerAndDaySpecification`
- [x] **T013** `[P]` Create `TravelBuffer` entity in `src/FurryFriends.Core/TimeslotAggregate/TravelBuffer.cs` with properties: Id, BookingId, PreviousBookingId, OriginAddress, DestinationAddress, BufferDurationMinutes, StartTime, EndTime, CreatedAt
- [x] **T014** `[P]` Create `CustomTimeRequest` entity in `src/FurryFriends.Core/TimeslotAggregate/CustomTimeRequest.cs` with properties: Id, ClientId, PetWalkerId, RequestedDate, PreferredStartTime, PreferredEndTime, PreferredDurationMinutes, Status, ClientAddress, CounterOfferedTime, CounterOfferedDate, ResponseReason, CreatedAt, UpdatedAt
- [x] **T015** `[P]` Create `CustomTimeRequestStatus` enum in `src/FurryFriends.Core/TimeslotAggregate/Enums/CustomTimeRequestStatus.cs`
- [x] **T016** `[P]` Create `CustomTimeRequest` Specifications in `src/FurryFriends.Core/TimeslotAggregate/Specifications/CustomTimeRequestSpecifications.cs`:
  - `PendingCustomTimeRequestsByPetWalkerSpecification`
  - `CustomTimeRequestsByClientSpecification`

### Infrastructure (Database)

- [x] **T017** Create EF Core configuration for `Timeslot` in `src/FurryFriends.Infrastructure/Data/Config/TimeslotConfiguration.cs`
- [x] **T018** Create EF Core configuration for `WorkingHours` in `src/FurryFriends.Infrastructure/Data/Config/WorkingHoursConfiguration.cs`
- [x] **T019** Create EF Core configuration for `TravelBuffer` in `src/FurryFriends.Infrastructure/Data/Config/TravelBufferConfiguration.cs`
- [x] **T020** Create EF Core configuration for `CustomTimeRequest` in `src/FurryFriends.Infrastructure/Data/Config/CustomTimeRequestConfiguration.cs`
- [x] **T021** Update `Booking` entity in `src/FurryFriends.Core/BookingAggregate/Booking.cs` to add TimeslotId and ClientAddress properties
- [x] **T022** Create database migration for new tables: Timeslots, WorkingHours, TravelBuffers, CustomTimeRequests, and Booking updates

---

## Phase 3: US1 - Petwalker Defines Weekly Working Hours

### Use Cases (CQRS) - Test-First

- [x] **T023** `[P]` Write unit tests for `CreateWorkingHoursCommand` handler in `tests/FurryFriends.UnitTests/FurrFriends.UnitTests/UseCase/Timeslots/CreateWorkingHoursTests.cs`
- [x] **T024** `[P]` Write unit tests for `CreateWorkingHoursValidator` in `tests/FurryFriends.UnitTests/FurrFriends.UnitTests/UseCase/Timeslots/CreateWorkingHoursValidatorTests.cs`
- [x] **T025** `[P]` Write unit tests for `GetWorkingHoursQuery` handler in `tests/FurryFriends.UnitTests/FurrFriends.UnitTests/UseCase/Timeslots/GetWorkingHoursTests.cs`
- [x] **T026** `[P]` Write unit tests for `UpdateWorkingHoursCommand` handler in `tests/FurryFriends.UnitTests/FurrFriends.UnitTests/UseCase/Timeslots/UpdateWorkingHoursTests.cs`
- [x] **T027** `[P]` Write unit tests for `DeleteWorkingHoursCommand` handler in `tests/FurryFriends.UnitTests/FurrFriends.UnitTests/UseCase/Timeslots/DeleteWorkingHoursTests.cs`

### Use Cases Implementation

- [x] **T028** Create `CreateWorkingHoursCommand` in `src/FurryFriends.UseCases/Timeslots/WorkingHours/CreateWorkingHoursCommand.cs`
- [x] **T029** Create `CreateWorkingHoursHandler` in `src/FurryFriends.UseCases/Timeslots/WorkingHours/CreateWorkingHoursHandler.cs` with validation: EndTime > StartTime, no overlapping shifts
- [x] **T030** Create `CreateWorkingHoursValidator` in `src/FurryFriends.UseCases/Timeslots/WorkingHours/CreateWorkingHoursValidator.cs`
- [x] **T031** Create `GetWorkingHoursQuery` in `src/FurryFriends.UseCases/Timeslots/WorkingHours/GetWorkingHoursQuery.cs`
- [x] **T032** Create `GetWorkingHoursHandler` in `src/FurryFriends.UseCases/Timeslots/WorkingHours/GetWorkingHoursHandler.cs`
- [x] **T033** Create `GetWorkingHoursDto` in `src/FurryFriends.UseCases/Timeslots/WorkingHours/WorkingHoursDto.cs`
- [x] **T034** Create `UpdateWorkingHoursCommand` in `src/FurryFriends.UseCases/Timeslots/WorkingHours/UpdateWorkingHoursCommand.cs`
- [x] **T035** Create `UpdateWorkingHoursHandler` in `src/FurryFriends.UseCases/Timeslots/WorkingHours/UpdateWorkingHoursHandler.cs`
- [x] **T036** Create `UpdateWorkingHoursValidator` in `src/FurryFriends.UseCases/Timeslots/WorkingHours/UpdateWorkingHoursValidator.cs`
- [x] **T037** Create `DeleteWorkingHoursCommand` in `src/FurryFriends.UseCases/Timeslots/WorkingHours/DeleteWorkingHoursCommand.cs`
- [x] **T038** Create `DeleteWorkingHoursHandler` in `src/FurryFriends.UseCases/Timeslots/WorkingHours/DeleteWorkingHoursHandler.cs`

### API Endpoints

- [x] **T039** Create `CreateWorkingHoursEndpoint` in `src/FurryFriends.Web/Endpoints/TimeslotEndpoints/WorkingHours/CreateWorkingHours/CreateWorkingHours.cs`
- [x] **T040** Create `CreateWorkingHoursRequest` in `src/FurryFriends.Web/Endpoints/TimeslotEndpoints/WorkingHours/CreateWorkingHours/CreateWorkingHoursRequest.cs`
- [x] **T041** Create `CreateWorkingHoursResponse` in `src/FurryFriends.Web/Endpoints/TimeslotEndpoints/WorkingHours/CreateWorkingHours/CreateWorkingHoursResponse.cs`
- [x] **T042** Create `CreateWorkingHoursValidator` in `src/FurryFriends.Web/Endpoints/TimeslotEndpoints/WorkingHours/CreateWorkingHours/CreateWorkingHoursValidator.cs`
- [x] **T043** Create `GetWorkingHoursEndpoint` in `src/FurryFriends.Web/Endpoints/TimeslotEndpoints/WorkingHours/GetWorkingHours/GetWorkingHours.cs`
- [x] **T044** Create `GetWorkingHoursRequest` in `src/FurryFriends.Web/Endpoints/TimeslotEndpoints/WorkingHours/GetWorkingHours/GetWorkingHoursRequest.cs`
- [x] **T045** Create `GetWorkingHoursResponse` in `src/FurryFriends.Web/Endpoints/TimeslotEndpoints/WorkingHours/GetWorkingHours/GetWorkingHoursResponse.cs`
- [x] **T046** Create `UpdateWorkingHoursEndpoint` in `src/FurryFriends.Web/Endpoints/TimeslotEndpoints/WorkingHours/UpdateWorkingHours/UpdateWorkingHours.cs`
- [x] **T047** Create `UpdateWorkingHoursRequest` in `src/FurryFriends.Web/Endpoints/TimeslotEndpoints/WorkingHours/UpdateWorkingHours/UpdateWorkingHoursRequest.cs`
- [x] **T048** Create `UpdateWorkingHoursResponse` in `src/FurryFriends.Web/Endpoints/TimeslotEndpoints/WorkingHours/UpdateWorkingHours/UpdateWorkingHoursResponse.cs`
- [x] **T049** Create `UpdateWorkingHoursValidator` in `src/FurryFriends.Web/Endpoints/TimeslotEndpoints/WorkingHours/UpdateWorkingHours/UpdateWorkingHoursValidator.cs`
- [x] **T050** Create `DeleteWorkingHoursEndpoint` in `src/FurryFriends.Web/Endpoints/TimeslotEndpoints/WorkingHours/DeleteWorkingHours/DeleteWorkingHours.cs`
- [x] **T051** Create `DeleteWorkingHoursRequest` in `src/FurryFriends.Web/Endpoints/TimeslotEndpoints/WorkingHours/DeleteWorkingHours/DeleteWorkingHoursRequest.cs`

### Integration Tests

- [x] **T052** Write integration tests for `CreateWorkingHoursEndpoint` in `tests/FurryFriends.FunctionalTests/ApiEndpoints/Timeslots/WorkingHours/CreateWorkingHoursTests.cs`
- [x] **T053** Write integration tests for `GetWorkingHoursEndpoint` in `tests/FurryFriends.FunctionalTests/ApiEndpoints/Timeslots/WorkingHours/GetWorkingHoursTests.cs`
- [x] **T054** Write integration tests for `UpdateWorkingHoursEndpoint` in `tests/FurryFriends.FunctionalTests/ApiEndpoints/Timeslots/WorkingHours/UpdateWorkingHoursTests.cs`
- [x] **T055** Write integration tests for `DeleteWorkingHoursEndpoint` in `tests/FurryFriends.FunctionalTests/ApiEndpoints/Timeslots/WorkingHours/DeleteWorkingHoursTests.cs`

---

## Phase 4: US2 - Petwalker Creates Timeslots Within Working Hours

### Use Cases (CQRS) - Test-First

- [x] **T056** `[P]` Write unit tests for `CreateTimeslotCommand` handler in `tests/FurryFriends.UnitTests/FurrFriends.UnitTests/UseCase/Timeslots/CreateTimeslotTests.cs`
- [x] **T057** `[P]` Write unit tests for `CreateTimeslotValidator` in `tests/FurryFriends.UnitTests/FurrFriends.UnitTests/UseCase/Timeslots/CreateTimeslotValidatorTests.cs`
- [x] **T058** `[P]` Write unit tests for overlap detection logic in `tests/FurryFriends.UnitTests/FurrFriends.UnitTests/Core/TimeslotAggregate/TimeslotOverlapTests.cs`

### Use Cases Implementation

- [x] **T059** Create `CreateTimeslotCommand` in `src/FurryFriends.UseCases/Timeslots/Timeslot/CreateTimeslotCommand.cs`
- [x] **T060** Create `CreateTimeslotHandler` in `src/FurryFriends.UseCases/Timeslots/Timeslot/CreateTimeslotHandler.cs` with validations:
  - Duration 30-45 minutes
  - Within working hours
  - No overlapping slots
- [x] **T061** Create `CreateTimeslotValidator` in `src/FurryFriends.UseCases/Timeslots/Timeslot/CreateTimeslotValidator.cs`
- [x] **T062** Create `CreateTimeslotDto` in `src/FurryFriends.UseCases/Timeslots/Timeslot/TimeslotDto.cs`

### API Endpoints

- [x] **T063** Create `CreateTimeslotEndpoint` in `src/FurryFriends.Web/Endpoints/TimeslotEndpoints/Timeslot/CreateTimeslot/CreateTimeslot.cs`
- [x] **T064** Create `CreateTimeslotRequest` in `src/FurryFriends.Web/Endpoints/TimeslotEndpoints/Timeslot/CreateTimeslot/CreateTimeslotRequest.cs`
- [x] **T065** Create `CreateTimeslotResponse` in `src/FurryFriends.Web/Endpoints/TimeslotEndpoints/Timeslot/CreateTimeslot/CreateTimeslotResponse.cs`
- [x] **T066** Create `CreateTimeslotValidator` in `src/FurryFriends.Web/Endpoints/TimeslotEndpoints/Timeslot/CreateTimeslot/CreateTimeslotValidator.cs`

### Integration Tests

- [x] **T067** Write integration tests for `CreateTimeslotEndpoint` in `tests/FurryFriends.FunctionalTests/ApiEndpoints/Timeslots/Timeslot/CreateTimeslotTests.cs`

---

## Phase 5: US3 - Client Views Available Timeslots

### Use Cases (CQRS) - Test-First

- [x] **T068** `[P]` Write unit tests for `GetAvailableTimeslotsQuery` handler in `tests/FurryFriends.UnitTests/FurrFriends.UnitTests/UseCase/Timeslots/GetAvailableTimeslotsTests.cs`

### Use Cases Implementation

- [x] **T069** Create `GetAvailableTimeslotsQuery` in `src/FurryFriends.UseCases/Timeslots/Timeslot/GetAvailableTimeslotsQuery.cs`
- [x] **T070** Create `GetAvailableTimeslotsHandler` in `src/FurryFriends.UseCases/Timeslots/Timeslot/GetAvailableTimeslotsHandler.cs`
- [x] **T071** Create `GetAvailableTimeslotsDto` in `src/FurryFriends.UseCases/Timeslots/Timeslot/AvailableTimeslotDto.cs`

### API Endpoints

- [x] **T072** Create `GetAvailableTimeslotsEndpoint` in `src/FurryFriends.Web/Endpoints/TimeslotEndpoints/Timeslot/GetAvailableTimeslots/GetAvailableTimeslots.cs`
- [x] **T073** Create `GetAvailableTimeslotsRequest` in `src/FurryFriends.Web/Endpoints/TimeslotEndpoints/Timeslot/GetAvailableTimeslots/GetAvailableTimeslotsRequest.cs`
- [x] **T074** Create `GetAvailableTimeslotsResponse` in `src/FurryFriends.Web/Endpoints/TimeslotEndpoints/Timeslot/GetAvailableTimeslots/GetAvailableTimeslotsResponse.cs`

### Integration Tests

- [x] **T075** Write integration tests for `GetAvailableTimeslotsEndpoint` in `tests/FurryFriends.FunctionalTests/ApiEndpoints/Timeslots/Timeslot/GetAvailableTimeslotsTests.cs`

---

## Phase 6: US4 - Client Books a Timeslot

### Use Cases (CQRS) - Test-First

- [x] **T076** `[P]` Write unit tests for `BookTimeslotCommand` handler in `tests/FurryFriends.UnitTests/FurrFriends.UnitTests/UseCase/Timeslots/BookTimeslotTests.cs`
- [x] **T077** `[P]` Write unit tests for `BookTimeslotValidator` in `tests/FurryFriends.UnitTests/FurrFriends.UnitTests/UseCase/Timeslots/BookTimeslotValidatorTests.cs`
- [x] **T078** `[P]` Write unit tests for travel buffer calculation in `tests/FurryFriends.UnitTests/FurrFriends.UnitTests/UseCase/Timeslots/TravelBufferCalculationTests.cs`

### Use Cases Implementation

- [x] **T079** Create `BookTimeslotCommand` in `src/FurryFriends.UseCases/Timeslots/Booking/BookTimeslotCommand.cs`
- [x] **T080** Create `BookTimeslotHandler` in `src/FurryFriends.UseCases/Timeslots/Booking/BookTimeslotHandler.cs` with:
  - Atomic booking and status update
  - Travel buffer calculation
  - Creates TravelBuffer entity when addresses differ
- [x] **T081** Create `BookTimeslotValidator` in `src/FurryFriends.UseCases/Timeslots/Booking/BookTimeslotValidator.cs`
- [x] **T082** Create `BookTimeslotDto` in `src/FurryFriends.UseCases/Timeslots/Booking/BookTimeslotDto.cs`
- [x] **T083** Implement travel buffer calculation logic in `src/FurryFriends.UseCases/Timeslots/Booking/TravelBufferCalculator.cs`

### API Endpoints

- [x] **T084** Create `BookTimeslotEndpoint` in `src/FurryFriends.Web/Endpoints/TimeslotEndpoints/Booking/BookTimeslot/BookTimeslot.cs`
- [x] **T085** Create `BookTimeslotRequest` in `src/FurryFriends.Web/Endpoints/TimeslotEndpoints/Booking/BookTimeslot/BookTimeslotRequest.cs`
- [x] **T086** Create `BookTimeslotResponse` in `src/FurryFriends.Web/Endpoints/TimeslotEndpoints/Booking/BookTimeslot/BookTimeslotResponse.cs`
- [x] **T087** Create `BookTimeslotValidator` in `src/FurryFriends.Web/Endpoints/TimeslotEndpoints/Booking/BookTimeslot/BookTimeslotValidator.cs`

### Integration Tests

- [x] **T088** Write integration tests for `BookTimeslotEndpoint` in `tests/FurryFriends.FunctionalTests/ApiEndpoints/Timeslots/Booking/BookTimeslotTests.cs`

---

## Phase 7: US5 - Client Requests Custom Time

### Use Cases (CQRS) - Test-First

- [x] **T089** `[P]` Write unit tests for `RequestCustomTimeCommand` handler in `tests/FurryFriends.UnitTests/FurrFriends.UnitTests/UseCase/Timeslots/RequestCustomTimeTests.cs`
- [x] **T090** `[P]` Write unit tests for `RequestCustomTimeValidator` in `tests/FurryFriends.UnitTests/FurrFriends.UnitTests/UseCase/Timeslots/RequestCustomTimeValidatorTests.cs`
- [x] **T091** `[P]` Write unit tests for custom time request response handlers in `tests/FurryFriends.UnitTests/FurrFriends.UnitTests/UseCase/Timeslots/RespondToCustomTimeRequestTests.cs`

### Use Cases Implementation

- [x] **T092** Create `RequestCustomTimeCommand` in `src/FurryFriends.UseCases/Timeslots/CustomTimeRequest/RequestCustomTimeCommand.cs`
- [x] **T093** Create `RequestCustomTimeHandler` in `src/FurryFriends.UseCases/Timeslots/CustomTimeRequest/RequestCustomTimeHandler.cs` with: validate petwalker exists, check no duplicate pending requests, create CustomTimeRequest with Pending status
- [x] **T094** Create `RequestCustomTimeValidator` in `src/FurryFriends.UseCases/Timeslots/CustomTimeRequest/RequestCustomTimeValidator.cs`
- [x] **T095** Create `RequestCustomTimeDto` in `src/FurryFriends.UseCases/Timeslots/CustomTimeRequest/CustomTimeRequestDto.cs`
- [x] **T096** Create `RespondToCustomTimeRequestCommand` in `src/FurryFriends.UseCases/Timeslots/CustomTimeRequest/RespondToCustomTimeRequestCommand.cs`
- [x] **T097** Create `RespondToCustomTimeRequestHandler` in `src/FurryFriends.UseCases/Timeslots/CustomTimeRequest/RespondToCustomTimeRequestHandler.cs` for Accept/Decline/Counter-Offer
- [x] **T098** Create `RespondToCustomTimeRequestValidator` in `src/FurryFriends.UseCases/Timeslots/CustomTimeRequest/RespondToCustomTimeRequestValidator.cs`

### API Endpoints

- [x] **T099** Create `RequestCustomTimeEndpoint` in `src/FurryFriends.Web/Endpoints/TimeslotEndpoints/CustomTimeRequest/RequestCustomTime/RequestCustomTime.cs`
- [x] **T100** Create `RequestCustomTimeRequest` in `src/FurryFriends.Web/Endpoints/TimeslotEndpoints/CustomTimeRequest/RequestCustomTime/RequestCustomTimeRequest.cs`
- [x] **T101** Create `RequestCustomTimeResponse` in `src/FurryFriends.Web/Endpoints/TimeslotEndpoints/CustomTimeRequest/RequestCustomTime/RequestCustomTimeResponse.cs`
- [x] **T102** Create `RequestCustomTimeValidator` in `src/FurryFriends.Web/Endpoints/TimeslotEndpoints/CustomTimeRequest/RequestCustomTime/RequestCustomTimeValidator.cs`
- [x] **T103** Create `RespondToCustomTimeRequestEndpoint` in `src/FurryFriends.Web/Endpoints/TimeslotEndpoints/CustomTimeRequest/RespondToCustomTimeRequest/RespondToCustomTimeRequest.cs`
- [x] **T104** Create `RespondToCustomTimeRequestRequest` in `src/FurryFriends.Web/Endpoints/TimeslotEndpoints/CustomTimeRequest/RespondToCustomTimeRequest/RespondToCustomTimeRequestRequest.cs`
- [x] **T105** Create `RespondToCustomTimeRequestResponse` in `src/FurryFriends.Web/Endpoints/TimeslotEndpoints/CustomTimeRequest/RespondToCustomTimeRequest/RespondToCustomTimeRequestResponse.cs`
- [x] **T106** Create `RespondToCustomTimeRequestValidator` in `src/FurryFriends.Web/Endpoints/TimeslotEndpoints/CustomTimeRequest/RespondToCustomTimeRequest/RespondToCustomTimeRequestValidator.cs`

### Integration Tests

- [x] **T107** Write integration tests for `RequestCustomTimeEndpoint` in `tests/FurryFriends.FunctionalTests/ApiEndpoints/Timeslots/CustomTimeRequest/RequestCustomTimeTests.cs`
- [x] **T108** Write integration tests for `RespondToCustomTimeRequestEndpoint` in `tests/FurryFriends.FunctionalTests/ApiEndpoints/Timeslots/CustomTimeRequest/RespondToCustomTimeRequestTests.cs`

---

## Phase 8: US6 - Petwalker Manages Timeslots (Edit/Delete)

### Use Cases (CQRS) - Test-First

- [x] **T109** `[P]` Write unit tests for `GetTimeslotsQuery` handler in `tests/FurryFriends.UnitTests/FurrFriends.UnitTests/UseCase/Timeslots/GetTimeslotsTests.cs`
- [x] **T110** `[P]` Write unit tests for `UpdateTimeslotCommand` handler in `tests/FurryFriends.UnitTests/FurrFriends.UnitTests/UseCase/Timeslots/UpdateTimeslotTests.cs`
- [x] **T111** `[P]` Write unit tests for `UpdateTimeslotValidator` in `tests/FurryFriends.UnitTests/FurrFriends.UnitTests/UseCase/Timeslots/UpdateTimeslotValidatorTests.cs`
- [x] **T112** `[P]` Write unit tests for `DeleteTimeslotCommand` handler in `tests/FurryFriends.UnitTests/FurrFriends.UnitTests/UseCase/Timeslots/DeleteTimeslotTests.cs`

### Use Cases Implementation

- [x] **T113** Create `GetTimeslotsQuery` in `src/FurryFriends.UseCases/Timeslots/Timeslot/GetTimeslotsQuery.cs`
- [x] **T114** Create `GetTimeslotsHandler` in `src/FurryFriends.UseCases/Timeslots/Timeslot/GetTimeslotsHandler.cs`
- [x] **T115** Create `UpdateTimeslotCommand` in `src/FurryFriends.UseCases/Timeslots/Timeslot/UpdateTimeslotCommand.cs`
- [x] **T116** Create `UpdateTimeslotHandler` in `src/FurryFriends.UseCases/Timeslots/Timeslot/UpdateTimeslotHandler.cs` with validations: only Available slots, within working hours, no overlaps
- [x] **T117** Create `UpdateTimeslotValidator` in `src/FurryFriends.UseCases/Timeslots/Timeslot/UpdateTimeslotValidator.cs`
- [x] **T118** Create `DeleteTimeslotCommand` in `src/FurryFriends.UseCases/Timeslots/Timeslot/DeleteTimeslotCommand.cs`
- [x] **T119** Create `DeleteTimeslotHandler` in `src/FurryFriends.UseCases/Timeslots/Timeslot/DeleteTimeslotHandler.cs` with validation: only Available/Cancelled can be deleted
- [x] **T120** Create `DeleteTimeslotValidator` in `src/FurryFriends.UseCases/Timeslots/Timeslot/DeleteTimeslotValidator.cs`

### API Endpoints

- [x] **T121** Create `GetTimeslotsEndpoint` in `src/FurryFriends.Web/Endpoints/TimeslotEndpoints/Timeslot/GetTimeslots/GetTimeslots.cs`
- [x] **T122** Create `GetTimeslotsRequest` in `src/FurryFriends.Web/Endpoints/TimeslotEndpoints/Timeslot/GetTimeslots/GetTimeslotsRequest.cs`
- [x] **T123** Create `GetTimeslotsResponse` in `src/FurryFriends.Web/Endpoints/TimeslotEndpoints/Timeslot/GetTimeslots/GetTimeslotsResponse.cs`
- [x] **T124** Create `UpdateTimeslotEndpoint` in `src/FurryFriends.Web/Endpoints/TimeslotEndpoints/Timeslot/UpdateTimeslot/UpdateTimeslot.cs`
- [x] **T125** Create `UpdateTimeslotRequest` in `src/FurryFriends.Web/Endpoints/TimeslotEndpoints/Timeslot/UpdateTimeslot/UpdateTimeslotRequest.cs`
- [x] **T126** Create `UpdateTimeslotResponse` in `src/FurryFriends.Web/Endpoints/TimeslotEndpoints/Timeslot/UpdateTimeslot/UpdateTimeslotResponse.cs`
- [x] **T127** Create `UpdateTimeslotValidator` in `src/FurryFriends.Web/Endpoints/TimeslotEndpoints/Timeslot/UpdateTimeslot/UpdateTimeslotValidator.cs`
- [x] **T128** Create `DeleteTimeslotEndpoint` in `src/FurryFriends.Web/Endpoints/TimeslotEndpoints/Timeslot/DeleteTimeslot/DeleteTimeslot.cs`
- [x] **T129** Create `DeleteTimeslotRequest` in `src/FurryFriends.Web/Endpoints/TimeslotEndpoints/Timeslot/DeleteTimeslot/DeleteTimeslotRequest.cs`

### Integration Tests

- [x] **T130** Write integration tests for `GetTimeslotsEndpoint` in `tests/FurryFriends.FunctionalTests/ApiEndpoints/Timeslots/Timeslot/GetTimeslotsTests.cs`
- [x] **T131** Write integration tests for `UpdateTimeslotEndpoint` in `tests/FurryFriends.FunctionalTests/ApiEndpoints/Timeslots/Timeslot/UpdateTimeslotTests.cs`
- [x] **T132** Write integration tests for `DeleteTimeslotEndpoint` in `tests/FurryFriends.FunctionalTests/ApiEndpoints/Timeslots/Timeslot/DeleteTimeslotTests.cs`

---

## Phase 9: UI Implementation (Blazor)

### Petwalker UI

- [x] **T133** Create Petwalker availability management page in `src/FurryFriends.BlazorUI/Components/Pages/Timeslots/PetwalkerAvailability.razor`
- [x] **T134** Create code-behind in `src/FurryFriends.BlazorUI/Components/Pages/Timeslots/PetwalkerAvailability.razor.cs`
- [x] **T135** Create Petwalker availability service in `src/FurryFriends.BlazorUI/Services/Implementation/TimeslotService.cs`
- [x] **T136** Add timeslot management UI components (create/edit/delete slots)
- [x] **T137** Add working hours management UI components

### Client UI

- [x] **T138** Create Client booking page in `src/FurryFriends.BlazorUI/Components/Pages/Timeslots/ClientBooking.razor`
- [x] **T139** Create code-behind in `src/FurryFriends.BlazorUI/Components/Pages/Timeslots/ClientBooking.razor.cs`
- [x] **T140** Add available timeslots display and selection
- [x] **T141** Add custom time request form
- [x] **T142** Create client service interface in `src/FurryFriends.BlazorUI.Client/Services/ITimeslotService.cs`

### Custom Time Request Management (Petwalker)

- [x] **T143** Create custom time requests management page in `src/FurryFriends.BlazorUI/Components/Pages/Timeslots/CustomTimeRequests.razor`
- [x] **T144** Create code-behind in `src/FurryFriends.BlazorUI/Components/Pages/Timeslots/CustomTimeRequests.razor.cs`
- [x] **T145** Add accept/decline/counter-offer functionality

---

## Phase 10: Polish & Cross-Cutting Concerns

- [x] **T146** Add comprehensive Serilog logging throughout all handlers and endpoints
- [x] **T147** Add error handling middleware for clear conflict error messages
- [x] **T148** Add input sanitization for all user inputs
- [ ] **T149** Add performance optimization for timeslot queries (caching where appropriate)
- [ ] **T150** Write bUnit tests for critical Blazor components in `tests/FurryFriends.UnitTests/BlazorUI/`
- [x] **T151** Update `docs/FurryFriends_Technical_Guide.md` with timeslots feature documentation
- [x] **T152** Update `specs/003-petwalker-timeslots/spec.md` with any implementation decisions
- [ ] **T153** Run full integration test suite and fix any failures
- [ ] **T154** Verify all user stories pass acceptance criteria

---

## Parallel Execution Opportunities

The following tasks can be executed in parallel as they involve creating tests for independent entities:

- **T002, T003, T004, T005, T006, T007** - Domain entity unit tests (all independent)
- **T008, T009, T010, T011, T012, T013, T014, T015, T016** - Domain entities and specifications (independent)
- **T023, T024, T025, T026, T027** - WorkingHours use case tests (independent)
- **T056, T057, T058** - CreateTimeslot tests (independent)
- **T068** - GetAvailableTimeslots test
- **T076, T077, T078** - BookTimeslot tests (independent)
- **T089, T090, T091** - CustomTimeRequest tests (independent)
- **T109, T110, T111, T112** - Manage timeslots tests (independent)

---

## Task Dependencies Summary

```
Phase 2 (Foundational)
├── T002-T007: Domain entity tests (parallel)
└── T008-T016: Domain entities & specs

Phase 3 (US1 - Working Hours)
├── T023-T027: WorkingHours tests
├── T028-T038: WorkingHours use cases
├── T039-T051: WorkingHours API
└── T052-T055: WorkingHours integration tests

Phase 4 (US2 - Create Timeslots)
├── T056-T058: CreateTimeslot tests
├── T059-T062: CreateTimeslot use cases
├── T063-T066: CreateTimeslot API
└── T067: CreateTimeslot integration tests

Phase 5 (US3 - View Available)
├── T068: GetAvailableTimeslots tests
├── T069-T071: GetAvailableTimeslots use cases
├── T072-T074: GetAvailableTimeslots API
└── T075: GetAvailableTimeslots integration tests

Phase 6 (US4 - Book Timeslot)
├── T076-T078: BookTimeslot tests
├── T079-T083: BookTimeslot use cases
├── T084-T087: BookTimeslot API
└── T088: BookTimeslot integration tests

Phase 7 (US5 - Custom Time)
├── T089-T091: CustomTimeRequest tests
├── T092-T098: CustomTimeRequest use cases
├── T099-T106: CustomTimeRequest API
└── T107-T108: CustomTimeRequest integration tests

Phase 8 (US6 - Manage Timeslots)
├── T109-T112: Manage timeslots tests
├── T113-T120: Manage timeslots use cases
├── T121-T129: Manage timeslots API
└── T130-T132: Manage timeslots integration tests

Phase 9 (UI)
└── T133-T145: Blazor UI components

Phase 10 (Polish)
└── T146-T154: Final polish and verification
```

---

## Notes

- All use case handlers must return `Result<T>` from Ardalis.Result
- All commands/queries must have FluentValidation validators
- All database queries must use Ardalis.Specification pattern
- All method parameters must use Ardalis.GuardClauses for validation
- All logging must use Serilog structured logging
- Follow strict Clean Architecture: Core → UseCases → Infrastructure → Web → BlazorUI
- No direct SQL queries - use EF Core + Specifications
- No Console.WriteLine - use Serilog
- No throwing exceptions for expected failures - use Result pattern
- No manual null checks - use Guard Clauses
- No LINQ queries in handlers - use Specifications
