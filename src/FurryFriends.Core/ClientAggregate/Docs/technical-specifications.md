# Technical Specifications

## Aggregate Boundaries

### Client Aggregate Root
- Responsible for maintaining consistency of the entire aggregate
- Handles all modifications to pets within its boundary
- Enforces invariants across the entire aggregate

### Identifiers
- Client ID: GUID (UUID)
- Pet ID: GUID (UUID)
- Breed ID: Integer
- Species ID: Integer

### Persistence
- Uses EF Core for data access
- Implements optimistic concurrency using RowVersion
- Includes shadow properties for audit tracking

## Value Objects Implementation

### Name
```csharp
public class Name
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string FullName => $"{FirstName} {LastName}";
    
    // Max lengths
    public const int MaxFirstNameLength = 50;
    public const int MaxLastNameLength = 50;
}
```

### Email
```csharp
public class Email
{
    public string EmailAddress { get; private set; }
    
    // Validation regex
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase
    );
}
```

### PhoneNumber
```csharp
public class PhoneNumber
{
    public string CountryCode { get; private set; }
    public string Number { get; private set; }
    public string FullNumber => $"+{CountryCode}{Number}";
}
```

## Performance Considerations

### Indexing Strategy
- Email address (unique)
- Phone number
- Client name (for search)
- Pet name (for search)
- Species and breed relationships

### Caching
- Client details cached for 5 minutes
- Species and breeds cached for 1 hour
- Invalidation on updates

### Query Optimization
- Eager loading for common scenarios
- Selective loading for large collections
- Pagination for lists