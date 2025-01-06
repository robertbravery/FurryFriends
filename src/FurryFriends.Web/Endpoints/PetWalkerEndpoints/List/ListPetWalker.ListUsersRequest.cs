namespace FurryFriends.Web.Endpoints.UserEndpoints.List;

public class ListUsersRequest : IRequest<ListUsersResponse>
{
  public const string Route = "/user/list";
  public string? SearchTerm { get; set; }
  public int Page { get; set; } = 1;
  public int PageSize { get; set; } = 10;
}
