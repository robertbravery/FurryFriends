using System.ComponentModel.DataAnnotations;

namespace FurryFriends.Web.Endpoints.UserEndpoints.Get;

public class GetUserByEmailRequest
{
  public const string Route = "/users/{email}";

  [EmailAddress]
  public string Email { get; set; } = default!;
}
