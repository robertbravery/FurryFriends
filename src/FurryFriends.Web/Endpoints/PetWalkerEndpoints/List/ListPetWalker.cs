using Azure;
using FurryFriends.UseCase.Users.ListUser;


namespace FurryFriends.Web.Endpoints.PetWalkerEndpoints.List;

public class ListPetWalker(IMediator mediator, ILogger<ListPetWalker> logger)
  : Endpoint<ListPetWalkerRequest, ListPetWalkerResponse>()
{
  private readonly IMediator _mediator = mediator;
  private readonly ILogger<ListPetWalker> _logger = logger;

  public override void Configure()
  {
    Get(ListPetWalkerRequest.Route);
    AllowAnonymous();
    Options(o => o.WithName("ListUsers_" + Guid.NewGuid().ToString())); // Ensure unique name

    Summary(s =>
    {
      s.Summary = "Retrieve List of Users";
      s.Description = "Returns a list of users based on search criteria";
      s.Response<ListPetWalkerResponse>(200, "Users retrieved successfully");
      s.Response<Response>(400, "Failed to retrieve users");
      s.Response<Response>(401, "Unauthorized");
    });
  }

  public override async Task HandleAsync(ListPetWalkerRequest request, CancellationToken cancellationToken)
  {
    var petWalkerListQuery = new ListPetWalkerQuery(request.SearchTerm, request.Page, request.PageSize);
    var userListResult = await _mediator.Send(petWalkerListQuery, cancellationToken);

    if (!userListResult.IsSuccess)
    {
      _logger.LogError(userListResult.Errors.ToString());
      await SendNotFoundAsync(cancellationToken);
      return;
    }

    var userListResponse = userListResult.Value.Users
        .Select(user => new PetWalkerListResponseDto(user.Id, user.Name.FullName, user.Email.EmailAddress, user.Address.City, string.Empty)) //Note: Not going to list all the user service areas
        .ToList();

    var totalCount = userListResult.Value.TotalCount;
    string[] hideColumns = { "Id", "Location" };

    Response = new ListPetWalkerResponse(userListResponse, request.Page, request.PageSize, totalCount, hideColumns);

  }
}
