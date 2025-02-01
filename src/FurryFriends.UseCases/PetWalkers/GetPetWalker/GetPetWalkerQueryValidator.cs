using FluentValidation;

namespace FurryFriends.UseCases.PetWalkers.GetPetWalker;
public class GetPetWalkerQueryValidator : AbstractValidator<GetPetWalkerQuery>
{
  public GetPetWalkerQueryValidator()
  {
    RuleFor(x => x.Email).NotEmpty();
  }
}
