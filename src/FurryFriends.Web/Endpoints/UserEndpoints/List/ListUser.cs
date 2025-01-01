using Ardalis.Result;
using Azure;
using FurryFriends.UseCases.Users.List;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;


namespace FurryFriends.Web.Endpoints.UserEndpoints.List;

public class ListUser(IMediator mediator, ILogger<ListUser> logger)
  : Endpoint<ListUsersRequest, ListUsersResponse>()
{
  private readonly IMediator _mediator = mediator;
  private readonly ILogger<ListUser> _logger = logger;

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

  public override async Task HandleAsync(ListUsersRequest request, CancellationToken cancellationToken)
  {
    var userListQuery = new ListUsersQuery(request.SearchTerm, request.Page, request.PageSize);
    var userListResult = await _mediator.Send(userListQuery, cancellationToken);

    if (!userListResult.IsSuccess) 
    {
      _logger.LogError(userListResult.Errors.ToString());
      await SendNotFoundAsync(cancellationToken); 
      return;
    }

    var userListResponse = userListResult.Value.Users
        .Select(user => new UserListResponseDto(user.Id, user.Name.FullName, user.Email, user.Address.City))
        .ToList();

    var totalCount = userListResult.Value.TotalCount;
    string[] hideColumns = { "Id"};

    Response = new ListUsersResponse(userListResponse, request.Page, request.PageSize, totalCount, hideColumns);
    
  }
}
