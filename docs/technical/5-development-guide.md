# Development Guide

## 1. Local Development Environment Setup

### 1.1 Prerequisites
- .NET 8.0 SDK
- SQL Server 2019 or later
- Visual Studio 2022 or VS Code
- Git
- Node.js (for front-end development)

### 1.2 Initial Setup Steps
1. Clone the repository:
```bash
git clone https://github.com/yourusername/FurryFriends.git
cd FurryFriends
```

2. Install .NET dependencies:
```bash
dotnet restore
```

3. Set up the database:
```bash
cd src/FurryFriends.Infrastructure
dotnet ef database update
```

4. Configure user secrets:
```bash
cd ../FurryFriends.Web
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=.;Database=FurryFriends;Trusted_Connection=True;MultipleActiveResultSets=true"
```

5. Start the application:
```bash
dotnet run --project src/FurryFriends.AspireHost/FurryFriends.AspireHost.csproj
```

### 1.3 Environment Configuration
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=FurryFriends;Trusted_Connection=True"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  },
  "Authentication": {
    "JwtKey": "your-development-key",
    "JwtIssuer": "http://localhost:5000",
    "JwtAudience": "http://localhost:5000"
  }
}
```

## 2. Testing Strategy

### 2.1 Test Categories

#### Unit Tests
- Location: `tests/FurryFriends.UnitTests`
- Focuses on individual components
- Uses xUnit and Moq
- No database or external dependencies

```csharp
[Fact]
public async Task CreateClient_WithValidData_SuccessfullyCreatesClient()
{
    // Arrange
    var repository = new Mock<IRepository<Client>>();
    var service = new ClientService(repository.Object);
    
    // Act
    var result = await service.CreateClientAsync(
        Name.Create("John", "Doe"),
        Email.Create("john@example.com"),
        PhoneNumber.Create("+1", "1234567890"),
        Address.Create("123 St", "City", "State", "Country", "12345")
    );
    
    // Assert
    Assert.True(result.IsSuccess);
    repository.Verify(r => r.AddAsync(It.IsAny<Client>(), It.IsAny<CancellationToken>()), Times.Once);
}
```

#### Integration Tests
- Location: `tests/FurryFriends.IntegrationTests`
- Tests component interactions
- Uses in-memory database
- Tests repository implementations

```csharp
public class EfRepositoryTests : BaseEfRepoTestFixture
{
    [Fact]
    public async Task AddClient_PersistsClient()
    {
        var repository = GetRepository<Client>();
        var client = CreateTestClient();
        
        await repository.AddAsync(client);
        
        var loaded = await repository.GetByIdAsync(client.Id);
        Assert.NotNull(loaded);
        Assert.Equal(client.Email.EmailAddress, loaded.Email.EmailAddress);
    }
}
```

#### Functional Tests
- Location: `tests/FurryFriends.FunctionalTests`
- End-to-end API testing
- Uses CustomWebApplicationFactory
- Tests complete request/response cycles

```csharp
public class ClientEndpointTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    [Fact]
    public async Task CreateClient_ReturnsSuccessStatusCode()
    {
        var client = _factory.CreateClient();
        var request = new CreateClientRequest 
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com"
        };
        
        var response = await client.PostAsJsonAsync("/api/clients", request);
        
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}
```

### 2.2 Running Tests

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/FurryFriends.UnitTests

# Run tests with coverage
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura
```

## 3. Debugging Guide

### 3.1 Visual Studio Debug Configuration
```json
{
  "profiles": {
    "FurryFriends.Web": {
      "commandName": "Project",
      "launchBrowser": true,
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      },
      "applicationUrl": "https://localhost:5001;http://localhost:5000"
    }
  }
}
```

### 3.2 Common Debugging Scenarios

#### Database Connection Issues
1. Check connection string in appsettings.json
2. Verify SQL Server is running
3. Ensure database migrations are up to date
4. Check SQL Server logs

#### Authentication Problems
1. Verify JWT token configuration
2. Check token expiration
3. Validate client credentials
4. Inspect authorization headers

#### API Request Issues
1. Enable request logging:
```csharp
app.Use(async (context, next) =>
{
    var requestBody = await ReadRequestBody(context.Request);
    _logger.LogInformation($"Request: {requestBody}");
    await next();
});
```

2. Use Swagger UI for API testing
3. Check CORS configuration
4. Verify request payload format

### 3.3 Logging Configuration

```csharp
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
    logging.AddOpenTelemetry();
});
```

## 4. Performance Optimization

### 4.1 Database Performance
- Use SQL Server Profiler for query analysis
- Implement appropriate indexes
- Monitor query execution plans
- Use Entity Framework performance best practices

### 4.2 Application Performance
1. Enable response compression:
```csharp
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<GzipCompressionProvider>();
});
```

2. Implement caching:
```csharp
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
});
```

3. Use async/await consistently
4. Implement proper disposal patterns

### 4.3 Monitoring Tools
- Application Insights
- Health checks
- Performance counters
- Logging and tracing

## 5. Development Workflow

### 5.1 Branch Strategy
1. Main branch: production-ready code
2. Develop branch: integration branch
3. Feature branches: new features
4. Release branches: version preparation
5. Hotfix branches: production fixes

### 5.2 Code Review Process
1. Create feature branch
2. Implement changes
3. Run all tests
4. Create pull request
5. Code review
6. Address feedback
7. Merge when approved

### 5.3 Commit Guidelines
- Use conventional commits
- Include ticket reference
- Keep commits focused
- Write clear messages

Example:
```
feat(client): add pet management functionality #123

- Add ability to add pets to client profile
- Implement pet validation
- Add unit tests for pet management
```

## 6. Troubleshooting Guide

### 6.1 Common Issues

#### Entity Framework Migrations
```bash
# Add migration
dotnet ef migrations add MigrationName -p src/FurryFriends.Infrastructure

# Remove last migration
dotnet ef migrations remove -p src/FurryFriends.Infrastructure

# Update database
dotnet ef database update -p src/FurryFriends.Infrastructure
```

#### Authentication Issues
1. Check JWT token configuration
2. Verify user roles
3. Validate token expiration
4. Check authorization policies

#### API Errors
1. Enable detailed errors in development
2. Check request/response logging
3. Verify API routes
4. Check middleware order

### 6.2 Logging Locations
- Application logs: `logs/app.log`
- Error logs: `logs/error.log`
- SQL Server logs: SQL Server Log
- IIS logs (if applicable): IIS log directory

### 6.3 Support Resources
- Internal documentation
- API documentation
- Code comments
- Architecture diagrams