# Tasks: Enterprise UI Modernization

**Input**: Design documents from `/specs/010-ui-modernization/`
**Prerequisites**: plan.md, spec.md

## Format: `[ID] [P?] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- Include exact file paths in descriptions

---

## Phase 1: Setup & Design Tokens

- [x] T001 [P] Register Google Font `Inter` inside the `<head>` tag in `src/FurryFriends.BlazorUI/Components/App.razor`
- [x] T002 Refactor `src/FurryFriends.BlazorUI/wwwroot/app.css` to declare the HSL-based color tokens, elevated layout shadows, larger border radii, custom scrollbar styling, utility spacing classes, and default layout elements.

---

## Phase 2: Shell Layout Overhaul

- [x] T003 Redesign `src/FurryFriends.BlazorUI/Components/Layout/MainLayout.razor` shell hierarchy—modernizing the top-header search/actions, adding an animated notification badge, and constructing a profile menu dropdown.
- [x] T004 Update shell dimensions, backdrop filters, and overlay alignments in `src/FurryFriends.BlazorUI/Components/Layout/MainLayout.razor.css`
- [x] T005 [P] Replace emoji descriptors with FontAwesome vector icons (e.g. `fa-users`, `fa-person-walking`, `fa-calendar-days`, `fa-chart-simple`, `fa-gear`) in `src/FurryFriends.BlazorUI/Components/Layout/NavMenu.razor`
- [x] T006 Update list item active structures, text transitions, and spacing scales in `src/FurryFriends.BlazorUI/Components/Layout/NavMenu.razor.css`

---

## Phase 3: Common UI Component Foundations

- [x] T007 [P] Create a reusable `IconButton` Blazor component in `src/FurryFriends.BlazorUI.Client/Components/Common/IconButton.razor` and `src/FurryFriends.BlazorUI.Client/Components/Common/IconButton.razor.css` to wrap FontAwesome icons with transitions and custom color styles.
- [x] T008 [P] Create a reusable `SkeletonLoader` component in `src/FurryFriends.BlazorUI.Client/Components/Common/SkeletonLoader.razor` and `src/FurryFriends.BlazorUI.Client/Components/Common/SkeletonLoader.razor.css` to support pulsing table rows and grid card place-holders.

---

## Phase 4: Directories & Lists Upgrades

- [x] T009 Integrate `IconButton` actions and `SkeletonLoader` states in `src/FurryFriends.BlazorUI.Client/Pages/Clients/ClientList.razor` (replacing `👁️` / `✏️` and simple text loading).
- [x] T010 Update table row boundaries, responsive flex layouts, and hover highlighting inside `src/FurryFriends.BlazorUI.Client/Pages/Clients/ClientList.razor.css`
- [x] T011 Integrate `IconButton` actions and `SkeletonLoader` states in `src/FurryFriends.BlazorUI.Client/Pages/PetWalkers/PetWalkerList.razor`.
- [x] T012 Update table row boundaries, responsive flex layouts, and hover highlighting inside `src/FurryFriends.BlazorUI.Client/Pages/PetWalkers/PetWalkerList.razor.css`

---

## Phase 5: Booking Wizard Steppers

- [x] T013 Restructure the wizard step indicator progress headers in `src/FurryFriends.BlazorUI.Client/Components/Bookings/BookingFormComponent.razor`
- [x] T014 Implement styling for active/completed phases and connection lines in `src/FurryFriends.BlazorUI.Client/Components/Bookings/BookingFormComponent.razor.css`

---

## Phase 6: Verification

- [x] T015 Verify that the application builds successfully via `dotnet build` and ensure there are no UI rendering regressions.
