# Part 4: Security and Testing

This section covers comprehensive security implementation and testing strategies using the FurryFriends solution as a practical example.

## 1. Security Implementation

### 1.1 Authentication
```csharp
public static class AuthenticationConfig
{
    public static IServiceCollection AddAuthenticationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!))
            };

            // Enable token validation errors for debugging
            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                    {
                        context.Response.Headers.Add("Token-Expired", "true");
                    }
                    return Task.CompletedTask;
                }
            };
        });

        return services;
    }
}
```

### 1.2 Authorization Policies
```csharp
public static class AuthorizationConfig
{
    public static IServiceCollection AddAuthorizationPolicies(
        this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("RequirePetWalker", policy =>
                policy.RequireClaim("role", "petwalker"));

            options.AddPolicy("RequireClient", policy =>
                policy.RequireClaim("role", "client"));

            options.AddPolicy("RequireVerifiedPetWalker", policy =>
                policy.RequireAssertion(context =>
                    context.User.HasClaim("role", "petwalker") &&
                    context.User.HasClaim("verified", "true")));

            options.AddPolicy("RequireAdministrator", policy =>
                policy.RequireClaim("role", "admin"));
        });

        return services;
    }
}
```

### 1.3 Secure Data Access
```csharp
public class SecureBookingService : IBookingService
{
    private readonly ICurrentUserService _currentUser;
    private readonly IRepository<Booking> _repository;

    public async Task<Result<BookingDto>> GetBookingAsync(Guid bookingId)
    {
        var booking = await _repository.GetByIdAsync(bookingId);
        if (booking == null)
            return Result.NotFound<BookingDto>("Booking not found");

        // Security check: only allow access to own bookings
        if (!await CanAccessBooking(booking))
            return Result.Forbidden<BookingDto>("Access denied");

        return Result.Success(_mapper.Map<BookingDto>(booking));
    }

    private async Task<bool> CanAccessBooking(Booking booking)
    {
        if (_currentUser.IsInRole("admin"))
            return true;

        return booking.ClientId == _currentUser.UserId ||
               booking.PetWalkerId == _currentUser.UserId;
    }
}
```

## 2. Unit Testing

### 2.1 Domain Logic Tests
```csharp
public class BookingTests
{
    [Fact]
    public void Create_WithValidData_ReturnsSuccessResult()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var petWalkerId = Guid.NewGuid();
        var startTime = DateTime.UtcNow.AddDays(1);
        var endTime = startTime.AddHours(2);

        // Act
        var result = Booking.Create(
            clientId,
            petWalkerId,
            startTime,
            endTime);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(BookingStatus.Pending, result.Value.Status);
    }

    [Fact]
    public void Create_WithPastStartTime_ReturnsError()
    {
        // Arrange
        var startTime = DateTime.UtcNow.AddDays(-1);
        var endTime = startTime.AddHours(2);

        // Act
        var result = Booking.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            startTime,
            endTime);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("must be in the future", result.Error);
    }
}
```

### 2.2 Application Service Tests
```csharp
public class CreateBookingHandlerTests
{
    private readonly Mock<IRepository<Booking>> _mockRepo;
    private readonly Mock<ICurrentUserService> _mockUserService;
    private readonly CreateBookingHandler _handler;

    public CreateBookingHandlerTests()
    {
        _mockRepo = new Mock<IRepository<Booking>>();
        _mockUserService = new Mock<ICurrentUserService>();
        _handler = new CreateBookingHandler(_mockRepo.Object, _mockUserService.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_CreatesBooking()
    {
        // Arrange
        var command = new CreateBookingCommand(
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(1).AddHours(2));

        _mockUserService.Setup(x => x.UserId)
            .Returns(Guid.NewGuid());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _mockRepo.Verify(r => r.AddAsync(
            It.IsAny<Booking>(),
            It.IsAny<CancellationToken>()), 
            Times.Once);
    }
}
```

## 3. Integration Testing

### 3.1 Repository Tests
```csharp
public class BookingRepositoryTests : IClassFixture<TestDbFixture>
{
    private readonly TestDbFixture _fixture;
    private readonly IRepository<Booking> _repository;

    public BookingRepositoryTests(TestDbFixture fixture)
    {
        _fixture = fixture;
        _repository = new EfRepository<Booking>(_fixture.CreateContext());
    }

    [Fact]
    public async Task GetBySpec_ReturnsMatchingBookings()
    {
        // Arrange
        var date = DateTime.UtcNow.Date;
        var spec = new BookingsByDateSpec(date);

        // Act
        var bookings = await _repository.ListAsync(spec);

        // Assert
        Assert.NotEmpty(bookings);
        Assert.All(bookings, b => 
            Assert.Equal(date, b.StartTime.Date));
    }
}
```

### 3.2 API Integration Tests
```csharp
public class BookingEndpointsTests 
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public BookingEndpointsTests(
        CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateBooking_ReturnsCreatedResult()
    {
        // Arrange
        var command = new CreateBookingRequest
        {
            PetWalkerId = Guid.NewGuid(),
            StartTime = DateTime.UtcNow.AddDays(1),
            EndTime = DateTime.UtcNow.AddDays(1).AddHours(2)
        };

        await AuthenticateAsClient();

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/bookings", command);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    private async Task AuthenticateAsClient()
    {
        var token = _factory.GenerateJwtToken(
            "client@example.com", 
            new[] { "client" });
        _client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);
    }
}
```

## 4. UI Testing

### 4.1 Blazor Component Tests
```csharp
public class BookingFormTests : TestContext
{
    [Fact]
    public async Task SubmitValidForm_CallsCreateBooking()
    {
        // Arrange
        var mockBookingService = new Mock<IBookingService>();
        Services.AddScoped(_ => mockBookingService.Object);

        var cut = RenderComponent<BookingForm>();
        var form = cut.Find("form");

        // Act
        await cut.Find("#startTime").ChangeAsync(
            DateTime.Now.AddDays(1).ToString("yyyy-MM-ddTHH:mm"));
        await cut.Find("#endTime").ChangeAsync(
            DateTime.Now.AddDays(1).AddHours(2).ToString("yyyy-MM-ddTHH:mm"));
        await form.SubmitAsync();

        // Assert
        mockBookingService.Verify(s => s.CreateBookingAsync(
            It.IsAny<CreateBookingDto>(),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
```

### 4.2 End-to-End Tests
```csharp
public class BookingFlowTests : IClassFixture<PlaywrightFixture>
{
    private readonly IPlaywright _playwright;
    private readonly IBrowser _browser;

    [Fact]
    public async Task CompleteBookingFlow()
    {
        await using var context = await _browser.NewContextAsync();
        var page = await context.NewPageAsync();

        // Login
        await page.GotoAsync("/login");
        await page.FillAsync("#email", "client@example.com");
        await page.FillAsync("#password", "password");
        await page.ClickAsync("#login-button");

        // Navigate to booking
        await page.GotoAsync("/petwalkers");
        await page.ClickAsync("text=Book Now");

        // Fill booking form
        await page.FillAsync("#date", DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"));
        await page.SelectOptionAsync("#time", "14:00");
        await page.ClickAsync("#submit-booking");

        // Assert
        await Assertions.Expect(page.Locator(".booking-confirmation"))
            .ToContainTextAsync("Booking Confirmed");
    }
}
```

## Best Practices

1. **Security**
   - Implement proper authentication
   - Use authorization policies
   - Validate all input
   - Implement proper CORS policies
   - Use HTTPS
   - Handle sensitive data properly

2. **Unit Testing**
   - Test one thing at a time
   - Use meaningful test names
   - Follow AAA pattern
   - Mock external dependencies
   - Test edge cases

3. **Integration Testing**
   - Use test database
   - Clean up test data
   - Test complete workflows
   - Test error scenarios
   - Use proper test isolation

4. **UI Testing**
   - Test component behavior
   - Test user interactions
   - Test error states
   - Use proper test selectors
   - Implement proper cleanup

## Exercises

1. Implement role-based authorization for a new feature
2. Write comprehensive tests for a domain entity
3. Create integration tests for a complex workflow
4. Implement end-to-end tests for the booking process

## Common Pitfalls

1. Insufficient security testing
2. Not testing error scenarios
3. Tightly coupled tests
4. Poor test maintenance
5. Inadequate test coverage

## Additional Resources

1. [ASP.NET Core Security Documentation](https://docs.microsoft.com/aspnet/core/security/)
2. [Testing ASP.NET Core Applications](https://docs.microsoft.com/aspnet/core/test/integration-tests)
3. [bUnit Documentation](https://bunit.dev/)
4. [Playwright Testing](https://playwright.dev/dotnet/)

This concludes our comprehensive guide on professional Blazor and .NET development using FurryFriends as a case study.