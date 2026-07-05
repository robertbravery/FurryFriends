# Implementation Plan: JWT Authentication with Anonymous Access

**Branch**: `011-jwt-login` | **Date**: 2026-07-05 | **Spec**: [spec.md](spec.md)

---

## Summary

Implement JWT-based authentication for FurryFriends API with role-based access control (Admin, Petwalker, Client). All endpoints protected by default; `[Anonymous]` attribute explicitly opts out. Users authenticate via email/password, receive 8-hour signed JWT tokens. Integrates with existing AspNetCore.Identity user store, follows Clean Architecture layers (Core → UseCases → Infrastructure → Web → BlazorUI). Uses FastEndpoints for API, MediatR for CQRS, Ardalis.Result/GuardClauses/Specification, FluentValidation, Serilog.

---

## Technical Context

| Aspect               | Detail                                                                                                                                                                 |
| -------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Language/Version     | .NET 9 (FurryFriends standard)                                                                                                                                         |
| Primary Dependencies | FastEndpoints, MediatR, FluentValidation, Ardalis.Specification, Ardalis.Result, Ardalis.GuardClauses, Serilog, Microsoft.AspNetCore.Authentication.JwtBearer, EF Core |
| Storage | SQL Server with EF Core (Production), InMemory (Testing) |
| Testing              | xUnit (unit/integration), bUnit (Blazor components)                                                                                                                    |
| Target Platform      | Web application (Blazor Hybrid)                                                                                                                                        |
| Project Type         | Web (backend API + Blazor UI)                                                                                                                                          |
| Performance Goals    | <200ms API response time                                                                                                                                               |
| Constraints          | Integrate with AspNetCore.Identity, Clean Architecture layers, Directory.Packages.props for versions                                                                   |
| Scale/Scope          | Auth middleware, user endpoints, existing protected endpoints                                                                                                          |

---

## Approach & Structure

### Files to Create/Modify (dependency order)

**Core Layer** (no dependencies):

- `src/FurryFriends.Core/UserAggregate/User.cs` — User entity with Email, PasswordHash, Role, ProfileId, ProfileType, lockout tracking
- `src/FurryFriends.Core/UserAggregate/Specifications/UserByEmailSpec.cs` — Email lookup specification
- `src/FurryFriends.Core/UserAggregate/Events/UserAuthenticatedEvent.cs` — Domain event for successful auth

**UseCases Layer** (depends on Core):

- `src/FurryFriends.UseCases/User/Registration/RegisterCommand.cs` — Registration command DTO
- `src/FurryFriends.UseCases/User/Registration/RegisterCommandHandler.cs` — Creates User, hashes password, assigns role, links profile
- `src/FurryFriends.UseCases/User/Registration/RegisterCommandValidator.cs` — FluentValidation: email format, password complexity (>8 chars, upper/lower/digit/special)
- `src/FurryFriends.UseCases/User/Login/LoginQuery.cs` — Login query DTO
- `src/FurryFriends.UseCases/User/Login/LoginQueryHandler.cs` — Validates credentials, generates JWT via JwtTokenService
- `src/FurryFriends.UseCases/User/Login/LoginQueryValidator.cs` — FluentValidation for login request

**Infrastructure Layer** (depends on Core):

- `src/FurryFriends.Infrastructure/Data/Configurations/UserConfiguration.cs` — EF Core mapping for User
- `src/FurryFriends.Infrastructure/Services/JwtTokenService.cs` — Generates/validates JWT tokens; reads config from appsettings.json (Issuer, Audience, SigningKey, ExpiryHours)

**Web API Layer** (depends on Infrastructure):

- `src/FurryFriends.Web/Endpoints/Auth/Login/LoginRequest.cs` — Login endpoint request DTO
- `src/FurryFriends.Web/Endpoints/Auth/Login/LoginResponse.cs` — Login response with Token, ExpiresAt, Role
- `src/FurryFriends.Web/Endpoints/Auth/Login/Login.cs` — FastEndpoints endpoint: POST /api/auth/login, AllowAnonymous
- `src/FurryFriends.Web/Endpoints/Auth/Register/RegisterRequest.cs` — Registration request DTO
- `src/FurryFriends.Web/Endpoints/Auth/Register/RegisterResponse.cs` — Registration response
- `src/FurryFriends.Web/Endpoints/Auth/Register/Register.cs` — FastEndpoints endpoint: POST /api/auth/register, AllowAnonymous
- `src/FurryFriends.Web/Program.cs` — Add JWT auth middleware, configure JwtBearer options, register JwtTokenService, add authorization policies (Admin, Petwalker, Client), map endpoints

**Blazor UI Layer** (depends on Web API):

- `src/FurryFriends.BlazorUI/Pages/Login.razor` + `.razor.cs` — Login form, calls API, stores JWT
- `src/FurryFriends.BlazorUI/Pages/Register.razor` + `.razor.cs` — Registration form with role selection (Client/Petwalker)
- `src/FurryFriends.BlazorUI/Shared/NavMenu.razor` — Role-based navigation links
- `src/FurryFriends.BlazorUI/Services/AuthService.cs` — Token storage, auth state, role checks, HTTP header injection

---

## Constitution Check

| Requirement                   | Status | Notes                                                                                              |
| ----------------------------- | ------ | -------------------------------------------------------------------------------------------------- |
| Clean Architecture layers     | ✅     | Core → UseCases → Infrastructure → Web → BlazorUI                                                  |
| FastEndpoints pattern         | ✅     | Request/Response/Endpoint separation                                                               |
| Blazor server-side HTTP       | ✅     | No direct HttpClient in client                                                                     |
| CQRS with MediatR             | ✅     | Commands for writes, Queries for reads                                                             |
| Repository + Specification    | ✅     | Ardalis.Specification for queries                                                                  |
| Ardalis.Result returns        | ✅     | All handlers                                                                                       |
| FluentValidation validators   | ✅     | All commands/queries                                                                               |
| Guard Clauses                 | ✅     | Ardalis.GuardClauses for preconditions                                                             |
| Serilog + OpenTelemetry       | ✅     | Structured logging                                                                                 |
| Test-First (unit/integration) | ✅     | Tests before implementation                                                                        |
| bUnit component tests         | ⚠️     | Deferred to implementation phase                                                                   |
| Contract tests                | ⚠️     | Create with contracts/ if API contracts exposed                                                    |
| CI/CD: build passes           | ⚠️     | Verify on implement                                                                                |
| CI/CD: 80% coverage           | ⚠️     | Verify on implement                                                                                |
| Directory.Packages.props only | ✅     | No package versions in .csproj                                                                     |
| No prohibited practices       | ✅     | No direct SQL, Console.WriteLine, exceptions for expected failures, manual nulls, LINQ in handlers |

**Violations Requiring Justification**: None — all requirements satisfied or deferred with documented justification.

---

## Out of Scope

- Refresh token mechanism (short-lived tokens only)
- External identity providers (Auth0, IdentityServer, etc.)
- Email verification flow
- Password reset/forgot password
- Multi-factor authentication
- Rate limiting on auth endpoints (may be added later)
- User profile management UI beyond registration

---

## Decisions (folded from research.md)

- **JWT Library**: Microsoft.AspNetCore.Authentication.JwtBearer — standard, full framework integration
- **Token Expiry**: 8 hours (configurable via appsettings.json)
- **Signing**: Symmetric key from configuration (appsettings.json: `Jwt:SigningKey`)
- **Role Claims**: Included in JWT for stateless authorization
- **Development Mode**: Admin-only login via `IHostEnvironment.IsDevelopment()` check
- **Anonymous Pattern**: FastEndpoints `AllowAnonymous()` on login/register endpoints
- **Password Hashing**: ASP.NET Core Identity PBKDF2 via `IPasswordHasher<User>`
