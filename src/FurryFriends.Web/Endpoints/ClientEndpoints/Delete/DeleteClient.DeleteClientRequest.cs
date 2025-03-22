namespace FurryFriends.Web.Endpoints.ClientEndpoints.Delete;

public class DeleteClientRequest
{
  public Guid ClientId { get; set; }
  public static string Route => "/Clients/{ClientId}";
}
