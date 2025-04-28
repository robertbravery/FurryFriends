using FurryFriends.Core.LocationAggregate;

namespace FurryFriends.UseCases.Services.LocationService;

public interface ILocationService
{
  Task<List<Region>> GetAllRegionsAsync(CancellationToken cancellationToken = default);
  Task<List<Locality>> GetLocalitiesByRegionAsync(Guid regionId, CancellationToken cancellationToken = default);
}
