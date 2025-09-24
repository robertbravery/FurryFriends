namespace FurryFriends.Web.Endpoints.ClientEndpoints.Get;

public class GetClientByIdRequest
{
  public const string Route = "/Clients/id/{ID:Guid}";

  
  public Guid ID { get; set; } = default!;
}
