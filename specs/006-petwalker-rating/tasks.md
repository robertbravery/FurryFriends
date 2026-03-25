# Tasks: Petwalker Rating System

**Input**: Design documents from `/specs/006-petwalker-rating/`
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
- **Single project**: `src/`, `tests/` at repository root
- **Web app**: `backend/src/`, `frontend/src/`
- **Mobile**: `api/src/`, `ios/src/` or `android/src/`
- Paths shown below assume FurryFriends Clean Architecture structure

## User Story Phases

### Phase 1: Setup
- [ ] T001 Review existing Rating entity in `src/FurryFriends.Core/RatingAggregate/Rating.cs`
- [ ] T002 Create EF Core migration for Rating table changes (BookingId, ModifiedDate columns)
- [ ] T003 [P] Configure database unique constraint on BookingId

## Phase 2: Tests First (TDD) ⚠️ MUST COMPLETE BEFORE PHASE 3
**CRITICAL: These tests MUST be written and MUST FAIL before ANY implementation**
- [ ] T005 [P] Contract test POST /api/ratings in `tests/FurryFriends.IntegrationTests/RatingEndpoints/CreateRatingTests.cs`
- [ ] T006 [P] Contract test PUT /api/ratings/{id} in `tests/FurryFriends.IntegrationTests/RatingEndpoints/UpdateRatingTests.cs`
- [ ] T007 [P] Contract test GET /api/petwalkers/{id}/ratings in `tests/FurryFriends.IntegrationTests/RatingEndpoints/GetRatingsForPetWalkerTests.cs`
- [ ] T008 [P] Contract test GET /api/petwalkers/{id}/ratings/summary in `tests/FurryFriends.IntegrationTests/RatingEndpoints/GetPetWalkerRatingSummaryTests.cs`
- [ ] T009 [P] Unit test CreateRatingValidator in `tests/FurryFriends.UnitTests/UseCases/Rating/CreateRatingValidatorTests.cs`
- [ ] T010 [P] Unit test UpdateRatingValidator in `tests/FurryFriends.UnitTests/UseCases/Rating/UpdateRatingValidatorTests.cs`

## Phase 3: Core Implementation (ONLY after tests are failing)
### Rating Entity Extensions
- [ ] T011 [P] Add BookingId field to `src/FurryFriends.Core/RatingAggregate/Rating.cs`
- [ ] T012 [P] Add ModifiedDate field to `src/FurryFriends.Core/RatingAggregate/Rating.cs`
- [ ] T013 [P] Add CanUpdate() method to Rating entity
- [ ] T014 [P] Add UpdateRatingValue() and UpdateComment() methods to Rating entity

### Specifications (Query Objects)
- [ ] T015 [P] Create GetRatingsForPetWalkerSpecification in `src/FurryFriends.Core/RatingAggregate/Specifications/GetRatingsForPetWalkerSpecification.cs`
- [ ] T016 [P] Create GetRatingByBookingIdSpecification in `src/FurryFriends.Core/RatingAggregate/Specifications/GetRatingByBookingIdSpecification.cs`
- [ ] T017 [P] Create GetPetWalkerRatingSummarySpecification in `src/FurryFriends.Core/RatingAggregate/Specifications/GetPetWalkerRatingSummarySpecification.cs`

### UseCases - Create Rating
- [ ] T018 CreateRatingCommand in `src/FurryFriends.UseCases/Rating/CreateRating/CreateRatingCommand.cs`
- [ ] T019 CreateRatingHandler in `src/FurryFriends.UseCases/Rating/CreateRating/CreateRatingHandler.cs`
- [ ] T020 CreateRatingValidator in `src/FurryFriends.UseCases/Rating/CreateRating/CreateRatingValidator.cs`
- [ ] T021 CreateRatingDto in `src/FurryFriends.UseCases/Rating/CreateRating/CreateRatingDto.cs`

### UseCases - Update Rating
- [ ] T022 UpdateRatingCommand in `src/FurryFriends.UseCases/Rating/UpdateRating/UpdateRatingCommand.cs`
- [ ] T023 UpdateRatingHandler in `src/FurryFriends.UseCases/Rating/UpdateRating/UpdateRatingHandler.cs`
- [ ] T024 UpdateRatingValidator in `src/FurryFriends.UseCases/Rating/UpdateRating/UpdateRatingValidator.cs`

### UseCases - Get Ratings for PetWalker
- [ ] T025 GetRatingsForPetWalkerQuery in `src/FurryFriends.UseCases/Rating/GetRatingsForPetWalker/GetRatingsForPetWalkerQuery.cs`
- [ ] T026 GetRatingsForPetWalkerHandler in `src/FurryFriends.UseCases/Rating/GetRatingsForPetWalker/GetRatingsForPetWalkerHandler.cs`
- [ ] T027 RatingDto in `src/FurryFriends.UseCases/Rating/GetRatingsForPetWalker/RatingDto.cs`

### UseCases - Get PetWalker Rating Summary
- [ ] T028 GetPetWalkerRatingSummaryQuery in `src/FurryFriends.UseCases/Rating/GetPetWalkerRatingSummary/GetPetWalkerRatingSummaryQuery.cs`
- [ ] T029 GetPetWalkerRatingSummaryHandler in `src/FurryFriends.UseCases/Rating/GetPetWalkerRatingSummary/GetPetWalkerRatingSummaryHandler.cs`
- [ ] T030 PetWalkerRatingSummaryDto in `src/FurryFriends.UseCases/Rating/GetPetWalkerRatingSummary/PetWalkerRatingSummaryDto.cs`

### API Endpoints
- [ ] T031 CreateRating endpoint in `src/FurryFriends.Web/Endpoints/RatingEndpoints/CreateRating/CreateRating.cs`
- [ ] T032 CreateRatingRequest in `src/FurryFriends.Web/Endpoints/RatingEndpoints/CreateRating/CreateRatingRequest.cs`
- [ ] T033 CreateRatingResponse in `src/FurryFriends.Web/Endpoints/RatingEndpoints/CreateRating/CreateRatingResponse.cs`
- [ ] T034 CreateRatingValidator in `src/FurryFriends.Web/Endpoints/RatingEndpoints/CreateRating/CreateRatingValidator.cs`
- [ ] T035 UpdateRating endpoint in `src/FurryFriends.Web/Endpoints/RatingEndpoints/UpdateRating/UpdateRating.cs`
- [ ] T036 UpdateRatingRequest in `src/FurryFriends.Web/Endpoints/RatingEndpoints/UpdateRating/UpdateRatingRequest.cs`
- [ ] T037 UpdateRatingResponse in `src/FurryFriends.Web/Endpoints/RatingEndpoints/UpdateRating/UpdateRatingResponse.cs`
- [ ] T038 UpdateRatingValidator in `src/FurryFriends.Web/Endpoints/RatingEndpoints/UpdateRating/UpdateRatingValidator.cs`
- [ ] T039 GetRatingsForPetWalker endpoint in `src/FurryFriends.Web/Endpoints/RatingEndpoints/GetRatingsForPetWalker/GetRatingsForPetWalker.cs`
- [ ] T040 GetPetWalkerRatingSummary endpoint in `src/FurryFriends.Web/Endpoints/RatingEndpoints/GetPetWalkerRatingSummary/GetPetWalkerRatingSummary.cs`

## Phase 4: Integration
- [ ] T041 [P] Add Rating configuration in `src/FurryFriends.Infrastructure/Data/Config/RatingConfiguration.cs`
- [ ] T042 [P] Register Rating endpoints in `src/FurryFriends.Web/Program.cs`
- [ ] T043 Add logging to all Rating handlers using Serilog
- [ ] T044 Verify authentication/authorization for rating endpoints

## Phase 4.5: Blazor UI
- [ ] T051 [P] Create IRatingService interface in `src/FurryFriends.BlazorUI.Client/Services/IRatingService.cs`
- [ ] T052 [P] Create RatingService implementation in `src/FurryFriends.BlazorUI/Services/Implementation/RatingService.cs`
- [ ] T053 [P] Create RatingSubmissionComponent in `src/FurryFriends.BlazorUI/Components/Pages/Rating/RatingSubmission.razor`
- [ ] T054 [P] Create RatingSubmissionComponent code-behind in `src/FurryFriends.BlazorUI/Components/Pages/Rating/RatingSubmission.razor.cs`
- [ ] T055 Create PetWalkerRatingsDashboard page in `src/FurryFriends.BlazorUI/Components/Pages/Rating/PetWalkerRatingsDashboard.razor`
- [ ] T056 Create PetWalkerRatingsDashboard code-behind in `src/FurryFriends.BlazorUI/Components/Pages/Rating/PetWalkerRatingsDashboard.razor.cs`
- [ ] T057 Add star rating display component in `src/FurryFriends.BlazorUI/Components/Common/StarRating.razor`
- [ ] T058 [P] Add navigation links for rating pages in `src/FurryFriends.BlazorUI/Components/Layout/NavMenu.razor`

## Phase 5: Polish
- [ ] T045 [P] Unit tests for Rating entity business rules in `tests/FurryFriends.UnitTests/Core/RatingAggregate/RatingTests.cs`
- [ ] T046 [P] Unit tests for Specifications in `tests/FurryFriends.UnitTests/Core/RatingAggregate/Specifications/`
- [ ] T047 Performance test rating submission - measure POST /api/ratings endpoint with in-memory DB, target <500ms p95
- [ ] T048 Performance test summary calculation - measure GET /api/petwalkers/{id}/ratings/summary with 1000 ratings, target <300ms p95
- [ ] T049 Update API documentation in `docs/api.md`
- [ ] T059 Run quickstart.md validation scenarios

## Dependencies
- Tests (T005-T010) before implementation (T011-T040)
- T011-T014 block T018-T024 (entity changes needed for commands)
- T015-T017 blocks T025-T030 (specifications needed for queries)
- T018-T021 blocks T031-T034 (use cases needed for endpoint)
- T022-T024 blocks T035-T038 (use cases needed for endpoint)
- T025-T027 blocks T039 (use cases needed for endpoint)
- T028-T030 blocks T040 (use cases needed for endpoint)
- T031-T040 block T042 (endpoints need registration)
- T041 blocks T042 (DB config needed before endpoint registration)
- T042 blocks T051-T058 (endpoints must be registered before UI can call them)
- T031-T040 block T051-T058 (use cases needed for UI service calls)
- Implementation before polish (T045-T050, T059)

## Parallel Example
```
# Launch T005-T010 together (contract and validator tests):
Task: "Contract test POST /api/ratings"
Task: "Contract test PUT /api/ratings/{id}"
Task: "Contract test GET /api/petwalkers/{id}/ratings"
Task: "Contract test GET /api/petwalkers/{id}/ratings/summary"
Task: "Unit test CreateRatingValidator"
Task: "Unit test UpdateRatingValidator"

# Launch T011-T014 together (Rating entity extensions):
Task: "Add BookingId field to Rating entity"
Task: "Add ModifiedDate field to Rating entity"
Task: "Add CanUpdate() method"
Task: "Add UpdateRatingValue() and UpdateComment() methods"

# Launch T015-T017 together (Specifications):
Task: "Create GetRatingsForPetWalkerSpecification"
Task: "Create GetRatingByBookingIdSpecification"
Task: "Create GetPetWalkerRatingSummarySpecification"

# Launch T031-T034 together (Create Rating endpoint):
Task: "CreateRating endpoint"
Task: "CreateRatingRequest"
Task: "CreateRatingResponse"
Task: "CreateRatingValidator"
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
   - Client rates completed booking → Create Rating workflow
   - Petwalker views ratings → Get Ratings workflow
   - Client updates rating within 7 days → Update Rating workflow

4. **Ordering**:
   - Setup → Tests → Models → Specifications → UseCases → Endpoints → Polish
   - Dependencies block parallel execution

## Validation Checklist
*GATE: Checked by main() before returning*

- [x] All contracts have corresponding tests (4 endpoints → 4 contract tests)
- [x] All entities have model tasks (Rating entity extension)
- [x] All specifications have tasks (3 specifications)
- [x] All tests come before implementation
- [x] Parallel tasks truly independent
- [x] Each task specifies exact file path
- [x] No task modifies same file as another [P] task
- [x] All UI components have tasks (RatingSubmissionComponent, PetWalkerRatingsDashboard, StarRating)
- [x] Phase ordering is sequential (Phase 1 → 2 → 3 → 4 → 4.5 → 5)
