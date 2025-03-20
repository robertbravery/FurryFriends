# Domain Events and Handlers

## Core Domain Events

### 1. CountryCreatedEvent
```csharp
public class CountryCreatedEvent : DomainEventBase
{
    public Guid CountryId { get; }
    public string CountryName { get; }
    public DateTime CreatedAt { get; }
}

public class CountryCreatedHandler : INotificationHandler<CountryCreatedEvent>
{
    private readonly IGeographicDataService _geoService;
    private readonly ISearchIndexService _searchIndex;

    public async Task Handle(CountryCreatedEvent notification, CancellationToken cancellationToken)
    {
        // Initialize country-specific geographic data
        // Update search indices
        // Initialize postal code validation rules
    }
}
```

### 2. RegionAddedEvent
```csharp
public class RegionAddedEvent : DomainEventBase
{
    public Guid RegionId { get; }
    public Guid CountryId { get; }
    public string RegionName { get; }
}

public class RegionAddedHandler : INotificationHandler<RegionAddedEvent>
{
    private readonly ISearchIndexService _searchIndex;
    
    public async Task Handle(RegionAddedEvent notification, CancellationToken cancellationToken)
    {
        // Update geographic search indices
        // Update service area calculations
        // Notify affected pet walkers
    }
}
```

### 3. LocalityAddedEvent
```csharp
public class LocalityAddedEvent : DomainEventBase
{
    public Guid LocalityId { get; }
    public Guid RegionId { get; }
    public string LocalityName { get; }
}

public class LocalityAddedHandler : INotificationHandler<LocalityAddedEvent>
{
    private readonly IServiceAreaCalculator _serviceAreaCalculator;
    
    public async Task Handle(LocalityAddedEvent notification, CancellationToken cancellationToken)
    {
        // Update service area boundaries
        // Recalculate affected coverage areas
        // Update search indices
    }
}
```