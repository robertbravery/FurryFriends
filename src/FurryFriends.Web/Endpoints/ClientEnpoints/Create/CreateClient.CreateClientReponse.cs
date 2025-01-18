namespace FurryFriends.Web.Endpoints.ClientEnpoints.Create;

//public class CreateClientReponse(string clientId, bool success = true, string message = "Success", List<string>? errors = null)
//  : ResponseBase<string>(clientId, success, message, errors)
//{ }

public record CreateClientReponse(string clientId);
