namespace FurryFriends.Web.Endpoints.ClientEnpoints.Delete;

public class DeleteClientRequest
{
  public Guid ClientId { get; set; }
  public static string Route => "/Clients/{ClientId}";
}
