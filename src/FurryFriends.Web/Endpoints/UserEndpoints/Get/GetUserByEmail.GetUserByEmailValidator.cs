using FluentValidation;

namespace FurryFriends.Web.Endpoints.UserEndpoints.Get;

public class GetUserValidator : Validator<GetUserByEmailRequest>
{
  public GetUserValidator()
  {
    RuleFor(x => x.Email).NotEmpty().EmailAddress()
      .Matches(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")
      .WithMessage("Invalid email format.");
  }
}
