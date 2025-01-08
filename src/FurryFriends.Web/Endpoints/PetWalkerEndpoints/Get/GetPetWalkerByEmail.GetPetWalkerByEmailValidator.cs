using FluentValidation;

namespace FurryFriends.Web.Endpoints.PetWalkerEndpoints.Get;

public class GetPetWalkerValidator : Validator<GetPetWalkerByEmailRequest>
{
  public GetPetWalkerValidator()
  {
    RuleFor(x => x.Email).NotEmpty().EmailAddress()
      .WithMessage("Empty email.")
      .Matches(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")
      .WithMessage("Invalid email format.");
  }
}
