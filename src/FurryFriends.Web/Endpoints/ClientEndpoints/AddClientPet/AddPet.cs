using FurryFriends.UseCases.Domain.Clients.Command.AddPet;
using FurryFriends.Web.Endpoints.Base;


namespace FurryFriends.Web.Endpoints.ClientEndpoints.AddClientPet;

public class AddPet(IMediator mediator, ILogger<AddPet> logger) : BaseEndpoint<AddPetRequest, Result<AddPetResponse>>
{
  private readonly IMediator _mediator = mediator;
  private readonly ILogger<AddPet> _logger = logger;

  public override void Configure()
  {
    Post(AddPetRequest.Route);
    Options(x => x.WithName("AddClientPet_" + Guid.NewGuid().ToString()));
    Description(d => d
        .Produces<Result<int>>(201)
        .Produces(400)
        .WithTags("ClientPets"));
    AllowAnonymous(); // Replace with proper authorization
  }

  public override async Task HandleAsync(AddPetRequest request, CancellationToken cancellationToken)
  {
    try
    {
      var clientId = Route<Guid>("clientId");

      var command = CreateCommand(request, clientId);

      var result = await _mediator.Send(command, cancellationToken);

      if (result.IsSuccess)
      {
        Response = new AddPetResponse(result.Value);
        await SendOkAsync(Response, cancellationToken);
      }
      else if (result.Status == ResultStatus.NotFound)
      {
        await SendNotFoundAsync(cancellationToken);
      }
      else
      {
        await HandleResultErrorsAsync(result, cancellationToken);
      }
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error adding pet to client {ClientId}", Route<Guid>("clientId"));
      await SendErrorsAsync(cancellation: cancellationToken);
    }
  }

  private static AddPetCommand CreateCommand(AddPetRequest request, Guid clientId)
  {
    return new AddPetCommand
    {
      ClientId = clientId,
      Name = request.Name,
      BreedId = request.BreedId,
      Age = request.Age,
      Weight = request.Weight,
      Color = request.Color,
      Gender = request.Gender,
      IsSterilized = request.IsSterilized,
      MedicalConditions = request.MedicalConditions,
      IsVaccinated = request.IsVaccinated,
      FavoriteActivities = request.FavoriteActivities,
      DietaryRestrictions = request.DietaryRestrictions,
      SpecialNeeds = request.SpecialNeeds
    };
  }

  private async Task HandleResultErrorsAsync(Result<Guid> result, CancellationToken cancellationToken)
  {
    if (result?.ValidationErrors?.Any() == true)
    {
      foreach (var error in result.ValidationErrors)
      {
        AddError(error.ErrorMessage);
      }
      _logger.LogError("Error adding pet to client {ClientId}: {Errors}",
          Route<Guid>("clientId"),
          string.Join(", ", result.ValidationErrors));
    }

    if (result?.Errors?.Any() == true)
    {
      foreach (var error in result.Errors)
      {
        AddError(error);
      }
      _logger.LogError("Error adding pet to client {ClientId}: {Errors}",
          Route<Guid>("clientId"),
          string.Join(", ", result.Errors));
    }

    await SendErrorsAsync(
        result!.IsSuccess ?
            StatusCodes.Status500InternalServerError :
            StatusCodes.Status400BadRequest,
        cancellationToken);
  }

}
