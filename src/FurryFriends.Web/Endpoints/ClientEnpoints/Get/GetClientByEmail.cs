using Azure;
using FurryFriends.UseCases.Domain.Clients.Query.GetClient;
using FurryFriends.Web.Endpoints.Base;
using FurryFriends.Web.Endpoints.PetWalkerEndpoints.Get;

namespace FurryFriends.Web.Endpoints.ClientEnpoints.Get;

public class GetClientByEmail : Endpoint<GetClientRequest, ResponseBase<ClientRecord>>
{
  private readonly IMediator _mediator;

  public GetClientByEmail(IMediator mediator)
  {
    _mediator = mediator;
  }

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
      if (result.IsNotFound())
      {
        //Response = new ResponseBase<ClientRecord>(null, false, "Client not found");
        var message = "Client not found";

        Response = ResponseBase<ClientRecord>.NotFound(message, result.Errors.ToList());
        await SendAsync(Response, 404, ct);
        return;
      }

      Response = new ResponseBase<ClientRecord>(null, false, "Failed to retrieve Client");
      await SendAsync(Response, 400, ct);
      return;
    }
    else
    {
      var clientRecord = new ClientRecord(
          result.Value.Id,
          result.Value.Name,
          result.Value.Email,
          result.Value.PhoneNumber,
          result.Value.Street,
          result.Value.City,
          result.Value.State,
          result.Value.ZipCode,
          result.Value.ClientType,
          result.Value.PreferredContactTime,
          result.Value.ReferralSource);

      Response = new ResponseBase<ClientRecord>(clientRecord);
      await SendAsync(Response, 200, ct);
    }
  }
}
