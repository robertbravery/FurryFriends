namespace FurryFriends.BlazorUI.Client.Models.Clients;

public class ClientDto
{
  public Guid Id { get; set; }
  public string? Name { get; set; }
  public string? EmailAddress { get; set; }
  public string? City { get; set; }
  public int TotalPets { get; set; }
  public Dictionary<string, int>? PetsBySpecies { get; set; }

}
