using Ardalis.GuardClauses;

namespace FurryFriends.Core.ClientAggregate;

public partial class Pet
{
  public void UpdateGeneralInfo(string name, int age, double weight, string color,
      string? medicalHistory,
      bool isVaccinated,
      string? favoriteActivities,
      string? dietaryRestrictions,
      string? specialNeeds,
      string? photo,
      int breedId = 0)
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
    
    // Update BreedId if provided
    if (breedId > 0)
    {
      BreedId = breedId;
    }
  }
}
