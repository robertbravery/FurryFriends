﻿using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace FurryFriends.UseCases.Domain.PetWalkers.Command.UpdatePhoto;

public class UpdatePhotoValidator : AbstractValidator<UpdatePhotoCommand>
{
  public UpdatePhotoValidator()
  {
    RuleFor(x => x.PetWalkerId)
      .NotEmpty()
      .WithMessage("PetWalker ID is required");

    RuleFor(x => x.PhotoId)
      .NotEmpty()
      .WithMessage("Photo ID is required");

    RuleFor(x => x.File)
      .NotNull()
      .WithMessage("File is required");

    RuleFor(x => x.File.Length)
      .LessThanOrEqualTo(5 * 1024 * 1024) // 5MB
      .WithMessage("File size must be less than 5MB")
      .When(x => x.File != null);

    RuleFor(x => x.File.ContentType)
      .Must(BeValidImageType)
      .WithMessage("File must be a valid image (JPEG, PNG, or GIF)")
      .When(x => x.File != null);

    RuleFor(x => x.Description)
      .MaximumLength(500)
      .WithMessage("Description must be less than 500 characters");
  }

  private bool BeValidImageType(string contentType)
  {
    return contentType == "image/jpeg" || 
           contentType == "image/png" || 
           contentType == "image/gif";
  }
}
