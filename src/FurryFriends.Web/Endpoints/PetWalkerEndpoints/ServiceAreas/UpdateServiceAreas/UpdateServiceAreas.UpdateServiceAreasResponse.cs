namespace FurryFriends.Web.Endpoints.PetWalkerEndpoints.ServiceAreas.UpdateServiceAreas;

public class UpdateServiceAreasResponse
{
    public Guid PetWalkerId { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}
