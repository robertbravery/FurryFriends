using FluentValidation;

namespace FurryFriends.UseCases.Domain.PetWalkers.Query.GetPetWalker;
public class GetPetWalkerQueryValidator : AbstractValidator<GetPetWalkerQuery>
{
  public GetPetWalkerQueryValidator()
  {
    RuleFor(x => x.EmailAddress).NotEmpty().EmailAddress();
  }
}
