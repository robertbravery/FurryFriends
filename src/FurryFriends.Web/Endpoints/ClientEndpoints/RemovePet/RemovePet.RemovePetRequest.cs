namespace FurryFriends.Web.Endpoints.ClientEndpoints.RemovePet;

public class RemovePetRequest
{
  public const string Route = "/Clients/RemovePet/{PetId:guid}";

  public static string BuildRoute(Guid petId) => Route.Replace("{PetId:guid}", petId.ToString());

  public Guid PetId { get; set; }
}
