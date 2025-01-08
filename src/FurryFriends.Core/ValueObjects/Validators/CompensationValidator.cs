using FluentValidation;
namespace FurryFriends.Core.ValueObjects.Validators;

public class CompensationValidator : AbstractValidator<Compensation>
{
  public CompensationValidator()
  {
    RuleFor(x => x.HourlyRate)
        .GreaterThan(0)
        .WithMessage("Hourly rate must be greater than 0");

    RuleFor(x => x.Currency)
        .NotEmpty()
        .Length(3)
        .Matches("^[A-Z]{3}$")
        .WithMessage("Currency must be a valid 3-letter ISO currency code (e.g. USD, EUR, GBP)");
  }
}


