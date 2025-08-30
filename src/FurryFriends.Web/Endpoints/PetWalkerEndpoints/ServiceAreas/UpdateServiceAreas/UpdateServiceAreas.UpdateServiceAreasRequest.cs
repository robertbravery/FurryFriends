using System.ComponentModel.DataAnnotations;

namespace FurryFriends.Web.Endpoints.PetWalkerEndpoints.ServiceAreas.UpdateServiceAreas;

public class UpdateServiceAreasRequest
{
  public const string Route = "/Petwalker/{PetWalkerId:guid}/ServiceAreas";

  [Required]
  public Guid PetWalkerId { get; set; }

  [Required]
  public List<ServiceAreaItem> ServiceAreas { get; set; } = new();

  public class ServiceAreaItem
  {
    [Required]
    public Guid LocalityId { get; set; }
  }
}
