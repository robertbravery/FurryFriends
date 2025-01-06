using System.ComponentModel.DataAnnotations;

namespace FurryFriends.Web.Endpoints.PetWalkerEndpoints.Get;

public class GetPetWalkerByEmailRequest
{
  public const string Route = "/PetWalker/email/{email}";

  [EmailAddress]
  public string Email { get; set; } = default!;
}
