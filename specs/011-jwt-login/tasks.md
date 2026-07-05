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
- Parallel CI/CD pipeline tasks (build, tests, publish) can run concurrently.
- Security validation checks across multiple layers can run in parallel.

---

## Setup

- [ ] T001 Ensure .NET 9 SDK is installed and the solution builds.
- [ ] T002 Add required NuGet packages: FastEndpoints, MediatR, FluentValidation, Ardalis.Specification, Ardalis.Result, Ardalis.GuardClauses, Serilog, Microsoft.AspNetCore.Authentication.JwtBearer, Microsoft.AspNetCore.Http.Abstractions, Microsoft.AspNetCore.Authorization.

---

## Foundational

- [ ] T003 Create `src/FurryFriends.Core/UserAggregate/User.cs` – define User entity with Email, PasswordHash, Role, ProfileId, ProfileType, and lockout tracking
- [ ] T004 Create `src/FurryFriends.Core/UserAggregate/Specifications/UserByEmailSpec.cs` – specification for email lookup
- [ ] T005 Create `src/FurryFriends.Core/UserAggregate/Events/UserAuthenticatedEvent.cs` – domain event for successful authentication
- [ ] T026 Create `src/FurryFriends.Core/UserAggregate/GuardClauses/UserGuardClauses.cs` – domain guard clauses for user validation.

---

## Core Work

- [ ] T006 Create `src/FurryFriends.UseCases/User/Registration/RegisterCommand.cs` – command DTO for user registration.
- [ ] T007 Create `src/FurryFriends.UseCases/User/Registration/RegisterCommandHandler.cs` – handler that creates User, hashes password, assigns role.
- [ ] T008 Create `src/FurryFriends.UseCases/User/Registration/RegisterCommandValidator.cs` – FluentValidation rules for email format and password complexity.
- [ ] T009 Create `src/FurryFriends.UseCases/User/Registration/ExtendedUserMetadata.cs` – model for additional client/petwalker details.
- [ ] T033 Create `src/FurryFriends.UseCases/User/Authentication/LoginCommand.cs` – command DTO for user login.
- [ ] T034 Create `src/FurryFriends.UseCases/User/Authentication/LoginCommandHandler.cs` – handler that validates credentials and returns JWT.
- [ ] T035 Create `src/FurryFriends.UseCases/User/Authentication/LoginCommandValidator.cs` – FluentValidation rules for login credentials.
- [ ] T036 Create `src/FurryFriends.UseCases/User/Authentication/JwtResponse.cs` – response model for JWT tokens.

---

## Infrastructure

- [ ] T010 Create `src/FurryFriends.Infrastructure/Data/Configurations/UserConfiguration.cs` – EF Core mapping for User entity.
- [ ] T011 Create `src/FurryFriends.Infrastructure/Services/JwtTokenService.cs` – service to generate and validate JWT tokens.
- [ ] T032 Create `src/FurryFriends.Infrastructure/Configurations/AuthConfiguration.cs` – configuration for authentication services.
- [ ] T037 Create `src/FurryFriends.Infrastructure/Validators/PasswordComplexityValidator.cs` – validator for password complexity requirements.

---

## Web API Layer

- [ ] T012 Create `src/FurryFriends.Web/Endpoints/Auth/Login/LoginRequest.cs` – request DTO for login.
- [ ] T013 Create `src/FurryFriends.Web/Endpoints/Auth/Login/LoginResponse.cs` – response DTO containing JWT token.
- [ ] T014 Create `src/FurryFriends.Web/Endpoints/Auth/Login/Login.cs` – FastEndpoints endpoint handling login logic.
- [ ] T015 Update `src/FurryFriends.Web/Program.cs` – configure JWT authentication middleware and register JwtTokenService.
- [ ] T038 Create `src/FurryFriends.Web/Configurations/IdensenConfiguration.cs` – configuration for identity services.
- [ ] T016 Create `src/FurryFriends.Web/Endpoints/Auth/Register/RegisterRequest.cs` – request DTO for registration.
- [ ] T017 Create `src/FurryFriends.Web/Endpoints/Auth/Register/RegisterResponse.cs` – response DTO confirming registration.
- [ ] T018 Create `src/FurryFriends.Web/Endpoints/Auth/Register/Register.cs` – FastEndpoints endpoint handling user registration.
- [ ] T039 Create `src/FurryFriends.Web/Endpoints/Auth/Logout/Logout.cs` – FastEndpoints endpoint for token logout.
- [ ] T040 Create `src/FurryFriends.Web/Endpoints/Auth/Refresh/RefreshRequest.cs` – request DTO for token refresh.
- [ ] T041 Create `src/FurryFriends.Web/Endpoints/Auth/Refresh/RefreshResponse.cs` – response DTO containing refreshed JWT.
- [ ] T042 Create `src/FurryFriends.Web/Endpoints/Auth/Refresh/Refresh.cs` – FastEndpoints endpoint handling token refresh.

---

## Blazor UI Layer

- [ ] T019 Create `src/FurryFriends.BlazorUI/Pages/Login.razor` – UI markup for login form with email/password fields.
- [ ] T020 Create `src/FurryFriends.BlazorUI/Pages/Login.razor.cs` – code‑behind handling form submission, calling login endpoint, storing JWT.
- [ ] T043 Create `src/FurryFriends.BlazorUI/Pages/RefreshToken.razor` – UI component for handling token refresh.
- [ ] T044 Create `src/FurryFriends.BlazorUI/Pages/RefreshToken.razor.cs` – code‑behind for silent token refresh.
- [ ] T021 Create `src/FurryFriends.BlazorUI/Pages/Register.razor` – UI markup for registration form with role selection.
- [ ] T022 Create `src/FurryFriends.BlazorUI/Pages/Register.razor.cs` – code‑behind handling registration flow.
- [ ] T045 Create `src/FurryFriends.BlazorUI/Services/IInterceptorService.cs` – service for HTTP request/response interception.
- [ ] T046 Create `src/FurryFriends.BlazorUI/Services/InterceptorService.cs` – implementation of response interceptor for auto-refresh.
- [ ] T023 Update `src/FurryFriends.BlazorUI/Shared/NavMenu.razor` – add role‑based navigation links for Admin, Petwalker, Client.
- [ ] T024 Create `src/FurryFriends.BlazorUI/Services/AuthService.cs` – service to manage JWT token, authentication state, and role checks.

---

## Testing

- [ ] T025 Create `tests/FurryFriends.UnitTests/UseCases/User/Registration/RegisterCommandHandlerTests.cs` – unit tests for registration handler.
- [ ] T026 Create `tests/FurryFriends.UnitTests/User/GuardClauses/UserGuardClausesTests.cs` – unit tests for user guard clauses.
- [ ] T027 Create `tests/FurryFriends.UnitTests/Authentication/LoginCommandHandlerTests.cs` – unit tests for login handler.
- [ ] T028 Create `tests/FurryFriends.IntegrationTests/AuthEndpoints/RegisterEndpointTests.cs` – integration tests for registration endpoint.
- [ ] T029 Create `tests/FurryFriends.IntegrationTests/AuthEndpoints/LoginEndpointTests.cs` – integration tests for login endpoint.
- [ ] T030 Create `tests/FurryFriends.IntegrationTests/AuthEndpoints/RefreshEndpointTests.cs` – integration tests for token refresh.
- [ ] T031 Create `tests/FurryFriends.IntegrationTests/AuthEndpoints/LogoutEndpointTests.cs` – integration tests for logout.

---

## CI/CD and Security

- [ ] T047 Create pipeline configuration for CI/CD, including security scan stage.
- [ ] T048 Implement GitHub Actions workflow for automated testing and deployment.
- [ ] T049 Add OWASP security scan configuration for JWT authentication.
- [ ] T050 Create rate limiting configuration for authentication endpoints.
- [ ] T051 Create logging configuration for authentication events and security alerts.

---

## Polish

- [ ] T052 Update documentation to reflect new authentication flow and UI components.
- [ ] T053 Verify all tests pass and meet the Success Criteria in the spec.
- [ ] T054 Perform a code review and address any static analysis warnings.
- [ ] T055 Run OWASP security scan and fix findings.
- [ ] T056 Add dependency scan for NuGet Serilog packages used by logging.
- [ ] T057 Create security incident response plan for authentication service.
