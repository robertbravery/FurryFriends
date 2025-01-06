namespace FurryFriends.Web.Endpoints.PetWalkerEndpoints.List;

public class ListPetWalkerRequest : IRequest<ListPetWalkerResponse>
{
  public const string Route = "/PetWalker/list";
  public string? SearchTerm { get; set; }
  public int Page { get; set; } = 1;
  public int PageSize { get; set; } = 10;
}
