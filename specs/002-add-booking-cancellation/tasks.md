# Tasks: Add Booking Cancellation

**Input**: Design documents from `/specs/002-add-booking-cancellation/`
**Prerequisites**: plan.md (required), research.md, data-model.md, contracts/

## Execution Flow (main)
```
1. Load plan.md from feature directory
   → If not found: ERROR "No implementation plan found"
   → Extract: tech stack, libraries, structure
2. Load optional design documents:
   → data-model.md: Extract entities → model tasks
   → contracts/: Each file → contract test task
   → research.md: Extract decisions → setup tasks
3. Generate tasks by category:
   → Setup: project init, dependencies, linting
   → Tests: contract tests, integration tests
   → Core: models, services, CLI commands
   → Integration: DB, middleware, logging
   → Polish: unit tests, performance, docs
4. Apply task rules:
   → Different files = mark [P] for parallel
   → Same file = sequential (no [P])
   → Tests before implementation (TDD)
5. Number tasks sequentially (T001, T002...)
6. Generate dependency graph
7. Create parallel execution examples
8. Validate task completeness:
   → All contracts have tests?
   → All entities have models?
   → All endpoints implemented?
9. Return: SUCCESS (tasks ready for execution)
```

## Format: `[ID] [P?] Description`
- **[P]**: Can run in parallel (different files, no dependencies)
- Include exact file paths in descriptions

## Path Conventions
- **API**: `src/FurryFriends.Web`
- **Use Cases**: `src/FurryFriends.UseCases`
- **Core**: `src/FurryFriends.Core`
- **Infrastructure**: `src/FurryFriends.Infrastructure`
- **Blazor UI**: `src/FurryFriends.BlazorUI`
- **Blazor UI Client**: `src/FurryFriends.BlazorUI.Client`

## Phase 3.1: Setup
- [ ] T001 Create project structure per implementation plan
- [ ] T002 Initialize .NET 9 project with FastEndpoints, MediatR, FluentValidation, Ardalis.Specification, Ardalis.Result, Ardalis.GuardClauses, Serilog, Entity Framework Core, Blazor Server + WebAssembly dependencies
- [ ] T003 [P] Configure linting and formatting tools

## Phase 3.2: Tests First (TDD) ⚠️ MUST COMPLETE BEFORE 3.3
**CRITICAL: These tests MUST be written and MUST FAIL before ANY implementation**
- [ ] T004 [P] Contract test POST /api/bookings/{BookingId}/cancel in specs/002-add-booking-cancellation/contracts/CancelBookingContractTests.cs
- [ ] T005 [P] Integration test client cancels a confirmed booking in tests/FurryFriends.IntegrationTests/BookingEndpoints/test_booking_cancellation.py
- [ ] T006 [P] Integration test pet walker cancels a confirmed booking in tests/FurryFriends.IntegrationTests/BookingEndpoints/test_booking_cancellation.py
- [ ] T007 [P] Integration test attempt to cancel a completed booking in tests/FurryFriends.IntegrationTests/BookingEndpoints/test_booking_cancellation.py
- [ ] T008 [P] Integration test attempt to cancel a cancelled booking in tests/FurryFriends.IntegrationTests/BookingEndpoints/test_booking_cancellation.py


## Phase 3.3: Core Implementation (ONLY after tests are failing)
- [ ] T009 [P] Booking cancellation method in src/FurryFriends.Core/BookingAggregate/Booking.cs
- [ ] T010 [P] Cancellation model in src/FurryFriends.Core/BookingAggregate/Cancellation.cs
- [ ] T011 [P] AuditLog model in src/FurryFriends.Core/BookingAggregate/AuditLog.cs
- [ ] T012 [P] BookingService CRUD in src/FurryFriends.UseCases/Services/BookingService/BookingService.cs
- [ ] T013 POST src/FurryFriends.Web/Endpoints/BookingEndpoints/CancelBookingbookings/{BookingId} endpoint in src/FurryFriends.Web/Endpoints/BookingEndpoints/CancelBooking.cs
- [ ] T014 Input validation in src/FurryFriends.Web/Endpoints/BookingEndpoints/CancelBookingbookings/CancelBookingValidator.cs
- [ ] T015 Error handling and logging in src/FurryFriends.Web/Endpoints/BookingEndpoints/CancelBooking.cs

## Phase 3.4: Integration
- [ ] T016 Connect BookingService to DB
- [ ] T017 Implement pessimistic locking mechanism using EF Core to prevent race conditions during concurrent cancellation requests
- [ ] T018 Update Blazor UI to allow booking cancellation

## Phase 3.5: Polish
- [ ] T019 [P] Unit tests for validation in tests/FurryFriends.UnitTests/UseCases/test_validation.py
- [ ] T020 Performance tests (<200ms)
- [ ] T021 [P] Update docs/api.md
- [ ] T022 Remove duplication
- [ ] T023 Run manual-testing.md

## Dependencies
- Tests (T004-T008) before implementation (T009-T015)
- T009 blocks T012, T016
- T016 blocks T018
- Implementation before polish (T019-T023)

## Parallel Example
```
# Launch T004-T008 together:
Task: "Contract test POST /api/bookings/{BookingId}/cancel in specs/002-add-booking-cancellation/contracts/CancelBookingContractTests.cs"
Task: "Integration test client cancels a confirmed booking in tests/FurryFriends.IntegrationTests/BookingEndpoints/test_booking_cancellation.py"
Task: "Integration test pet walker cancels a confirmed booking in tests/FurryFriends.IntegrationTests/BookingEndpoints/test_booking_cancellation.py"
Task: "Integration test attempt to cancel a completed booking in tests/FurryFriends.IntegrationTests/BookingEndpoints/test_booking_cancellation.py"
```

## Notes
- [P] tasks = different files, no dependencies
- Verify tests fail before implementing
- Commit after each task
- Avoid: vague tasks, same file conflicts

## Task Generation Rules
*Applied during main() execution*

1. **From Contracts**:
   - Each contract file → contract test task [P]
   - Each endpoint → implementation task
   
2. **From Data Model**:
   - Each entity → model creation task [P]
   - Relationships → service layer tasks
   
3. **From User Stories**:
   - Each story → integration test [P]
   - Quickstart scenarios → validation tasks

4. **Ordering**:
   - Setup → Tests → Models → Services → Endpoints → Polish
   - Dependencies block parallel execution

## Validation Checklist
*GATE: Checked by main() before returning*

- [ ] All contracts have corresponding tests
- [ ] All entities have model tasks
- [ ] All tests come before implementation
- [ ] Parallel tasks truly independent
- [ ] Each task specifies exact file path
- [ ] No task modifies same file as another [P] task