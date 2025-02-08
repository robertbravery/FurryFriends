using System.ComponentModel.DataAnnotations;

namespace FurryFriends.Web.Endpoints.ClientEnpoints.Get;

public class GetClientRequest
{
  public const string Route = "/Clients/email/{email}";

  [EmailAddress]
  public string Email { get; set; } = default!;
}
