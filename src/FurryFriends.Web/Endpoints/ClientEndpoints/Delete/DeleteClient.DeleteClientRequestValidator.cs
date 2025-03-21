﻿namespace FurryFriends.Web.Endpoints.ClientEndpoints.Delete;

public class DeleteClientRequestValidator : AbstractValidator<DeleteClientRequest>
{
  public DeleteClientRequestValidator()
  {
    RuleFor(x => x).NotEmpty().WithMessage("Request cannot be empty");
    RuleFor(x => x.ClientId)
        .NotEmpty()
        .WithMessage("Client ID is required");
  }
}
