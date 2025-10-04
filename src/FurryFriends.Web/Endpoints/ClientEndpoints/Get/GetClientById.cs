using System.Net;
using Ardalis.GuardClauses;
using Azure;
using FurryFriends.UseCases.Domain.Clients.DTO;
using FurryFriends.UseCases.Domain.Clients.Query.GetClient;
using FurryFriends.Web.Endpoints.Base;
using FurryFriends.Web.Endpoints.ClientEndpoints.Records;
using FurryFriends.Web.Endpoints.PetWalkerEndpoints.Get;

namespace FurryFriends.Web.Endpoints.ClientEndpoints.Get;

public class GetClientById(IMediator mediator) : Endpoint<GetClientByIdRequest, ResponseBase<ClientRecord>, ClientDTOMapper>
{
  private readonly IMediator _mediator = Guard.Against.Null(mediator);

  public override void Configure()
  {
    Get(GetClientByIdRequest.Route);
    Options(o => o.WithName("GetClientById_" + Guid.NewGuid().ToString())); // Ensure unique name
    Summary(s =>
    {
      s.Summary = "Get Client By Client ID";
      s.Description = "Returns a Client by The Guid ID";
      s.Response<GetPetWalkerByEmailResponse>(200, "Get Client By ID");
      s.Response<Response>(400, "Failed to retrieve Client");
      s.Response<Response>(401, "Unauthorized");
      s.Response<Response>(404, "Not Found");
    });
    AllowAnonymous();
  }

  public override async Task HandleAsync(GetClientByIdRequest request, CancellationToken ct)
  {
    var query = new GetClientByIdQuery(request.ID);
    var result = await _mediator.Send(query, ct);

    if (result.Value is null || !result.IsSuccess)
    {
      await HandleFailedResult(result, ct);
      return;
    }

    var clientRecord = Map.FromEntity(result.Value);
    Response = new GetClientByIdResponse(clientRecord);

  }

  async Task HandleFailedResult(Result<ClientDTO> result, CancellationToken ct)
  {
    if (result.IsNotFound())
    {
      var message = "Client not found";
      AddError(e => e.ID, message);
      await SendErrorsAsync((int)HttpStatusCode.NotFound, ct);
      return;
    }

    await SendErrorsAsync(cancellation: ct);
    return;
  }
}

