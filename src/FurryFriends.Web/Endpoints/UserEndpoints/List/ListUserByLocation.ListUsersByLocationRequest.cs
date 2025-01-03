namespace FurryFriends.Web.Endpoints.UserEndpoints.List;

public class ListUsersByLocationRequest : IRequest<ListUsersByLocationRequest>
{
  public const string Route = "/users/location/{page:int?}/{pageSize:int?}";
  public string? SearchTerm { get; set; }
  public Guid? Location { get; set; }
  public int Page { get; set; } = 1;
  public int PageSize { get; set; } = 10;

  // You can add any query parameters or filters here
}
