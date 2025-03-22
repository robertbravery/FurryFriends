# Search and Filtering Specifications

## Base Specifications

### PetWalkerByLocationSpec
```csharp
public class PetWalkerByLocationSpec : Specification<PetWalker>
{
    public PetWalkerByLocationSpec(string zipCode, int radiusInMiles)
    {
        Query
            .Where(pw => pw.IsActive && pw.IsVerified)
            .Where(pw => pw.ServiceAreas.Any(sa => 
                sa.IsActive && 
                sa.Covers(zipCode, radiusInMiles)));
    }
}
```

### PetWalkerWithAvailabilitySpec
```csharp
public class PetWalkerWithAvailabilitySpec : Specification<PetWalker>
{
    public PetWalkerWithAvailabilitySpec(DateTime startTime, DateTime endTime)
    {
        Query
            .Where(pw => pw.IsActive && pw.IsVerified)
            .Where(pw => pw.Schedule.HasAvailability(startTime, endTime));
    }
}
```

## Combined Search Specifications

### AvailablePetWalkersSpec
```csharp
public class AvailablePetWalkersSpec : Specification<PetWalker>
{
    public AvailablePetWalkersSpec(
        string zipCode, 
        int radiusInMiles,
        DateTime startTime,
        DateTime endTime,
        decimal? maxHourlyRate = null,
        int? minimumRating = null)
    {
        Query
            .Where(pw => pw.IsActive && pw.IsVerified)
            .Where(pw => pw.ServiceAreas.Any(sa => 
                sa.IsActive && 
                sa.Covers(zipCode, radiusInMiles)))
            .Where(pw => pw.Schedule.HasAvailability(startTime, endTime));

        if (maxHourlyRate.HasValue)
        {
            Query.Where(pw => pw.Compensation.HourlyRate <= maxHourlyRate.Value);
        }

        if (minimumRating.HasValue)
        {
            Query.Where(pw => pw.Rating >= minimumRating.Value);
        }

        Query.OrderByDescending(pw => pw.Rating)
             .ThenBy(pw => pw.Compensation.HourlyRate);
    }
}
```

## Search Implementation

### Search Service
```csharp
public class PetWalkerSearchService : IPetWalkerSearchService
{
    private readonly IReadRepository<PetWalker> _repository;
    
    public async Task<SearchResult<PetWalker>> SearchPetWalkers(
        SearchCriteria criteria,
        int page = 1,
        int pageSize = 20)
    {
        var spec = new AvailablePetWalkersSpec(
            criteria.ZipCode,
            criteria.RadiusInMiles,
            criteria.StartTime,
            criteria.EndTime,
            criteria.MaxHourlyRate,
            criteria.MinimumRating);

        var totalItems = await _repository.CountAsync(spec);
        var items = await _repository.ListAsync(spec
            .Skip((page - 1) * pageSize)
            .Take(pageSize));

        return new SearchResult<PetWalker>(
            items,
            totalItems,
            page,
            pageSize);
    }
}