﻿using FurryFriends.Core.ClientAggregate.Enums;

namespace FurryFriends.Web.Endpoints.ClientEndpoints.AddClientPet;

public class AddPetRequest
{
  public const string Route = "/clients/{clientId}/pets";
  // Core properties required for Pet creation
  public string Name { get; set; } = string.Empty;
  public int BreedId { get; set; }
  public int Age { get; set; }
  public string Species { get; set; } = string.Empty;
  public double Weight { get; set; }
  public string Color { get; set; } = string.Empty;

  // Optional properties that can be set after creation
  public PetGender Gender { get; set; }
  public bool IsSterilized { get; set; }
  public string? MedicalConditions { get; set; }
  public bool IsVaccinated { get; set; }
  public string? FavoriteActivities { get; set; }
  public string? DietaryRestrictions { get; set; }
  public string SpecialNeeds { get; set; } = default!;
}

