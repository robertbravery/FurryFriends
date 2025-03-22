namespace FurryFriends.Web.Endpoints.ClientEndpoints.UpdatePet;

public class UpdateClientPet : Endpoint<UpdateClientPetRequest, Result>
{
  private readonly IMediator _mediator;

  public UpdateClientPet(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {

    Put(UpdateClientPetRequest.Route);
    AllowAnonymous();
    Options(x => x.WithName("UpdateClientPet_" + Guid.NewGuid().ToString()));
    Description(d => d
        .Produces<Result<int>>(201)
        .Produces(400)
        .WithTags("ClientPets"));
  }


  public override async Task<Result> HandleAsync(UpdateClientPetRequest request, CancellationToken cancellationToken = default)
  {
    var command = new UpdatePetInfoCommand(
        request.ClientId,
        request.PetId,
        request.Name,
        request.Age,
        request.Weight,
        request.Color,
        request.DietaryRestrictions,
        request.FavoriteActivities);

    return await _mediator.Send(command, cancellationToken);
  }
}
