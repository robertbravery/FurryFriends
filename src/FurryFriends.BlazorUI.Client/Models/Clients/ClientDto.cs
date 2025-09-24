namespace FurryFriends.BlazorUI.Client.Models.Clients;

public class ClientDto
{
  public Guid Id { get; set; }
  public string? FullName { get; set; }
  public string? EmailAddress { get; set; }
  public string? City { get; set; }
  public string PhoneNumber { get; set; } = null!;
  public int TotalPets { get; set; }
  public Dictionary<string, int>? PetsBySpecies { get; set; }

}
