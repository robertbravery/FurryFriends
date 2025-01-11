using FurryFriends.Web.Endpoints.Base;
using FurryFriends.Web.Endpoints.UserEndpoints.Records;

namespace FurryFriends.Web.Endpoints.PetWalkerEndpoints.Get;

public class GetPetWalkerByEmailResponse(PetWalkerRecord? data, bool success = true, string message = "Success", List<string>? errors = null)
  : ResponseBase<PetWalkerRecord>(data, success, message, errors)
{
}
