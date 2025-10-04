
# Implementation Plan: [FEATURE]

**Branch**: `[###-feature-name]` | **Date**: [DATE] | **Spec**: [link]
**Input**: Feature specification from `/specs/[###-feature-name]/spec.md`

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
6. Execute Phase 1 → contracts, data-model.md, quickstart.md, agent-specific template file (e.g., `CLAUDE.md` for Claude Code, `.github/copilot-instructions.md` for GitHub Copilot, `GEMINI.md` for Gemini CLI, `QWEN.md` for Qwen Code, or `AGENTS.md` for all other agents).
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
[Extract from feature spec: primary requirement + technical approach from research]

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
- Entity Framework Core (data access)
- Blazor Server + WebAssembly (UI)

**Storage**: SQL Server with EF Core
**Testing**: xUnit (unit/integration), bUnit (Blazor components)
**Target Platform**: Web application (Blazor Hybrid)
**Project Type**: Web (backend API + Blazor UI)
**Performance Goals**: [e.g., <200ms API response time, 60fps UI or NEEDS CLARIFICATION]
**Constraints**: [e.g., must work with existing Client/PetWalker aggregates or NEEDS CLARIFICATION]
**Scale/Scope**: [e.g., affects 3 aggregates, 5 endpoints, 2 Blazor pages or NEEDS CLARIFICATION]

## Constitution Check
*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Architecture Compliance** (Constitution v2.0.0):
- [ ] Follows Clean Architecture layers (Core → UseCases → Infrastructure/Web/BlazorUI)
- [ ] API endpoints use FastEndpoints pattern (Request/Response/Endpoint separation)
- [ ] Blazor components follow server-side HTTP pattern (no direct HttpClient in client)
- [ ] Uses CQRS with MediatR (Commands for writes, Queries for reads)
- [ ] Data access uses Repository + Specification pattern

**Required Patterns**:
- [ ] All handlers return `Result<T>` or `Result` (Ardalis.Result)
- [ ] All commands/queries have FluentValidation validators
- [ ] All database queries use Specifications (Ardalis.Specification)
- [ ] All method parameters validated with Guard Clauses (Ardalis.GuardClauses)
- [ ] All logging uses Serilog structured logging

**Testing Requirements**:
- [ ] Test-First approach (tests written before implementation)
- [ ] Unit tests for handlers, validators, specifications
- [ ] Integration tests for API endpoints
- [ ] Blazor component tests with bUnit

**Prohibited Practices**:
- [ ] No direct SQL queries (use EF Core + Specifications)
- [ ] No `Console.WriteLine` (use Serilog)
- [ ] No throwing exceptions for expected failures (use Result pattern)
- [ ] No manual null checks (use Guard Clauses)
- [ ] No LINQ queries in handlers (use Specifications)

**Violations Requiring Justification**:
[List any violations with justification in Complexity Tracking section]

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
├── [Aggregate]Aggregate/
│   ├── [Entity].cs
│   ├── Specifications/
│   │   └── [Entity]Specification.cs
│   └── Events/

src/FurryFriends.UseCases/
├── [Aggregate]/
│   ├── Create[Entity]/
│   │   ├── Create[Entity]Command.cs
│   │   ├── Create[Entity]Handler.cs
│   │   └── Create[Entity]Validator.cs
│   ├── Get[Entity]/
│   │   ├── Get[Entity]Query.cs
│   │   ├── Get[Entity]Handler.cs
│   │   └── [Entity]Dto.cs
│   └── Update[Entity]/

src/FurryFriends.Infrastructure/
├── Data/
│   ├── AppDbContext.cs
│   └── Config/
│       └── [Entity]Configuration.cs
└── Repositories/

src/FurryFriends.Web/
├── Endpoints/
│   └── [Aggregate]Endpoints/
│       └── [Operation]/
│           ├── [Operation]Request.cs
│           ├── [Operation]Response.cs
│           ├── [Operation]Validator.cs
│           └── [Operation].cs (endpoint)
└── Program.cs

src/FurryFriends.BlazorUI/
├── Components/
│   ├── Pages/
│   │   └── [Feature]/
│   │       ├── [Page].razor
│   │       ├── [Page].razor.cs
│   │       └── [Page].razor.css
│   └── Common/
│       └── [SharedComponent].razor
└── Services/
    └── Implementation/
        └── [Entity]Service.cs

src/FurryFriends.BlazorUI.Client/
├── Pages/
│   └── [Feature]/
└── Services/
    └── I[Entity]Service.cs

tests/FurryFriends.UnitTests/
├── Core/
│   └── [Aggregate]/
│       └── Specifications/
└── UseCases/
    └── [Aggregate]/

tests/FurryFriends.IntegrationTests/
└── [Aggregate]Endpoints/
```

**Structure Decision**: FurryFriends uses Clean Architecture with strict layer separation.
This feature will add files to the appropriate layers based on the aggregate being modified.

## Phase 0: Outline & Research
1. **Extract unknowns from Technical Context** above:
   - For each NEEDS CLARIFICATION → research task
   - For each dependency → best practices task
   - For each integration → patterns task

2. **Generate and dispatch research agents**:
   ```
   For each unknown in Technical Context:
     Task: "Research {unknown} for {feature context}"
   For each technology choice:
     Task: "Find best practices for {tech} in {domain}"
   ```

3. **Consolidate findings** in `research.md` using format:
   - Decision: [what was chosen]
   - Rationale: [why chosen]
   - Alternatives considered: [what else evaluated]

**Output**: research.md with all NEEDS CLARIFICATION resolved

## Phase 1: Design & Contracts
*Prerequisites: research.md complete*

1. **Extract entities from feature spec** → `data-model.md`:
   - Entity name, fields, relationships
   - Validation rules from requirements
   - State transitions if applicable

2. **Generate API contracts** from functional requirements:
   - For each user action → endpoint
   - Use standard REST/GraphQL patterns
   - Output OpenAPI/GraphQL schema to `/contracts/`

3. **Generate contract tests** from contracts:
   - One test file per endpoint
   - Assert request/response schemas
   - Tests must fail (no implementation yet)

4. **Extract test scenarios** from user stories:
   - Each story → integration test scenario
   - Quickstart test = story validation steps

5. **Update agent file incrementally** (O(1) operation):
   - Run `.specify/scripts/powershell/update-agent-context.ps1 -AgentType roo`
     **IMPORTANT**: Execute it exactly as specified above. Do not add or remove any arguments.
   - If exists: Add only NEW tech from current plan
   - Preserve manual additions between markers
   - Update recent changes (keep last 3)
   - Keep under 150 lines for token efficiency
   - Output to repository root

**Output**: data-model.md, /contracts/*, failing tests, quickstart.md, agent-specific file

## Phase 2: Task Planning Approach
*This section describes what the /tasks command will do - DO NOT execute during /plan*

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
*These phases are beyond the scope of the /plan command*

**Phase 3**: Task execution (/tasks command creates tasks.md)  
**Phase 4**: Implementation (execute tasks.md following constitutional principles)  
**Phase 5**: Validation (run tests, execute quickstart.md, performance validation)

## Complexity Tracking
*Fill ONLY if Constitution Check has violations that must be justified*

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| [e.g., 4th project] | [current need] | [why 3 projects insufficient] |
| [e.g., Repository pattern] | [specific problem] | [why direct DB access insufficient] |


## Progress Tracking
*This checklist is updated during execution flow*

**Phase Status**:
- [ ] Phase 0: Research complete (/plan command)
- [ ] Phase 1: Design complete (/plan command)
- [ ] Phase 2: Task planning complete (/plan command - describe approach only)
- [ ] Phase 3: Tasks generated (/tasks command)
- [ ] Phase 4: Implementation complete
- [ ] Phase 5: Validation passed

**Gate Status**:
- [ ] Initial Constitution Check: PASS
- [ ] Post-Design Constitution Check: PASS
- [ ] All NEEDS CLARIFICATION resolved
- [ ] Complexity deviations documented

---
*Based on Constitution v2.0.0 - See `.specify/memory/constitution.md`*
*For implementation guidance, see `docs/FurryFriends_Technical_Guide.md`*
