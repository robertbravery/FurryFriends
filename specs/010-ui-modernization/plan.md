# Implementation Plan: Enterprise UI Modernization

**Branch**: `010-ui-modernization` | **Date**: 2026-05-23 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/010-ui-modernization/spec.md`

---

## 1. Summary
Refactor the FurryFriends layout shell, core design tokens, tables, and wizard components to bring them up to a modern, premium enterprise design standard. This plan replaces browser system fonts, rigid layouts, text-based emojis, and basic lists with unified HSL styling variables, vector-based iconography, loading skeleton overlays, and micro-animations.

---

## 2. Technical Context
* **Language/Version**: .NET 9 (Blazor Hybrid)
* **Styling**: Vanilla CSS with custom properties (CSS variables), CSS utility classes, and component-specific isolation (`.razor.css`).
* **Iconography**: FontAwesome v6.4.0 (already imported in [App.razor](file:///c:/Users/rbrav/source/repos/FurryFriends/src/FurryFriends.BlazorUI/Components/App.razor)).
* **Client Architecture**: Blazor WebAssembly (`FurryFriends.BlazorUI.Client`) rendering inside Blazor Server host (`FurryFriends.BlazorUI`).
* **Constraints**: Clean segregation between markup (`.razor`) and logic (`.razor.cs`). All style overrides must not break functional behavior or component event bindings.

---

## 3. Constitution Check

**Architecture Compliance**:
- [x] Follows Blazor Hybrid clean separation.
- [x] Keeps logic in partial code-behind classes (`.razor.cs`), markup in `.razor`.
- [x] Custom styles are component-scoped (`.razor.css`) where possible, fallback to global `app.css` variables.

**Required Patterns**:
- [x] All styles use CSS custom properties from the central theme.
- [x] Consistent responsive design layout scales (8px-grid values).
- [x] Vector icons (FontAwesome/SVG) exclusively; no raw emojis in buttons or headings.
- [x] Loading phases display structured skeletons rather than text indicators.

---

## 4. Project Structure

### Documentation & Specification
```
specs/010-ui-modernization/
├── spec.md              # Specification document (WHAT and WHY)
├── plan.md              # Implementation plan (HOW)
└── tasks.md             # Task breakdown list (TODO checklist)
```

### Affected Source Files
```
src/FurryFriends.BlazorUI/
├── Components/
│   └── Layout/
│       ├── MainLayout.razor       # Shell structure adjustments
│       ├── MainLayout.razor.css   # Shell styling (sidebar & header)
│       ├── NavMenu.razor          # Navigation layout & vector icons
│       └── NavMenu.razor.css      # Nav link styling & active state highlights
└── wwwroot/
    └── app.css                    # Design token definition & utility styles

src/FurryFriends.BlazorUI.Client/
├── Components/
│   ├── Common/
│   │   ├── SkeletonLoader.razor     # [NEW] Reusable skeleton loading screen
│   │   ├── SkeletonLoader.razor.css # [NEW] Pulse animations and structure
│   │   ├── IconButton.razor         # [NEW] Stylized button with tooltip
│   │   └── IconButton.razor.css     # [NEW] Icon button theme/sizes
│   └── Bookings/
│       ├── BookingFormComponent.razor     # Timeline stepper markup
│       └── BookingFormComponent.razor.css # Stepper connection lines & visual states
└── Pages/
    ├── Clients/
    │   ├── ClientList.razor       # Replace emojis, add table spacing
    │   └── ClientList.razor.css   # Directory table borders and overrides
    └── PetWalkers/
        ├── PetWalkerList.razor    # Replace emojis, add layout spacing
        └── PetWalkerList.razor.css# Directory table borders and overrides
```

---

## 5. Development Phases

### Phase 0: Setup & Tokens
1. Update [app.css](file:///c:/Users/rbrav/source/repos/FurryFriends/src/FurryFriends.BlazorUI/wwwroot/app.css) to establish a comprehensive HSL theme system, utility spacings, modern transitions, and standard overlay shadows.
2. Link Google Font `Inter` in [App.razor](file:///c:/Users/rbrav/source/repos/FurryFriends/src/FurryFriends.BlazorUI/Components/App.razor) for professional typeface rendering.

### Phase 1: Layout Shell Overhaul
1. Modernize [MainLayout.razor](file:///c:/Users/rbrav/source/repos/FurryFriends/src/FurryFriends.BlazorUI/Components/Layout/MainLayout.razor) and [MainLayout.razor.css](file:///c:/Users/rbrav/source/repos/FurryFriends/src/FurryFriends.BlazorUI/Components/Layout/MainLayout.razor.css) for a clean dark sidebar and floating profile header menu.
2. Refactor [NavMenu.razor](file:///c:/Users/rbrav/source/repos/FurryFriends/src/FurryFriends.BlazorUI/Components/Layout/NavMenu.razor) with FontAwesome icons (e.g., `fa-users`, `fa-walking`, `fa-calendar-days`) and hover pills.

### Phase 2: Common Component Creation
1. Implement a reusable [SkeletonLoader.razor](file:///c:/Users/rbrav/source/repos/FurryFriends/src/FurryFriends.BlazorUI.Client/Components/Common/SkeletonLoader.razor) component supporting both `Table` and `Card` skeleton variations.
2. Create [IconButton.razor](file:///c:/Users/rbrav/source/repos/FurryFriends/src/FurryFriends.BlazorUI.Client/Components/Common/IconButton.razor) to encapsulate vector action triggers.

### Phase 3: Directory Pages Upgrades
1. Rewrite [ClientList.razor](file:///c:/Users/rbrav/source/repos/FurryFriends/src/FurryFriends.BlazorUI.Client/Pages/Clients/ClientList.razor) table rows and action columns to utilize `IconButton` components and skeleton rows.
2. Refactor [PetWalkerList.razor](file:///c:/Users/rbrav/source/repos/FurryFriends/src/FurryFriends.BlazorUI.Client/Pages/PetWalkers/PetWalkerList.razor) tables with similar styling.

### Phase 4: Form Steppers & Wizards
1. Modernize the booking wizard step header in [BookingFormComponent.razor](file:///c:/Users/rbrav/source/repos/FurryFriends/src/FurryFriends.BlazorUI.Client/Components/Bookings/BookingFormComponent.razor), transforming the indicator to a connected progress line with highlight states.

### Phase 5: Verification & Polish
1. Run functional test check to ensure no logic regressions.
2. Validate responsiveness across desktop, tablet, and mobile layouts.
