# Constitution Prompt for FurryFriends Project

Create a Constitution for a .NET 9 enterprise pet care management system (FurryFriends) that follows Clean Architecture principles with the following characteristics:

## Current Architecture
- **Solution Structure**: Multi-project solution with Clean Architecture layers
  - Core: Domain entities, value objects, aggregates, specifications (Ardalis.Specification)
  - UseCases: CQRS commands/queries, DTOs, handlers (MediatR pattern)
  - Infrastructure: EF Core repositories, SQL Server database
  - Web: FastEndpoints API with minimal API approach
  - BlazorUI: Blazor Server with InteractiveServer render mode
  - BlazorUI.Client: Blazor WebAssembly client components

## Domain Model
- **Aggregates**: Client, Pet, PetWalker, Booking, Location (Region/Locality)
- **Value Objects**: Name, Email, PhoneNumber, Address, Compensation
- **Enums**: PhotoType (BioPic, PetPhoto, PetWalkerPhoto), GenderType, ClientType, ReferralSource
- **Patterns**: Repository pattern with generic base, Specification pattern for queries

## Technology Stack
- .NET 9 (preview)
- Entity Framework Core with SQL Server
- FastEndpoints for API endpoints
- MediatR for CQRS
- FluentValidation for validation
- Ardalis.Specification and Ardalis.Result for domain patterns
- Serilog for logging
- Blazor Server + WebAssembly (InteractiveServer mode)
- Bootstrap 5 + FontAwesome 6.4.0 for UI

## Current Patterns & Practices
- **API Design**: FastEndpoints with separate Request/Response/Endpoint files per feature
- **Data Access**: Generic repository pattern, specifications for complex queries
- **Validation**: FluentValidation validators for commands
- **Error Handling**: Result pattern (Ardalis.Result) for operation outcomes
- **Logging**: Serilog with file-based logging in Logs folder
- **UI Architecture**: 
  - Blazor Server handles all HTTP communication
  - Client project contains interfaces and models only
  - Service implementations in BlazorUI project
  - Scoped CSS for component styling
  - Popup-based workflows for CRUD operations

## Desired Best Practices to Add
1. **Testing Strategy**:
   - Unit tests for domain logic and handlers
   - Integration tests for API endpoints
   - Component tests for Blazor UI
   - Test naming conventions and organization

2. **API Standards**:
   - Consistent pagination approach (page, pageSize, totalCount, totalPages)
   - Standard error response format
   - API versioning strategy
   - OpenAPI/Swagger documentation standards

3. **Security**:
   - Authentication/authorization patterns
   - API key management
   - Data protection and encryption
   - CORS policies

4. **Performance**:
   - Caching strategies (in-memory, distributed)
   - Query optimization guidelines
   - Lazy loading vs eager loading rules
   - Response compression

5. **Code Quality**:
   - Naming conventions (private fields with underscore prefix)
   - Code organization (code-behind for Blazor components)
   - Dependency injection patterns
   - Async/await best practices

6. **Documentation**:
   - XML documentation for public APIs
   - Mermaid diagrams for architecture (C4, sequence, class diagrams)
   - README files per project
   - API endpoint documentation

7. **Data Management**:
   - Migration strategy and naming
   - Seed data approach
   - Soft delete patterns
   - Audit trail implementation

8. **UI/UX Standards**:
   - Consistent popup/modal styling
   - Form validation display patterns
   - Loading states and error messages
   - Responsive design breakpoints
   - Accessibility (ARIA labels, keyboard navigation)

9. **DevOps**:
   - CI/CD pipeline structure
   - Environment configuration management
   - Database migration deployment
   - Logging and monitoring

10. **File Organization**:
    - Feature folder structure for endpoints
    - Aggregate-based organization in Core
    - Shared components location
    - Static assets management

## Special Considerations
- Photo/file upload handling with local storage for development
- Multi-tenant considerations for future scaling
- Real-time updates for booking management
- Complex filtering and sorting for list views
- Pagination with filtering applied before pagination (not after)
- Service area management with hierarchical location data (Region > Locality)

## Coding Conventions Already Established
- Private fields use underscore prefix (_fieldName)
- Async methods end with "Async"
- DTOs end with "Dto" suffix
- Specifications end with "Specification" suffix
- Handlers end with "Handler" suffix
- Validators end with "Validator" suffix
- Separate files for Request, Response, and Endpoint in FastEndpoints

Please create a Constitution that:
1. Preserves our current successful patterns
2. Adds industry best practices where we have gaps
3. Provides clear guidelines for new features
4. Includes examples from our existing codebase
5. Addresses scalability and maintainability
6. Supports both experienced and new team members