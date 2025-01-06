using FurryFriends.Web.Endpoints.UserEndpoints.Records;

namespace FurryFriends.Web.Endpoints.UserEndpoints.Get;

public class GetUserByEmailResponse(PetWalkerRecord? data, bool success = true, string message = "Success", List<string>? errors = null) 
  : ResponseBase<PetWalkerRecord>(data, success, message, errors)
{
}
