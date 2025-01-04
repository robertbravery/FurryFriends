﻿using FluentValidation;

namespace FurryFriends.UseCases.Users.UpdateUser;

public class UpdateUserHourlyRateCommandValidator : AbstractValidator<UpdateUserHourlyRateCommand>
{
  public UpdateUserHourlyRateCommandValidator()
  {
    RuleFor(x => x.UserId)
        .NotEmpty().WithMessage("UserId is required.");

    RuleFor(x => x.HourlyRate)
        .GreaterThan(0).WithMessage("Hourly rate must be greater than zero.");

    RuleFor(x => x.Currency)
        .NotEmpty().WithMessage("Currency is required.")
        .Length(3).WithMessage("Currency must be a 3-letter ISO code.");
  }
}
