# FurryFriends

FurryFriends is a .NET Core 9 project built using the Clean Architecture and Domain-Driven Design (DDD) principles. This project leverages the [Ardalis.CleanArchitecture](https://github.com/ardalis/CleanArchitecture) template to ensure a maintainable and scalable codebase.

## Table of Contents

- [Overview](#overview)
- [Getting Started](#getting-started)
- [Project Structure](#project-structure)
- [Technologies Used](#technologies-used)
- [Running Tests](#running-tests)
- [Contributing](#contributing)
- [License](#license)

## Overview

FurryFriends is designed to manage user data and their associated locations. The project follows Clean Architecture principles to separate concerns and ensure a clear dependency flow. Domain-Driven Design (DDD) is used to model the core business logic.

## Getting Started

To get started with the project, follow these steps:

1. **Clone the repository:**
   

2. **Install dependencies:**
   Ensure you have the .NET SDK 9 installed. You can download it from the [official .NET website](https://dotnet.microsoft.com/download).

3. **Build the project:**
   
4. **Run the application:**


## Project Structure

The project is organized into several layers:

- **src/FurryFriends.Core**: Contains the core domain logic, including entities, value objects, and domain services.
- **src/FurryFriends.Infrastructure**: Contains infrastructure concerns such as data access and external service integrations.
- **src/FurryFriends.Web**: The entry point of the application, containing the API controllers and configuration.
- **tests/FurryFriends.UnitTests**: Contains unit tests for the core domain logic.
- **tests/FurryFriends.FunctionalTests**: Contains functional tests for the API endpoints.

## Technologies Used

- **.NET Core 9**
- **Entity Framework Core**
- **FluentAssertions**
- **xUnit**
- **Ardalis.CleanArchitecture**

## Running Tests

To run the xUnit tests from the command line or from within the VS Code Terminal, you can use the following command: `dotnet test`

This command will discover and run all the tests in your solution. If you want to run tests for a specific project, you can specify the path to the project file: `dotnet test path/to/your/test/project.csproj`
You can also run tests for specific classes or methods by using the `--filter` option: `dotnet test --filter "YourNamespace.YourClass=YourMethod"`
You can also run tests for specific namespaces or classes by using the `--namespace` option: `dotnet test --namespace YourNamespace`
You can also run tests for specific methods by using the `--method` option: `dotnet test --method YourNamespace.YourClass.YourMethod`
You can also run tests for specific assemblies by using the `--assembly` option: `dotnet test --assembly YourNamespace.dll`


For example, to run the tests in the `FurryFriends.FunctionalTests` project, you would use: `dotnet test tests/FurryFriends.FunctionalTests/FurryFriends.FunctionalTests.csproj`
This command will execute the tests and display the results in the terminal.


## Contributing

Contributions are welcome! Please follow these steps to contribute:

1. Fork the repository.
2. Create a new branch (`git checkout -b feature/your-feature`).
3. Commit your changes (`git commit -am 'Add some feature'`).
4. Push to the branch (`git push origin feature/your-feature`).
5. Create a new Pull Request.

## License

This project is licensed under the MIT License. 


