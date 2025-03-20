# Domain Events and Handlers

## Core Domain Events

### 1. PetWalkerCreatedEvent
```csharp
public class PetWalkerCreatedEvent : DomainEventBase
{
    public Guid PetWalkerId { get; }
    public string Email { get; }
    public string FullName { get; }
    public DateTime CreatedAt { get; }
}

public class PetWalkerCreatedHandler : INotificationHandler<PetWalkerCreatedEvent>
{
    private readonly IEmailService _emailService;
    private readonly IBackgroundCheckService _backgroundCheckService;

    public async Task Handle(PetWalkerCreatedEvent notification, CancellationToken cancellationToken)
    {
        // Send welcome email
        // Initiate background check process
        // Create payment account
    }
}
```

### 2. VerificationSubmittedEvent
```csharp
public class VerificationSubmittedEvent : DomainEventBase
{
    public Guid PetWalkerId { get; }
    public IReadOnlyList<Document> Documents { get; }
    public DateTime SubmittedAt { get; }
}

public class VerificationSubmittedHandler : INotificationHandler<VerificationSubmittedEvent>
{
    private readonly IVerificationService _verificationService;
    
    public async Task Handle(VerificationSubmittedEvent notification, CancellationToken cancellationToken)
    {
        // Queue documents for review
        // Update verification status
        // Notify admin team
    }
}
```

### 3. ServiceAreaAddedEvent
```csharp
public class ServiceAreaAddedEvent : DomainEventBase
{
    public Guid PetWalkerId { get; }
    public string ZipCode { get; }
    public int RadiusInMiles { get; }
}

public class ServiceAreaAddedHandler : INotificationHandler<ServiceAreaAddedEvent>
{
    private readonly ISearchIndexService _searchIndexService;
    
    public async Task Handle(ServiceAreaAddedEvent notification, CancellationToken cancellationToken)
    {
        // Update search index
        // Notify potential clients in area
    }
}
```