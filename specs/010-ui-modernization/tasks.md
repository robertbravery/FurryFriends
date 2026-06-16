# Tasks: 010-ui-modernization - Enterprise UI Modernization

**Input**: Design documents from `/specs/010-ui-modernization/`

## Phase 1: Setup
- [X] T001 Create project structure per implementation plan in `src/FurryFriends.BlazorUI` and `src/FurryFriends.BlazorUI.Client`
- [X] T002 Initialize FontAwesome 6 CSS dependency via `Components/App.razor`
- [X] T003 Set up linting and formatting tools for Razor and CSS files

## Phase 2: Core UI Components
- [X] T004 Implement HSL color palette in `wwwroot/css/color-theme.css` using dark-mode HSL(200,100%,50%) and light-mode HSL(340,100%,90%)
- [X] T005 Create SVG spritesheet for common UI elements in `wwwroot/icons/sprites.svg`
- [X] T006 Implement status pill chips component in `src/FurryFriends.BlazorUI.Client/Components/Common/StatusPill.razor`
- [X] T007 Implement skeleton loader component in `src/FurryFriends.BlazorUI.Client/Components/Common/SkeletonLoader.razor`

## Phase 3: Sidebar & Navigation (US1 - Primary User Story)
- [X] T008 Replace emoji navigation icons with FontAwesome 6 icons in `src/FurryFriends.BlazorUI/Components/Layout/NavMenu.razor`
- [X] T009 Implement dark high-contrast sidebar styling in `Components/Layout/NavMenu.razor.css`
- [X] T010 Add pill-shaped active state highlights in `Components/Layout/NavMenu.razor.css`

## Phase 4: Directory Tables (US2)
- [X] T011 Replace emoji action buttons (👁️, ✏️, +🐾) with FontAwesome icons in `src/FurryFriends.BlazorUI.Client/Pages/Clients/ClientList.razor`
- [X] T012 Add status chips for Active/Inactive/Verified in `Pages/Clients/ClientList.razor`
- [X] T013 Implement rounded borders and hover transitions in `Pages/Clients/ClientList.razor.css`
- [X] T014 Replace emoji icons in `src/FurryFriends.BlazorUI.Client/Pages/PetWalkers/PetWalkerList.razor`
- [X] T013 Add status chips for Pet Walker states in `Pages/PetWalkers/PetWalkerList.razor`

## Phase 5: Pet Walker Profile (US3)
- [X] T014 Create tabbed interface structure in `src/FurryFriends.BlazorUI.Client/Pages/PetWalkers/PetWalkerViewPopup.razor`
- [X] T015 Add "About" tab content in `Pages/PetWalkers/PetWalkerViewPopup.razor`
- [X] T016 Add "Schedule & Areas" tab content in `Pages/PetWalkers/PetWalkers/PetWalkerViewPopup.razor`
- [X] T017 Add "Reviews" tab content in `Pages/PetWalkers/PetWalkerViewPopup.razor`

## Phase 6: Performance & Loading (US4)
- [X] T018 Integrate skeleton loaders into ClientList.razor for loading states
- [X] T019 Integrate skeleton loaders into PetWalkerList.razor for loading states

## Phase 7: Micro-interactions (US5)
- [X] T020 Add hover micro-interactions to buttons in `src/FurryFriends.BlazorUI.Client/Components/Common/IconButton.razor`
- [X] T021 Add hover micro-interactions to navigation links in `src/FurryFriends.BlazorUI/Components/Layout/NavMenu.razor`

## Phase 8: Testing (TDD Required)
- [X] T022 [P] Write bUnit tests for StatusPill component in `tests/FurryFriends.UnitTests/BlazorUI/StatusPillTests.cs`
- [X] T023 [P] Write bUnit tests for SkeletonLoader component in `tests/FurryFriends.UnitTests/BlazorUI/SkeletonLoaderTests.cs`
- [X] T024 [P] Write bUnit tests for PetWalkerViewPopup tabs in `tests/FurryFriends.UnitTests/BlazorUI/PetWalkerViewPopupTests.cs`
- [X] T025 [P] Write integration tests for HSL theme switching in `tests/FurryFriends.FunctionalTests/UIThemeTests.cs`

## Phase 9: Additional UI Enhancements (Gaps Identified)
- [X] T026 Implement empty state vector placeholder graphics for tables in `Components/Common/EmptyState.razor`
- [X] T027 Style header search bar and profile dropdown with rounded boundaries per FR-010-002 in `Components/Layout/MainLayout.razor.css`
- [X] T028 Add rounded pill-shaped active state highlights to NavMenu items in `Components/Layout/NavMenu.razor.css`
- [X] T029 Add animated timeline styling for BookingFormComponent step transitions in `Components/Bookings/BookingFormComponent.razor.css`
- [X] T030 Implement responsive design breakpoints in `wwwroot/css/responsive.css`
- [X] T031 Add responsive navigation collapse for mobile view in `Components/Layout/NavMenu.razor`

## Phase 10: Final Polish & Validation
- [X] T032 Run accessibility audit for WCAG compliance
- [X] T033 Update component styles for header search bar and profile menu in `Components/Layout/MainLayout.razor.css`
- [X] T034 Remove duplication in pasted CSS across components
- [X] T035 Run manual-testing.md validation checklist

## Dependencies
- T001-T003 must complete before Phase 2
- T004-T007 must complete before Phases 3-9
- T018-T019 depend on T007 (skeleton loader component)
- T026-T030 depend on component implementations

## Parallel Opportunities
- T004-006 can execute in parallel (different file types)
- T008-010 can execute in parallel after T001-T003
- T011-014 can execute in parallel after T006
- T014-019 can execute in parallel after T001-T003
- T022-025 can execute in parallel post-implementation
- T026-031 can execute in parallel after CSS prerequisite tasks

## Notes
- Follow Blazor Hybrid UI architecture (server handles HTTP, client renders)
- Use scoped CSS (.razor.css) for component styling
- Maintain backward compatibility with existing workflows
