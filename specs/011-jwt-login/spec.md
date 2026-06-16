# Feature Specification: JWT Authentication with Anonymous Access

**Feature Branch**: `011-jwt-login`
**Created**: 2026-06-16
**Status**: Draft
**Input**: User description: login with jwt tokens. Api should be protected unless they have an anonymouse attribute. Create a new branch following the current branch specification and number from previous specifications

---

## User Scenarios & Testing *(mandatory)*

### Primary User Story
Users access the system and authenticate via JWT tokens. Authenticated users get full API access, while anonymous users can only access endpoints marked as public with the `[Anonymous]` attribute.

### Acceptance Scenarios
1. **Given** a user submits valid credentials, **When** they log in, **Then** they receive a JWT token and are authenticated for subsequent protected requests.
2. **Given** a user holds a valid JWT token, **When** they access a protected endpoint with the `[Anonymous]` attribute, **Then** they can access it regardless of authentication status.
3. **Given** a user requests a protected endpoint without authentication, **When** the endpoint has no `[Anonymous]` flag, **Then** they receive a 401 Unauthorized response.

### Edge Cases
- When an `[Anonymous]`-marked endpoint is accessed without authentication, the system must allow access if the attribute is correctly applied.
- If an endpoint claims `[Anonymous]` but lacks the attribute, the system treats it as protected.
- When a JWT token expires (after 8 hours), the user must re-authenticate to obtain a new valid token.
- If a JWT token is used after expiration, the system returns a 401 Unauthorized response.

## Requirements *(mandatory)*

### Functional Requirements
- **FR-011-001**: The system MUST allow users to sign in using an email and password.
- **FR-011-002**: The system MUST validate email addresses for correct syntax before authentication.
- **FR-011-003**: The system MUST enforce password complexity: more than 8 characters, with uppercase, lowercase, numeric, and special characters.
- **FR-011-004**: The system MUST issue internally generated JWT tokens that expire after 8 hours. Tokens must be signed with a strong secret and validated on each request.

- **FR-011-005**: The system MUST protect API endpoints by default and only allow anonymous access on explicitly marked public endpoints via the `[Anonymous]` attribute.
- **FR-011-006**: The system MUST interpret JWT claims to assign the authenticated user one of the role groups: Admin, Petwalker, or Client.
- **FR-011-007**: The system MUST restrict Petwalker users to viewing only their own clients and petwalker-scoped data.
- **FR-011-008**: The system MUST restrict Client users to viewing only their own profile details while allowing them to browse petwalker listings.
- **FR-011-009**: The system MUST allow Admin users full access to all features and all data.
- **FR-011-010**: In development mode the system MUST allow login only for Admin users. Implementation must use GuardClauses (e.g., Guard.Against.Null) rather than explicit null checks.
- **FR-011-011**: The system MUST link authenticated users to their corresponding Client or Petwalker profile and provide a path to create a missing profile.
- **FR-011-012**: The system MUST return a clear invalid-login or unauthorized response on authentication failure.
- **FR-011-013**: The system MUST use an in-process JWT provider (JwtSecurityTokenHandler) encapsulated in JwtTokenService.cs. The implementation must be configurable via appsettings.json and used for both token generation and validation.

### Key Entities
- **User**: A login identity with email, password hash, and role claims.
- **JWT Token**: A self-issued bearer token containing authenticated user identity and role claims, valid for 8 hours.
- **Role**: The permission group assigned to a user: Admin, Petwalker, or Client.
- **Protected Endpoint**: An API route that requires authentication unless explicitly marked with the `[Anonymous]` attribute.
- **Anonymous Endpoint**: A protected endpoint with the `[Anonymous]` attribute applied to its controller/action.

---

## Assumptions
- A user store exists for email, password hash, and role claims.
- JWT tokens are issued and validated internally without relying on an external provider.
- Development mode is configured for Admin-only login behavior.
- The domain model supports linking a user to a Client or Petwalker profile record.
- Client and Petwalker users have role-specific views and permissions.
- Endpoints marked with `[Anonymous]` override default protection rules.
- No refresh token mechanism is implemented in this version (short-lived tokens only).