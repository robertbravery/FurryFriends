using Ardalis.GuardClauses;
using FluentValidation;
using FurryFriends.Core.PetWalkerAggregate;
using FurryFriends.Core.PetWalkerAggregate.Specifications;
using FurryFriends.UseCases.Services.PictureService;
using Microsoft.Extensions.Logging;

namespace FurryFriends.UseCases.Domain.PetWalkers.Command.UpdatePhoto;

public class UpdatePhotoHandler : ICommandHandler<UpdatePhotoCommand, Result<DetailedPhotoDto>>
{
  private readonly IRepository<PetWalker> _repository;
  private readonly IValidator<UpdatePhotoCommand> _validator;
  private readonly IPictureService _pictureService;
  private readonly ILogger<UpdatePhotoHandler> _logger;

  public UpdatePhotoHandler(
      IRepository<PetWalker> repository,
      IValidator<UpdatePhotoCommand> validator,
      IPictureService pictureService,
      ILogger<UpdatePhotoHandler> logger)
  {
    _repository = Guard.Against.Null(repository);
    _validator = Guard.Against.Null(validator);
    _pictureService = Guard.Against.Null(pictureService);
    _logger = Guard.Against.Null(logger);
  }

  public async Task<Result<DetailedPhotoDto>> Handle(UpdatePhotoCommand command, CancellationToken cancellationToken)
  {
    // Validate the command
    var validationResult = await _validator.ValidateAsync(command, cancellationToken);
    if (!validationResult.IsValid)
    {
      var errors = validationResult.Errors.Select(e => new ValidationError(e.PropertyName, e.ErrorMessage)).ToList();
      return Result.Invalid(errors);
    }

    try
    {
      // Get the pet walker with photos
      var spec = new GetPetWalkerPicturesSpecification(command.PetWalkerId);
      var petWalker = await _repository.FirstOrDefaultAsync(spec, cancellationToken);

      if (petWalker == null)
      {
        _logger.LogWarning("PetWalker with ID {PetWalkerId} not found", command.PetWalkerId);
        return Result.NotFound($"PetWalker with ID {command.PetWalkerId} not found");
      }

      // Find the photo to update
      var photo = petWalker.Photos.FirstOrDefault(p => p.Id == command.PhotoId);
      if (photo == null)
      {
        _logger.LogWarning("Photo with ID {PhotoId} not found for PetWalker {PetWalkerId}",
            command.PhotoId, command.PetWalkerId);
        return Result.NotFound($"Photo with ID {command.PhotoId} not found");
      }

      // Update the photo using the picture service
      var updatedPhoto = await _pictureService.UpdatePetWalkerPhotoAsync(
          command.PetWalkerId,
          command.PhotoId,
          command.File,
          command.Description,
          cancellationToken);

      if (!updatedPhoto.IsSuccess)
      {
        _logger.LogError("Failed to update photo: {ErrorMessage}", updatedPhoto.Errors);
        return Result.Error((ErrorList)updatedPhoto.Errors);
      }

      // Return the updated photo details
      return Result.Success(new DetailedPhotoDto
      {
        Id = updatedPhoto.Value.Id,
        PhotoType = updatedPhoto.Value.PhotoType,
        Url = updatedPhoto.Value.Url,
        Description = updatedPhoto.Value.Description
      });
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error updating photo for PetWalker {PetWalkerId}", command.PetWalkerId);
      return Result.Error($"Error updating photo: {ex.Message}");
    }
  }
}
