# Data Access

This document outlines the data access strategy for the FurryFriends application, focusing on the use of Generic Repositories, the Specification pattern, and Entity Framework Core.

## Architecture Overview

Data access is primarily handled within the `Infrastructure` project. This project contains the implementation of the Generic Repository pattern and the Entity Framework Core `DbContext`. The `Core` project defines the domain entities and the Specification interfaces.

## Generic Repository Pattern

The application utilizes a Generic Repository pattern to abstract data access operations. This provides a consistent interface for interacting with various data sources and promotes reusability.

**Key benefits:**

*   **Abstraction:** Decouples the application from the underlying data storage technology (Entity Framework Core in this case).
*   **Reusability:** Common data access operations (add, update, delete, get by ID, get all) are implemented once in a generic fashion.
*   **Testability:** Easier to mock data access for unit testing.

## Specification Pattern

The Specification pattern is employed to encapsulate query logic and business rules related to data retrieval. Specifications are defined in the `Core` project, ensuring that business rules are close to the domain.

**Key benefits:**

*   **Encapsulation of Query Logic:** Complex query criteria are defined in a single, reusable object.
*   **Improved Readability:** Queries become more expressive and easier to understand.
*   **Maintainability:** Changes to query logic are isolated within the Specification.
*   **Composability:** Multiple specifications can be combined to form more complex queries.

### How Specifications are Used

When data is accessed via the Generic Repository, a `Specification` object can be passed to filter and shape the results. The repository then translates this specification into an Entity Framework Core query.

## Service Layer

The Use Case projects (`FurryFriends.UseCases`) do not directly access the repositories. Instead, they call out to a **Service** that encapsulates the business logic for a particular aggregate. Each service has an interface, such as `IClientService`, which defines the contract for that service.

These services leverage the **Generic Repository** and the **Specification Pattern** to retrieve and persist data. This approach ensures a clean separation of concerns, where the Use Case orchestrates the flow of operations, the Service contains the business logic, and the Repository handles the data access.

### Examples

Here is an example of the `GetClientAsync` method from the `IClientService` interface and its implementation in `ClientService`.

#### IClientService.cs

```csharp
Task<Result<Client>> GetClientAsync(string emailAddress, CancellationToken cancellationToken);
```

#### ClientService.cs

```csharp
public async Task<Result<Client>> GetClientAsync(string emailAddress, CancellationToken cancellationToken)
{
    var existingClientSpec = new ClientByEmailSpec(emailAddress, true);
    var client = await _repository.FirstOrDefaultAsync(existingClientSpec, cancellationToken);
    if (client == null)
    {
        return Result.Error("Client not found");
    }
    return Result.Success(client);
}
```

## Data Access Mechanisms

Data in the FurryFriends application is accessed through two primary mechanisms within the `Infrastructure` project:

1.  **Generic Repositories with Specification Queries:**
    *   This is the primary method for retrieving and manipulating individual entities or collections of entities based on specific criteria.
    *   The `IRepository<TEntity>` interface (defined in `Core`) provides basic CRUD operations.
    *   Implementations in `Infrastructure` (e.g., `EfRepository<TEntity>`) leverage Entity Framework Core.
    *   Queries are constructed by passing `Specification<TEntity>` objects to repository methods.

    **Example**