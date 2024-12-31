using FurryFriends.Web.Endpoints.UserEndpoints.Records;

namespace FurryFriends.Web.Endpoints.UserEndpoints.Get;

public class GetUserByEmailResponse(UserRecord? data, bool success = true, string message = "Success", List<string>? errors = null) 
  : ResponseBase<UserRecord>(data, success, message, errors)
{
}
