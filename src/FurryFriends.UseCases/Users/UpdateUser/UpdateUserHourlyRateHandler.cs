using System.Numerics;
using Ardalis.Specification;
using FluentValidation;
using FurryFriends.UseCases.Services;

namespace FurryFriends.UseCases.Users.UpdateUser;
public class UpdateUserHourlyRateHandler(IUserService _userService, IValidator<UpdateUserHourlyRateCommand> _validator) : ICommandHandler<UpdateUserHourlyRateCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateUserHourlyRateCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            var validationErrors = validationResult.Errors.Select(e => new ValidationError(e.PropertyName, e.ErrorMessage)).ToList();
            return Result.Invalid(validationErrors);
        }
        var updateResult = await _userService.UpdateUserHourlyRateAsync(command.UserId, command.HourlyRate, command.Currency, cancellationToken);
    if (!updateResult.IsSuccess)
    {
      var errorMessages = updateResult.Errors.ToArray();
      return Result.NotFound(errorMessages);
    }
    return Result.Success(true);
    }
}
