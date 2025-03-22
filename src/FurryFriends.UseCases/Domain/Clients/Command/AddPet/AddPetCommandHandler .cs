using FluentValidation;
using FurryFriends.UseCases.Services.ClientService;
using Serilog;

namespace FurryFriends.UseCases.Domain.Clients.Command.AddPet;

public class AddPetCommandHandler : ICommandHandler<AddPetCommand, Result<Guid>>
{
  private readonly IClientService _clientService;
  private readonly IValidator<AddPetCommand> _validator;
  private readonly ILogger _logger;

  public AddPetCommandHandler(IClientService clientService, IValidator<AddPetCommand> validator, ILogger logger)
  {
    _clientService = clientService;
    _validator = validator;
    _logger = logger;
  }

  public async Task<Result<Guid>> Handle(AddPetCommand command, CancellationToken cancellationToken)
  {
    // Validate the command
    var validationResult = await _validator.ValidateAsync(command, cancellationToken);
    if (!validationResult.IsValid)
    {
      var validationErrors = validationResult.Errors.Select(e => new ValidationError(e.PropertyName, e.ErrorMessage)).ToList();
      return Result.Invalid(validationErrors);
    }

    try
    {
      // Add pet to client through the service
      var result = await _clientService.AddPetToClientAsync(
          command.ClientId,
          command.Name,
          command.BreedId,
          command.Age,
          command.Weight,
          command.Color,
          command.SpecialNeeds,
          command.DietaryRestrictions,
          cancellationToken);

      if (!result.IsSuccess)
      {
        if (result.Errors.Any() || result.ValidationErrors.Any())
        {
          return Result.Error(new ErrorList(result.Errors.Concat(result.ValidationErrors.Select(e => e.ErrorMessage))));
        }
      }

      // Apply additional updates using the specialized methods
      var petId = result.Value;

      // Apply each specialized update if needed
      if (!string.IsNullOrEmpty(command.MedicalConditions))
      {
        await _clientService.AddPetMedicalConditionAsync(command.ClientId, petId, command.MedicalConditions, cancellationToken);
      }

      if (!string.IsNullOrEmpty(command.FavoriteActivities))
      {
        await _clientService.UpdatePetFavoriteActivitiesAsync(command.ClientId, petId, command.FavoriteActivities, cancellationToken);
      }

      if (!string.IsNullOrEmpty(command.SpecialNeeds))
      {
        await _clientService.UpdatePetSpecialNeedsAsync(command.ClientId, petId, command.SpecialNeeds, cancellationToken);
      }

      await _clientService.UpdatePetVaccinationStatusAsync(command.ClientId, petId, command.IsVaccinated, cancellationToken);

      await _clientService.UpdatePetSterilizationStatusAsync(command.ClientId, petId, command.IsSterilized, cancellationToken);

      return Result.Success(petId);
    }
    catch (Exception ex)
    {
      _logger.Error(ex, "Error adding pet to client {ClientId}", command.ClientId);
      return Result.Error($"Error adding pet: {ex.Message}");
    }
  }
}
