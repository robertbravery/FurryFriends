﻿using FluentValidation;

namespace FurryFriends.UseCases.Domain.Clients.Command.CreateClient;
internal class CreateClientCommandValidator : AbstractValidator<CreateClientCommand>
{
  public CreateClientCommandValidator()
  {
    // Client-specific business rules
    RuleFor(x => x.PreferredContactTime)
        .Must(BeWithinBusinessHours)
        .WithMessage("Preferred contact time must be during business hours")
        .WithErrorCode(errorCode: "InvalidPreferredContactTime");

    RuleFor(x => x.ClientType)
        .IsInEnum()
        .WithMessage("Invalid client type selected");

    RuleFor(x => x.ReferralSource).IsInEnum();
  }

  private bool BeWithinBusinessHours(TimeOnly? time)
  {
    if (time == null) return true;
    return time.Value.Hour >= 9 && time.Value.Hour <= 17;
  }
}
