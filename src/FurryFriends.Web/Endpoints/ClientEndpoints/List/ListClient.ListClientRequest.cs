using FurryFriends.Web.Endpoints.Base;

namespace FurryFriends.Web.Endpoints.ClientEndpoints.List;

public class ListClientRequest : ListRequestBase, IRequest<ListResponse<ClientDto>>
{
  public const string Route = "/Clients/list";
}

