namespace FurryFriends.Web.Endpoints.PetWalkerEndpoints.Get;

public class GetPetWalkerValidator : Validator<GetPetWalkerByEmailRequest>
{
  public GetPetWalkerValidator()
  {
    RuleFor(x => x.Email).NotEmpty().EmailAddress()
     .WithMessage("Invalid email");
  }
}
