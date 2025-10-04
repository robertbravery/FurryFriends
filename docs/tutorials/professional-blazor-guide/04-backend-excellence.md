# Part 3: Backend Excellence

This section covers advanced backend development techniques using practical examples from the FurryFriends solution.

## 1. Efficient API Design

### 1.1 API Endpoints with FastEndpoints
```csharp
// Clean, focused endpoint implementation
public class ListPetWalkers : EndpointBaseAsync
    .WithRequest<ListPetWalkersRequest>
    .WithActionResult<List<PetWalkerResponse>>
{
    private readonly IMediator _mediator;
    
    public ListPetWalkers(IMediator mediator) => _mediator = mediator;

    [HttpGet("/api/pet-walkers")]
    public override async Task<ActionResult<List<PetWalkerResponse>>> HandleAsync(
        [FromQuery] ListPetWalkersRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = new GetPetWalkersQuery(
            request.Location,
            request.MaxDistance,
            request.AvailableDate);

        var result = await _mediator.Send(query, cancellationToken);

        return result.IsSuccess 
            ? Ok(result.Value)
            : BadRequest(result.Error);
    }
}

// Request validation
public class ListPetWalkersValidator : Validator<ListPetWalkersRequest>
{
    public ListPetWalkersValidator()
    {
        RuleFor(x => x.Location)
            .NotNull()
            .SetValidator(new LocationValidator());

        RuleFor(x => x.MaxDistance)
            .GreaterThan(0)
            .LessThanOrEqualTo(50); // Maximum 50km radius

        RuleFor(x => x.AvailableDate)
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date);
    }
}
```

### 1.2 Response Caching
```csharp
[ResponseCache(Duration = 60)] // Cache for 1 minute
[HttpGet("/api/breeds")]
public async Task<ActionResult<List<BreedResponse>>> HandleAsync(
    CancellationToken cancellationToken)
{
    return await _cache.GetOrCreateAsync(
        CacheKeys.Breeds,
        async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            var breeds = await _repository.ListAsync(new AllBreedsSpec());
            return _mapper.Map<List<BreedResponse>>(breeds);
        });
}
```

## 2. Advanced Entity Framework Patterns

### 2.1 Custom Value Converters
```csharp
public class MoneyConverter : ValueConverter<Money, decimal>
{
    public MoneyConverter()
        : base(
            money => money.Amount,
            value => Money.Create(value).Value)
    {
    }
}

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.Property(c => c.Balance)
            .HasConversion<MoneyConverter>()
            .HasPrecision(18, 2);
    }
}
```

### 2.2 Advanced Query Specifications
```csharp
public class AvailablePetWalkersSpec : Specification<PetWalker>
{
    public AvailablePetWalkersSpec(
        Location location,
        double maxDistance,
        DateTime date)
    {
        Query
            .Include(pw => pw.ServiceAreas)
            .Include(pw => pw.Availability)
            .Where(pw => pw.ServiceAreas.Any(sa => 
                sa.CalculateDistance(location) <= maxDistance))
            .Where(pw => pw.Availability.Any(a => 
                a.Date.Date == date.Date &&
                !a.IsBooked));

        // Add sorting by distance
        Query.OrderBy(pw => pw.ServiceAreas
            .Min(sa => sa.CalculateDistance(location)));
    }
}
```

### 2.3 Efficient Loading Patterns
```csharp
public class ClientByIdWithDetailsSpec : Specification<Client>
{
    public ClientByIdWithDetailsSpec(Guid clientId)
    {
        Query
            .Include(c => c.Pets)
                .ThenInclude(p => p.Breed)
            .Include(c => c.Bookings
                .Where(b => b.Status != BookingStatus.Cancelled))
                .ThenInclude(b => b.PetWalker)
            .AsSplitQuery() // Split into multiple queries for better performance
            .Where(c => c.Id == clientId);
    }
}
```

## 3. Caching Strategies

### 3.1 Distributed Caching
```csharp
public class CachedPetWalkerService : IPetWalkerService
{
    private readonly IPetWalkerService _inner;
    private readonly IDistributedCache _cache;
    private readonly ILogger<CachedPetWalkerService> _logger;

    public async Task<Result<List<PetWalkerDto>>> GetAvailablePetWalkersAsync(
        Location location,
        double maxDistance,
        DateTime date,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"pet-walkers:{location}:{maxDistance}:{date:yyyyMMdd}";

        try
        {
            var cached = await _cache.GetAsync<List<PetWalkerDto>>(cacheKey);
            if (cached != null)
            {
                _logger.LogInformation("Cache hit for key: {CacheKey}", cacheKey);
                return Result.Success(cached);
            }

            var result = await _inner.GetAvailablePetWalkersAsync(
                location, maxDistance, date, cancellationToken);

            if (result.IsSuccess)
            {
                await _cache.SetAsync(cacheKey, result.Value, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error accessing cache");
            return await _inner.GetAvailablePetWalkersAsync(
                location, maxDistance, date, cancellationToken);
        }
    }
}
```

### 3.2 Cache Invalidation
```csharp
public class BookingCreatedEventHandler 
    : INotificationHandler<BookingCreatedEvent>
{
    private readonly IDistributedCache _cache;
    
    public async Task Handle(
        BookingCreatedEvent notification,
        CancellationToken cancellationToken)
    {
        // Invalidate related caches
        var date = notification.BookingDate.Date;
        var pattern = $"pet-walkers:*:{date:yyyyMMdd}";
        
        await _cache.RemoveByPatternAsync(pattern);
    }
}
```

## 4. Background Processing

### 4.1 Background Service Implementation
```csharp
public class BookingReminderService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<BookingReminderService> _logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _services.CreateScope();
                var bookingService = scope.ServiceProvider
                    .GetRequiredService<IBookingService>();

                await bookingService.SendUpcomingBookingReminders(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, 
                    "Error occurred while sending booking reminders");
            }

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}
```

### 4.2 Job Scheduling
```csharp
public class BookingCleanupJob : IJob
{
    private readonly IRepository<Booking> _repository;
    
    public async Task Execute(IJobExecutionContext context)
    {
        var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
        
        var spec = new CompletedBookingsOlderThanSpec(thirtyDaysAgo);
        var bookings = await _repository.ListAsync(spec);
        
        foreach (var booking in bookings)
        {
            booking.ArchiveBooking();
        }
        
        await _repository.SaveChangesAsync();
    }
}

// Job registration
public static class QuartzConfig
{
    public static void AddQuartzJobs(this IServiceCollection services)
    {
        services.AddQuartz(q =>
        {
            q.UseMicrosoftDependencyInjectionJobFactory();

            var jobKey = new JobKey("BookingCleanup");
            q.AddJob<BookingCleanupJob>(opts => opts.WithIdentity(jobKey));

            q.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity("BookingCleanup-Trigger")
                .WithCronSchedule("0 0 0 * * ?")); // Run daily at midnight
        });
    }
}
```

## Best Practices

1. **API Design**
   - Use consistent response formats
   - Implement proper validation
   - Handle errors gracefully
   - Document endpoints properly

2. **Entity Framework**
   - Use specifications for complex queries
   - Implement proper loading strategies
   - Handle concurrency properly
   - Use value conversions for complex types

3. **Caching**
   - Implement proper cache invalidation
   - Use appropriate cache duration
   - Handle cache failures gracefully
   - Cache at appropriate levels

4. **Background Processing**
   - Handle errors properly
   - Implement proper logging
   - Use appropriate scheduling
   - Consider scalability

## Exercises

1. Implement API versioning with FastEndpoints
2. Create a complex specification with multiple includes
3. Implement a distributed caching solution
4. Create a background job with proper error handling

## Common Pitfalls

1. N+1 query problems
2. Improper cache invalidation
3. Memory leaks in background services
4. Lack of proper error handling
5. Poor performance due to inefficient queries

## Additional Resources

1. [FastEndpoints Documentation](https://fast-endpoints.com/)
2. [Entity Framework Core Best Practices](https://docs.microsoft.com/ef/core/performance/)
3. [Distributed Caching in ASP.NET Core](https://docs.microsoft.com/aspnet/core/performance/caching/distributed)
4. [Background Tasks with hosted services](https://docs.microsoft.com/aspnet/core/fundamentals/host/hosted-services)

Next section: Security and Testing