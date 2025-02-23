using FurryFriends.Web.Endpoints.ClientEnpoints.Request;

namespace FurryFriends.Web.Endpoints.ClientEnpoints.Update;

public class UpdateClientRequest : ClientRequest
{
  public const string Route = "/Clients/{ClientId}";
  public Guid ClientId { get; set; }

}
