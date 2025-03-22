namespace FurryFriends.Web.Endpoints.ClientEndpoints.RemovePet;

public class RemovePetRequest
{
    public const string Route = "/Clients/{ClientId}/Pets/{PetId}";
    public static string BuildRoute(Guid clientId, Guid petId) => Route
        .Replace("{ClientId}", clientId.ToString())
        .Replace("{PetId}", petId.ToString());

    public Guid ClientId { get; set; }
    public Guid PetId { get; set; }
}