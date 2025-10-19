<!--
Sync Impact Report:
- Version change: 0.1.0 -> 2.0.0
- List of modified principles:
  - "Library-First" -> "Clean Architecture Layers"
  - "CLI Interface" -> REMOVED (not applicable to .NET API + Blazor)
  - "Test-First" -> Enhanced with .NET testing specifics
  - "Integration Testing" -> Enhanced with API and Blazor testing
  - "Observability, Versioning & Breaking Changes, Simplicity" -> Split into separate principles
- Added sections (MAJOR additions):
  - I. Clean Architecture Layers (NON-NEGOTIABLE)
  - II. API Design Standards
  - III. Blazor UI Architecture
  - IV. CQRS & MediatR Usage
  - V. Data Access Patterns
  - VI. Test-First Development (NON-NEGOTIABLE)
  - VII. Integration Testing Standards
  - VIII. Observability & Logging (Serilog)
  - IX. Versioning & Breaking Changes
  - X. Simplicity & YAGNI
  - XI. Result Pattern (Ardalis.Result) (NON-NEGOTIABLE) - DETAILED
  - XII. FluentValidation (NON-NEGOTIABLE) - DETAILED
  - XIII. Specification Pattern (Ardalis.Specification) (NON-NEGOTIABLE) - DETAILED
  - XIV. Serilog Structured Logging (NON-NEGOTIABLE) - DETAILED
  - XV. Guard Clauses (Ardalis.GuardClauses) (NON-NEGOTIABLE) - DETAILED
- Removed sections:
  - CLI Interface (not applicable)
  - Library-First (not applicable)
- Templates requiring updates:
  - .specify/templates/plan-template.md: ⚠ pending review
  - .specify/templates/spec-template.md: ⚠ pending review
  - .specify/templates/tasks-template.md: ⚠ pending review
- Documentation References Added:
  - Link to Technical Guide (docs/FurryFriends_Technical_Guide.md)
  - Documentation & Learning Resources section
  - Clear distinction between Constitution (governance) and Technical Guide (education)
- Follow-up TODOs: None
- Notes: MAJOR version bump due to complete restructuring from generic library/CLI to .NET API + Blazor architecture
-->
# FurryFriends Constitution

**Project**: FurryFriends - Pet Care Management System
**Architecture**: .NET 9 Clean Architecture with FastEndpoints API + Blazor Hybrid UI

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

**Example - Endpoint Configuration**:
```csharp
// In the Request DTO, e.g., GetClientByIdRequest.cs
public class GetClientByIdRequest
{
    public const string Route = "/api/clients/{ID}";
    public Guid ID { get; set; }
}

// In the Endpoint class, e.g., GetClientById.cs
public override void Configure()
{
    Get(GetClientByIdRequest.Route);
    Options(o => o.WithName("GetClientById"));
    Summary(s =>
    {
        s.Summary = "Get a client by their ID";
        s.Description = "This endpoint returns a single client based on the provided ID.";
        s.Response<ClientRecord>(200, "Client found and returned.");
        s.Response(404, "The client was not found.");
    });
}
```

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
- **Component Tests**: Blazor components with bUnit
- Tests written → User approved → Tests fail → Implementation → Tests pass
- Minimum 80% code coverage for business logic
- Test naming: `MethodName_Scenario_ExpectedBehavior`

**Rationale**: Ensures code quality, prevents regressions, and serves as living documentation.

### VII. Integration Testing Standards

Integration tests REQUIRED for:
- New API endpoints (request/response validation)
- Database migrations and data access
- API contract changes (breaking changes detection)
- Authentication/authorization flows
- File upload/download operations
- External service integrations

**Rationale**: Validates system behavior end-to-end and catches integration issues early.

### VIII. Observability & Logging (Serilog)

All components MUST implement proper logging using Serilog - see Principle XIV for detailed requirements.

**Rationale**: Enables debugging, monitoring, and troubleshooting in production.

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

**Result Types**:
- `Result.Success()` - Operation succeeded with no return value
- `Result<T>.Success(value)` - Operation succeeded with return value
- `Result.NotFound()` - Entity not found (404)
- `Result.Invalid(validationErrors)` - Validation failed (400)
- `Result.Error(message)` - Business rule violation or error (400/500)
- `Result.Forbidden()` - Authorization failed (403)
- `Result.Unauthorized()` - Authentication failed (401)

**Example - Command Handler**:
```csharp
public class CreateBookingHandler : ICommandHandler<CreateBookingCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateBookingCommand command, CancellationToken ct)
    {
        // Guard clauses for null checks
        Guard.Against.Null(command, nameof(command));

        // Business validation
        if (petWalker.DailyPetWalkLimit <= currentBookings)
        {
            return Result.Error("Pet walker has reached daily booking limit");
        }

        // Success case
        var booking = Booking.Create(command.ClientId, command.PetWalkerId, command.Date);
        await _repository.AddAsync(booking, ct);

        return Result<Guid>.Success(booking.Id);
    }
}
```

**Example - API Endpoint**:
```csharp
public class CreateBookingEndpoint : Endpoint<CreateBookingRequest, CreateBookingResponse>
{
    public override async Task HandleAsync(CreateBookingRequest req, CancellationToken ct)
    {
        var command = new CreateBookingCommand(req.ClientId, req.PetWalkerId, req.Date);
        var result = await _mediator.Send(command, ct);

        if (result.IsSuccess)
        {
            await SendCreatedAtAsync<GetBookingEndpoint>(
                new { id = result.Value },
                new CreateBookingResponse { BookingId = result.Value },
                cancellation: ct);
        }
        else if (result.Status == ResultStatus.NotFound)
        {
            await SendNotFoundAsync(ct);
        }
        else if (result.Status == ResultStatus.Invalid)
        {
            await SendAsync(new { Errors = result.ValidationErrors }, 400, ct);
        }
        else
        {
            await SendAsync(new { Error = result.Errors.FirstOrDefault() }, 500, ct);
        }
    }
}
```

**Prohibited**:
- Throwing exceptions for expected business failures (e.g., entity not found, validation errors)
- Returning null to indicate failure
- Using boolean return values without error information
- Catching exceptions and returning success Result

**When to Throw Exceptions**:
- Unexpected system failures (database connection lost, out of memory)
- Programming errors (null reference, index out of range)
- Infrastructure failures (file system errors, network timeouts)

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

**Example - Command Validator**:
```csharp
public class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
{
    public CreateBookingCommandValidator()
    {
        RuleFor(x => x.ClientId)
            .NotEmpty()
            .WithMessage("Client ID is required");

        RuleFor(x => x.PetWalkerId)
            .NotEmpty()
            .WithMessage("Pet Walker ID is required");

        RuleFor(x => x.BookingDate)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("Booking date must be in the future");

        RuleFor(x => x.Duration)
            .GreaterThan(0)
            .LessThanOrEqualTo(480)
            .WithMessage("Duration must be between 1 and 480 minutes");

        RuleFor(x => x.ServiceType)
            .IsInEnum()
            .WithMessage("Invalid service type");

        // Custom business rule
        RuleFor(x => x)
            .Must(HaveValidTimeSlot)
            .WithMessage("Selected time slot is not available");
    }

    private bool HaveValidTimeSlot(CreateBookingCommand command)
    {
        // Complex validation logic
        return true;
    }
}
```

**Example - API Request Validator**:
```csharp
public class CreateBookingRequestValidator : Validator<CreateBookingRequest>
{
    public CreateBookingRequestValidator()
    {
        RuleFor(x => x.ClientId)
            .NotEmpty()
            .WithMessage("Client ID is required");

        RuleFor(x => x.Pets)
            .NotEmpty()
            .WithMessage("At least one pet must be selected")
            .Must(pets => pets.Count <= 5)
            .WithMessage("Maximum 5 pets per booking");

        RuleForEach(x => x.Pets)
            .SetValidator(new PetSelectionValidator());
    }
}
```

**Validation Error Handling**:
- FastEndpoints automatically returns 400 Bad Request with validation errors
- Validation errors MUST be returned in consistent format:
```json
{
  "errors": {
    "ClientId": ["Client ID is required"],
    "BookingDate": ["Booking date must be in the future"]
  }
}
```

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

**Example - Simple Specification**:
```csharp
public class PetWalkerByIdSpecification : Specification<PetWalker>
{
    public PetWalkerByIdSpecification(Guid petWalkerId)
    {
        Query
            .Where(pw => pw.Id == petWalkerId)
            .Include(pw => pw.Photos)
            .Include(pw => pw.ServiceAreas)
                .ThenInclude(sa => sa.Locality);
    }
}
```

**Example - Complex Specification with Pagination**:
```csharp
public class ListPetWalkerByLocationSpecification : Specification<PetWalker>
{
    public ListPetWalkerByLocationSpecification(
        string? searchTerm,
        int? localityId,
        int page,
        int pageSize)
    {
        // Base query - active pet walkers only
        Query.Where(pw => pw.IsActive);

        // Search filter
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            Query.Where(pw =>
                pw.Name.FirstName.Contains(searchTerm) ||
                pw.Name.LastName.Contains(searchTerm) ||
                pw.Email.EmailAddress.Contains(searchTerm));
        }

        // Location filter
        if (localityId.HasValue)
        {
            Query.Where(pw =>
                pw.ServiceAreas.Any(sa => sa.Locality.Id == localityId.Value));
        }

        // Include related entities
        Query
            .Include(pw => pw.Photos.Where(p => p.PhotoType == PhotoType.BioPic))
            .Include(pw => pw.ServiceAreas)
                .ThenInclude(sa => sa.Locality);

        // Sorting
        Query.OrderBy(pw => pw.Name.FirstName)
             .ThenBy(pw => pw.Name.LastName);

        // Pagination
        Query
            .Skip((page - 1) * pageSize)
            .Take(pageSize);

        // Performance optimization for read-only queries
        Query.AsNoTracking();
    }
}
```

**Example - Count Specification**:
```csharp
public class CountPetWalkerByLocationSpecification : Specification<PetWalker>
{
    public CountPetWalkerByLocationSpecification(string? searchTerm, int? localityId)
    {
        Query.Where(pw => pw.IsActive);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            Query.Where(pw =>
                pw.Name.FirstName.Contains(searchTerm) ||
                pw.Name.LastName.Contains(searchTerm));
        }

        if (localityId.HasValue)
        {
            Query.Where(pw =>
                pw.ServiceAreas.Any(sa => sa.Locality.Id == localityId.Value));
        }
    }
}
```

**Example - Repository Usage**:
```csharp
public class ListPetWalkerByLocationHandler : IQueryHandler<ListPetWalkerByLocationQuery, Result<PaginatedList<PetWalkerDto>>>
{
    private readonly IRepository<PetWalker> _repository;

    public async Task<Result<PaginatedList<PetWalkerDto>>> Handle(
        ListPetWalkerByLocationQuery query,
        CancellationToken ct)
    {
        // Get total count
        var countSpec = new CountPetWalkerByLocationSpecification(
            query.SearchTerm,
            query.LocalityId);
        var totalCount = await _repository.CountAsync(countSpec, ct);

        // Get paginated results
        var listSpec = new ListPetWalkerByLocationSpecification(
            query.SearchTerm,
            query.LocalityId,
            query.Page,
            query.PageSize);
        var petWalkers = await _repository.ListAsync(listSpec, ct);

        // Map to DTOs
        var dtos = petWalkers.Select(pw => MapToDto(pw)).ToList();

        return Result<PaginatedList<PetWalkerDto>>.Success(
            new PaginatedList<PetWalkerDto>(dtos, totalCount, query.Page, query.PageSize));
    }
}
```

**Specification Best Practices**:
- Keep specifications focused on single query purpose
- Create separate specifications for count queries (no includes, no pagination)
- Use `AsNoTracking()` for read-only queries
- Apply filtering BEFORE pagination (not after)
- Reuse common specifications through composition
- Test specifications independently with in-memory database

**Prohibited**:
- Writing raw SQL queries
- Using LINQ directly in handlers or services
- Mixing business logic with query logic
- Applying pagination before filtering
- Using `ToList()` before applying specifications

**Rationale**: Specification pattern encapsulates query logic, enables reusability, improves testability, and keeps domain logic separate from data access concerns.

### XIV. Serilog Structured Logging (NON-NEGOTIABLE)

All logging MUST use Serilog with structured logging patterns:

**Required Configuration**:
- Serilog configured in `Program.cs` with file and console sinks
- Log files written to `Logs/` folder with rolling file policy
- File naming: `log-{Date}.txt` (e.g., `log-20251004.txt`)
- Retention policy: Keep 30 days of logs
- Minimum level: Information (Debug in Development environment)

**Example - Serilog Configuration**:
```csharp
// Program.cs
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentName()
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File(
        path: "Logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();
```

**Log Levels**:
- **Trace**: Very detailed diagnostic information (rarely used)
- **Debug**: Detailed information for debugging (Development only)
- **Information**: General informational messages (default minimum)
- **Warning**: Potentially harmful situations that don't prevent operation
- **Error**: Error events that might still allow application to continue
- **Critical**: Critical failures requiring immediate attention

**Structured Logging Patterns**:

**Example - Handler Logging**:
```csharp
public class CreateBookingHandler : ICommandHandler<CreateBookingCommand, Result<Guid>>
{
    private readonly ILogger<CreateBookingHandler> _logger;

    public async Task<Result<Guid>> Handle(CreateBookingCommand command, CancellationToken ct)
    {
        _logger.LogInformation(
            "Creating booking for Client {ClientId} with PetWalker {PetWalkerId} on {BookingDate}",
            command.ClientId,
            command.PetWalkerId,
            command.BookingDate);

        try
        {
            // Business logic
            var booking = Booking.Create(command.ClientId, command.PetWalkerId, command.BookingDate);
            await _repository.AddAsync(booking, ct);

            _logger.LogInformation(
                "Successfully created booking {BookingId} for Client {ClientId}",
                booking.Id,
                command.ClientId);

            return Result<Guid>.Success(booking.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to create booking for Client {ClientId} with PetWalker {PetWalkerId}",
                command.ClientId,
                command.PetWalkerId);

            return Result<Guid>.Error("Failed to create booking");
        }
    }
}
```

**Example - Service Logging**:
```csharp
public class BookingService : IBookingService
{
    private readonly ILogger<BookingService> _logger;

    public async Task<ApiResponse<List<BookingDto>>> GetClientBookingsAsync(Guid clientId)
    {
        _logger.LogInformation("Retrieving bookings for Client {ClientId}", clientId);

        try
        {
            var response = await _httpClient.GetAsync($"/api/bookings/client/{clientId}");

            if (response.IsSuccessStatusCode)
            {
                var bookings = await response.Content.ReadFromJsonAsync<List<BookingDto>>();

                _logger.LogInformation(
                    "Successfully retrieved {BookingCount} bookings for Client {ClientId}",
                    bookings?.Count ?? 0,
                    clientId);

                return ApiResponse<List<BookingDto>>.Success(bookings);
            }

            _logger.LogWarning(
                "Failed to retrieve bookings for Client {ClientId}. Status: {StatusCode}",
                clientId,
                response.StatusCode);

            return ApiResponse<List<BookingDto>>.Failure("Failed to retrieve bookings");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error retrieving bookings for Client {ClientId}",
                clientId);

            return ApiResponse<List<BookingDto>>.Failure("An error occurred");
        }
    }
}
```

**Example - API Endpoint Logging**:
```csharp
public class GetAvailablePetWalkersEndpoint : Endpoint<GetAvailablePetWalkersRequest, GetAvailablePetWalkersResponse>
{
    private readonly ILogger<GetAvailablePetWalkersEndpoint> _logger;

    public override async Task HandleAsync(GetAvailablePetWalkersRequest req, CancellationToken ct)
    {
        _logger.LogInformation(
            "Getting available pet walkers. Page: {Page}, PageSize: {PageSize}, LocalityId: {LocalityId}",
            req.Page,
            req.PageSize,
            req.LocalityId);

        var query = new ListPetWalkerByLocationQuery(req.SearchTerm, req.LocalityId, req.Page, req.PageSize);
        var result = await _mediator.Send(query, ct);

        if (result.IsSuccess)
        {
            _logger.LogInformation(
                "Successfully retrieved {Count} pet walkers (Page {Page} of {TotalPages})",
                result.Value.Items.Count,
                result.Value.PageNumber,
                result.Value.TotalPages);

            await SendOkAsync(MapToResponse(result.Value), ct);
        }
        else
        {
            _logger.LogWarning(
                "Failed to retrieve pet walkers. Error: {Error}",
                result.Errors.FirstOrDefault());

            await SendAsync(new { Error = result.Errors.FirstOrDefault() }, 500, ct);
        }
    }
}
```

**Logging Best Practices**:
- Use structured logging with named properties (not string interpolation)
- Log method entry with key parameters (Information level)
- Log method exit with results (Information level)
- Log all exceptions with full context (Error level)
- Log business rule violations (Warning level)
- Include correlation IDs for request tracing
- Avoid logging sensitive data (passwords, credit cards, PII)
- Use consistent property names across application

**Example - Structured Properties**:
```csharp
// ✅ CORRECT - Structured logging
_logger.LogInformation(
    "User {UserId} created booking {BookingId} for pet {PetId}",
    userId,
    bookingId,
    petId);

// ❌ WRONG - String interpolation
_logger.LogInformation($"User {userId} created booking {bookingId} for pet {petId}");

// ❌ WRONG - Concatenation
_logger.LogInformation("User " + userId + " created booking " + bookingId);
```

**Prohibited**:
- Using `Console.WriteLine()` for logging
- String interpolation in log messages
- Logging sensitive data (passwords, tokens, credit cards)
- Swallowing exceptions without logging
- Logging at incorrect levels (e.g., Information for errors)
- Logging inside tight loops (performance impact)

**Rationale**: Serilog structured logging enables powerful log querying, filtering, and analysis. Consistent logging patterns improve debugging, monitoring, and troubleshooting in production environments.

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

**Example - Entity Constructor**:
```csharp
public class Booking : EntityBase<Guid>
{
    private Booking() { } // EF Core constructor

    private Booking(
        Guid clientId,
        Guid petWalkerId,
        DateTime bookingDate,
        int durationMinutes,
        ServiceType serviceType)
    {
        // Guard clauses
        Guard.Against.Default(clientId, nameof(clientId));
        Guard.Against.Default(petWalkerId, nameof(petWalkerId));
        Guard.Against.OutOfRange(durationMinutes, nameof(durationMinutes), 15, 480);
        Guard.Against.InvalidInput(
            bookingDate,
            nameof(bookingDate),
            date => date > DateTime.UtcNow,
            "Booking date must be in the future");

        Id = Guid.NewGuid();
        ClientId = clientId;
        PetWalkerId = petWalkerId;
        BookingDate = bookingDate;
        DurationMinutes = durationMinutes;
        ServiceType = serviceType;
        Status = BookingStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public static Booking Create(
        Guid clientId,
        Guid petWalkerId,
        DateTime bookingDate,
        int durationMinutes,
        ServiceType serviceType)
    {
        return new Booking(clientId, petWalkerId, bookingDate, durationMinutes, serviceType);
    }
}
```

**Example - Value Object**:
```csharp
public class Email : ValueObject
{
    public string EmailAddress { get; private set; }

    private Email() { } // EF Core constructor

    public Email(string emailAddress)
    {
        Guard.Against.NullOrWhiteSpace(emailAddress, nameof(emailAddress));
        Guard.Against.InvalidInput(
            emailAddress,
            nameof(emailAddress),
            email => IsValidEmail(email),
            "Invalid email address format");

        EmailAddress = emailAddress.ToLowerInvariant();
    }

    private static bool IsValidEmail(string email)
    {
        return email.Contains("@") && email.Contains(".");
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return EmailAddress;
    }
}
```

**Example - Handler with Guard Clauses**:
```csharp
public class CreateBookingHandler : ICommandHandler<CreateBookingCommand, Result<Guid>>
{
    private readonly IRepository<Booking> _repository;
    private readonly IRepository<Client> _clientRepository;
    private readonly IRepository<PetWalker> _petWalkerRepository;
    private readonly ILogger<CreateBookingHandler> _logger;

    public CreateBookingHandler(
        IRepository<Booking> repository,
        IRepository<Client> clientRepository,
        IRepository<PetWalker> petWalkerRepository,
        ILogger<CreateBookingHandler> logger)
    {
        // Guard clauses for dependencies
        _repository = Guard.Against.Null(repository, nameof(repository));
        _clientRepository = Guard.Against.Null(clientRepository, nameof(clientRepository));
        _petWalkerRepository = Guard.Against.Null(petWalkerRepository, nameof(petWalkerRepository));
        _logger = Guard.Against.Null(logger, nameof(logger));
    }

    public async Task<Result<Guid>> Handle(CreateBookingCommand command, CancellationToken ct)
    {
        // Guard clauses for command
        Guard.Against.Null(command, nameof(command));

        _logger.LogInformation(
            "Creating booking for Client {ClientId} with PetWalker {PetWalkerId}",
            command.ClientId,
            command.PetWalkerId);

        // Verify client exists
        var client = await _clientRepository.GetByIdAsync(command.ClientId, ct);
        if (client == null)
        {
            return Result<Guid>.NotFound($"Client {command.ClientId} not found");
        }

        // Verify pet walker exists
        var petWalker = await _petWalkerRepository.GetByIdAsync(command.PetWalkerId, ct);
        if (petWalker == null)
        {
            return Result<Guid>.NotFound($"Pet walker {command.PetWalkerId} not found");
        }

        // Business validation
        if (!petWalker.IsActive)
        {
            return Result<Guid>.Error("Pet walker is not currently active");
        }

        // Create booking (entity constructor has guard clauses)
        var booking = Booking.Create(
            command.ClientId,
            command.PetWalkerId,
            command.BookingDate,
            command.DurationMinutes,
            command.ServiceType);

        await _repository.AddAsync(booking, ct);

        _logger.LogInformation(
            "Successfully created booking {BookingId}",
            booking.Id);

        return Result<Guid>.Success(booking.Id);
    }
}
```

**Example - Service Method**:
```csharp
public class BookingService : IBookingService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<BookingService> _logger;

    public BookingService(HttpClient httpClient, ILogger<BookingService> logger)
    {
        _httpClient = Guard.Against.Null(httpClient, nameof(httpClient));
        _logger = Guard.Against.Null(logger, nameof(logger));
    }

    public async Task<ApiResponse<BookingDto>> GetBookingAsync(Guid bookingId)
    {
        Guard.Against.Default(bookingId, nameof(bookingId));

        _logger.LogInformation("Retrieving booking {BookingId}", bookingId);

        try
        {
            var response = await _httpClient.GetAsync($"/api/bookings/{bookingId}");

            if (response.IsSuccessStatusCode)
            {
                var booking = await response.Content.ReadFromJsonAsync<BookingDto>();
                Guard.Against.Null(booking, nameof(booking), "API returned null booking");

                return ApiResponse<BookingDto>.Success(booking);
            }

            return ApiResponse<BookingDto>.Failure("Booking not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving booking {BookingId}", bookingId);
            return ApiResponse<BookingDto>.Failure("An error occurred");
        }
    }
}
```

**Custom Guard Clauses**:
```csharp
public static class CustomGuards
{
    public static Guid AgainstEmptyGuid(this IGuardClause guardClause, Guid input, string parameterName)
    {
        if (input == Guid.Empty)
        {
            throw new ArgumentException($"{parameterName} cannot be empty GUID", parameterName);
        }

        return input;
    }

    public static string AgainstInvalidEmail(this IGuardClause guardClause, string input, string parameterName)
    {
        Guard.Against.NullOrWhiteSpace(input, parameterName);

        if (!input.Contains("@") || !input.Contains("."))
        {
            throw new ArgumentException($"{parameterName} is not a valid email address", parameterName);
        }

        return input;
    }
}

// Usage
Guard.Against.EmptyGuid(clientId, nameof(clientId));
Guard.Against.InvalidEmail(email, nameof(email));
```

**Guard Clauses vs FluentValidation**:
- **Guard Clauses**: Use for method preconditions, constructor parameters, dependency injection
- **FluentValidation**: Use for command/query validation, API request validation, complex business rules

**Example - When to Use Each**:
```csharp
// Guard Clauses - Constructor validation
public class Booking
{
    public Booking(Guid clientId, DateTime bookingDate)
    {
        Guard.Against.Default(clientId, nameof(clientId));
        Guard.Against.InvalidInput(
            bookingDate,
            nameof(bookingDate),
            date => date > DateTime.UtcNow,
            "Booking date must be in the future");

        ClientId = clientId;
        BookingDate = bookingDate;
    }
}

// FluentValidation - Command validation
public class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
{
    public CreateBookingCommandValidator()
    {
        RuleFor(x => x.ClientId)
            .NotEmpty()
            .WithMessage("Client ID is required");

        RuleFor(x => x.BookingDate)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("Booking date must be in the future");

        // Complex business rule
        RuleFor(x => x)
            .Must(HaveAvailableTimeSlot)
            .WithMessage("Selected time slot is not available");
    }
}
```

**Prohibited**:
- Skipping null checks on method parameters
- Using if/throw statements instead of guard clauses
- Allowing invalid state in constructors
- Returning null instead of throwing exceptions for precondition violations
- Mixing guard clauses with business logic

**Rationale**: Guard clauses fail fast with clear error messages, prevent invalid state, improve code readability, and make preconditions explicit. They complement FluentValidation by handling method-level validation while FluentValidation handles command/query-level validation.

## Additional Constraints

### Technology Stack

**Required Technologies**:
- **.NET 9** (preview accepted for development)
- **Entity Framework Core** with SQL Server for data persistence
- **FastEndpoints** for API endpoints (minimal API approach)
- **MediatR** for CQRS pattern (commands and queries)
- **FluentValidation** for input validation (see Principle XII)
- **Ardalis.Specification** for repository specifications (see Principle XIII)
- **Ardalis.Result** for operation outcomes (see Principle XI)
- **Ardalis.GuardClauses** for precondition validation (see Principle XV)
- **Serilog** for structured logging (see Principle XIV)
- **Blazor Server + WebAssembly** (InteractiveServer render mode)
- **Bootstrap 5** for UI framework
- **FontAwesome 6.4.0** for icons
- **xUnit** for unit testing
- **bUnit** for Blazor component testing

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

### File Organization

**Project Structure**:
- Feature folders in Web project (e.g., `/BookingEndpoints/GetAvailablePetWalkers/`)
- Aggregate folders in Core project (e.g., `/PetWalkerAggregate/`)
- Shared components in BlazorUI.Client `/Components/Common/`
- Static assets in `wwwroot/`
- Documentation in `/docs/` with Mermaid diagrams

### Security Standards

**Required**:
- Input validation on all API endpoints
- SQL injection prevention (use parameterized queries via EF Core)
- XSS prevention in Blazor (automatic with Razor)
- CORS policies properly configured
- Sensitive data (passwords, keys) in configuration, not code
- Authentication/authorization for protected endpoints

### Documentation & Learning Resources

**Required Documentation**:
- XML documentation for all public APIs
- README files for each major project
- Mermaid diagrams for architecture (C4, sequence, class diagrams)
- API endpoint documentation (FastEndpoints summaries)

**Learning Resources**:
- **Technical Guide**: `docs/FurryFriends_Technical_Guide.md` - Comprehensive guide for learning intermediate and advanced techniques
- **Architecture Diagrams**: `docs/` folder - C4 models, sequence diagrams, class diagrams
- **Constitution**: `.specify/memory/constitution.md` - This document (governance and standards)

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
- Pass all automated tests (unit, integration, component)
- Follow naming conventions and coding standards
- Include XML documentation for public APIs
- Update relevant documentation (README, diagrams)
- Have descriptive commit messages (conventional commits)
- Be reviewed by at least one team member

### Testing Gates

Before merge, code MUST:
- Have 80%+ code coverage for business logic
- Pass all existing tests
- Include new tests for new functionality
- Pass integration tests for API changes
- Pass component tests for UI changes

### Deployment Process

Deployments MUST:
- Run database migrations before application deployment
- Verify health checks pass
- Monitor logs for errors post-deployment
- Have rollback plan documented
- Follow environment promotion: Dev → Test → Staging → Production

## Governance

### Constitution Authority

This Constitution supersedes all other development practices and guidelines. When conflicts arise, Constitution principles take precedence.

### Amendment Process

Constitution amendments require:
1. Documented proposal with rationale
2. Team discussion and approval
3. Version bump following semantic versioning
4. Update to all dependent templates and documentation
5. Migration plan for existing code if needed

### Compliance

- All PRs MUST verify compliance with Constitution principles
- Deviations require explicit justification and approval
- Complexity beyond simple solutions MUST be justified
- Regular Constitution reviews (quarterly) to ensure relevance

### Version Control

- Constitution changes tracked in git with detailed commit messages
- Sync Impact Report maintained at top of file
- Breaking changes documented in CHANGELOG.md

**Version**: 2.0.0 | **Ratified**: 2025-10-04 | **Last Amended**: 2025-10-04