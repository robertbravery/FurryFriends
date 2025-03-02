using Ardalis.GuardClauses;
using Azure;
using FurryFriends.UseCases.Domain.Clients.Query;
using FurryFriends.UseCases.Domain.Clients.Query.GetClient;
using FurryFriends.Web.Endpoints.Base;
using FurryFriends.Web.Endpoints.PetWalkerEndpoints.Get;

namespace FurryFriends.Web.Endpoints.ClientEnpoints.Get;

public class GetClientByEmail(IMediator mediator) : Endpoint<GetClientRequest, ResponseBase<ClientRecord>>
{
  private readonly IMediator _mediator = Guard.Against.Null(mediator);

  public override void Configure()
  {
    Get(GetClientRequest.Route);
    Options(o => o.WithName("GetClientByEmail_" + Guid.NewGuid().ToString())); // Ensure unique name
    Summary(s =>
    {
      s.Summary = "Get Client By Email";
      s.Description = "Returns a Client by email";
      s.Response<GetPetWalkerByEmailResponse>(200, "Get Client By Email");
      s.Response<Response>(400, "Failed to retrieve Client");
      s.Response<Response>(401, "Unauthorized");
      s.Response<Response>(404, "Not Found");
    });
    AllowAnonymous();
  }

  public override async Task HandleAsync(GetClientRequest request, CancellationToken ct)
  {
    var query = new GetClientQuery(request.Email);
    var result = await _mediator.Send(query, ct);

    if (result.Value is null || !result.IsSuccess)
    {
      await HandleFailedResult(result, ct);
      return;
    }

    var clientRecord = MapToClientRecord(result.Value);
    Response = new ResponseBase<ClientRecord>(clientRecord);
    await SendAsync(Response, 200, ct);

  }

  async Task HandleFailedResult(Result<ClientDTO> result, CancellationToken ct)
  {
    if (result.IsNotFound())
    {
      var message = "Client not found";
      Response = ResponseBase<ClientRecord>.NotFound(message, result.Errors.ToList());
      await SendAsync(Response, 404, ct);
      return;
    }

    Response = new ResponseBase<ClientRecord>(null, false, "Failed to retrieve Client");
    await SendAsync(Response, 400, ct);
  }

  ClientRecord MapToClientRecord(ClientDTO client)
  {
    return new ClientRecord(
        client.Id,
        client.Name,
        client.Email,
        client.PhoneNumber,
        client.Street,
        client.City,
        client.State,
        client.ZipCode,
        client.ClientType,
        client.PreferredContactTime,
        client.ReferralSource
    );
  }
}
