# Location Specifications

## Base Specifications

### CountryWithRegionsSpec
```csharp
public class CountryWithRegionsSpec : Specification<Country>
{
    public CountryWithRegionsSpec(Guid countryId)
    {
        Query
            .Include(c => c.Regions)
            .Where(c => c.Id == countryId);
    }
}
```

### RegionWithLocalitiesSpec
```csharp
public class RegionWithLocalitiesSpec : Specification<Region>
{
    public RegionWithLocalitiesSpec(Guid regionId)
    {
        Query
            .Include(r => r.Localities)
            .Include(r => r.Country)
            .Where(r => r.Id == regionId);
    }
}
```

### LocalityByNameSpec
```csharp
public class LocalityByNameSpec : Specification<Locality>
{
    public LocalityByNameSpec(string name, Guid regionId)
    {
        Query
            .Include(l => l.Region)
            .Where(l => l.LocalityName.ToLower() == name.ToLower() &&
                       l.RegionID == regionId);
    }
}
```

## Search Implementation

### LocationSearchService
```csharp
public class LocationSearchService : ILocationSearchService
{
    private readonly IReadRepository<Country> _countryRepo;
    private readonly IReadRepository<Region> _regionRepo;
    private readonly IReadRepository<Locality> _localityRepo;
    
    public async Task<SearchResult<Locality>> SearchLocalities(
        string searchTerm,
        Guid? countryId = null,
        Guid? regionId = null,
        int page = 1,
        int pageSize = 20)
    {
        var query = new LocalitySearchSpec(searchTerm, countryId, regionId);
        
        var totalItems = await _localityRepo.CountAsync(query);
        var items = await _localityRepo.ListAsync(query
            .Skip((page - 1) * pageSize)
            .Take(pageSize));

        return new SearchResult<Locality>(
            items,
            totalItems,
            page,
            pageSize);
    }
}