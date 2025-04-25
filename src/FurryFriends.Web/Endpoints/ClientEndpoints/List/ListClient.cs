using Ardalis.GuardClauses;
using FurryFriends.UseCases.Domain.Clients.Query.ListClients;
using FurryFriends.Web.Endpoints.Base;

namespace FurryFriends.Web.Endpoints.ClientEndpoints.List;

public class ListClient(IMediator mediator, ILogger<ListClient> logger) : Endpoint<ListClientRequest, ListResponse<ClientDto>>()
{
  private readonly IMediator _mediator = mediator;
  private readonly ILogger<ListClient> _logger = logger;
  public override void Configure()
  {
    Get(ListClientRequest.Route);
    AllowAnonymous();
    Options(o => o.WithName("ListClients_" + Guid.NewGuid().ToString())); // Ensure unique name
    Summary(s =>
    {
      s.Summary = "Retrieve List of Clients";
      s.Description = "Returns a list of clients based on search criteria";
      s.Response<ListResponse<ClientDto>>(200, "Clients retrieved successfully");
      s.Response<ListResponse<ClientDto>>(400, "Failed to retrieve clients");
      s.Response<ListResponse<ClientDto>>(401, "Unauthorized");
    });
  }
  public override async Task HandleAsync(ListClientRequest request, CancellationToken cancellationToken)
  {
    Guard.Against.Null(request, nameof(request));
    Guard.Against.Negative(request.Page, nameof(request.Page), "Page must be greater than 0");
    Guard.Against.NegativeOrZero(request.PageSize, nameof(request.PageSize), "PageSize must be greater than 0");

    var clientListQuery = new ListClientQuery(request.SearchTerm, request.Page, request.PageSize);
    var clientListResult = await _mediator.Send(clientListQuery, cancellationToken);

    if (!clientListResult.IsSuccess)
    {
      _logger.LogError(clientListResult.Errors.ToString());
      await SendNotFoundAsync(cancellationToken);
      return;
    }
    var clientListResponse = clientListResult.Value.Clients
        .Select(client => new ClientDto(
            client.Id,
            client.Name,
            client.Email,
            client.City,
            client.Pets.Count(p => p.IsActive),
            client.Pets
                .Where(p => p.IsActive)
                .GroupBy(p => p.Breed) //ToDo: Group By Species
                .ToDictionary(g => g.Key, g => g.Count())))
        .ToList();
    var totalCount = clientListResult.Value.TotalCount;
    string[] hideColumns = { "Id", "Location" };

    Response = new ListResponse<ClientDto>(clientListResponse, request.Page, request.PageSize, totalCount, hideColumns);
  }
}
