using Ardalis.GuardClauses;
using FurryFriends.UseCases.Domain.Clients.Command.RemovePet;

namespace FurryFriends.Web.Endpoints.ClientEndpoints.RemovePet;

public class RemovePet : Endpoint<RemovePetRequest, Result>
{
  private readonly IMediator _mediator;
  private readonly ILogger<RemovePet> _logger;

  public RemovePet(IMediator mediator, ILogger<RemovePet> logger)
  {
    _mediator = Guard.Against.Null(mediator);
    _logger = Guard.Against.Null(logger);
  }

  public override void Configure()
  {
    Delete(RemovePetRequest.Route);
    AllowAnonymous(); // Update with proper authorization
    Options(x => x.WithName("RemovePet_" + Guid.NewGuid().ToString()));
    Description(d => d
        .Produces(204)
        .Produces(404)
        .Produces(400)
        .WithTags("ClientPets"));
  }

  public override async Task HandleAsync(RemovePetRequest request, CancellationToken ct)
  {
    var command = new RemovePetCommand(request.ClientId, request.PetId);
    var result = await _mediator.Send(command, ct);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(ct);
      return;
    }

    if (!result.IsSuccess)
    {
      await SendErrorsAsync(400, ct);
      return;
    }

    await SendOkAsync(ct);
  }
}
