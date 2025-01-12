using FluentValidation;

namespace FurryFriends.Web.Endpoints.ClientEnpoints.Create;

public class CreateClientRequestValidator : AbstractValidator<CreateClientRequest>
{
  public CreateClientRequestValidator()
  {

    RuleFor(x => x.FirstName)
               .NotEmpty()
               .WithMessage("First name is required");

    RuleFor(x => x.LastName)
        .NotEmpty()
        .WithMessage("Last name is required");

    RuleFor(x => x.Email)
        .NotEmpty()
        .EmailAddress()
        .WithMessage("Email address is required and must be in a valid format");

    RuleFor(x => x.PhoneCountryCode)
        .NotEmpty()
        .Matches(@"^[1-9]\d{0,2}$")
        .WithMessage("Phone country code is required and must be between 1 and 3 digits");

    RuleFor(x => x.PhoneNumber)
        .NotEmpty()
        .Matches(@"^\d{5,15}$")
        .WithMessage("Phone number is required and must be between 5 and 15 digits");

    RuleFor(x => x.Street)
        .NotEmpty()
        .WithMessage("Street is required");

    RuleFor(x => x.City)
        .NotEmpty()
        .WithMessage("City or Town is required");

    RuleFor(x => x.State)
        .NotEmpty()
        .WithMessage("State or province is required");

    RuleFor(x => x.ZipCode)
        .NotEmpty()
        .Matches(@"^\d{5}?$")
        .WithMessage("Zip or postal code is required");
  }
}
