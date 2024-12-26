using FluentValidation;

namespace FurryFriends.Web.Endpoints.UserEndpoints.Create;

public class CreateUserRequestValidator : Validator<CreateUserRequest>
{
  public CreateUserRequestValidator()
  {
    RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
    RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Invalid email format.");
    RuleFor(x => x.CountryCode).NotEmpty().WithMessage("Country code is required.");
    RuleFor(x => x.AreaCode).NotEmpty().WithMessage("Area code is required.");
    RuleFor(x => x.Number).NotEmpty().WithMessage("Number is required.");
    RuleFor(x => x.Street).NotEmpty().WithMessage("Street is required.");
    RuleFor(x => x.City).NotEmpty().WithMessage("City is required.");
    RuleFor(x => x.State).NotEmpty().WithMessage("State is required.");
    RuleFor(x => x.PostalCode).NotEmpty().WithMessage("Postal code is required.");
  }
}
