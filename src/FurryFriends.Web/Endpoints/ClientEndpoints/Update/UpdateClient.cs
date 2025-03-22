using FurryFriends.UseCases.Domain.Clients.Command.UpdateClient;

namespace FurryFriends.Web.Endpoints.ClientEndpoints.Update;

public class UpdateClient(IMediator mediator) : Endpoint<UpdateClientRequest, Result<UpdateClientResponse>, ClientResponseMapper>
{
  private readonly IMediator _mediator = mediator;

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

    Response = Map.FromEntity(client);

  }
}
