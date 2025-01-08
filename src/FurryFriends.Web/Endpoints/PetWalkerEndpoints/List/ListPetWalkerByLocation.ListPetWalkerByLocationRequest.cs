namespace FurryFriends.Web.Endpoints.PetWalkerEndpoints.List;

public class ListPetWalkerByLocationRequest : IRequest<ListPetWalkerByLocationRequest>
{
  public const string Route = "/PetWalker/location";
  public string? SearchTerm { get; set; }
  public Guid? LocationId { get; set; }
  public int Page { get; set; } = 1;
  public int PageSize { get; set; } = 10;
}

