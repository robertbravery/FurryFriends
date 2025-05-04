﻿namespace FurryFriends.Web.Endpoints.PetWalkerEndpoints.Create;

public class AddPhotoRequestValidator : Validator<AddPhotoRequest>
{
  public AddPhotoRequestValidator()
  {
    RuleFor(x => x.PetWalkerId)
      .NotEmpty()
      .WithMessage("PetWalker ID is required");

    RuleFor(x => x.File)
      .NotNull()
      .WithMessage("File is required");

    RuleFor(x => x.File.Length)
      .LessThanOrEqualTo(5 * 1024 * 1024) // 5MB
      .WithMessage("File size must be less than 5MB")
      .When(x => x.File != null);

    RuleFor(x => x.File.ContentType)
      .Must(x => x == "image/jpeg" || x == "image/png" || x == "image/gif")
      .WithMessage("File must be a valid image (JPEG, PNG, or GIF)")
      .When(x => x.File != null);

    RuleFor(x => x.Description)
      .MaximumLength(500)
      .WithMessage("Description must be less than 500 characters");
  }
}
