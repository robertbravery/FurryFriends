using FurryFriends.Core.LocationAggregate;
using FurryFriends.Core.PetWalkerAggregate;
using FurryFriends.Core.PetWalkerAggregate.Specifications;
//using Microsoft.EntityFrameworkCore


namespace FurryFriends.UseCases.Services.PetWalkerService;

public class ServiceAreaService : IServiceAreaService
{
  private readonly IRepository<ServiceArea> _serviceArearepository;
  private readonly IReadRepository<Locality> _localityRepository;

  public ServiceAreaService(IRepository<ServiceArea> serviceArearepository, IReadRepository<Locality> localityRepository)
  {
    _serviceArearepository = serviceArearepository;
    _localityRepository = localityRepository;
  }
  public async Task<Result> AddServiceAreaAsync(Guid petWalkerId, Guid localityId, CancellationToken cancellationToken = default)
  {
    try
    {
      // Get the locality
      var locality = await _localityRepository.GetByIdAsync(localityId, cancellationToken);
      if (locality == null)
      {
        return Result.Error("Locality not found.");
      }

      // Check if the service area already exists
      var spec = new GetServiceAreaByPetwalkerId(petWalkerId);
      var currentServiceAreas = await _serviceArearepository.ListAsync(spec, cancellationToken);
      if (currentServiceAreas.Any(sa => sa.LocalityID == localityId))
      {
        return Result.Error("Service area already assigned.");
      }

      // Create the service area
      var serviceArea = ServiceArea.Create(petWalkerId, localityId);

      //currentServiceAreas.Add(serviceArea);

      // Save changes
      await _serviceArearepository.AddAsync(serviceArea, cancellationToken);

      return Result.Success();
    }
    catch (Exception ex)
    {
      return Result.Error($"Failed to add service area: {ex.Message}");
    }
  }

  public async Task<Result> RemoveServiceAreaAsync(Guid serviceAreaId, CancellationToken cancellationToken = default)
  {
    try
    {
      // Get the service area by its ID
      var serviceArea = await _serviceArearepository.GetByIdAsync(serviceAreaId, cancellationToken);
      if (serviceArea is null)
      {
        return Result.Error($"Service area with ID {serviceAreaId} not found.");
      }

      // Delete the service area
      await _serviceArearepository.DeleteAsync(serviceArea, cancellationToken);

      return Result.Success();
    }
    catch (Exception ex)
    {
      return Result.Error($"Failed to remove service area {serviceAreaId}: {ex.Message}");
    }
  }
}
