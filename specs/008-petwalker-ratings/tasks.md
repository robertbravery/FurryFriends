# Tasks: PetWalker Rating Aggregation

**Input**: Design documents from `/specs/008-petwalker-ratings/`
**Prerequisites**: plan.md, research.md, data-model.md, contracts/

## Format: `[ID] [P?] [Story?] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[US1]**: User Story 1 (Rating submission, viewing, and aggregate display)
- Include exact file paths in descriptions

## Phase 1: Setup

- [ ] T001 Create EF Core migration — DELETE all existing Ratings data, remove BookingId column from Ratings table, add AverageRating (`double?` null) and TotalRatingsCount (int, default 0) to PetWalkers table in `src/FurryFriends.Infrastructure/Migrations/`

## Phase 2: Foundational (Domain Layer)

**⚠️ MUST complete before any user story work**

- [x] T002 [P] Add RatingStatus enum (Active, Moderated, Removed) in `src/FurryFriends.Core/RatingAggregate/RatingStatus.cs`
- [x] T003 [P] Add domain events (RatingAddedEvent, RatingUpdatedEvent, RatingRemovedEvent) in `src/FurryFriends.Core/PetWalkerAggregate/Events/`
- [x] T004 Update Rating entity — remove BookingId property, add Status property (RatingStatus), add CreatedDate/ModifiedDate, update Create() factory method, add CanEdit() method (24h window based on CreatedDate) in `src/FurryFriends.Core/RatingAggregate/Rating.cs`
- [x] T005 Update PetWalker entity — add AverageRating (`double?`) and TotalRatingsCount (int) fields, add UpdateRatingAggregate() method (recalculates average rounded to 1 decimal), add RegisterDomainEvent calls in `src/FurryFriends.Core/PetWalkerAggregate/PetWalker.cs`
- [x] T006 [P] Create domain event handler RecalculateRatingOnEventChangeHandler that recalculates AverageRating (rounded to 1 decimal) and TotalRatingsCount from active ratings when events fire in `src/FurryFriends.Core/PetWalkerAggregate/Events/RecalculateRatingOnEventChangeHandler.cs`
- [x] T007 [P] Create CountCompletedBookingsForClientPetWalkerSpecification in `src/FurryFriends.Core/BookingAggregate/Specifications/`
- [x] T008 [P] Create GetActiveRatingsForPetWalkerSpecification in `src/FurryFriends.Core/RatingAggregate/Specifications/`
- [x] T009 Update RatingConfiguration (EF) — remove BookingId mapping, add Status mapping in `src/FurryFriends.Infrastructure/Data/Config/RatingConfiguration.cs`
- [x] T010 Update PetWalkerConfiguration (EF) — add AverageRating (`double?`) and TotalRatingsCount column mappings in `src/FurryFriends.Infrastructure/Data/Config/PetWalkerConfiguration.cs`

## Phase 3: User Story 1 — Tests (MUST FAIL before implementation)

**User Story**: As a pet owner who has used a petwalker's services, I want to submit an overall rating for that petwalker. As a potential client, I want to view a petwalker's overall rating.

**Independent Test**: Create a completed booking between client and petwalker, submit rating via POST endpoint, verify GET summary reflects new average, verify GET ratings list includes the new entry.

- [x] T011 [P] [US1] Unit tests for Rating entity (creation with valid 1-5 range, CanEdit within/after 24h of CreatedDate, invalid values rejected) in `tests/FurryFriends.UnitTests/Core/RatingAggregate/RatingTests.cs`
- [x] T012 [P] [US1] Unit tests for PetWalker rating aggregate recalculation (average of multiple ratings, zero ratings, single rating, rounding to 1 decimal) in `tests/FurryFriends.UnitTests/Core/PetWalkerAggregate/PetWalkerTests.cs`
- [x] T013 [P] [US1] Use case tests for CreateRatingHandler (valid eligibility, no completed booking, ratings exceed bookings, ratings ≤ bookings constraint replaced active) in `tests/FurryFriends.UnitTests/UseCases/Rating/CreateRatingTests.cs`
- [x] T014 [P] [US1] Use case tests for GetRatingsForPetWalkerHandler (paginated results, empty list) in `tests/FurryFriends.UnitTests/UseCases/Rating/GetRatingsForPetWalkerTests.cs`
- [x] T015 [P] [US1] Use case tests for GetPetWalkerRatingSummaryHandler (with ratings, no ratings displays "No ratings yet") in `tests/FurryFriends.UnitTests/UseCases/Rating/GetPetWalkerRatingSummaryTests.cs`
- [x] T016 [P] [US1] Integration tests for POST /api/ratings (create with valid eligibility, reject without completed booking, reject exceeding booking limit, replace previous active rating) in `tests/FurryFriends.FunctionalTests/RatingEndpoints/CreateRatingTests.cs`
- [x] T017 [P] [US1] Integration tests for PUT /api/ratings/{id} (update within 24h of CreatedDate succeeds, update after 24h fails, update Moderated rating fails) in `tests/FurryFriends.FunctionalTests/RatingEndpoints/UpdateRatingTests.cs`
- [x] T018 [P] [US1] Integration tests for DELETE /api/ratings/{id} (delete within 24h of CreatedDate succeeds, delete after 24h fails, delete Moderated rating fails) in `tests/FurryFriends.FunctionalTests/RatingEndpoints/DeleteRatingTests.cs`
- [x] T019 [P] [US1] Integration tests for GET /api/ratings/petwalker/{petWalkerId} (paginated list, empty result) in `tests/FurryFriends.FunctionalTests/RatingEndpoints/GetRatingsTests.cs`
- [x] T020 [P] [US1] Integration tests for GET /api/ratings/petwalker/{petWalkerId}/summary (average and count, petwalker not found) in `tests/FurryFriends.FunctionalTests/RatingEndpoints/GetRatingSummaryTests.cs`

## Phase 4: User Story 1 — Use Cases (Business Logic)

- [x] T021 [P] [US1] Create DTOs: RatingDto and PetWalkerRatingSummaryDto (AverageRating as `double?`) in `src/FurryFriends.UseCases/Domain/Ratings/`
- [x] T022 [US1] Implement CreateRatingCommand and CreateRatingHandler with eligibility checks (completed bookings count, ratings ≤ bookings constraint, replace active rating, dispatch domain event) in `src/FurryFriends.UseCases/Domain/Ratings/CreateRating/`
- [x] T023 [US1] Implement CreateRatingValidator (PetWalkerId not empty, ClientId not empty, RatingValue 1-5 inclusive, Comment max 1000 chars) in `src/FurryFriends.UseCases/Domain/Ratings/CreateRating/CreateRatingValidator.cs`
- [x] T024 [US1] Implement UpdateRatingCommand and UpdateRatingHandler with 24h window check (using CreatedDate) and Status = Active requirement in `src/FurryFriends.UseCases/Domain/Ratings/UpdateRating/`
- [x] T025 [US1] Implement UpdateRatingValidator — validates RatingId not empty, RatingValue 1-5 inclusive (if provided), Comment max 1000 chars, at least RatingValue or Comment must be provided in `src/FurryFriends.UseCases/Domain/Ratings/UpdateRating/UpdateRatingValidator.cs`
- [x] T026 [US1] Implement DeleteRatingCommand, DeleteRatingHandler, and DeleteRatingValidator (RatingId not empty, ClientId not empty) with 24h window check (using CreatedDate), Status = Active requirement, set status to Removed, dispatch RatingRemovedEvent in `src/FurryFriends.UseCases/Domain/Ratings/DeleteRating/`
- [x] T027 [US1] Implement GetRatingsForPetWalkerQuery and handler (paginated, active-only by default), with FluentValidation pagination validator (PageNumber ≥ 1, PageSize 1-100) in `src/FurryFriends.UseCases/Domain/Ratings/GetRatingsForPetWalker/`
- [x] T028 [US1] Implement GetPetWalkerRatingSummaryQuery and handler (return AverageRating + TotalRatingsCount, handle 0 ratings gracefully) in `src/FurryFriends.UseCases/Domain/Ratings/GetPetWalkerRatingSummary/`

## Phase 5: API Endpoints (FastEndpoints)

- [x] T029 [P] [US1] Create POST /api/ratings endpoint in `src/FurryFriends.Web/Endpoints/RatingEndpoints/Create/CreateRatingEndpoint.cs`
- [x] T030 [P] [US1] Create PUT /api/ratings/{id} endpoint in `src/FurryFriends.Web/Endpoints/RatingEndpoints/Update/UpdateRatingEndpoint.cs`
- [x] T031 [P] [US1] Create DELETE /api/ratings/{id} endpoint in `src/FurryFriends.Web/Endpoints/RatingEndpoints/Delete/DeleteRatingEndpoint.cs`
- [x] T032 [P] [US1] Create GET /api/ratings/petwalker/{petWalkerId} endpoint in `src/FurryFriends.Web/Endpoints/RatingEndpoints/GetRatingsForPetWalker/GetRatingsForPetWalkerEndpoint.cs`
- [x] T033 [P] [US1] Create GET /api/ratings/petwalker/{petWalkerId}/summary endpoint in `src/FurryFriends.Web/Endpoints/RatingEndpoints/GetPetWalkerRatingSummary/GetPetWalkerRatingSummaryEndpoint.cs`
- [x] T034 Register Rating-related services, validators, and MediatR handlers in `src/FurryFriends.Web/Program.cs`

## Phase 6: Blazor UI

- [ ] T035 [P] [US1] Update IRatingService interface with new method signatures (create, update, delete, get ratings, get summary) in `src/FurryFriends.BlazorUI.Client/Services/IRatingService.cs`
- [ ] T036 [US1] Update RatingService implementation to call new API endpoints via HttpClient in `src/FurryFriends.BlazorUI/Services/Implementation/RatingService.cs`
- [ ] T037 [P] [US1] Create reusable RatingDisplay component (stars, average, total count, "No ratings yet" for zero) in `src/FurryFriends.BlazorUI/Components/Common/RatingDisplay.razor` + `.razor.cs` + `.razor.css`
- [ ] T038 [P] [US1] Create RatingSubmission component (score 1-5 selector, optional comment textarea, validation feedback, submit/save/delete actions) in `src/FurryFriends.BlazorUI/Components/Pages/Ratings/RatingSubmission.razor` + `.razor.cs` + `.razor.css`
- [ ] T039 [P] [US1] Create PetWalkerRatings component (paginated rating list, individual rating details) in `src/FurryFriends.BlazorUI/Components/Pages/Ratings/PetWalkerRatings.razor` + `.razor.cs` + `.razor.css`

## Phase 7: Polish & Cross-Cutting Concerns

- [ ] T040 Add Serilog structured logging to all handlers and endpoints (log entry/exit, eligibility failures, rating created/updated/deleted)
- [ ] T041 Run quickstart.md verification checklist — confirm all scenarios pass (create, update, delete, view, summary), all unit + integration tests are green, `dotnet build` succeeds across all projects, verify GET summary <200ms response time
- [ ] T042 [P] bUnit component tests for RatingDisplay (renders stars, zero-ratings message, average display) in `tests/FurryFriends.UnitTests/BlazorUI/RatingDisplayTests.cs`
- [ ] T043 [P] bUnit component tests for RatingSubmission (renders score selector 1-5, comment textarea, submit button; shows validation errors; displays success/error messages) in `tests/FurryFriends.UnitTests/BlazorUI/RatingSubmissionTests.cs`
- [ ] T044 [P] bUnit component tests for PetWalkerRatings (renders paginated rating list, individual rating details, empty state, loading state) in `tests/FurryFriends.UnitTests/BlazorUI/PetWalkerRatingsTests.cs`

## Dependencies

- **Phase 2 (Foundational)** blocks all user story phases
- **T001 (Migration)** must complete before T010 (PetWalkerConfiguration requires new columns)
- **T004 (Rating entity)** before T022, T023 (CreateRating relies on updated Rating model)
- **T005 (PetWalker entity)** before T006 (event handler requires UpdateRatingAggregate)
- **Phase 3 (Tests)** before Phase 4 (Use Cases) — TDD: tests must fail first
- **Phase 4 (Use Cases)** before Phase 5 (Endpoints) — endpoints delegate to handlers
- **Phase 5 (Endpoints)** before Phase 6 (Blazor UI) — UI calls API endpoints
- **T034 (DI registration)** depends on T029-T033 (endpoints must exist to register)
- **T042, T043, T044 (bUnit tests)** depend on Phase 6 (components must exist first)

**Within Phase 2 (parallel opportunities)**:

- T002, T003, T006, T007, T008 can run in parallel (different files)
- T004 blocks T022 (Rating entity required for handler)
- T009, T010 can run in parallel with other Phase 2 tasks

**Within Phase 3 (parallel opportunities)**:

- T011-T020 all marked [P] — fully parallelizable (separate test files, all should fail first)

**Within Phase 4 (parallel opportunities)**:

- T021 can run in parallel with T022-T028 (DTOs are independent)
- T023 depends on T022 (validator for existing command)
- T027, T028 are independent of T022-T026 (read queries vs write commands)

**Within Phase 5 (parallel opportunities)**:

- T029-T033 all marked [P] — different endpoint files/directories
- T034 depends on T029-T033 (register all endpoints)

**Within Phase 6 (parallel opportunities)**:

- T035, T037, T038, T039 all marked [P] — different component files
- T036 depends on T035 (implementation needs interface)

**Within Phase 7 (parallel opportunities)**:

- T042, T043, T044 are [P] — different test files, all depend on Phase 6 completion

## Parallel Example

```
# Phase 2 (Foundational - launch together after T001):
Task: "T002 Add RatingStatus enum in src/FurryFriends.Core/RatingAggregate/RatingStatus.cs"
Task: "T003 Add domain events in src/FurryFriends.Core/PetWalkerAggregate/Events/"
Task: "T007 Create CountCompletedBookingsForClientPetWalkerSpecification in BookingAggregate"
Task: "T008 Create GetActiveRatingsForPetWalkerSpecification"
Task: "T009 Update RatingConfiguration in src/FurryFriends.Infrastructure/Data/Config/RatingConfiguration.cs"
Task: "T010 Update PetWalkerConfiguration in src/FurryFriends.Infrastructure/Data/Config/PetWalkerConfiguration.cs"

# Phase 3 (Tests - launch all together after Phase 2):
Task: "T011 Unit tests Rating entity"
Task: "T012 Unit tests PetWalker aggregate recalculation (include rounding assertion)"
Task: "T013 Use case tests CreateRatingHandler"
Task: "T014 Use case tests GetRatingsForPetWalkerHandler"
Task: "T015 Use case tests GetPetWalkerRatingSummaryHandler"
Task: "T016 Integration test POST /api/ratings"
Task: "T017 Integration test PUT /api/ratings/{id} (include Moderated rejection)"
Task: "T018 Integration test DELETE /api/ratings/{id} (include Moderated rejection)"
Task: "T019 Integration test GET /api/ratings/petwalker/{petWalkerId}"
Task: "T020 Integration test GET /api/ratings/petwalker/{petWalkerId}/summary"

# Phase 5 (Endpoints - launch all together after Phase 4):
Task: "T029 Create POST /api/ratings endpoint"
Task: "T030 Create PUT /api/ratings/{id} endpoint"
Task: "T031 Create DELETE /api/ratings/{id} endpoint"
Task: "T032 Create GET /api/ratings/petwalker/{petWalkerId} endpoint"
Task: "T033 Create GET /api/ratings/petwalker/{petWalkerId}/summary endpoint"
```

## Implementation Strategy

### MVP (User Story 1 — Rating submission and display)

- Phase 1 → Phase 2 → Phase 3 (tests fail) → Phase 4 → Phase 5 → Phase 6 → Phase 7
- All phases contribute to the single user story
- After Phase 5, the core API is functional; Phase 6 adds the UI layer

### Incremental Delivery

1. Core domain + tests failing (Phase 1-3): Foundation ready ✅
2. Business logic + API (Phase 4-5): Backend functional, integrable ✅
3. Blazor UI (Phase 6): Full user-facing feature ✅
4. Polish + bUnit tests (Phase 7): Production quality ✅

## Notes

- [P] tasks = different files, no dependencies within phase
- TDD order: Tests must fail before implementation
- FR-011 (moderation) and FR-012 (account deletion) are deferred to v2 per DEFERRED_REQUIREMENTS.md — no tasks for them
- Rating entity has no Booking reference — eligibility checked via specification
- One active rating per client per petwalker — create replaces previous active
- Denormalized AverageRating/TotalRatingsCount on PetWalker updated via domain events only
- Average rating rounded to one decimal place; rating values are whole numbers 1-5 only
- AverageRating type: `double?` across all layers (migration, entity, config, DTOs)
- CountCompletedBookingsForClientPetWalkerSpecification lives in BookingAggregate/Specifications/ (it queries Bookings)
- All validators use FluentValidation per Constitution XII — T026 includes DeleteRatingValidator alongside Command and Handler
- All 24h window checks use CreatedDate (not ModifiedDate)
- Update/Delete handlers reject non-Active (Moderated/Removed) ratings
- Migration includes DELETE of existing Ratings data before schema changes (fresh start)
- T041 includes build verification and performance target verification
