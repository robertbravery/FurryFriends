using FurryFriends.Web.Endpoints.ClientEndpoints.Request;

namespace FurryFriends.Web.Endpoints.ClientEndpoints.Update;

public class UpdateClientRequest : ClientRequest
{
  public const string Route = "/Clients/{ClientId}";
  public Guid ClientId { get; set; }

}
