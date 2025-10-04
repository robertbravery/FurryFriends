namespace FurryFriends.Web.Endpoints.LocationEndpoints.GetLocalitiesByRegion;

public class GetLocalitiesByRegionRequest
{
    public const string Route = "/Locations/regions/{RegionId}/localities";
    
    public Guid RegionId { get; set; }
}
