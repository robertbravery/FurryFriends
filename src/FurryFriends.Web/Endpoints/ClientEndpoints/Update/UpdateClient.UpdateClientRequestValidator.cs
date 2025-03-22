using FurryFriends.Web.Endpoints.ClientEndpoints.Validator;

namespace FurryFriends.Web.Endpoints.ClientEndpoints.Update;

public class UpdateClientRequestValidator : Validator<UpdateClientRequest>
{
  public UpdateClientRequestValidator()
  {

    Include(new ClientRequestValidator<UpdateClientRequest>());
  }
}
