using FurryFriends.Web.Endpoints.Base;
using FurryFriends.Web.Endpoints.ClientEndpoints.Records;

namespace FurryFriends.Web.Endpoints.ClientEndpoints.Get;

public class GetClientByEmailResponse(ClientRecord? data, bool success = true, string message = "Success", List<string>? errors = null)
  : ResponseBase<ClientRecord>(data, success, message, errors)
{
}
