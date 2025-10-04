namespace FurryFriends.BlazorUI.Client.Models.Clients;

public class PetDto
{
  public Guid Id { get; set; } = default!;
  public string Name { get; set; } = default!;
  public string Species { get; set; } = default!;
  public string Breed { get; set; } = default!;
  public int BreedId { get; set; }
  public int Age { get; set; }
  public int Weight { get; set; }
  public string SpecialNeeds { get; set; } = default!;
  public string MedicalConditions { get; set; } = default!;
  public bool isActive { get; set; }
  public string Photo { get; set; } = default!;
}
