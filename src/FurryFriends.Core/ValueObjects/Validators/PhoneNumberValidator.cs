using FluentValidation;
namespace FurryFriends.Core.ValueObjects.Validators;

  public class PhoneNumberValidator : AbstractValidator<PhoneNumber>
  {
    
     public PhoneNumberValidator()
    {
        RuleFor(x => x.CountryCode).NotEmpty().WithMessage("Country code is required.");
        RuleFor(x => x.AreaCode).NotEmpty().WithMessage("Area code is required.");
        RuleFor(x => x.Number).NotEmpty().WithMessage("Number is required.");


    }
  }

