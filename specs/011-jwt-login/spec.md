# Feature Specification: JWT Authentication with Anonymous Access

**Feature Branch**: `011-jwt-login`
**Created**: 2026-06-16
**Status**: Draft
**Input**: User description: login with jwt tokens. Api should be protected unless they have an anonymouse attribute. Create a new branch following the current branch specification and number from previous specifications

---

## Overview

The system provides JWT-based authentication allowing users to sign in with email and password to receive a time-limited bearer token. All API endpoints are protected by default; only endpoints explicitly marked with an `[Anonymous]` attribute permit unauthenticated access. Role-based access control restricts data visibility per role: Admin (full access), Petwalker (own clients and scoped data), Client (own profile and petwalker listings).

---

## Functional Requirements

- **FR-001**: The system MUST allow users to sign in using an email and password.
- **FR-002**: The system MUST validate email addresses for correct syntax before authentication.
- **FR-003**: The system MUST enforce password complexity: more than 8 characters, with uppercase, lowercase, numeric, and special characters. Mandatory complexity requirements: minimum 256-bit entropy enforced via _PasswordComplexityValidator._
- **FR-004**: The system MUST issue internally generated JWT tokens that expire after 8 hours. Tokens must be cryptographically signed and validated on each request.
- **FR-005**: The system MUST protect API endpoints by default and only allow anonymous access on explicitly marked public endpoints via the `[Anonymous]` attribute.
- **FR-006**: The system MUST assign the authenticated user one of the role groups: Admin, Petwalker, or Client based on token claims.
- **FR-007**: The system MUST restrict Petwalker users to viewing only their own clients and petwalker-scoped data.
- **FR-008**: The system MUST restrict Client users to viewing only their own profile details while allowing them to browse petwalker listings.
- **FR-009**: The system MUST allow Admin users full access to all features and all data.
- **FR-010**: In development mode the system MUST allow login only for Admin users.
- **FR-011**: The system MUST link authenticated users to their corresponding Client or Petwalker profile and provide a path to create a missing profile.
- **FR-012**: The system MUST return a clear invalid-login or unauthorized response on authentication failure.
- **FR-013**: The system MUST use an in-process JWT provider configurable via application settings. Configuration MUST include: `Jwt:SigningKey`, `Jwt:Issuer`, `Jwt:Audience`, and `Jwt:ExpiryHours`, with fallback to environment variables for production security.
- **FR-014**: The system MUST reject expired or invalid tokens with a 401 Unauthorized response. Token validation MUST check: (1) 8-hour expiry window with ±5min tolerance, (2) cryptographic signature verification, (3) issuer/audience validation, (4) proper token structure using JWT standards.
- **FR-015**: The system MUST support user registration with role selection (Client or Petwalker) and profile creation.

---

## Success Criteria

- **SC-001**: Valid credentials produce a JWT token within 500ms under normal load.
- **SC-002**: Expired or invalid tokens are rejected with 401 Unauthorized on 100% of protected endpoint requests.
- **SC-003**: Endpoints marked with `[Anonymous]` return 200 OK without authentication on 100% of requests.
- **SC-004**: Role-based data restrictions are enforced: Petwalker sees only own data, Client sees only own profile + listings, Admin sees all.
- **SC-005**: Development-mode Admin-only login restriction is active when environment is Development.
- **SC-006**: Password complexity requirements reject non-compliant passwords on 100% of registration attempts.
- **SC-007**: Email syntax validation rejects malformed addresses on 100% of registration/login attempts.
- **SC-008**: User-profile linkage succeeds for 100% of authenticated users with existing profiles.
- **SC-009**: Token expiry occurs at 8 hours ± 5 minutes from issuance.

---

## Assumptions

- A user store exists for email, password hash, and role claims.
- JWT tokens are issued and validated internally without relying on an external identity provider.
- Development mode is detected via standard ASP.NET Core environment configuration.
- The domain model supports linking a user to a Client or Petwalker profile record.
- Client and Petwalker users have role-specific views and permissions.
- Endpoints marked with `[Anonymous]` override default protection rules.
- No refresh token mechanism is implemented in this version (short-lived tokens only).
- Password hashing uses ASP.NET Core Identity's default PBKDF2 implementation.
- The `[Anonymous]` attribute maps to FastEndpoints' `AllowAnonymous()` method.
- Application settings for JWT (issuer, audience, signing key) are provided via standard configuration.