namespace FurryFriends.Web.Endpoints.UserEndpoints.List;

public class ListUsersByLocationRequest : IRequest<ListUsersByLocationRequest>
{
  public const string Route = "/user/location";
  public string? SearchTerm { get; set; }
  public Guid? LocationId { get; set; }
  public int Page { get; set; } = 1;
  public int PageSize { get; set; } = 10;
}

