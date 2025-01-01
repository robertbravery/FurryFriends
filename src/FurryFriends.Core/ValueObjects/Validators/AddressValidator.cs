using FluentValidation;

namespace FurryFriends.Core.ValueObjects.Validators;

public class AddressValidator : AbstractValidator<Address>
{
  public AddressValidator()
  {
    RuleFor(x => x.Street).NotEmpty().WithMessage("Street is required.");
    RuleFor(x => x.City).NotEmpty().WithMessage("City is required.");
    RuleFor(x => x.StateProvinceRegion).NotEmpty().WithMessage("State/Province/Region is required.");
    RuleFor(x => x.Country).NotEmpty().WithMessage("Country is required.");
    RuleFor(x => x.ZipCode).NotEmpty().WithMessage("Zip Code is required.")
     .Matches(@"^\d{4,5}(?:-\d{4})?$").WithMessage("Zip Code must be in the format XXXX, XXXX-XXXX, or XXXXX.");

  }
}
