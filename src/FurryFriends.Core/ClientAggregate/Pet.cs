using FurryFriends.Core.ClientAggregate.Enums;
using FurryFriends.Core.Common;

namespace FurryFriends.Core.ClientAggregate;
public partial class Pet : AuditableEntity<Guid>
{
  private const string NONE = "None";

  public string Name { get; private set; } = default!;
  public int BreedId { get; private set; }
  public int Age { get; private set; }
  public PetGender Gender { get; private set; }
  public double Weight { get; private set; }
  public string Color { get; private set; } = default!;
  public bool IsSterilized { get; set; }
  public string? MedicalHistory { get; private set; }
  public string? MedicalConditions { get; private set; }
  public bool IsVaccinated { get; set; }
  public string? FavoriteActivities { get; set; }
  public string? DietaryRestrictions { get; set; }
  public string? SpecialNeeds { get; set; } = NONE;
  public string? Photo { get; set; }
  public bool IsActive { get; private set; } = true;
  public DateTime? DeactivatedAt { get; private set; }
  public Guid OwnerId { get; set; }
  public virtual Client Owner { get; set; } = default!;
  public virtual Breed BreedType { get; private set; } = default!;

  // Add a convenience property to get Species name through the relationship
  //[NotMapped]
  //public string Species => BreedType.Species.Name;

  private Pet() { } // Required by EF Core

  private Pet(string name, int breedId, int age, double weight, string color, string specialNeeds, Client owner)
  {
    Guard.Against.NullOrWhiteSpace(name, nameof(name));
    Guard.Against.OutOfRange(name.Length, nameof(name), 1, 50);
    Guard.Against.NegativeOrZero(breedId, nameof(breedId));
    Guard.Against.NegativeOrZero(age, nameof(age));
    Guard.Against.NegativeOrZero(weight, nameof(weight));
    Guard.Against.OutOfRange(weight, nameof(weight), 0.1, 200);
    Guard.Against.NullOrWhiteSpace(color, nameof(color));
    Guard.Against.OutOfRange(color.Length, nameof(color), 1, 30);
    Guard.Against.Null(owner, nameof(owner));
    Guard.Against.NullOrWhiteSpace(specialNeeds, nameof(specialNeeds));

    Name = name;
    BreedId = breedId;
    Age = age;
    Weight = weight;
    Color = color;
    SpecialNeeds = specialNeeds;
    OwnerId = owner.Id;
    Owner = owner;
    IsActive = true;
  }

  public static Pet Create(
    string name, int breedId, int age, double weight, string color, string specialNeeds, Client owner, string? medicalHistory = null, bool isVaccinated = false, string? favoriteActivities = null, string? dietaryRestrictions = null, string? photo = null
    )
  {
    var pet = new Pet(name, breedId, age, weight, color, specialNeeds, owner);
    pet.MedicalHistory = medicalHistory;
    pet.IsVaccinated = isVaccinated;
    pet.FavoriteActivities = favoriteActivities;
    pet.DietaryRestrictions = dietaryRestrictions;
    pet.Photo = photo;
    return pet;
  }

  public static Pet Create(
    string name,
    int age,
    double weight,
    string color,
    string specialNeeds,
    Client owner,
    Breed breed,
    string? medicalHistory = null,
    bool isVaccinated = false,
    string? favoriteActivities = null,
    string? dietaryRestrictions = null,
    string? photo = null)
  {
    var pet = new Pet(name, breed.Id, age, weight, color, specialNeeds, owner);
    pet.MedicalHistory = medicalHistory;
    pet.IsVaccinated = isVaccinated;
    pet.FavoriteActivities = favoriteActivities;
    pet.DietaryRestrictions = dietaryRestrictions;
    pet.Photo = photo;
    pet.BreedType = breed;
    return pet;
  }

  public void UpdateDetails(string name, int breedId, int age, double weight, string color)
  {
    Guard.Against.NullOrWhiteSpace(name, nameof(name));
    Guard.Against.OutOfRange(name.Length, nameof(name), 1, 50);
    Guard.Against.NegativeOrZero(breedId, nameof(breedId));
    Guard.Against.NegativeOrZero(age, nameof(age));
    Guard.Against.NegativeOrZero(weight, nameof(weight));
    Guard.Against.OutOfRange(weight, nameof(weight), 0.1, 200);
    Guard.Against.NullOrWhiteSpace(color, nameof(color));
    Guard.Against.OutOfRange(color.Length, nameof(color), 1, 30);

    Name = name;
    BreedId = breedId;
    Age = age;
    Weight = weight;
    Color = color;
  }

  public void UpdateGeneralInfo(string name, int age, double weight, string color,
      string? medicalHistory,
      bool isVaccinated,
      string? favoriteActivities,
      string? dietaryRestrictions,
      string? specialNeeds,
      string? photo)
  {
    Guard.Against.NullOrWhiteSpace(name, nameof(name));
    Guard.Against.OutOfRange(name.Length, nameof(name), 1, 50);
    Guard.Against.NegativeOrZero(age, nameof(age));
    Guard.Against.NegativeOrZero(weight, nameof(weight));
    Guard.Against.OutOfRange(weight, nameof(weight), 0.1, 200);
    Guard.Against.NullOrWhiteSpace(color, nameof(color));
    Guard.Against.OutOfRange(color.Length, nameof(color), 1, 30);

    Name = name;
    Age = age;
    Weight = weight;
    Color = color;
    MedicalHistory = medicalHistory;
    IsVaccinated = isVaccinated;
    FavoriteActivities = favoriteActivities;
    DietaryRestrictions = dietaryRestrictions;
    SpecialNeeds = specialNeeds;
    Photo = photo;
  }

  public void AddMedicalCondition(string medicalCondition)
  {
    Guard.Against.NullOrWhiteSpace(medicalCondition, nameof(medicalCondition));
    Guard.Against.OutOfRange(medicalCondition.Length, nameof(medicalCondition), 1, 500);
    MedicalConditions = string.IsNullOrEmpty(MedicalConditions) ? medicalCondition : $"{MedicalConditions}, {medicalCondition}";
  }

  public void UpdateVaccinationStatus(bool vaccinationStatus)
  {
    IsVaccinated = vaccinationStatus;
  }

  public void UpdateFavoriteActivities(string favoriteActivities)
  {
    Guard.Against.NullOrWhiteSpace(favoriteActivities, nameof(favoriteActivities));
    Guard.Against.OutOfRange(favoriteActivities.Length, nameof(favoriteActivities), 1, 500);
    FavoriteActivities = favoriteActivities;
  }

  public void UpdateDietaryRestrictions(string dietaryRestrictions)
  {
    Guard.Against.NullOrWhiteSpace(dietaryRestrictions, nameof(dietaryRestrictions));
    Guard.Against.OutOfRange(dietaryRestrictions.Length, nameof(dietaryRestrictions), 1, 500);
    DietaryRestrictions = dietaryRestrictions;
  }

  public void MarkAsInactive()
  {
    if (!IsActive) return;

    IsActive = false;
    DeactivatedAt = DateTime.UtcNow;
  }

  public void MarkAsActive()
  {
    if (IsActive) return;

    IsActive = true;
    DeactivatedAt = null;
  }

}
