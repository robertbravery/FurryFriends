using Ardalis.GuardClauses;
using FurryFriends.UseCases.Services.LocationService;
using FurryFriends.Web.Endpoints.LocationEndpoints.Records;

namespace FurryFriends.Web.Endpoints.LocationEndpoints.GetRegions;

public class GetRegions(ILocationService locationService, ILogger<GetRegions> logger)
    : EndpointWithoutRequest<List<RegionRecord>>
{
    private readonly ILocationService _locationService = Guard.Against.Null(locationService);
    private readonly ILogger<GetRegions> _logger = Guard.Against.Null(logger);

    public override void Configure()
    {
        Get("/Locations/regions");
        AllowAnonymous();
        Options(o => o.WithName("GetRegions_" + Guid.NewGuid().ToString()));
        Summary(s =>
        {
            s.Summary = "Get All Regions";
            s.Description = "Returns a list of all regions";
            s.Response<List<RegionRecord>>(200, "Regions retrieved successfully");
            s.Response<List<RegionRecord>>(400, "Failed to retrieve regions");
        });
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        try
        {
            var regions = await _locationService.GetAllRegionsAsync(cancellationToken);

            var regionRecords = regions.Select(r => new RegionRecord(
                r.Id,
                r.RegionName,
                r.CountryID
            )).ToList();

            await SendOkAsync(regionRecords, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving regions");
            await SendErrorsAsync(500, cancellationToken);
        }
    }
}
