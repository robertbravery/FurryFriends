using Ardalis.Specification;
using FluentValidation;
using FurryFriends.UseCases.Services.PetWalkerService;

namespace FurryFriends.UseCases.PetWalkers.UpdatePetWalker;
public class UpdatePetWalkerHourlyRateHandler(IPetWalkerService _petWalkerService, IValidator<UpdatePetWalkerHourlyRateCommand> _validator) : ICommandHandler<UpdatePetWalkerHourlyRateCommand, Result<bool>>
{
  public async Task<Result<bool>> Handle(UpdatePetWalkerHourlyRateCommand command, CancellationToken cancellationToken)
  {
    var validationResult = await _validator.ValidateAsync(command, cancellationToken);
    if (!validationResult.IsValid)
    {
      var validationErrors = validationResult.Errors.Select(e => new ValidationError(e.PropertyName, e.ErrorMessage)).ToList();
      return Result.Invalid(validationErrors);
    }
    var updateResult = await _petWalkerService.UpdatePetWalkerHourlyRateAsync(command.UserId, command.HourlyRate, command.Currency, cancellationToken);
    if (!updateResult.IsSuccess)
    {
      var errorMessages = updateResult.Errors.ToArray();
      return Result.NotFound(errorMessages);
    }
    return Result.Success(true);
  }
}
