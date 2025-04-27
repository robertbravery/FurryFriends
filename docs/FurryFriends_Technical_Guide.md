# FurryFriends Technical Guide

## Introduction

This technical guide uses the FurryFriends solution to teach developers intermediate and advanced techniques in Blazor and Dotnet. The FurryFriends solution is a Pet Walking/Day Care application that allows users to find and book pet walkers and day care services for their pets. This guide will walk you through the process of using the FurryFriends solution to learn about various Blazor and Dotnet techniques.

## Prerequisites

Before you start this technical guide, you should have the following prerequisites:

-   A basic understanding of Blazor and Dotnet.
-   Visual Studio 2022 or later.
-   .NET 7 SDK or later.
-   A basic understanding of Git and GitHub.

## Intermediate Techniques

### Dependency Injection

Dependency Injection (DI) is a design pattern that allows you to develop loosely coupled code. In DI, dependencies are injected into a class rather than being created within the class. This makes the code more testable, maintainable, and reusable.

In the FurryFriends solution, Dependency Injection is used extensively throughout the application. For example, the `PetWalkerService` class uses Dependency Injection to get an instance of the `HttpClient` class. This allows the `PetWalkerService` class to communicate with the PetWalker API without having to create its own instance of the `HttpClient` class.

Here's an example of how Dependency Injection is used in the `PetWalkerService` class:

```csharp
public class PetWalkerService : IPetWalkerService
{
  private readonly HttpClient _httpClient;
  private readonly string _apiBaseUrl;

  public PetWalkerService(HttpClient httpClient, IConfiguration configuration)
  {
    _httpClient = httpClient;
    _apiBaseUrl = configuration["ApiBaseUrl"];
  }
}
```

In this example, the `HttpClient` and `IConfiguration` classes are injected into the `PetWalkerService` class through its constructor. This allows the `PetWalkerService` class to use these classes without having to create its own instances.

### Asynchronous Programming

Asynchronous programming is a technique that allows you to perform long-running operations without blocking the main thread. This is important for UI applications, as it prevents the UI from freezing while the operation is running.

In the FurryFriends solution, asynchronous programming is used extensively throughout the application. For example, the `GetPetWalkersAsync` method in the `PetWalkerService` class uses asynchronous programming to retrieve a list of pet walkers from the API. This prevents the UI from freezing while the data is being retrieved.

Here's an example of how asynchronous programming is used in the `GetPetWalkersAsync` method:

```csharp
public async Task<ListResponse<PetWalkerDto>> GetPetWalkersAsync(int page, int pageSize)
{
  return await GetListAsync(page, pageSize);
}
```

In this example, the `async` keyword is used to indicate that the method is asynchronous. The `await` keyword is used to wait for the `GetListAsync` method to complete before continuing. This allows the UI thread to remain responsive while the data is being retrieved.

### LINQ

LINQ (Language Integrated Query) is a powerful query language that allows you to query data from various sources, such as databases, collections, and XML files. LINQ provides a consistent way to query data, regardless of the data source.

In the FurryFriends solution, LINQ is used extensively throughout the application. For example, the `GetPetWalkerDetailsByEmailAsync` method in the `PetWalkerService` class uses LINQ to filter the list of pet walkers based on their email address.

Here's an example of how LINQ is used in the `GetPetWalkerDetailsByEmailAsync` method:

```csharp
var serviceAreas = apiResponse.Data.GetProperty("locations")
    .EnumerateArray()
    .Select(x => x.GetString() ?? string.Empty)
    .Where(x => !string.IsNullOrEmpty(x))
    .ToList();
```

In this example, LINQ is used to select the `locations` property from the `apiResponse.Data` object, enumerate the array, select the string value of each element, filter out the empty strings, and convert the result to a list.

### Entity Framework Core

Entity Framework Core (EF Core) is a modern object-relational mapper (ORM) for .NET. It allows you to interact with databases using .NET objects, without having to write SQL queries. EF Core supports a variety of database providers, including SQL Server, PostgreSQL, and MySQL.

In the FurryFriends solution, EF Core is used to interact with the database. For example, the `FurryFriends.Infrastructure` project contains the EF Core context and entities.

To use EF Core, you first need to define your entities. Entities are classes that represent tables in the database. For example, the `Client` class represents the `Clients` table in the database.

You then need to create a DbContext class. The DbContext class represents a session with the database. It allows you to query and save data to the database.

Here's an example of how to use EF Core to query the database:

```csharp
public class FurryFriendsContext : DbContext
{
  public FurryFriendsContext(DbContextOptions<FurryFriendsContext> options)
      : base(options)
  {
  }

  public DbSet<Client> Clients { get; set; }
}
```

In this example, the `FurryFriendsContext` class inherits from the `DbContext` class. It defines a `DbSet<Client>` property, which represents the `Clients` table in the database.

You can then use LINQ to query the `Clients` table:

```csharp
var clients = _context.Clients.ToList();
```

### FluentValidation

FluentValidation is a popular .NET validation library for building strongly-typed validation rules. It provides a fluent interface for defining validation rules, making it easy to create complex validation logic.

In the FurryFriends solution, FluentValidation is used to validate the data that is entered by users. For example, the `CreatePetWalkerCommandValidator` class in the `src/FurryFriends.UseCases` project uses FluentValidation to validate the data that is used to create a new pet walker.

Here's an example of how to use FluentValidation to validate the `CreatePetWalkerCommand` class:

```csharp
public class CreatePetWalkerCommandValidator : AbstractValidator<CreatePetWalkerCommand>
{
  public CreatePetWalkerCommandValidator()
  {
    RuleFor(v => v.FirstName)
      .NotEmpty()
      .WithMessage("FirstName is required.")
      .MaximumLength(50)
      .WithMessage("FirstName must not exceed 50 characters.");

    RuleFor(v => v.LastName)
      .NotEmpty()
      .WithMessage("LastName is required.")
      .MaximumLength(50)
      .WithMessage("LastName must not exceed 50 characters.");

    RuleFor(v => v.Email)
      .NotEmpty()
      .WithMessage("Email is required.")
      .EmailAddress()
      .WithMessage("Email is not a valid email address.");
  }
}
```

In this example, the `CreatePetWalkerCommandValidator` class inherits from the `AbstractValidator<CreatePetWalkerCommand>` class. It defines validation rules for the `FirstName`, `LastName`, and `Email` properties of the `CreatePetWalkerCommand` class.

## Advanced Techniques

### Clean Architecture

Clean Architecture is a software design philosophy that emphasizes separation of concerns. The main goal of Clean Architecture is to create systems that are:

-   Independent of Frameworks. The architecture does not depend on the existence of some library of feature laden software. This allows you to use such frameworks as tools, rather than having to cram your system into their limited constraints.
-   Testable. The business rules can be tested without the UI, Database, Web Server, or any other external element.
-   Independent of UI. The UI can change easily, without changing the rest of the system. A Web UI could be replaced by a console UI, without forcing any change to the business rules.
-   Independent of Database. You can swap out Oracle or SQL Server, for Mongo, BigTable, CouchDB, or something else. Your business rules are not bound to the database.
-   Independent of any external agency. Your business rules simply don’t know anything at all about interfaces to the outside world.

In the FurryFriends solution, Clean Architecture is implemented by separating the application into the following layers:

-   **Core:** Contains the domain entities and business logic.
-   **UseCases:** Contains the application-specific use cases.
-   **Infrastructure:** Contains the implementation details, such as the database and API clients.
-   **Web:** Contains the UI and API endpoints.

This separation of concerns makes the FurryFriends solution more testable, maintainable, and reusable.

### CQRS

CQRS (Command Query Responsibility Segregation) is a design pattern that separates read and write operations for a data store. Implementing CQRS in your application can maximize its performance, scalability, and security.

In the FurryFriends solution, CQRS is implemented using the MediatR library. The `CreatePetWalkerCommand` and `GetPetWalkerQuery` classes are examples of CQRS commands and queries.

The `CreatePetWalkerCommand` class is used to create a new pet walker. The `GetPetWalkerQuery` class is used to retrieve a pet walker by their email address.

By separating the read and write operations, CQRS allows you to optimize each operation independently. For example, you can use a different database for read operations than you use for write operations. You can also scale the read and write operations independently.

### MediatR

MediatR is a simple, unambitious mediator implementation in .NET. It allows you to send messages between different parts of your application without having to create dependencies between them.

In the FurryFriends solution, MediatR is used to implement CQRS. The `IMediator` interface is used to send commands and queries to their respective handlers.

Here's an example of how to use MediatR to send a command:

```csharp
public class CreatePetWalker(IMediator _mediator)
  : Endpoint<CreatePetWalkerRequest, Result<CreatePetWalkerResponse>>
{
  public override async Task HandleAsync(CreatePetWalkerRequest request, CancellationToken cancellationToken)
  {
    var userCommand = CreateCommand(request);

    var result = await _mediator.Send(userCommand, cancellationToken);
```

In this example, the `IMediator` interface is injected into the `CreatePetWalker` class through its constructor. The `Send` method is used to send the `CreatePetWalkerCommand` to its handler.

### FastEndpoints

FastEndpoints is a light-weight .NET web framework for building REST APIs. It's an alternative to ASP.NET Core MVC and provides a simpler and faster way to create APIs.

In the FurryFriends solution, FastEndpoints is used to define the API endpoints. For example, the `CreatePetWalker` class in the `src/FurryFriends.Web` project uses FastEndpoints to define the API endpoint for creating a new pet walker.

Here's an example of how to use FastEndpoints to define an API endpoint:

```csharp
public class CreatePetWalker(IMediator _mediator)
  : Endpoint<CreatePetWalkerRequest, Result<CreatePetWalkerResponse>>
{
  public override void Configure()
  {
    Post(CreatePetWalkerRequest.Route);
    AllowAnonymous();
    Options(o => o.WithName("CreatePetWalker_" + Guid.NewGuid().ToString())); // Ensure unique name

    Summary(s =>
    {
      s.Summary = "Create a new User";
      s.Description = "Creates a new user by providing a username and email. This endpoint allows anonymous access.";

      s.ExampleRequest = new CreatePetWalkerRequest
      {
        FirstName = "John",
        LastName = "Smith",
        Email = "john.smith@example.com",
        PhoneCountryCode = "1",
        PhoneNumber = "1234567",
        Street = "123 Main St",
        City = "Springfield",
        State = "IL",
        PostalCode = "62701"
      };
    });
  }
```

In this example, the `CreatePetWalker` class inherits from the `Endpoint<CreatePetWalkerRequest, Result<CreatePetWalkerResponse>>` class. The `Configure` method is used to configure the endpoint. The `Post` method is used to specify that the endpoint handles HTTP POST requests. The `AllowAnonymous` method is used to allow anonymous access to the endpoint. The `Summary` method is used to provide a summary of the endpoint.

### Aspire

Aspire is a collection of tools and guidance for building observable, production ready, distributed applications. Aspire simplifies the development, deployment, and management of cloud-native applications.

The FurryFriends solution uses Aspire to orchestrate the different services that make up the application. The `FurryFriends.AspireHost` project defines the Aspire host, which is responsible for deploying and managing the other services.

By using Aspire, the FurryFriends solution can be easily deployed to a variety of cloud environments, such as Azure, AWS, and GCP. Aspire also provides built-in support for observability, which makes it easy to monitor the health and performance of the application.
