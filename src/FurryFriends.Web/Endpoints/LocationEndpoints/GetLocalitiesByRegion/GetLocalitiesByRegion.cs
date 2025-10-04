using Ardalis.GuardClauses;
using FurryFriends.UseCases.Services.LocationService;
using FurryFriends.Web.Endpoints.LocationEndpoints.Records;

namespace FurryFriends.Web.Endpoints.LocationEndpoints.GetLocalitiesByRegion;

public class GetLocalitiesByRegion(ILocationService locationService, ILogger<GetLocalitiesByRegion> logger)
    : Endpoint<GetLocalitiesByRegionRequest, List<LocalityRecord>>
{
  private readonly ILocationService _locationService = Guard.Against.Null(locationService);
  private readonly ILogger<GetLocalitiesByRegion> _logger = Guard.Against.Null(logger);

  public override void Configure()
  {
    Get(GetLocalitiesByRegionRequest.Route);
    AllowAnonymous();
    Options(o => o.WithName("GetLocalitiesByRegion_" + Guid.NewGuid().ToString()));
    Summary(s =>
    {
      s.Summary = "Get Localities By Region";
      s.Description = "Returns a list of localities for a specific region";
      s.Response<List<LocalityRecord>>(200, "Localities retrieved successfully");
      s.Response<List<LocalityRecord>>(400, "Failed to retrieve localities");
      s.Response<List<LocalityRecord>>(404, "Region not found");
    });
  }

  public override async Task HandleAsync(GetLocalitiesByRegionRequest request, CancellationToken cancellationToken)
  {
    try
    {
      var localities = await _locationService.GetLocalitiesByRegionAsync(request.RegionId, cancellationToken);

      if (localities == null || !localities.Any())
      {
        await SendNotFoundAsync(cancellationToken);
        return;
      }

      var localityRecords = localities.Select(l => new LocalityRecord(
          l.Id,
          l.LocalityName,
          l.RegionID
      )).ToList();

      await SendOkAsync(localityRecords, cancellationToken);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error retrieving localities for region {RegionId}", request.RegionId);
      await SendErrorsAsync(500, cancellationToken);
    }
  }
}
