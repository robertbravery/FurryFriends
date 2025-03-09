# FurryFriends: Pet Walking & Pet Care API

Welcome to **FurryFriends** – a robust web API designed to power pet walking and pet care services. This project is built on .NET Core 9 using the **Ardalis Clean Architecture Template** as a baseline, ensuring our system adheres to Clean Architecture and Domain-Driven Design (DDD) principles.

---

## Table of Contents

- [Project Overview](#project-overview)
- [Architecture & Technology Stack](#architecture--technology-stack)
- [Getting Started](#getting-started)
- [Configuration & Deployment](#configuration--deployment)
- [Testing](#testing)
- [Contributing](#contributing)
- [License](#license)
- [Contact](#contact)

---

## Project Overview

**FurryFriends** is a web API dedicated to managing pet care operations, including pet walking and overall pet management. The project is built with a clear separation of concerns to foster scalability, maintainability, and clarity. Key features include:

- **Fast Endpoints & Fluent Validation:** For streamlined HTTP request routing and robust input validation.
- **MediatR Integration:** For decoupled handling of commands, queries, and notifications.
- **EF Core & MS SQL:** Providing a reliable persistence layer with well-structured data access patterns.
- **Domain Integrity:** Implemented via Ardalis Guard Clauses and the Specification pattern (using Ardalis.Specifications) to enforce business rules.
- **Uniform Response Patterns:** Via the Ardalis.Results pattern, ensuring clear and consistent API responses.
- **Generic Repositories:** Utilizing the Ardalis Shared Kernel for consistent data access.
- **Cross-cutting Concerns:** Managed with a Global Exception Middleware and structured logging using Serilog integrated with Open Telemetry.
- **Robust Testing:** Covered by xUnit, Fluent Assertions, and Moq for confidence in code changes.

---

## Architecture & Technology Stack

FurryFriends leverages modern best practices and a rich technology stack:

- **Base Template:** Built using the [Ardalis Clean Architecture Template](https://github.com/ardalis/CleanArchitecture) to ensure a well-organized project structure.
- **Framework:** .NET Core 9
- **Design Principles:** Clean Architecture & Domain-Driven Design (DDD)
- **API Layer:**
  - **Fast Endpoints:** For defining and handling HTTP endpoints.
  - **Fluent Validation:** For validating incoming requests.
- **Domain Layer:**
  - **Specifications:** Organized per Aggregate Root using *Ardalis.Specifications* to encapsulate query logic.
  - **Guard Clauses:** Enforced via *Ardalis Guard Clauses* to maintain domain invariants.
- **Application Layer:**
  - **MediatR:** Dispatching commands, queries, and events to the respective Use Cases.
- **Persistence & Infrastructure:**
  - **EF Core:** For object-relational mapping.
  - **MS SQL:** As the backend database.
  - **Generic Repositories:** Implemented via the Ardalis Shared Kernel.
- **Error Handling & Diagnostics:**
  - **Global Exception Middleware:** For consistent error management.
  - **Serilog + Open Telemetry:** For comprehensive logging and distributed tracing.
- **Response Structuring:**
  - **Ardalis.Results Pattern:** For clear API response formats.
- **Testing:**
  - **xUnit:** For unit and integration testing.
  - **Fluent Assertions & Moq:** For expressive and robust tests.

---

## Getting Started

### Prerequisites

Ensure you have the following installed:

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [MS SQL Server](https://www.microsoft.com/en-us/sql-server)
- Your preferred IDE (Visual Studio, VS Code, JetBrains Rider, etc.)

### Clone the Repository

Clone the repository to your local machine:

```bash
git clone https://github.com/yourusername/FurryFriends.git
cd FurryFriends

```

### Building the Application

Restore dependencies and build the solution:

```bash
dotnet restore
dotnet build
```

### Running the Application

To run the application, navigate to the API project directory and execute:

```bash
dotnet run --project src/FurryFriends.Api
```

The API will be running on your localhost (default: http://localhost:5000).

---

## Configuration & Deployment

### Configuration

- **Database:** Update your connection string in the `appsettings.json` file (or environment-specific configuration file) within the API project.
- **Logging & Telemetry:** Configure Serilog and Open Telemetry within the logging section in `appsettings.json`.

### Database Migrations

To apply the latest migrations with EF Core, navigate to the Infrastructure project directory and run:

```bash
dotnet ef database update --project src/FurryFriends.Infrastructure
```

### Deployment

FurryFriends can be deployed on any hosting environment that supports .NET. Ensure that your environment has the correct connection strings and that any necessary environment variables are configured properly.

---

## Testing

Our comprehensive suite of tests ensures reliability and maintainability:

- **xUnit:** For writing and running tests.
- **Fluent Assertions:** To provide rich assertion capabilities.
- **Moq:** For creating mocks and simulating dependencies.

To run all tests, execute:

```bash
dotnet test
```

---

## Contributing

We welcome contributions that help improve FurryFriends. To contribute:

1. **Fork** the repository.
2. **Create a feature branch:**  
   ```bash
   git checkout -b feature/YourFeature
   ```
3. **Commit your changes:**  
   ```bash
   git commit -am 'Add new feature'
   ```
4. **Push to your branch:**  
   ```bash
   git push origin feature/YourFeature
   ```
5. **Create a Pull Request:** Provide a detailed description of your changes.

For significant changes, please open an issue before submitting a pull request.

---

## License

Distributed under the MIT License. See [LICENSE](LICENSE) for more details.

---

## Contact

For questions or further information, please contact:

- **Project Lead:** [Robert Bravery](mailto:rbravery@iqbusiness.net)
- **GitHub Issues:** Please use the [Issues](https://github.com/robertbravery/FurryFriends/issues) page for bug reports or feature requests.

---

*FurryFriends* is continuously evolving. Stay tuned for updates and know that contributions are always welcome to enhance pet walking and pet care experiences!
```

