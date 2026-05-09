<!--
Sync Impact Report:
- Version change: 2.0.2 -> 2.1.0
- Modified items:
  - MINOR version bump (new principles added): 2.0.2 -> 2.1.0
  - Date updated: 2026-04-12 -> 2026-05-09
  - Enhanced Principle VIII/XIV (Observability & Logging) - added OpenTelemetry, monitoring, alerting
  - Enhanced Principle XI (Result Pattern) - added distributed transactions, idempotency, cross-service patterns
  - Enhanced Principle XVI (Blazor Hybrid UI) - added bUnit testing, component testing, Hybrid SDKs
  - Enhanced Governance Amendment Process - formalized RFC process with steps
  - Added Principle XVII (CI/CD & Automation Pipeline) - new section
  - Added Principle XVIII (Dependency Management) - new section
  - Added Principle XIX (Enhanced Error Handling for Distributed Systems) - new section
  - Added Principle XX (Blazor Component Testing & Tooling) - new section
- Templates status: ⚠ pending check - plan-template.md, spec-template.md, tasks-template.md
- Follow-up TODOs: Update plan-template.md Constitution Check to reference v2.1.0 and new principles
-->

# FurryFriends Constitution

**Project**: FurryFriends - Pet Care Management System
**Architecture**: .NET 9 Clean Architecture with FastEndpoints API + Blazor Hybrid UI, using DDD & CQRS

## Core Principles

### I. Clean Architecture Layers (NON-NEGOTIABLE)

All code MUST follow Clean Architecture with strict dependency rules:

- **Core**: Domain entities, value objects, aggregates, specifications (no external dependencies except Ardalis.Specification)
- **UseCases**: CQRS commands/queries, DTOs, handlers using MediatR (depends only on Core)
- **Infrastructure**: EF Core repositories, database access (depends on Core and UseCases)
- **Web**: FastEndpoints API (depends on UseCases and Infrastructure)
- **BlazorUI**: Blazor Server with InteractiveServer render mode (depends on UseCases via HTTP)
- **BlazorUI.Client**: Blazor WebAssembly client components (no direct backend dependencies)

**Rationale**: Maintains separation of concerns, testability, and allows independent evolution of layers.

### II. API Design Standards

All API endpoints MUST follow FastEndpoints patterns:

- Separate files for Request, Response, and Endpoint per feature.
- Feature folder organization (e.g., `/BookingEndpoints/GetAvailablePetWalkers/`).
- Endpoints MUST override the `Configure()` method to define their behavior.
- Inside `Configure()`, a specific HTTP verb method (e.g., `Get()`, `Post()`, `Put()`, `Delete()`) MUST be called, passing the route from the request DTO.
- The route MUST be a constant in the request DTO (e.g., `public const string Route = "/api/clients/{ID}";`).
- Endpoints MUST include OpenAPI documentation via the `Summary()` method, detailing the summary, description, and possible responses.
- The `Options()` method should be used to provide a unique name for the endpoint to avoid conflicts.
- Consistent pagination: `PageNumber`, `PageSize`, `TotalCount`, `TotalPages`, `HasPreviousPage`, `HasNextPage`.
- Result pattern (Ardalis.Result) for operation outcomes (see Principle XI).
- FluentValidation for request validation (see Principle XII).
- Proper HTTP status codes (200, 201, 400, 404, 500).
- Use `BaseEndpoint<TRequest, TResponse>` base class for centralized error handling via `HandleResultAsync`.

**Rationale**: Ensures API consistency, discoverability, and maintainability across the team.

### III. Blazor UI Architecture

Blazor components MUST follow these patterns:

- **Blazor Server handles ALL HTTP communication** - Client project NEVER calls HttpClient directly
- Service interfaces in BlazorUI.Client, implementations in BlazorUI (server-side)
- Code-behind files (.razor.cs) for complex component logic
- Scoped CSS (.razor.css) for component-specific styling
- Popup/modal-based workflows for CRUD operations
- Proper loading states and error handling with user-friendly messages
- Private fields use underscore prefix (`_fieldName`)
- Component testing with bUnit for all interactive components (see Principle XX)

**Rationale**: Maintains clear separation between client and server, improves testability, and ensures consistent UX.

### IV. CQRS & MediatR Usage

All business operations MUST use CQRS pattern via MediatR:

- Commands for state changes (Create, Update, Delete)
- Queries for data retrieval (Get, List, Search)
- Handlers in UseCases project with single responsibility
- DTOs for data transfer (suffix with `Dto`)
- Validators for commands (suffix with `Validator`)
- Return `Result<T>` or `Result` from handlers

**Rationale**: Separates reads from writes, improves scalability, and makes business logic testable.

### V. Data Access Patterns

All data access MUST follow repository and specification patterns:

- Generic repository pattern with Ardalis.Specification
- Specifications for complex queries (suffix with `Specification`) - see Principle XIII
- Aggregate-based organization in Core project
- EF Core with SQL Server for persistence
- Migrations with descriptive names and proper versioning
- No direct DbContext access outside Infrastructure layer

**Rationale**: Abstracts data access, enables testing without database, and maintains domain focus.

### VI. Test-First Development (NON-NEGOTIABLE)

TDD is mandatory for all new features:

- **Unit Tests**: Domain logic, handlers, specifications (xUnit)
- **Integration Tests**: API endpoints with in-memory database or test containers
- **Component Tests**: Blazor components with bUnit (see Principle XX)
- **Functional Tests**: End-to-end API tests via `CustomWebApplicationFactory`
- Tests written → User approved → Tests fail → Implementation → Tests pass
- Minimum 80% code coverage for business logic
- Test naming: `MethodName_Scenario_ExpectedBehavior`
- Test projects: `FurryFriends.FunctionalTests/` for integration/API tests, `tests/FurryFriends.UnitTests/` for unit tests

**Rationale**: Ensures code quality, prevents regressions, and serves as living documentation.

### VII. Integration Testing Standards

Integration tests REQUIRED for:

- New API endpoints (request/response validation)
- Database migrations and data access
- API contract changes (breaking changes detection)
- Authentication/authorization flows
- File upload/download operations
- External service integrations
- Use `CustomWebApplicationFactory` with in-memory database for functional tests
- Contract tests in `specs/[###-feature-name]/contracts/` for API contract validation

**Rationale**: Validates system behavior end-to-end and catches integration issues early.

### VIII. Observability & Logging (NON-NEGOTIABLE)

All components MUST implement structured logging using Serilog with OpenTelemetry for distributed tracing:

#### Serilog Configuration:

- Serilog configured in `Program.cs` with file and console sinks
- Log files written to `Logs/` folder with rolling file policy
- File naming: `web-log-.txt` (Web API) and `blazorui-log-.txt` (Blazor UI) with rolling interval
- Retention policy: Keep 31 days of logs (index 0-30), max 10MB per file, roll on file size limit
- Minimum level: Information (Debug in Development environment)

**Example - Serilog Configuration** (actual from `Program.cs`):

```csharp
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        Path.Combine(logsDirectory, "web-log-.txt"),
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
        fileSizeLimitBytes: 10 * 1024 * 1024,
        retainedFileCountLimit: 31,
        rollOnFileSizeLimit: true)
    .CreateLogger();
```

#### OpenTelemetry Integration:

- OpenTelemetry MUST be configured for distributed tracing and metrics
- Required instrumentation: ASP.NET Core, HTTP Client
- Metrics exported via console exporter and OpenTelemetry Protocol (OTLP)
- Tracing exported via console exporter and OTLP
- Configured in `Program.cs` with `AddOpenTelemetry()`:

```csharp
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation())
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation());
```

#### Log Levels:

- **Trace**: Very detailed diagnostic information (rarely used)
- **Debug**: Detailed information for debugging (Development only)
- **Information**: General informational messages (default minimum)
- **Warning**: Potentially harmful situations that don't prevent operation
- **Error**: Error events that might still allow application to continue
- **Critical**: Critical failures requiring immediate attention

#### Monitoring & Alerting:

- Health checks MUST be configured for all endpoints
- Application performance metrics MUST be collected via OpenTelemetry
- Error rates and request latencies MUST be monitored (target <200ms p95)
- Log aggregation tools (e.g., Seq, Elasticsearch, or Application Insights) SHOULD be used in production
- Alert thresholds SHOULD be defined for: 5xx error rate >1%, p95 latency >500ms, health check failures

**Rationale**: Enables debugging, monitoring, and troubleshooting in production. OpenTelemetry provides vendor-neutral observability across the distributed system.

### IX. Versioning & Breaking Changes

API versioning MUST follow semantic versioning:

- Version format: `MAJOR.MINOR.PATCH`
- MAJOR: Breaking API changes (incompatible)
- MINOR: New features (backward compatible)
- PATCH: Bug fixes (backward compatible)
- Document breaking changes in CHANGELOG.md
- Deprecation warnings before removal (minimum 1 minor version)

**Rationale**: Manages API evolution while maintaining client compatibility.

### X. Simplicity & YAGNI

Code MUST prioritize simplicity:

- Start with simplest solution that works
- YAGNI (You Aren't Gonna Need It) - no speculative features
- Refactor when patterns emerge (Rule of Three)
- Avoid premature optimization
- Clear, self-documenting code over clever solutions
- Complexity requires explicit justification

**Rationale**: Reduces maintenance burden, improves readability, and accelerates development.

### XI. Result Pattern (Ardalis.Result) (NON-NEGOTIABLE)

All business operations MUST return `Result<T>` or `Result` instead of throwing exceptions for expected failures:

**Required Usage**:

- Command handlers MUST return `Result` or `Result<T>`
- Query handlers MUST return `Result<T>`
- Service methods MUST return `Result<T>` for operations that can fail
- API endpoints MUST map Result to appropriate HTTP status codes
- Use `BaseEndpoint<TRequest, TResponse>.HandleResultAsync()` for centralized Result handling

**Result Types**:

- `Result.Success()` - Operation succeeded with no return value
- `Result<T>.Success(value)` - Operation succeeded with return value
- `Result.NotFound()` - Entity not found (404)
- `Result.Invalid(validationErrors)` - Validation failed (400)
- `Result.Error(message)` - Business rule violation or error (400/500)
- `Result.Forbidden()` - Authorization failed (403)
- `Result.Unauthorized()` - Authentication failed (401)

**Error Response Format** (via `ResponseBase<T>`):

```csharp
public class ResponseBase<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public T? Data { get; set; }
    public ICollection<string>? Errors { get; set; }
    public string? ErrorCode { get; set; }
    public DateTime Timestamp { get; set; }
}
```

**Centralized Error Handling**:

- `BaseEndpoint<TRequest, TResponse>` base class handles Result-pattern error mapping
- `GlobalExceptionHandlerMiddleware` catches unhandled exceptions and returns standardized error responses
- `ResponseBase.FromException()` maps exceptions to consistent format with `InternalServerError` error code
- FastEndpoints `UseProblemDetails()` configures structured error response format

**Prohibited**:

- Throwing exceptions for expected business failures (e.g., entity not found, validation errors)
- Returning null to indicate failure
- Using boolean return values without error information
- Catching exceptions and returning success Result

**Rationale**: Result pattern makes error handling explicit, improves API consistency, enables better error messages, and avoids expensive exception throwing for expected failures.

### XII. FluentValidation (NON-NEGOTIABLE)

All input validation MUST use FluentValidation for commands, queries, and API requests:

**Required Usage**:

- Every Command MUST have a corresponding Validator (suffix with `Validator`)
- Every API Request MUST have a corresponding Validator
- Validators MUST be registered in DI container
- FastEndpoints automatically executes validators before endpoint execution

**Validation Rules**:

- `NotEmpty()` - Required fields
- `NotNull()` - Non-nullable reference types
- `MaximumLength(n)` - String length limits
- `EmailAddress()` - Email format validation
- `GreaterThan(n)` / `LessThan(n)` - Numeric range validation
- `Must(predicate)` - Custom business rules
- `SetValidator(new ChildValidator())` - Nested object validation

**Prohibited**:

- Manual validation with if/else statements for simple rules
- Throwing exceptions for validation failures
- Returning generic error messages without field-specific details
- Validating in multiple places (validate once at entry point)

**Rationale**: FluentValidation provides declarative, testable, reusable validation rules with clear error messages and automatic integration with FastEndpoints.

### XIII. Specification Pattern (Ardalis.Specification) (NON-NEGOTIABLE)

All complex database queries MUST use Specification pattern with Ardalis.Specification:

**Required Usage**:

- Specifications MUST be in Core project under `{Aggregate}/Specifications/` folder
- Specifications MUST suffix with `Specification`
- Repository methods MUST accept specifications for queries
- Specifications MUST be reusable and composable

**Specification Components**:

- `Query.Where()` - Filtering criteria
- `Query.Include()` - Eager loading related entities
- `Query.OrderBy()` / `Query.OrderByDescending()` - Sorting
- `Query.Skip()` / `Query.Take()` - Pagination
- `Query.AsNoTracking()` - Read-only queries for performance

**Prohibited**:

- Writing raw SQL queries
- Using LINQ directly in handlers or services
- Mixing business logic with query logic
- Applying pagination before filtering
- Using `ToList()` before applying specifications

**Rationale**: Specification pattern encapsulates query logic, enables reusability, improves testability, and keeps domain logic separate from data access concerns.

### XIV. Serilog Structured Logging

See **Principle VIII** (Observability & Logging) for all logging requirements. This principle is consolidated under Principle VIII.

### XV. Guard Clauses (Ardalis.GuardClauses) (NON-NEGOTIABLE)

All method parameters and preconditions MUST be validated using Guard Clauses:

**Required Usage**:

- Guard clauses MUST be at the beginning of methods
- Use Ardalis.GuardClauses library for consistency
- Guard against null, empty, invalid ranges, and business rule violations
- Throw descriptive exceptions with parameter names

**Common Guard Clauses**:

- `Guard.Against.Null(parameter, nameof(parameter))` - Null checks
- `Guard.Against.NullOrEmpty(string, nameof(string))` - Null or empty strings
- `Guard.Against.NullOrWhiteSpace(string, nameof(string))` - Null or whitespace strings
- `Guard.Against.NegativeOrZero(number, nameof(number))` - Positive number validation
- `Guard.Against.OutOfRange(value, nameof(value), min, max)` - Range validation
- `Guard.Against.InvalidInput(value, nameof(value), predicate)` - Custom validation
- `Guard.Against.Default(guid, nameof(guid))` - Non-default GUID validation

**Prohibited**:

- Skipping null checks on method parameters
- Using if/throw statements instead of guard clauses
- Allowing invalid state in constructors
- Returning null instead of throwing exceptions for precondition violations
- Mixing guard clauses with business logic

**Rationale**: Guard clauses fail fast with clear error messages, prevent invalid state, improve code readability, and make preconditions explicit. They complement FluentValidation by handling method-level validation while FluentValidation handles command/query-level validation.

### XVI. Blazor Hybrid UI Best Practices and Patterns

All Blazor Hybrid components MUST follow these enhanced patterns:

#### 1. Component Structure and Organization

- Use partial classes (.razor.cs for logic, .razor for markup) to maintain separation
- Implement component lifecycle methods (OnInitialized, OnParametersSet) for state management
- Organize components in logical folders (e.g., `/Components/Bookings/`, `/Components/Common/`)
- Example: `Components/Bookings/BookingFormComponent.razor` (markup) + `BookingFormComponent.razor.cs` (logic) + `BookingFormComponent.razor.css` (styles)

#### 2. State Management

- Use services for cross-component state sharing (e.g., `IBookingService`)
- Avoid global state where possible; prefer component-level state
- Example: `Services/Implementation/BookingService.cs` managing booking status

#### 3. Routing and Navigation

- Implement client-side routing with `<Router>` component
- Use `NavigationManager` for server-side navigation
- Define routes in `Program.cs` for Blazor Hybrid

#### 4. Styling and Theming

- Apply scoped CSS (.razor.css) for component-specific styles
- Use CSS variables for theme customization
- Implement responsive design with `@media` queries
- Global styles in `wwwroot/css/` (e.g., `booking-components.css`, `booking-responsive.css`, `booking-themes.css`)

#### 5. Accessibility

- Add ARIA attributes (e.g., `aria-label`, `role`) for screen readers
- Ensure keyboard navigation support
- Maintain sufficient color contrast

#### 6. Performance Optimization

- Implement lazy loading for large components
- Use `ShouldRender` to minimize re-renders
- Optimize state updates with `StateHasChanged`
- Example: Virtual scrolling in `Components/Pages/Timeslots/ScheduleDisplayComponent.razor`

**Rationale**: These patterns ensure maintainable, performant, and accessible Blazor Hybrid applications while aligning with the project's Clean Architecture principles.

### XVII. CI/CD & Automation Pipeline (NEW)

All code changes MUST pass automated CI/CD pipelines before merge:

#### Required CI Pipeline Steps:

1. **Build**: All projects MUST build successfully (`dotnet build --no-restore`)
2. **Static Analysis**: Run code analysis (`dotnet analyze` or equivalent)
3. **Unit Tests**: Execute all unit tests (`dotnet test tests/FurryFriends.UnitTests/`)
4. **Functional Tests**: Execute all functional/integration tests (`dotnet test FurryFriends.FunctionalTests/`)
5. **Code Coverage**: Report code coverage (minimum 80% for business logic)
6. **Security Scan**: Scan dependencies for known vulnerabilities (`dotnet list package --vulnerable`)

#### Required CD Pipeline Steps:

1. **Build & Package**: Build and package deployable artifacts
2. **Run Migrations**: Apply database migrations before deployment
3. **Health Check Verification**: Verify health endpoints respond successfully
4. **Smoke Tests**: Run critical path smoke tests post-deployment
5. **Rollback Plan**: Documented rollback procedure for each deployment

#### Environment Promotion:

```
Development → Test → Staging → Production
```

#### Quality Gates:

- All CI steps MUST pass before PR merge
- Failed pipeline MUST block merge
- Pipeline configuration stored in `.github/workflows/` (GitHub Actions) or equivalent
- Pipeline definitions MUST be version-controlled alongside application code

**Rationale**: Automates quality verification, catches regressions early, and ensures reliable, repeatable deployments.

### XVIII. Dependency Management (NEW)

All NuGet dependencies MUST follow centralized management:

#### Central Package Management:

- Package versions are managed in `Directory.Packages.props` (Central Package Management)
- Individual `.csproj` files MUST NOT specify `<PackageReference>` versions
- Use `<PackageVersion Include="..." Version="..." />` in `Directory.Packages.props` only

#### Versioning Rules:

- Prefer stable releases; preview packages MUST be justified
- All packages for the same library family MUST use consistent major versions
- Security patches SHOULD be applied within 30 days of release
- Breaking version upgrades require documented migration plan

#### Update Policy:

- `dotnet outdated` or equivalent tool SHOULD be run monthly
- Patch version updates: Safe to apply automatically
- Minor version updates: Review changelog, run full test suite
- Major version updates: Requires team review and migration plan

#### Security:

- Run `dotnet list package --vulnerable` before each release
- Known vulnerabilities MUST be addressed before production deployment
- Use GitHub Dependabot or equivalent for automated vulnerability alerts

#### Prohibited:

- Hardcoding version numbers in `.csproj` files
- Using packages with known critical vulnerabilities
- Adding unnecessary dependencies (YAGNI applies to packages too)
- Mixing major versions of the same package across projects in the solution

**Rationale**: Centralized package management ensures consistency, simplifies updates, and prevents version conflicts across the solution.

### XIX. Enhanced Error Handling for Distributed Systems (NEW)

Beyond the Result pattern, distributed operations MUST follow these patterns:

#### Idempotency:

- All mutation endpoints (POST, PUT, PATCH, DELETE) MUST be idempotent where possible
- Use idempotency keys (`Idempotency-Key` header) for operations that cannot be naturally idempotent
- Idempotency keys MUST be stored with expiry (minimum 24 hours)
- Duplicate requests with same idempotency key MUST return the original result

#### Distributed Transactions:

- Prefer eventual consistency over distributed transactions (2PC)
- Use Outbox pattern for reliable message delivery across services
- Saga pattern for multi-step workflows requiring compensation actions

```csharp
// Example - Idempotent operation pattern
public async Task<Result<Guid>> Handle(CreateBookingCommand command, CancellationToken ct)
{
    // Check idempotency
    if (command.IdempotencyKey.HasValue)
    {
        var existing = await _idempotencyStore
            .GetAsync(command.IdempotencyKey.Value, ct);
        if (existing is not null)
            return Result<Guid>.Success(existing.BookingId);
    }

    // Business logic
    var booking = Booking.Create(/* ... */);
    await _repository.AddAsync(booking, ct);

    // Store idempotency record
    if (command.IdempotencyKey.HasValue)
    {
        await _idempotencyStore.SetAsync(
            command.IdempotencyKey.Value,
            new IdempotencyRecord(booking.Id),
            TimeSpan.FromHours(24), ct);
    }

    return Result<Guid>.Success(booking.Id);
}
```

#### Cross-Service Error Propagation:

- Use consistent error codes across all services
- Include correlation IDs in all error responses for request tracing
- Map downstream service errors to appropriate HTTP status codes
- Log downstream failures with full context (service name, endpoint, status code)

#### Graceful Degradation:

- External service failures MUST NOT cascade (use circuit breakers)
- Implement timeouts for all outbound HTTP calls (default: 10s)
- Fallback strategies for non-critical external dependencies
- Health check endpoints MUST reflect dependency health

**Rationale**: Distributed systems require explicit handling of partial failures, retries, and consistency guarantees beyond simple request-response patterns.

### XX. Blazor Component Testing & Tooling (NEW)

All interactive Blazor components MUST have corresponding bUnit tests:

#### Required Tooling:

- **bUnit**: Component testing for Blazor interactive components
- **FluentAssertions**: Readable assertions in test code
- **xUnit**: Test framework for all test types
- **AutoFixture/Bogus**: Test data generation

#### Component Test Requirements:

- Every interactive component MUST have at least one bUnit test
- Tests MUST verify: rendering, user interactions, state changes, error states
- Component tests MUST NOT rely on backend services (use mocked service interfaces)

```csharp
// Example - bUnit component test
public class BookingFormComponentTests : TestContext
{
    [Fact]
    public void BookingForm_SubmitWithValidData_ShowsConfirmation()
    {
        // Arrange
        var mockService = new Mock<IBookingService>();
        Services.AddSingleton(mockService.Object);

        var cut = RenderComponent<BookingFormComponent>();

        // Act
        cut.Find("form").Submit();

        // Assert
        cut.FindComponent<BookingConfirmationComponent>()
            .Should().NotBeNull();
    }
}
```

#### Testing Patterns:

- Test rendering: Verify HTML output matches expected structure
- Test interactions: Click buttons, fill forms, verify callbacks
- Test parameter changes: Re-render with different parameters
- Test error states: Mock service failures, verify error display
- Test lifecycle: `OnInitializedAsync`, `OnParametersSet`, `Dispose`

#### Prohibited:

- Testing backend logic in component tests (use handler unit tests)
- Testing through the actual HTTP stack in bUnit tests
- Bypassing mocked services (defeats isolation purpose)

**Rationale**: bUnit component testing ensures UI reliability, validates user interactions, and prevents regressions in the Blazor frontend without requiring a full browser.

## Additional Constraints

### Technology Stack

**Required Technologies**:

- **.NET 9** (target framework for all projects)
- **Entity Framework Core** with SQL Server for data persistence
- **FastEndpoints** for API endpoints (5.35.0)
- **MediatR** for CQRS pattern (12.4.1)
- **FluentValidation** for input validation (11.11.0)
- **Ardalis.Specification** for repository specifications (8.0.0)
- **Ardalis.Result** for operation outcomes (10.1.0)
- **Ardalis.GuardClauses** for precondition validation (5.0.0)
- **Serilog** for structured logging (4.2.0)
- **OpenTelemetry** for distributed tracing and metrics (1.11.2)
- **Blazor Server + WebAssembly** (InteractiveServer render mode) (9.0.4)
- **Bootstrap 5** for UI framework
- **FontAwesome 6.4.0** for icons
- **xUnit** for unit testing (2.9.3)
- **bUnit** for Blazor component testing
- **FluentAssertions** for test assertions (7.1.0)
- **Bogus** for test data generation (35.6.1)

**Prohibited**:

- Direct SQL queries (use EF Core with specifications)
- Reflection-heavy solutions (impacts performance)
- Global state/singletons (except DI-registered services)
- Magic strings (use constants or enums)
- `Console.WriteLine` for logging (use Serilog)
- Throwing exceptions for expected failures (use Result pattern)
- Manual null checks (use Guard Clauses)
- LINQ queries in handlers (use Specifications)

### Naming Conventions

**MUST follow**:

- Private fields: `_camelCase` with underscore prefix
- Public properties: `PascalCase`
- Methods: `PascalCase`
- Async methods: `MethodNameAsync` suffix
- Interfaces: `IInterfaceName` prefix
- DTOs: `EntityNameDto` suffix
- Specifications: `EntityNameSpecification` suffix
- Handlers: `CommandOrQueryNameHandler` suffix
- Validators: `CommandNameValidator` suffix
- Response types: `EntityNameResponse` or `ResponseBase<T>` for generic responses

### File Organization

**Project Structure**:

- Feature folders in Web project (e.g., `/BookingEndpoints/GetAvailablePetWalkers/`)
- Aggregate folders in Core project (e.g., `/PetWalkerAggregate/`)
- Shared components in BlazorUI.Client `/Components/Common/`
- Static assets in `wwwroot/`
- Documentation in `/docs/` with Mermaid diagrams
- Specifications in `/specs/[###-feature-name]/` with contracts, data-model, plan, tasks
- Feature contracts in `specs/[###-feature-name]/contracts/`

### Security Standards

**Required**:

- Input validation on all API endpoints
- SQL injection prevention (use parameterized queries via EF Core)
- XSS prevention in Blazor (automatic with Razor)
- CORS policies properly configured (policy name: `AllowBlazorClient`)
- Sensitive data (passwords, keys) in configuration, not code
- Authentication/authorization for protected endpoints
- Dependency vulnerability scanning as part of CI pipeline

### Documentation & Learning Resources

**Required Documentation**:

- XML documentation for all public APIs
- README files for each major project
- Mermaid diagrams for architecture (C4, sequence, class diagrams)
- API endpoint documentation (FastEndpoints summaries with Swagger)
- Feature specifications in `/specs/` following Speckit templates
- Contract documentation for API endpoints in `contracts/` directories

**Learning Resources**:

- **Technical Guide**: `docs/FurryFriends_Technical_Guide.md` - Comprehensive guide for learning intermediate and advanced techniques
- **Architecture Diagrams**: `docs/` folder - C4 models, sequence diagrams, class diagrams
- **Constitution**: `.specify/memory/constitution.md` - This document (governance and standards)
- **Speckit Templates**: `.specify/templates/` - Reusable templates for plans, specs, tasks

**Technical Guide Coverage**:
The Technical Guide provides detailed explanations and examples for:

- **Intermediate Techniques**: Dependency Injection, Async Programming, LINQ, Entity Framework Core, FluentValidation
- **Advanced Techniques**: Clean Architecture, CQRS, MediatR, FastEndpoints, Aspire

**When to Use Each**:

- **Constitution**: Reference for "what" and "why" (principles, rules, standards)
- **Technical Guide**: Reference for "how" (implementation details, learning, examples)
- **Code Examples**: Both documents provide examples, but Technical Guide is more educational

**Keeping Documentation Updated**:

- Update Technical Guide when adding new patterns or techniques
- Update Constitution when changing architectural decisions or standards
- Keep both documents in sync regarding technology stack and patterns
- Technical Guide examples should follow Constitution principles

## Development Workflow

### Code Review Requirements

All code changes MUST:

- Pass all automated tests (unit, integration, component, functional)
- Follow naming conventions and coding standards
- Include XML documentation for public APIs
- Update relevant documentation (README, diagrams)
- Have descriptive commit messages (conventional commits)
- Be reviewed by at least one team member
- Pass CI/CD pipeline (all stages)
- Comply with all Constitution principles (checked via plan-template Constitution Check)

### Testing Gates

Before merge, code MUST:

- Have 80%+ code coverage for business logic
- Pass all existing tests
- Include new tests for new functionality
- Pass integration tests for API changes
- Pass component tests for UI changes
- Pass contract tests for API contract changes
- Have no known vulnerable dependencies

### Deployment Process

Deployments MUST:

- Run database migrations before application deployment
- Verify health checks pass
- Monitor logs for errors post-deployment
- Have rollback plan documented
- Follow environment promotion: Dev → Test → Staging → Production
- Run smoke tests against the deployed environment
- Alert on error rate spikes within first hour of deployment

## Governance

### Constitution Authority

This Constitution supersedes all other development practices and guidelines. When conflicts arise, Constitution principles take precedence.

### Amendment Process

Constitution amendments require a formal RFC process:

1. **Proposal**: Documented RFC with rationale, impact analysis, and migration plan
2. **Review**: Team discussion period (minimum 48 hours for feedback)
3. **Approval**: Majority team vote required for adoption
4. **Version Bump**: Semantic versioning rules apply:
   - MAJOR: Backward incompatible governance/principle removals or redefinitions
   - MINOR: New principle/section added or materially expanded guidance
   - PATCH: Clarifications, wording, typo fixes, non-semantic refinements
5. **Propagation**: Update all dependent templates and documentation
6. **Migration**: Plan for existing code if needed (with timeline)
7. **Recording**: RFC archived in repository for historical reference

### Compliance

- All PRs MUST verify compliance with Constitution principles (via plan-template Constitution Check)
- Deviations require explicit justification and approval
- Complexity beyond simple solutions MUST be justified
- Regular Constitution reviews (quarterly) to ensure relevance

### Version Control

- Constitution changes tracked in git with detailed commit messages
- Sync Impact Report maintained at top of file
- Breaking changes documented in CHANGELOG.md
- Suggested commit message format: `docs: amend constitution to vX.Y.Z (principle additions + governance update)`

**Version**: 2.1.0 | **Ratified**: 2025-10-04 | **Last Amended**: 2026-05-09
