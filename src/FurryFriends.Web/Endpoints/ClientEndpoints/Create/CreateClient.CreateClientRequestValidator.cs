using FurryFriends.Web.Endpoints.ClientEndpoints.Validator;

namespace FurryFriends.Web.Endpoints.ClientEndpoints.Create;

public class CreateClientRequestValidator : Validator<CreateClientRequest>
{
  public CreateClientRequestValidator()
  {

    Include(new ClientRequestValidator<CreateClientRequest>());
  }
}
