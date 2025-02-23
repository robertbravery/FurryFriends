using FurryFriends.Web.Endpoints.Base;

namespace FurryFriends.Web.Endpoints.PetWalkerEndpoints.List;

public class ListPetWalkerRequest : ListRequestBase, IRequest<ListResponse<PetWalkerListResponseDto>>
{
  public const string Route = "/PetWalker/list";
}

