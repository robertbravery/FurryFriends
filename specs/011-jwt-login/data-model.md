# Data Model: JWT Authentication with Anonymous Access

## Entities Extracted from Feature Spec

### User
**Purpose**: Login identity with email, password hash, role claims, and profile linkage.

**Fields**:
- `Id` (Guid) - Primary key
- `Email` (string) - Unique login identifier
- `PasswordHash` (string) - Hashed password
- `Role` (enum/string) - Admin, Petwalker, Client
- `ProfileId` (Guid?) - Links to Client or PetWalker profile
- `ProfileType` (enum) - Client or PetWalker
- `IsLockedOut` (bool) - Account lockout state
- `FailedLoginAttempts` (int) - Failed login tracking
- `LastLoginAt` (DateTime?) - Last successful login
- `CreatedAt` (DateTime) - Registration timestamp
- `UpdatedAt` (DateTime) - Last update timestamp

**Validation Rules**:
- Email must be valid format (FR-011-002)
- Password must be >8 chars with uppercase, lowercase, numeric, special (FR-011-003)
- Role must be one of: Admin, Petwalker, Client (FR-011-006)
- Profile linkage required after registration (FR-011-011)

**Relationships**:
- One-to-one with Client profile (when ProfileType = Client)
- One-to-one with PetWalker profile (when ProfileType = PetWalker)
- Many-to-many with roles via claims (Admin can have multiple roles)

**State Transitions**:
- `Created` → `EmailVerified` → `Authenticated`
- `Authenticated` → `LockedOut` (after failed attempts threshold)
- `LockedOut` → `Created` (after unlock/reset)

### JWT Token
**Purpose**: Self-issued bearer token containing authenticated user identity and role claims.

**Fields**:
- `Token` (string) - JWT string
- `IssuedAt` (DateTime) - Issuance timestamp
- `ExpiresAt` (DateTime) - 8-hour expiration (FR-011-004a)
- `Issuer` (string) - Internal issuer
- `Audience` (string) - API audience
- `UserId` (Guid) - Subject claim
- `RoleClaims` (string[]) - Role claims array

**Validation Rules**:
- Must expire after 8 hours (FR-011-004a)
- Must contain userId, role, and expiration claims
- Must be signed with internal key (FR-011-013)

### Role
**Purpose**: Permission group assigned to a user.

**Values**:
- Admin
- Petwalker
- Client

**Access Rules**:
- Admin: Full access to all features and all data (FR-011-009)
- Petwalker: Own clients and petwalker-scoped data only (FR-011-007)
- Client: Own profile details + browse petwalker listings (FR-011-008)

### Protected Endpoint
**Purpose**: API route that requires authentication unless explicitly marked with `[Anonymous]`.

**Attributes**:
- `Route` (string)
- `RequiresAuth` (bool) - default true
- `AllowAnonymous` (bool) - explicit opt-out
- `RequiredRoles` (string[]) - optional role restrictions

### Anonymous Endpoint
**Purpose**: Protected endpoint with `[Anonymous]` attribute applied.

**Rules**:
- Must be explicitly marked with `[Anonymous]`
- Allows unauthenticated access
- Used for public listing pages and login/register flows

## Blazor Hybrid UI Model

### Components
**Login.razor**:
- Email/password form
- Validation messages
- Navigation to registration
- JWT storage coordination

**Register.razor**:
- Role selection (Client/Petwalker)
- Profile creation flow
- Email/password validation
- Success confirmation

**NavMenu.razor**:
- Role-based navigation links
- Hide/show Admin, Petwalker, Client sections
- Authenticated state display

### Services
**AuthService.cs**:
- Token storage
- Authentication state management
- Role checks
- Logout handling
- HTTP header injection for API calls

## API Contracts

### Login Endpoint
- POST `/api/auth/login`
- Request: Email, Password
- Response: Token, ExpiresAt, Role

### Register Endpoint
- POST `/api/auth/register`
- Request: Email, Password, Role, ProfileDetails
- Response: UserId, Role, ProfileCreated

### Protected Endpoint Pattern
- All endpoints require authentication by default
- Use `[AllowAnonymous]` only where explicitly required
- Role-based authorization via claims

## Testing Strategy

### Unit Tests
- User entity validation
- Password hashing
- JWT token generation
- Role claim parsing

### Integration Tests
- Login with valid credentials
- Login with invalid credentials
- Protected endpoint without token
- Anonymous endpoint without token
- Expired token handling

### bUnit Tests
- Login form validation
- Register form role selection
- NavMenu role-based rendering
- Error state display
