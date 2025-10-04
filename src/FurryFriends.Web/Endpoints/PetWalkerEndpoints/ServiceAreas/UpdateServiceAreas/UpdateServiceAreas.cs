using Ardalis.GuardClauses;
using FurryFriends.UseCases.Services.PetWalkerService;

namespace FurryFriends.Web.Endpoints.PetWalkerEndpoints.ServiceAreas.UpdateServiceAreas;

public class UpdateServiceAreas(IServiceAreaService serviceAreaService, IPetWalkerService petWalkerService, ILogger<UpdateServiceAreas> logger)
    : Endpoint<UpdateServiceAreasRequest, UpdateServiceAreasResponse>
{
  private readonly IServiceAreaService _serviceAreaService = Guard.Against.Null(serviceAreaService);
  private readonly ILogger<UpdateServiceAreas> _logger = Guard.Against.Null(logger);
  private readonly IPetWalkerService _petWalkerService = petWalkerService;

  public override void Configure()
  {
    Put(UpdateServiceAreasRequest.Route);
    AllowAnonymous();
    Options(x => x.WithName("UpdateServiceAreas_" + Guid.NewGuid().ToString()));
    Description(d => d
        .Produces<UpdateServiceAreasResponse>(200)
        .Produces(400)
        .WithTags("Petwalker"));
  }

  public override async Task HandleAsync(UpdateServiceAreasRequest request, CancellationToken cancellationToken)
  {
    try
    {
      _logger.LogInformation("Updating service areas for pet walker {PetWalkerId}", request.PetWalkerId);

      // Get the pet walker
      var petWalkerResult = await _petWalkerService.GetPetWalkerByIdAsync(request.PetWalkerId, true, cancellationToken);
      if (!petWalkerResult.IsSuccess)
      {
        _logger.LogWarning("Pet walker {PetWalkerId} not found", request.PetWalkerId);
        await SendNotFoundAsync(cancellationToken);
        return;
      }

      var petWalker = petWalkerResult.Value;

      // Get current service areas
      var currentServiceAreas = petWalker.ServiceAreas.ToList();

      // Determine which service areas to add and which to remove
      var currentLocalityIds = currentServiceAreas.Select(sa => sa.LocalityID).ToHashSet();
      var newLocalityIds = request.ServiceAreas.Select(sa => sa.LocalityId).ToHashSet();

      // Service areas to remove (in current but not in new)
      var serviceAreasToRemove = currentServiceAreas
          .Where(sa => !newLocalityIds.Contains(sa.LocalityID))
          .ToList();

      // Service areas to add (in new but not in current)
      var localityIdsToAdd = newLocalityIds
          .Where(id => !currentLocalityIds.Contains(id))
          .ToList();

      // Remove service areas that are no longer needed
      foreach (var serviceArea in serviceAreasToRemove)
      {
        _logger.LogInformation("Removing service area {ServiceAreaId} for pet walker {PetWalkerId}",
            serviceArea.Id, request.PetWalkerId);

        var removeResult = await _serviceAreaService.RemoveServiceAreaAsync(serviceArea.Id, cancellationToken);

        if (!removeResult.IsSuccess)
        {
          _logger.LogWarning("Failed to remove service area {ServiceAreaId} for pet walker {PetWalkerId}: {Error}",
              serviceArea.Id, request.PetWalkerId, string.Join(", ", removeResult.Errors));
        }
        else
        {
          _logger.LogInformation("Successfully removed service area {ServiceAreaId} for pet walker {PetWalkerId}",
              serviceArea.Id, request.PetWalkerId);
        }
      }

      // Add new service areas
      foreach (var localityId in localityIdsToAdd)
      {
        var addResult = await _serviceAreaService.AddServiceAreaAsync(
            request.PetWalkerId, localityId, cancellationToken);

        if (!addResult.IsSuccess)
        {
          _logger.LogWarning("Failed to add service area for locality {LocalityId}: {Error}",
              localityId, string.Join(", ", addResult.Errors));
        }
      }

      await SendAsync(new UpdateServiceAreasResponse
      {
        PetWalkerId = request.PetWalkerId,
        Success = true,
        Message = "Service areas updated successfully"
      });
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error updating service areas for pet walker {PetWalkerId}", request.PetWalkerId);
      await SendErrorsAsync(500, cancellationToken);
    }
  }
}
