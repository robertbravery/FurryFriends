using System.ComponentModel.DataAnnotations;

namespace FurryFriends.Web.Endpoints.UserEndpoints.Get;

public class GetUserByEmailRequest
{
  public const string Route = "/user/email/{email}";

  [EmailAddress]
  public string Email { get; set; } = default!;
}
