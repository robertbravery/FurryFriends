using Azure;
using FurryFriends.UseCases.Domain.PetWalkers.Query.ListPetWalker;


namespace FurryFriends.Web.Endpoints.PetWalkerEndpoints.List;

public class ListPetWalkerByLocation(IMediator mediator, ILogger<ListPetWalker> logger)
  : Endpoint<ListPetWalkerByLocationRequest, ListPetWalkerByLocationResponse>()
{
  private readonly IMediator _mediator = mediator;
  private readonly ILogger<ListPetWalker> _logger = logger;

  public override void Configure()
  {
    Get(ListPetWalkerByLocationRequest.Route);
    AllowAnonymous();
    Options(o => o.WithName("ListUsersByLocation_" + Guid.NewGuid().ToString())); // Ensure unique name

    Summary(s =>
    {
      s.Summary = "Retrieve List of Users by Location";
      s.Description = "Returns a list of users based on search criteria and or filtered by location";
      s.Response<Response>(200, "Users retrieved successfully");
      s.Response<Response>(400, "Failed to retrieve users");
      s.Response<Response>(401, "Unauthorized");
    });
  }

  public override async Task HandleAsync(ListPetWalkerByLocationRequest request, CancellationToken cancellationToken)
  {
    var petWalkerListQuery = new ListPetWalkerByLocationQuery(request.SearchTerm, request.LocationId, request.Page, request.PageSize);
    var userListResult = await _mediator.Send(petWalkerListQuery, cancellationToken);

    if (!userListResult.IsSuccess)
    {
      _logger.LogError(userListResult.Errors.ToString());
      await SendNotFoundAsync(cancellationToken);
      return;
    }

    var userListResponse = userListResult.Value.Users
        .Select(user => new PetWalkerListResponseDto(user.Id,
                                                     user.Name.FullName,
                                                     user.Email.EmailAddress,
                                                     user.Address.City,
                                                     user.ServiceAreas?.FirstOrDefault()?.Locality?.LocalityName ?? string.Empty,
                                                     user.PhoneNumber.Number))
        .ToList();

    var totalCount = userListResult.Value.TotalCount;
    string[] hideColumns = { "Id", "Location" };

    Response = new ListPetWalkerByLocationResponse(userListResponse, request.Page, request.PageSize, totalCount, hideColumns);

  }
}
