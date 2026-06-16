# Tasks for JWT Authentication with Anonymous Access

## Dependencies

- Core Layer files must exist before UseCases can reference them.
- UseCases must be completed before Infrastructure can map entities and provide services.
- Infrastructure services must be registered before Web API endpoints can use them.
- UI components depend on the Web API endpoints and AuthService.
- Tests depend on the implementation files they target.

## Parallelizable Tasks

- Core Layer file creations are independent and can run in parallel.
- UI page components are independent of each other and can run in parallel.
- Test files can be authored in parallel after their corresponding implementation files exist.

---

## Setup

- [ ] T001 Ensure .NET 9 SDK is installed and the solution builds.
- [ ] T002 Add required NuGet packages: FastEndpoints, MediatR, FluentValidation, Ardalis.Specification, Ardalis.Result, Ardalis.GuardClauses, Serilog, Microsoft.AspNetCore.Authentication.JwtBearer.

---

## Foundational

- [ ] T003 Create `src/FurryFriends.Core/UserAggregate/User.cs` – define User entity with Email, PasswordHash, Role, and registration metadata.
- [ ] T004 Create `src/FurryFriends.Core/UserAggregate/Specifications/UserByEmailSpec.cs` – specification for email lookup.
- [ ] T005 Create `src/FurryFriends.Core/UserAggregate/Events/UserAuthenticatedEvent.cs` – domain event for successful authentication.

---

## Core Work

- [ ] T006 Create `src/FurryFriends.UseCases/User/Registration/RegisterCommand.cs` – command DTO for user registration.
- [ ] T007 Create `src/FurryFriends.UseCases/User/Registration/RegisterCommandHandler.cs` – handler that creates User, hashes password, assigns role.
- [ ] T008 Create `src/FurryFriends.UseCases/User/Registration/RegisterCommandValidator.cs` – FluentValidation rules for email format and password complexity.
- [ ] T009 Create `src/FurryFriends.UseCases/User/Registration/ExtendedUserMetadata.cs` – model for additional client/petwalker details.

---

## Infrastructure

- [ ] T010 Create `src/FurryFriends.Infrastructure/Data/Configurations/UserConfiguration.cs` – EF Core mapping for User entity.
- [ ] T011 Create `src/FurryFriends.Infrastructure/Services/JwtTokenService.cs` – service to generate and validate JWT tokens.

---

## Web API Layer

- [ ] T012 Create `src/FurryFriends.Web/Endpoints/Auth/Login/LoginRequest.cs` – request DTO for login.
- [ ] T013 Create `src/FurryFriends.Web/Endpoints/Auth/Login/LoginResponse.cs` – response DTO containing JWT token.
- [ ] T014 Create `src/FurryFriends.Web/Endpoints/Auth/Login/Login.cs` – FastEndpoints endpoint handling login logic.
- [ ] T015 Update `src/FurryFriends.Web/Program.cs` – configure JWT authentication middleware and register JwtTokenService.
- [ ] T016 Create `src/FurryFriends.Web/Endpoints/Auth/Register/RegisterRequest.cs` – request DTO for registration.
- [ ] T017 Create `src/FurryFriends.Web/Endpoints/Auth/Register/RegisterResponse.cs` – response DTO confirming registration.
- [ ] T018 Create `src/FurryFriends.Web/Endpoints/Auth/Register/Register.cs` – FastEndpoints endpoint handling user registration.

---

## Blazor UI Layer

- [ ] T019 Create `src/FurryFriends.BlazorUI/Pages/Login.razor` – UI markup for login form with email/password fields.
- [ ] T020 Create `src/FurryFriends.BlazorUI/Pages/Login.razor.cs` – code‑behind handling form submission, calling login endpoint, storing JWT.
- [ ] T021 Create `src/FurryFriends.BlazorUI/Pages/Register.razor` – UI markup for registration form with role selection.
- [ ] T022 Create `src/FurryFriends.BlazorUI/Pages/Register.razor.cs` – code‑behind handling registration flow.
- [ ] T023 Update `src/FurryFriends.BlazorUI/Shared/NavMenu.razor` – add role‑based navigation links for Admin, Petwalker, Client.
- [ ] T024 Create `src/FurryFriends.BlazorUI/Services/AuthService.cs` – service to manage JWT token, authentication state, and role checks.

---

## Testing

- [ ] T025 Create `tests/FurryFriends.UnitTests/UseCases/User/Registration/RegisterCommandHandlerTests.cs` – unit tests for registration handler.
- [ ] T026 Create `tests/FurryFriends.IntegrationTests/AuthEndpoints/RegisterEndpointTests.cs` – integration tests for registration endpoint.
- [ ] T027 Create `tests/FurryFriends.IntegrationTests/AuthEndpoints/LoginEndpointTests.cs` – integration tests for login endpoint.

---

## Polish

- [ ] T028 Update documentation to reflect new authentication flow and UI components.
- [ ] T029 Verify all tests pass and meet the Success Criteria in the spec.
- [ ] T030 Perform a code review and address any static analysis warnings.
