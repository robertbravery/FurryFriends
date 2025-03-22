namespace FurryFriends.Web.Endpoints.ClientEnpoints.Get;

public class GetClientRequestValidator: Validator<GetClientRequest>
{
  public GetClientRequestValidator()
  {
    RuleFor(x => x.Email).NotEmpty().EmailAddress()
     .WithMessage("Invalid email");
  }
}
