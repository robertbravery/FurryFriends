using Azure;
using FurryFriends.UseCases.Users.ListUser;


namespace FurryFriends.Web.Endpoints.UserEndpoints.List;

public class ListUserByLocation(IMediator mediator, ILogger<ListUser> logger)
  : Endpoint<ListUsersByLocationRequest, ListUsersByLocationResponse>()
{
  private readonly IMediator _mediator = mediator;
  private readonly ILogger<ListUser> _logger = logger;

  public override void Configure()
  {
    Get(ListUsersByLocationRequest.Route);
    AllowAnonymous();
    Options(o => o.WithName("ListUsersByLocation_" + Guid.NewGuid().ToString())); // Ensure unique name

    Summary(s =>
    {
      s.Summary = "Retrieve List of Users by Location";
      s.Description = "Returns a list of users based on search criteria and or filtered by location";
      s.Response<ListUsersResponse>(200, "Users retrieved successfully");
      s.Response<Response>(400, "Failed to retrieve users");
      s.Response<Response>(401, "Unauthorized");
    });
  }

  public override async Task HandleAsync(ListUsersByLocationRequest request, CancellationToken cancellationToken)
  {
    var userListQuery = new ListUsersByLocationQuery(request.SearchTerm, request.LocationId, request.Page, request.PageSize);
    var userListResult = await _mediator.Send(userListQuery, cancellationToken);

    if (!userListResult.IsSuccess) 
    {
      _logger.LogError(userListResult.Errors.ToString());
      await SendNotFoundAsync(cancellationToken); 
      return;
    }

    var userListResponse = userListResult.Value.Users
        .Select(user => new UserListResponseDto(user.Id, user.Name.FullName, user.Email.EmailAddress, user.Address.City, user.ServiceAreas?.FirstOrDefault()?.Locality?.LocalityName ?? string.Empty))
        .ToList();

    var totalCount = userListResult.Value.TotalCount;
    string[] hideColumns = { "Id", "Location" };

    Response = new ListUsersByLocationResponse(userListResponse, request.Page, request.PageSize, totalCount, hideColumns);
    
  }
}
