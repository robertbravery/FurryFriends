using FurryFriends.Web.Endpoints.ClientEndpoints.Request;

namespace FurryFriends.Web.Endpoints.ClientEndpoints.Validator;

public class ClientRequestValidator<T> : Validator<T> where T : ClientRequest
{
  public ClientRequestValidator()
  {

    RuleFor(x => x.FirstName)
        .NotEmpty()
        .WithMessage("First name is required!");

    RuleFor(x => x.LastName)
        .NotEmpty()
        .WithMessage("Last name is required");

    RuleFor(x => x.Email)
        .NotEmpty()
        .EmailAddress()
        .WithMessage("Email address is required and must be in a valid format");

    RuleFor(x => x.PhoneCountryCode)
        .NotEmpty()
        .WithMessage("Phone country code is required");

    RuleFor(x => x.PhoneNumber)
        .NotEmpty()
        .WithMessage("Phone number is required");

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
        .WithMessage("Zip or postal code is required");
  }
}
