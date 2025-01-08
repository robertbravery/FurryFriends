using FluentValidation;
namespace FurryFriends.Core.ValueObjects.Validators;

public class DateOfBirthValidator : AbstractValidator<DateOfBirth>
{
  public DateOfBirthValidator()
  {
    RuleFor(x => x.Date)
        .NotNull()
        .WithMessage("Date of birth cannot be null.")
        .LessThanOrEqualTo(DateTime.Now)
        .WithMessage("Date of birth cannot be in the future.")
        .GreaterThanOrEqualTo(DateTime.Now.AddYears(-120))
        .WithMessage("Date of birth is too far in the past.")
        .GreaterThanOrEqualTo(DateTime.Now.AddYears(-70))
        .WithMessage("Date of birth is too old for a labour-type worker.");
  }
}

