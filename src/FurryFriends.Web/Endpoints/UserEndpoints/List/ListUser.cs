using Azure;
using FurryFriends.UseCases.Users.List;


namespace FurryFriends.Web.Endpoints.UserEndpoints.List;

public class ListUser(IHttpContextAccessor httpContextAccessor, IMediator mediator)
  : BaseEndpoint<ListUsersRequest, Result<ListUsersResponse>>(httpContextAccessor)
{
  private readonly IMediator _mediator = mediator;

  public override void Configure()
  {
    Get(ListUsersRequest.Route);
    AllowAnonymous();
    Options(o => o.WithName("ListUsers_" + Guid.NewGuid().ToString())); // Ensure unique name

    Summary(s =>
    {
      s.Summary = "Retrieve List of Users";
      s.Description = "Returns a list of users based on search criteria";
      s.Response<ListUsersResponse>(200, "Users retrieved successfully");
      s.Response<Response>(400, "Failed to retrieve users");
      s.Response<Response>(401, "Unauthorized");
    });
  }

  public override async Task<Result> HandleAsync(ListUsersRequest request, CancellationToken cancellationToken)
  {
    var userListQuery = new ListUsersQuery(request.SearchTerm, request.Page, request.PageSize);
    var userListResult = await _mediator.Send(userListQuery, cancellationToken);

    if (!userListResult.IsSuccess)
    {
      Response = Result<ListUsersResponse>.Error("Failed to retrieve users");
      return Result.Invalid(new List<ValidationError>
            {
                new() {
                    Identifier = "Users",
                    ErrorMessage = "Failed to retrieve users"
                }
            });
    }

    var userListResponse = userListResult.Value.Users
        .Select(user => new UserListResponseDto(user.Id, user.Name, user.Email, user.Address.City))
        .ToList();

    var totalCount = userListResult.Value.TotalCount;

    Response = new ListUsersResponse(userListResponse, request.Page, request.PageSize, totalCount);
    return Result.Success();
  }
}
