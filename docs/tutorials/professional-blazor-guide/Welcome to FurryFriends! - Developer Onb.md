Welcome to FurryFriends! - Developer Onboarding Guide
1. Introduction
Welcome to the FurryFriends team! This guide is designed to help you, as an intermediate developer, get up to speed with the FurryFriends solution.

What is FurryFriends?

FurryFriends is a platform designed to connect pet owners with pet walkers and manage booking schedules. It serves as a practical example of modern .NET development practices, incorporating Blazor, Clean Architecture, Domain-Driven Design (DDD), and Command Query Responsibility Segregation (CQRS).

Purpose of this Document:

This document provides an overview of the project's requirements, architecture, design principles, and codebase structure to facilitate your onboarding process. For deeper dives into specific implementation patterns, please refer to the Blazor and .NET Masterclass.

2. Prerequisites
Before diving in, we expect you to have a solid understanding of:

C# and .NET (including async/await, LINQ)
ASP.NET Core fundamentals (Middleware, Dependency Injection, Configuration)
Blazor concepts (Components, Data Binding, Lifecycle)
Basic knowledge of Entity Framework Core (DbContext, Migrations)
Understanding of RESTful APIs
Experience with unit testing (xUnit or similar)
Familiarity with Git for version control
3. Project Requirements
(Note: Replace this section with specifics for your project)

The detailed functional and non-functional requirements for FurryFriends are maintained in [Specify Location - e.g., Azure DevOps Boards, Jira Project, Confluence Space, or a specific requirements document].

Key high-level features include:

Client Management: Registering pet owners (Clients).
Pet Management: Associating pets with clients.
Pet Walker Management: Registering pet walkers, defining their service areas and availability.
Booking System: Allowing clients to book available pet walkers for specific time slots.
Scheduling: Viewing and managing upcoming appointments.
Please familiarize yourself with the requirements backlog/documentation to understand the specific user stories and acceptance criteria.

4. Architecture Overview
FurryFriends is built using Clean Architecture. This architectural style emphasizes separation of concerns, making the system more maintainable, testable, and independent of external frameworks or databases.

Layers:

mermaid
graph TB
    A[Blazor UI (Presentation)] --> B[Web API (Application Interface)]
    B --> C[Use Cases (Application Core)]
    C --> D[Domain Core (Entities, Value Objects)]
    E[Infrastructure (EF Core, External Services)] --> C
    E --> D
Domain Core: Contains the enterprise business logic. This includes Entities (like Client, Pet, PetWalker, Booking), Value Objects (like Name, Email, TimeSlot), Aggregates, and Domain Events. It has no dependencies on other layers.
Use Cases (Application Core): Orchestrates the flow of data to and from the Domain entities. It contains application-specific business rules, Commands, Queries, Handlers (using MediatR), DTOs, and interfaces for repositories or external services (IClientRepository). It depends only on the Domain Core.
Web API (Application Interface): Exposes the application's functionality via RESTful endpoints. It handles requests, maps them to Commands or Queries, and returns responses. It depends on the Use Cases layer.
Blazor UI (Presentation): The user interface built with Blazor WebAssembly (WASM). It interacts with the Web API to display data and send user actions.
Infrastructure: Contains implementations for external concerns like databases (EF Core Repositories), external API clients, logging, email services, etc. It implements interfaces defined in the Use Cases layer and references technical libraries (like EF Core, SendGrid). It depends on the Use Cases layer.
Key Principle: Dependency Rule Dependencies flow inwards. The Domain Core is independent, while outer layers depend on inner layers. Interfaces are defined in inner layers and implemented in outer layers (Dependency Inversion).

(See Section 1 of the Blazor and .NET Masterclass for more details and code examples.)

5. Design Patterns & Principles
Several key design patterns and principles are employed throughout the solution:

Domain-Driven Design (DDD):
Entities: Objects with identity (e.g., Client, PetWalker).
Value Objects: Objects defined by their attributes, without identity (e.g., Email, Name, TimeSlot). They are immutable and encapsulate validation.
Aggregates: Clusters of domain objects (Entities, Value Objects) treated as a single unit, with one Entity acting as the Aggregate Root (e.g., Client is an Aggregate Root managing its Pet collection). Changes to the aggregate go through the root.
(See Section 2 of the Masterclass)
Command Query Responsibility Segregation (CQRS):
Separates operations that change state (Commands) from operations that read state (Queries).
Commands: Encapsulate the intent to change state (e.g., CreateBookingCommand). Handled by Command Handlers.
Queries: Encapsulate the intent to retrieve data (e.g., GetAvailablePetWalkersQuery). Handled by Query Handlers.
MediatR: Used to implement the pattern, decoupling senders of requests (Commands/Queries) from their handlers.
(See Section 3 of the Masterclass)
Dependency Inversion Principle (DIP):
High-level modules (Use Cases) do not depend on low-level modules (Infrastructure). Both depend on abstractions (interfaces defined in Use Cases).
Abstractions do not depend on details. Details (Infrastructure implementations) depend on abstractions.
Achieved via Dependency Injection (DI) provided by ASP.NET Core.
(See Section 1.2 of the Masterclass)
Repository Pattern: Abstracts data access logic. Interfaces (IClientRepository) are defined in the Use Cases layer, implementations (EfClientRepository) reside in Infrastructure.
Result Pattern: Used for returning outcomes of operations, clearly indicating success or failure with associated data or error messages, avoiding exceptions for control flow.
6. Technology Stack
.NET 8 (or latest LTS)
ASP.NET Core: For Web API and hosting.
Blazor WebAssembly (WASM): For the frontend UI.
Entity Framework Core: For data access (ORM).
MediatR: For implementing CQRS pattern.
FluentValidation: For validating requests/commands.
AutoMapper: For object-to-object mapping (e.g., Entities to DTOs).
xUnit: For unit and integration testing.
bUnit: For Blazor component testing.
Docker: (Optional) For containerization.
7. Code Structure & Key Areas
The solution (.sln) file organizes the codebase into projects that generally align with the Clean Architecture layers:

FurryFriends.Domain: Corresponds to the Domain Core layer. Contains Entities, Value Objects, Domain Events, Aggregate Roots, and core interfaces.
FurryFriends.Application: Corresponds to the Use Cases layer. Contains Commands, Queries, Handlers, DTOs, Interfaces (Repositories, Services), Validation logic, and Application-specific exceptions.
FurryFriends.Infrastructure: Corresponds to the Infrastructure layer. Contains EF Core DbContext, Migrations, Repository implementations, and implementations for other external services.
FurryFriends.Api: Corresponds to the Web API layer. Contains API Controllers, middleware, DI configuration, and startup logic.
FurryFriends.Web: Corresponds to the Blazor UI layer. Contains Blazor components, pages, state management, JS interop, and UI-specific logic.
FurryFriends.Tests.Unit: Unit tests, typically targeting Domain and Application layers.
FurryFriends.Tests.Integration: Integration tests, often testing API endpoints or Use Case handlers with infrastructure components (like a test database).
FurryFriends.Tests.Component: Blazor component tests using bUnit.
Key Functional Areas in Code:

Client Onboarding: Look at Client entity, CreateClientCommand and its handler.
Pet Walker Availability: See PetWalker aggregate, TimeSlot value object, and related commands/queries for managing availability.
Booking Process: Examine the Booking entity/aggregate, CreateBookingCommand, GetAvailablePetWalkersQuery, and related handlers.
Blazor State Management: Review how UI state is managed, potentially using simple service containers or more advanced libraries (see Section 4.1 of the Masterclass).
API Controllers: Check how controllers in FurryFriends.Api receive HTTP requests and dispatch them to MediatR.
8. Getting Started
Clone the Repository: git clone <repository-url>
Navigate to Solution Directory: cd FurryFriends
Restore Dependencies: dotnet restore
Database Setup:
Ensure you have the necessary database provider installed (e.g., SQL Server, PostgreSQL).
Update the connection string in FurryFriends.Api/appsettings.Development.json.
Apply EF Core Migrations: dotnet ef database update --project src/FurryFriends.Infrastructure --startup-project src/FurryFriends.Api
Build the Solution: dotnet build
Run the Application:
You can run the API and Blazor WASM app together by setting FurryFriends.Api as the startup project in Visual Studio or using dotnet run --project src/FurryFriends.Api.
Alternatively, run the API (dotnet run --project src/FurryFriends.Api) and the Blazor app (dotnet run --project src/FurryFriends.Web) separately if needed.
Access the Application: Open your browser to the URL specified during startup (e.g., https://localhost:7001).
9. Development Workflow
(Note: Adapt this section to your team's specific practices)

Branching: We use [Specify Branching Strategy - e.g., GitFlow, GitHub Flow]. Create feature branches from develop or main.
Commits: Write clear, concise commit messages.
Pull Requests: Submit Pull Requests (PRs) for review before merging. Ensure your code builds and passes all tests.
Code Reviews: Participate actively in code reviews, providing constructive feedback.
Testing: Write unit tests for business logic (Domain, Application) and integration tests for key workflows/API endpoints. Write component tests for complex Blazor components. Aim for good test coverage.
Coding Standards: Follow the established C# coding conventions and project-specific guidelines.
10. Further Learning
This document provides a starting point. To gain a deeper understanding, explore:

The Codebase: Browse the solution structure and read the code, focusing on the key areas mentioned above.
Blazor and .NET Masterclass: Work through this tutorial for detailed explanations and examples of the patterns used in FurryFriends.
Technical Documentation: Review any further technical design documents located in the /docs/technical folder.
Ask Questions: Don't hesitate to ask your teammates or lead for clarification!
We're excited to have you on the team!