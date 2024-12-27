using FurryFriends.UseCases.Users.List;


namespace FurryFriends.Web.Endpoints.UserEndpoints.List;

public class ListUser : Endpoint<ListUsersRequest, Result<ListUsersResponse>>
{
  private readonly IHttpContextAccessor _httpContextAccessor;
  private readonly IMediator _mediator;

  public ListUser(IHttpContextAccessor httpContextAccessor, IMediator mediator)
  {
    _httpContextAccessor = httpContextAccessor;
    _mediator = mediator; 
  }

  public override void Configure()
  {
    Get(ListUsersRequest.Route); // Specify the route
    AllowAnonymous(); // Adjust as needed
    Summary(s =>
    {
      s.Summary = "List Users";
      s.Description = "Retrieves a list of all users";
    });
  }

  public override async Task<Result> HandleAsync(ListUsersRequest request, CancellationToken cancellationToken)
  {
    var query = new ListUsersQuery(request.SearchTerm, request.Page, request.PageSize);

    var users = await _mediator.Send(query, cancellationToken);
    if (!users.IsSuccess)
    {
      if (_httpContextAccessor.HttpContext is not null)
      {
          _httpContextAccessor.HttpContext.Response.StatusCode = 400;
      }
    Response = Result<ListUsersResponse>.Error("Failed to retrieve users");
      return Result.Invalid(new List<ValidationError> {
            new ValidationError
            {
                Identifier = "Users",
                ErrorMessage = "Failed to retrieve users"
            }
        });
    }
    var x = users.Value.Users.ToList().ConvertAll(c => new UserListResponseDto(c.Id, c.Name, c.Email, c.Address.City));
    var totalCount = users.Value.TotalCount;

    Response = new ListUsersResponse(x, request.Page, request.PageSize, totalCount);
    return Result.Success();
  }

}
