namespace FurryFriends.Web.Endpoints.ClientEndpoints.UpdatePet;

public class UpdateClientPetRequest
{
  public const string Route = "/clients/{clientId}/pets/{petId}";
  public Guid ClientId { get; set; }
  public Guid PetId { get; set; }
  public string Name { get; set; } = default!;
  public int Age { get; set; }
  public double Weight { get; set; }
  public string Color { get; set; } = default!;
  public string DietaryRestrictions { get; set; } = default!;
  public string FavoriteActivities { get; set; } = default!;
}
