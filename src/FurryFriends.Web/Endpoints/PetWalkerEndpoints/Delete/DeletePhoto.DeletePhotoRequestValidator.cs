﻿namespace FurryFriends.Web.Endpoints.PetWalkerEndpoints.Delete;

public class DeletePhotoRequestValidator : Validator<DeletePhotoRequest>
{
  public DeletePhotoRequestValidator()
  {
    RuleFor(x => x.PetWalkerId)
      .NotEmpty()
      .WithMessage("PetWalker ID is required");

    RuleFor(x => x.PhotoId)
      .NotEmpty()
      .WithMessage("Photo ID is required");
  }
}
