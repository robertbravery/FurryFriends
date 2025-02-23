using FurryFriends.Web.Endpoints.ClientEnpoints.Validator;

namespace FurryFriends.Web.Endpoints.ClientEnpoints.Update;

public class UpdateClientRequestValidator : Validator<UpdateClientRequest>
{
  public UpdateClientRequestValidator()
  {

    Include(new ClientRequestValidator<UpdateClientRequest>());
  }
}
