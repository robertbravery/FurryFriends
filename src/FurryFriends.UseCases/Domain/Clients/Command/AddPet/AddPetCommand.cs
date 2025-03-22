using FurryFriends.Core.ClientAggregate.Enums;

namespace FurryFriends.UseCases.Domain.Clients.Command.AddPet;

public class AddPetCommand : ICommand<Result<Guid>>
{
  public Guid ClientId { get; set; }

  // Core properties required for Pet creation
  public string Name { get; set; } = string.Empty;
  public int BreedId { get; set; }
  public int Age { get; set; }
  public double Weight { get; set; }
  public string Color { get; set; } = string.Empty;
  public string SpecialNeeds { get; set; } = "None";

  // Optional properties that can be set after creation
  public PetGender Gender { get; set; }
  public bool IsSterilized { get; set; }
  public string? MedicalConditions { get; set; }
  public bool IsVaccinated { get; set; }
  public string? FavoriteActivities { get; set; }
  public string? DietaryRestrictions { get; set; }
}
