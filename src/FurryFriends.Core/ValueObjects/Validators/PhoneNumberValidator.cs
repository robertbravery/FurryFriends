using FluentValidation;
namespace FurryFriends.Core.ValueObjects.Validators;

public class PhoneNumberValidator : AbstractValidator<PhoneNumber>
{

  public PhoneNumberValidator()
  {
    RuleFor(x => x.CountryCode)
      .NotEmpty().WithMessage("Country code is required.")
      .Matches(@"^[0-9]\d{1,3}$").WithMessage("Country code must be between 1 and 3 digits.");

    RuleFor(x => x.Number).NotEmpty().WithMessage("Valid Phonenumber is required.")
      .Matches(@"^[0-9\s\(\)-]{5,15}$")
      .WithMessage("Phone number is required and must be between 5 and 15 digits"); ;
  }
}


