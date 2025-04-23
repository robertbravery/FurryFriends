namespace FurryFriends.Web.Endpoints.ClientEndpoints.ListBreeds;

public class ListBreedsRequest : IRequest<List<BreedDto>>
{
  public const string Route = "/Clients/breeds";
}
