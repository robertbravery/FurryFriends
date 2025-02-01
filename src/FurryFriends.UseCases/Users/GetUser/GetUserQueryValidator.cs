using FluentValidation;

namespace FurryFriends.UseCase.Users.GetUser;
public class GetUserQueryValidator : AbstractValidator<GetUserQuery>
{
  public GetUserQueryValidator()
  {
    RuleFor(x => x.Email).NotEmpty();
  }
}
