# Implementation Details

## Domain Models

### Client Aggregate
```csharp
public class Client : AggregateRoot<Guid>
{
    private readonly List<Pet> _pets = new();
    
    public Name Name { get; private set; }
    public Email Email { get; private set; }
    public PhoneNumber Phone { get; private set; }
    public Address Address { get; private set; }
    public ClientType Type { get; private set; }
    public TimeOnly? PreferredContactTime { get; private set; }
    public ReferralSource ReferralSource { get; private set; }
    public IReadOnlyCollection<Pet> Pets => _pets.AsReadOnly();

    private Client() { } // For EF Core

    public static Result<Client> Create(
        Name name,
        Email email,
        PhoneNumber phone,
        Address address,
        ClientType type = ClientType.Regular,
        TimeOnly? preferredContactTime = null,
        ReferralSource referralSource = ReferralSource.None)
    {
        // Implementation details...
    }

    public Result AddPet(Pet pet)
    {
        // Implementation details...
    }
}
```

## Value Objects

### Address Implementation
```csharp
public class Address : ValueObject
{
    public string Street { get; }
    public string City { get; }
    public string State { get; }
    public string Country { get; }
    public string ZipCode { get; }

    private Address() { } // For EF Core

    public static Result<Address> Create(
        string street,
        string city,
        string state,
        string country,
        string zipCode)
    {
        // Validation and creation logic...
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Street.ToLower();
        yield return City.ToLower();
        yield return State.ToLower();
        yield return Country.ToLower();
        yield return ZipCode.ToLower();
    }
}
```

## Repository Implementation
```csharp
public class ClientRepository : EfRepository<Client>, IClientRepository
{
    public ClientRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<bool> IsEmailUniqueAsync(string email, 
        CancellationToken cancellationToken = default)
    {
        return !await _dbContext.Clients
            .AnyAsync(c => c.Email.EmailAddress == email, 
                cancellationToken);
    }

    public async Task<Client?> GetByEmailAsync(string email, 
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Clients
            .Include(c => c.Pets)
            .FirstOrDefaultAsync(c => c.Email.EmailAddress == email, 
                cancellationToken);
    }
}
```