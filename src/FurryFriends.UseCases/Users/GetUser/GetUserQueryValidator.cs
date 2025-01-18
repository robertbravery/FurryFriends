using FluentValidation;

namespace FurryFriends.UseCases.Users.GetUser;
public class GetUserQueryValidator : AbstractValidator<GetUserQuery>
{
  public GetUserQueryValidator()
  {
    RuleFor(x => x.Email).NotEmpty();
  }
}
