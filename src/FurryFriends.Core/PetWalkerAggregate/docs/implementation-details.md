# Implementation Details

## Domain Model Implementation

```csharp
public class PetWalker : AggregateRoot<Guid>
{
    private readonly List<Photo> _photos = new();
    private readonly List<ServiceArea> _serviceAreas = new();
    
    public Name Name { get; private set; }
    public Email Email { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; }
    public Address Address { get; private set; }
    public GenderType Gender { get; private set; }
    public string? Biography { get; private set; }
    public DateTime DateOfBirth { get; private set; }
    public Compensation Compensation { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsVerified { get; private set; }
    public int YearsOfExperience { get; private set; }
    public bool HasInsurance { get; private set; }
    public bool HasFirstAidCertification { get; private set; }
    public int DailyPetWalkLimit { get; private set; }

    public IReadOnlyCollection<Photo> Photos => _photos.AsReadOnly();
    public IReadOnlyCollection<ServiceArea> ServiceAreas => _serviceAreas.AsReadOnly();

    private PetWalker() { } // For EF Core

    public static Result<PetWalker> Create(
        Name name,
        Email email,
        PhoneNumber phoneNumber,
        Address address,
        DateTime dateOfBirth)
    {
        // Validation and creation logic
    }

    public Result AddServiceArea(ServiceArea area)
    {
        // Implementation
    }

    public Result UpdateProfile(ProfileUpdateInfo info)
    {
        // Implementation
    }

    public Result SetCompensation(Money hourlyRate)
    {
        // Implementation
    }
}
```

## Repository Implementation

```csharp
public class PetWalkerRepository : EfRepository<PetWalker>, IPetWalkerRepository
{
    public PetWalkerRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<PetWalker?> GetByEmailAsync(string email, 
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.PetWalkers
            .Include(w => w.Photos)
            .Include(w => w.ServiceAreas)
            .FirstOrDefaultAsync(w => w.Email.EmailAddress == email, 
                cancellationToken);
    }

    public async Task<IReadOnlyList<PetWalker>> GetByServiceAreaAsync(
        string zipCode, 
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.PetWalkers
            .Include(w => w.ServiceAreas)
            .Where(w => w.ServiceAreas.Any(sa => sa.ZipCodes.Contains(zipCode)))
            .ToListAsync(cancellationToken);
    }
}