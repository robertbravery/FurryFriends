# Implementation Plan: JWT Authentication with Anonymous Access

**Branch**: `011-jwt-login` | **Date**: 2026-06-16 | **Spec**: [spec.md](spec.md)
**Input**: Feature specification from `/specs/011-jwt-login/spec.md`

## Execution Flow (/plan command scope)

```
1. Load feature spec from Input path
   → If not found: ERROR "No feature spec at {path}"
2. Fill Technical Context (scan for NEEDS CLARIFICATION)
   → Detect Project Type from file system structure or context (web=frontend+backend, mobile=app+api)
   → Set Structure Decision based on project type
3. Fill the Constitution Check section based on the content of the constitution document.
4. Evaluate Constitution Check section below
   → If violations exist: Document in Complexity Tracking
   → If no justification possible: ERROR "Simplify approach first"
   → Update Progress Tracking: Initial Constitution Check
5. Execute Phase 0 → research.md
   → If NEEDS CLARIFICATION remain: ERROR "Resolve unknowns"
6. Execute Phase 1 → contracts, data-model.md, quickstart.md, agent-specific template file
7. Re-evaluate Constitution Check section
   → If new violations: Refactor design, return to Phase 1
   → Update Progress Tracking: Post-Design Constitution Check
8. Plan Phase 2 → Describe task generation approach (DO NOT create tasks.md)
9. STOP - Ready for /tasks command
```

**IMPORTANT**: The /plan command STOPS at step 7. Phases 2-4 are executed by other commands:

- Phase 2: /tasks command creates tasks.md
- Phase 3-4: Implementation execution (manual or via tools)

## Summary

Implement JWT-based authentication for the FurryFriends API with role-based access control (Admin, Petwalker, Client) and `[Anonymous]` attribute for endpoints that bypass authentication. Users authenticate via email/password to receive 8-hour JWT tokens, with all protected endpoints returning 401 for unauthenticated requests unless explicitly marked.

## Technical Context

**Language/Version**: .NET 9 (FurryFriends standard)
**Primary Dependencies**:

- FastEndpoints (API endpoints)
- MediatR (CQRS)
- FluentValidation (validation)
- Ardalis.Specification (queries)
- Ardalis.Result (operation outcomes)
- Ardalis.GuardClauses (preconditions)
- Serilog (logging)
- Microsoft.AspNetCore.Authentication.JwtBearer
- Entity Framework Core (data access)
- Blazor Server + WebAssembly (UI)

**Storage**: SQL Server with EF Core
**Testing**: xUnit (unit/integration), bUnit (Blazor components)
**Target Platform**: Web application (Blazor Hybrid)
**Project Type**: Web (backend API + Blazor UI)
**Performance Goals**: <200ms API response time
**Constraints**: Must integrate with existing AspNetCore.Identity user store, follows Clean Architecture layers
**Scale/Scope**: Affects authentication middleware, user endpoints, existing protected endpoints

## Constitution Check

_GATE: Must pass before Phase 0 research. Re-check after Phase 1 design._

**Architecture Compliance** (Constitution v2.1.0):

- [x] Follows Clean Architecture layers (Core → UseCases → Infrastructure/Web/BlazorUI)
- [x] API endpoints use FastEndpoints pattern (Request/Response/Endpoint separation)
- [x] Blazor components follow server-side HTTP pattern (no direct HttpClient in client)
- [x] Uses CQRS with MediatR (Commands for writes, Queries for reads)
- [x] Data access uses Repository + Specification pattern

**Required Patterns**:

- [x] All handlers return `Result<T>` or `Result` (Ardalis.Result)
- [x] All commands/queries have FluentValidation validators
- [x] All database queries use Specifications (Ardalis.Specification)
- [x] All method parameters validated with Guard Clauses (Ardalis.GuardClauses)
- [x] All logging uses Serilog structured logging with OpenTelemetry

**Testing Requirements**:

- [x] Test-First approach (tests written before implementation)
- [x] Unit tests for handlers, validators, specifications
- [x] Integration tests for API endpoints (functional tests)
- [ ] Blazor component tests with bUnit (Principle XX) - deferred to Phase 3
- [ ] Contract tests for API contract changes (specs/[###-feature]/contracts/) - Phase 1

**CI/CD & Automation** (Principle XVII):

- [ ] Build passes all projects
- [ ] Unit and functional tests pass
- [ ] Code coverage meets 80% minimum
- [ ] No known vulnerable dependencies

**Dependency Management** (Principle XVIII):

- [ ] Package versions in Directory.Packages.props only (not .csproj files)
- [ ] No packages with known critical vulnerabilities
- [ ] No unnecessary dependencies added

**Error Handling** (Principle XIX):

- [ ] Idempotency considered for mutation endpoints
- [ ] Distributed transaction patterns evaluated (Outbox/Saga)
- [ ] Graceful degradation for external dependencies

**Prohibited Practices**:

- [x] No direct SQL queries (use EF Core + Specifications)
- [x] No `Console.WriteLine` (use Serilog)
- [x] No throwing exceptions for expected failures (use Result pattern)
- [x] No manual null checks (use Guard Clauses)
- [x] No LINQ queries in handlers (use Specifications)
- [x] No package versions in .csproj files (use Directory.Packages.props)
- [ ] No skipping bUnit component tests for interactive components

**Violations Requiring Justification**:
None - all Constitution requirements are satisfied or deferred with justification.

## Project Structure

### Documentation (this feature)

```
specs/[###-feature]/
├── plan.md              # This file (/plan command output)
├── research.md          # Phase 0 output (/plan command)
├── data-model.md        # Phase 1 output (/plan command)
├── quickstart.md        # Phase 1 output (/plan command)
├── contracts/           # Phase 1 output (/plan command)
└── tasks.md             # Phase 2 output (/tasks command - NOT created by /plan)
```

### Source Code (FurryFriends Clean Architecture)

```
src/FurryFriends.Core/
├── UserAggregate/
│   ├── User.cs
│   ├── Specifications/
│   │   └── UserByEmailSpec.cs
│   └── Events/
│       └── UserAuthenticatedEvent.cs

src/FurryFriends.UseCases/
├── User/
│   ├── Registration/
│   │   ├── RegisterCommand.cs
│   │   ├── RegisterCommandHandler.cs
│   │   └── RegisterCommandValidator.cs
│   └── Login/
│       ├── LoginQuery.cs
│       ├── LoginQueryHandler.cs
│       └── LoginQueryValidator.cs

src/FurryFriends.Infrastructure/
├── Data/
│   ├── AppDbContext.cs (update)
│   └── Config/
│       └── UserConfiguration.cs
└── Services/
    └── JwtTokenService.cs

src/FurryFriends.Web/
├── Endpoints/
│   └── AuthEndpoints/
│       ├── Login/
│       │   ├── LoginRequest.cs
│       │   ├── LoginResponse.cs
│       │   └── Login.cs
│       └── Register/
│           ├── RegisterRequest.cs
│           ├── RegisterResponse.cs
│           └── Register.cs
└── Program.cs (update)

src/FurryFriends.BlazorUI/
├── Pages/
│   ├── Login.razor
│   ├── Login.razor.cs
│   ├── Register.razor
│   └── Register.razor.cs
├── Shared/
│   └── NavMenu.razor
└── Services/
    └── AuthService.cs
```

**Structure Decision**: FurryFriends uses Clean Architecture with strict layer separation. JWT authentication adds a new User aggregate to Core, UseCases for commands/queries, and Infrastructure for token generation.

## Phase 0: Outline & Research

1. **Extract unknowns from Technical Context** above:
   - No NEEDS CLARIFICATION - all technical details are known from existing architecture

2. **Research completed via existing documentation**:
   - JWT authentication patterns in ASP.NET Core
   - Integration with ASP.NET Core Identity
   - Role-based authorization with claims

3. **Consolidate findings** in `research.md`:
   - Decision: Use Microsoft.AspNetCore.Authentication.JwtBearer
   - Rationale: Standard, well-tested library with full framework integration
   - Alternatives considered: Third-party providers (out of scope), custom tokens

**Output**: research.md with all NEEDS CLARIFICATION resolved

## Phase 1: Design & Contracts

_Prerequisites: research.md complete_

1. **Extract entities from feature spec** → `data-model.md`:
   - User entity with Email, PasswordHash, Role, and registration metadata
   - Client/PetWalker profiles linked to User
   - Validation: email format, password complexity (8+ chars, upper/lower/digit/special)

2. **Generate API contracts** in `/contracts/`:
   - `LoginContract.md` - POST /api/auth/login endpoint
   - `RegisterContract.md` - POST /api/auth/register endpoint
   - `TokenResponse.md` - JWT response schema

3. **Generate contract tests** from contracts:
   - One test file per endpoint validating request/response schemas

4. **Extract test scenarios** from user stories:
   - Login with valid credentials → 200 + JWT
   - Login with invalid credentials → 401
   - Access protected endpoint without token → 401
   - Access anonymous endpoint without token → 200

5. **Update agent file incrementally** (O(1) operation):
   - Run `.specify/scripts/powershell/update-agent-context.ps1 -AgentType roo`
   - Add NEW tech: JWT, Identity, Authentication

**Output**: data-model.md, /contracts/*, quickstart.md, AGENTS.md

## Phase 2: Task Planning Approach

_This section describes what the /tasks command will do - DO NOT execute during /plan_

**Task Generation Strategy**:

- Load `.specify/templates/tasks-template.md` as base
- Generate tasks from Phase 1 design docs (contracts, data model, quickstart)
- Each contract → contract test task [P]
- Each entity → model creation task [P]
- Each user story → integration test task
- Implementation tasks to make tests pass

**Ordering Strategy**:

- TDD order: Tests before implementation
- Dependency order: Models before services before UI
- Mark [P] for parallel execution (independent files)

**Estimated Output**: 25-30 numbered, ordered tasks in tasks.md

**IMPORTANT**: This phase is executed by the /tasks command, NOT by /plan

## Phase 3+: Future Implementation

_These phases are beyond the scope of the /plan command_

**Phase 3**: Task execution (/tasks command creates tasks.md)  
**Phase 4**: Implementation (execute tasks.md following constitutional principles)  
**Phase 5**: Validation (run tests, execute quickstart.md, performance validation)

## Complexity Tracking

_Fill ONLY if Constitution Check has violations that must be justified_

| Violation                  | Why Needed         | Simpler Alternative Rejected Because |
| -------------------------- | ------------------ | ------------------------------------ |
| Blazor component tests     | Phase 1 deliverable deferred to Phase 3 | Not blocking initial auth API implementation |

## Progress Tracking

_This checklist is updated during execution flow_

**Phase Status**:

- [x] Phase 0: Research complete (/plan command)
- [x] Phase 1: Design complete (/plan command)
- [x] Phase 2: Task planning complete (/plan command - describe approach only)
- [ ] Phase 3: Tasks generated (/tasks command)
- [ ] Phase 4: Implementation complete
- [ ] Phase 5: Validation passed

**Gate Status**:

- [x] Initial Constitution Check: PASS
- [ ] Post-Design Constitution Check: PASS
- [x] All NEEDS CLARIFICATION resolved
- [x] Complexity deviations documented

---

_Based on Constitution v2.1.0 - See `.specify/memory/constitution.md`_
_For implementation guidance, see `docs/FurryFriends_Technical_Guide.md`_