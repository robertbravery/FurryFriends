using FurryFriends.Core.Common;
using FurryFriends.Core.LocationAggregate;
using FurryFriends.Core.LocationAggregate.Specifications;

namespace FurryFriends.UseCases.Services.LocationService;

public class LocationService : ILocationService
{
  private readonly IRepository<Region> _regionRepository;
  private readonly IRepository<Locality> _localityRepository;

  public LocationService(IRepository<Region> regionRepository, IRepository<Locality> localityRepository)
  {
    _regionRepository = regionRepository;
    _localityRepository = localityRepository;
  }

  public async Task<List<Region>> GetAllRegionsAsync(CancellationToken cancellationToken = default)
  {
    var spec = new AllRegionsSpecification();
    var regions = await _regionRepository.ListAsync(spec, cancellationToken);
    return regions.ToList();
  }

  public async Task<List<Locality>> GetLocalitiesByRegionAsync(Guid regionId, CancellationToken cancellationToken = default)
  {
    var spec = new LocalitiesByRegionSpecification(regionId);
    var localities = await _localityRepository.ListAsync(spec, cancellationToken);
    return localities.ToList();
  }
}
