﻿using FluentValidation;

namespace FurryFriends.UseCases.Domain.PetWalkers.Command.AddPhotoPicture;
public sealed class AddPhotoValidator : AbstractValidator<AddPhotoCommand>
{
  public AddPhotoValidator()
  {
    RuleFor(x => x.UserId).NotEmpty().WithMessage("User id is required");
    RuleFor(x => x.BioPicture).NotEmpty().WithMessage("Photo is required");
    RuleFor(x => x.BioPicture.Description).NotEmpty().WithMessage("Description is required");
    RuleFor(x => x.BioPicture.PhotoType).IsInEnum().WithMessage("Invalid photo type");
  }
}
