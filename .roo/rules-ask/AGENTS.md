# AGENTS.md (Ask Mode)

## Documentation Context

- **Architecture decisions**: See `docs/architecture-decisions/adr-001-dotnet-di-adoption.md`
- **Technical guides**: `docs/technical/` contains detailed guides on logging, data access, authentication, and development workflow
- **Feature specs**: `specs/` contains detailed specifications with contracts and tasks for each feature (e.g., `specs/003-petwalker-timeslots/`, `specs/006-petwalker-rating/`)
- **Implementation plans**: `plans/` contains analysis and approach comparisons

## Counterintuitive Organization

- **Blazor hybrid**: Server project (`FurryFriends.BlazorUI`) hosts WebAssembly client (`FurryFriends.BlazorUI.Client`). Services are registered in server but consumed by client components.
- **Endpoint location**: All API endpoints are in `src/FurryFriends.Web/Endpoints/` (FastEndpoints), not Controllers folder.
- **Use cases**: Application logic lives in `FurryFriends.UseCases` with domain-specific subfolders (e.g., `Domain/PetWalkers/Query/`, `Timeslots/Timeslot/`).
- **Test data seeding**: `SeedData.PopulateTestDataAsync()` is used by both development and functional tests, but functional tests override DbContext with in-memory DB via `CustomWebApplicationFactory`.
- **Log files**: Separate log files for Web and BlazorUI projects, not consolidated.

## Key References

- Main README: `README.md` (high-level overview)
- Testing instructions: `TESTING_INSTRUCTIONS.md` (booking components)
- Development guide: `docs/technical/5-development-guide.md`
