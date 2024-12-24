using FluentValidation;
namespace FurryFriends.Core.ValueObjects.Validators;

public class PhoneNumberValidator : AbstractValidator<string>
{
  private const string PhoneFormat = @"^\+?(\d{1,3})[-.\s]?\(?(\d{3})\)?[-.\s]?(\d{4})(?:[-.\s]?(ext|x)\s*(\d{2,5}))?$";
  public PhoneNumberValidator()
  {
    RuleFor(phoneNumber => phoneNumber)
      .NotEmpty()
      .WithMessage("Phone number cannot be empty.");
    RuleFor(phoneNumber => phoneNumber)
        .Matches(PhoneFormat)
        .WithMessage("Invalid phone number format.");

  }
}
