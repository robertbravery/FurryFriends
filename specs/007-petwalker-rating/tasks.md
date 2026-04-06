# Tasks: Petwalker-Wide Rating System (007)

**Input**: Design documents from `/specs/007-petwalker-rating/`
**Prerequisites**: plan.md (required), data-model.md

## Overview
This implements Option B - rating the petwalker as a whole (not per booking). Key changes from 006: remove BookingId, add petwalker search, change unique constraint.

## Format: `[ID] [P?] Description`
- **[P]**: Can run in parallel (different files, no dependencies)
- Include exact file paths in descriptions

---

## Phase 1: Backend Core (Data Layer)

- [ ] T001 Review existing Rating entity in `src/FurryFriends.Core/RatingAggregate/Rating.cs`
- [ ] T002 Modify Rating entity: remove BookingId property, update constructor and Create() method
- [ ] T003 Create EF Core migration: remove BookingId column, add unique constraint on (ClientId, PetWalkerId)
- [ ] T004 [P] Update RatingConfiguration in `src/FurryFriends.Infrastructure/Data/Config/RatingConfiguration.cs`
- [ ] T005 [P] Add new specification `GetRatingByClientAndPetWalkerSpecification.cs` in `src/FurryFriends.Core/RatingAggregate/Specifications/`

---

## Phase 2: API Layer

- [ ] T006 Review existing CreateRating endpoint in `src/FurryFriends.Web/Endpoints/RatingEndpoints/Create/`
- [ ] T007 Modify CreateRatingRequest: replace BookingId with PetWalkerId
- [ ] T008 Modify CreateRatingCommand: replace BookingId with PetWalkerId
- [ ] T009 Modify CreateRatingHandler: update validation for one rating per client per petwalker
- [ ] T010 [P] Add new endpoint for searching petwalkers by name/location
- [ ] T011 [P] Update CreateRatingValidator: new validation rules

---

## Phase 3: Frontend

- [ ] T012 Review existing RatingSubmission.razor in `src/FurryFriends.BlazorUI.Client/Pages/Rating/`
- [ ] T013 Modify RatingSubmission.razor: replace BookingId textbox with petwalker search UI
- [ ] T014 Add petwalker search to RatingService in `src/FurryFriends.BlazorUI/Services/Implementation/`
- [ ] T015 Update IRatingService interface with search method

---

## Phase 4: Testing & Polish

- [ ] T016 Verify StarRating component still works (`src/FurryFriends.BlazorUI.Client/Components/Common/StarRating.razor`)
- [ ] T017 Verify PetWalkerRatingsDashboard still works (`src/FurryFriends.BlazorUI.Client/Pages/Rating/PetWalkerRatingsDashboard.razor`)
- [ ] T018 Test: Search for petwalker and submit rating
- [ ] T019 Test: Prevent duplicate rating (same client + same petwalker)
- [ ] T020 Test: Verify rating update within 7 days still works

---

## Parallel Opportunities

- T004 and T005 can run in parallel (different files)
- T010 and T011 can run in parallel (different files)
- T016 and T017 can run in parallel (different components)

## Notes
- Much of the rating infrastructure from 006 is reusable
- Primary work is swapping BookingId → PetWalkerId references
- New work: petwalker search functionality