using FurryFriends.Web.Endpoints.ClientEnpoints.Validator;

namespace FurryFriends.Web.Endpoints.ClientEnpoints.Create;

public class CreateClientRequestValidator : Validator<CreateClientRequest>
{
  public CreateClientRequestValidator()
  {

    Include(new ClientRequestValidator<CreateClientRequest>());
  }
}
