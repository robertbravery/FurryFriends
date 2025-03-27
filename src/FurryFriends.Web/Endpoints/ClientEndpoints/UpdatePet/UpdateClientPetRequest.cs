﻿namespace FurryFriends.Web.Endpoints.ClientEndpoints.UpdatePet;

public class UpdateClientPetRequest
{
  public const string Route = "/Clients/pets";
  public Guid ClientId { get; set; }
  public Guid PetId { get; set; }
  public string Name { get; set; } = default!;
  public int Age { get; set; }
  public double Weight { get; set; }
  public string Color { get; set; } = default!;
  public string? MedicalHistory { get; set; }
  public bool IsVaccinated { get; set; }
  public string? FavoriteActivities { get; set; }
  public string? DietaryRestrictions { get; set; }
  public string? SpecialNeeds { get; set; }
  public string? Photo { get; set; }
}
