# Client Queries

## Available Queries

### 1. GetClientQuery
Retrieves a single client by ID or email.
```csharp
public record GetClientQuery(
    string EmailAddress
) : IQuery<Result<ClientDTO>>;
```

### 2. ListClientsQuery
Retrieves a paginated list of clients.
```csharp
public record ListClientsQuery(
    string? SearchTerm,
    int Page,
    int PageSize
) : IQuery<Result<List<ClientDTO>>>;
```

### 3. SearchClientsQuery
Searches for clients based on various criteria.
```csharp
public record SearchClientsQuery(
    string? NameSearch,
    string? EmailSearch,
    ClientType? ClientType,
    int Page,
    int PageSize
) : IQuery<Result<PagedList<ClientDTO>>>;
```

## Query Results (DTOs)
```csharp
public record ClientDTO(
    Guid Id,
    string Name,
    string Email,
    string PhoneNumber,
    string Street,
    string City,
    string State,
    string ZipCode,
    ClientType ClientType,
    TimeOnly? PreferredContactTime,
    ReferralSource? ReferralSource
);
```