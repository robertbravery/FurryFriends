using FurryFriends.UseCases.Domain.Clients.Command.UpdateClient;

namespace FurryFriends.Web.Endpoints.ClientEnpoints.Update;

public class UpdateClient : Endpoint<UpdateClientRequest, UpdateClientResponse>
{
  private readonly IMediator _mediator;

  public UpdateClient(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {
    Put(UpdateClientRequest.Route);
    AllowAnonymous();
    Options(x => x.WithName("UpdateClient_" + Guid.NewGuid().ToString()));
    Description(d => d
        .Produces<Result<int>>(201)
        .Produces(400)
        .WithTags("Clients"));
  }

  public override async Task HandleAsync(UpdateClientRequest req, CancellationToken ct)
  {
    var command = new UpdateClientCommand
    {
      ClientId = req.ClientId,
      FirstName = req.FirstName,
      LastName = req.LastName,
      Email = req.Email,
      CountryCode = req.PhoneCountryCode,
      PhoneNumber = req.PhoneNumber,
      Street = req.Street,
      City = req.City,
      StateProvinceRegion = req.State,
      ZipCode = req.ZipCode,
      Country = req.Country,
      ClientType = req.ClientType,
      PreferredContactTime = req.PreferredContactTime,
      ReferralSource = req.ReferralSource
    };

    var client = await _mediator.Send(command, ct);

    var response = new UpdateClientResponse
    {
      ClientId = client.Id,
      FirstName = client.Name.FirstName,
      LastName = client.Name.LastName,
      Email = client.Email.EmailAddress,
      PhoneNumber = client.PhoneNumber.Number,
      Address = client.Address.ToString(),
      ClientType = client.ClientType,
      PreferredContactTime = client.PreferredContactTime,
      ReferralSource = client.ReferralSource
    };

    await SendAsync(response, cancellation: ct);
  }
}
