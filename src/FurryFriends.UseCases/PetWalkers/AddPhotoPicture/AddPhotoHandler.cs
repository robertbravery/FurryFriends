using FluentValidation;
using FurryFriends.UseCases.Services;
using Serilog;

namespace FurryFriends.UseCases.PetWalkers.AddPhotoPicture;
public class AddPhotoHandler(IPetWalkerService petWalkerService, IValidator<AddPhotoCommand> validator, ILogger logger) : ICommandHandler<AddPhotoCommand, Result>
{
  private readonly IPetWalkerService _userService = petWalkerService;
  private readonly IValidator<AddPhotoCommand> _validator = validator;
  private readonly ILogger _logger = logger;

  public async Task<Result> Handle(AddPhotoCommand command, CancellationToken cancellationToken)
  {
    var result = await _validator.ValidateAsync(command);
    if (!result.IsValid)
    {
      var message = "Add Biopicture failed: " + result.Errors.ToString(); ;
      _logger.Error(result.Errors?.ToString() ?? message);
      return Result.Error(result.Errors?.ToString() ?? message);
    }

    await _userService.AddBioPictureAsync(command.BioPicture, command.UserId);
    return Result.Success();

  }
}
