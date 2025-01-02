using FluentValidation;
using FurryFriends.UseCases.Services;
using Serilog;

namespace FurryFriends.UseCases.Users.AddBioPicture;
public class AddPhotoHandler(IUserService userService, IValidator<AddPhotoCommand> validator, ILogger logger) : ICommandHandler<AddPhotoCommand, Result>
{
  private readonly IUserService _userService = userService;
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
