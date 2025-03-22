using FurryFriends.Web.Endpoints.Base;

namespace FurryFriends.Web.Endpoints.PetWalkerEndpoints.Create;

public class CreatePetWalkerResponse(string data, bool success = true, string message = "Success", List<string>? errors = null)
  : ResponseBase<string>(data, success, message, errors)
{
}
