# Research Findings

## JWT Authentication Implementation Decisions

### Decision: Use Microsoft.AspNetCore.Authentication.JwtBearer
**Rationale**: This is the official Microsoft library for JWT authentication in ASP.NET Core. It provides:
- Built-in token validation with configurable parameters
- Integration with ASP.NET Core authentication middleware
- Support for symmetric and asymmetric signing
- Standard claims transformation

**Alternatives Considered**:
- IdentityServer4/Duende: Overkill for internal token issuance
- Custom JWT generation: Unnecessary complexity, security risks
- Third-party providers (Auth0, etc.): Out of scope per FR-011-013

### Decision: 8-Hour Token Expiration
**Rationale**: 
- Spec requirement FR-011-004a
- Balance between security (short expiry) and UX (not too frequent re-login)
- Standard for internal enterprise applications

### Decision: Role-Based Claims (Admin, Petwalker, Client)
**Rationale**:
- Maps directly to FR-011-006, FR-011-007, FR-011-008, FR-011-009
- Enables policy-based authorization
- Claims are included in JWT for stateless validation

### Decision: Development Mode Admin-Only Login
**Rationale**: 
- FR-011-010 requirement
- Implemented via `IHostEnvironment.IsDevelopment()` check
- Allows secure development without full user management

### Decision: Password Complexity Requirements
**Rationale**: 
- FR-011-003: >8 chars, uppercase, lowercase, numeric, special
- Implemented via FluentValidation + ASP.NET Core Identity password hashing

### Decision: [Anonymous] Attribute Pattern
**Rationale**:
- FR-011-005: Protect by default, allow explicit opt-out
- Uses FastEndpoints `AllowAnonymous()` method
- Simple, explicit, follows existing codebase patterns

## Technology Stack Confirmation

All required packages already in Directory.Packages.props:
- Microsoft.AspNetCore.Authentication.JwtBearer (9.0.x)
- Microsoft.AspNetCore.Identity.EntityFrameworkCore (9.0.x)
- Ardalis.Result (10.1.0)
- FluentValidation (11.11.0)
- MediatR (12.4.1)