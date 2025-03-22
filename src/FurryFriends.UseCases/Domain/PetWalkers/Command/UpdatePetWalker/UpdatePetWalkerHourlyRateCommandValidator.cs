using FluentValidation;

namespace FurryFriends.UseCases.Domain.PetWalkers.Command.UpdatePetWalker;

public class UpdatePetWalkerHourlyRateCommandValidator : AbstractValidator<UpdatePetWalkerHourlyRateCommand>
{
  public UpdatePetWalkerHourlyRateCommandValidator()
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
