namespace FurryFriends.Core.ClientAggregate;
public class Pet : EntityBase<Guid>
{
  public string Name { get; private set; } = default!;
  public int BreedId { get; private set; } = default!;
  public int Age { get; private set; }
  public string Species { get; private set; } = default!;
  public double Weight { get; private set; }
  public string Color { get; private set; } = default!;
  public string? MedicalConditions { get; private set; }
  public bool VaccinationStatus { get; private set; }
  public Guid OwnerId { get; private set; }
  public string? FavoriteActivities { get; private set; }
  public string? DietaryRestrictions { get; private set; }
  public virtual Client Owner { get; private set; } = default!;
  public virtual Breed BreedType { get; private set; } = default!;

  private Pet() { }

  private Pet(string name, int breedId, int age, string species, double weight, string color, Guid ownerId)
  {
    Guard.Against.NullOrWhiteSpace(name, nameof(name));
    Guard.Against.OutOfRange(name.Length, nameof(name), 1, 50);
    Guard.Against.NegativeOrZero(breedId, nameof(breedId));
    Guard.Against.NegativeOrZero(age, nameof(age));
    Guard.Against.NullOrWhiteSpace(species, nameof(species));
    Guard.Against.OutOfRange(species.Length, nameof(species), 1, 50);
    Guard.Against.NegativeOrZero(weight, nameof(weight));
    Guard.Against.OutOfRange(weight, nameof(weight), 0.1, 200);
    Guard.Against.NullOrWhiteSpace(color, nameof(color));
    Guard.Against.OutOfRange(color.Length, nameof(color), 1, 30);
    Guard.Against.Null(ownerId, nameof(ownerId));

    Id = Guid.NewGuid();
    Name = name;
    BreedId = breedId;
    Age = age;
    Species = species;
    Weight = weight;
    Color = color;
    OwnerId = ownerId;
  }

  public static Pet Create(string name, int breedId, int age, string species, double weight, string color, Guid ownerId)
  {
    return new Pet(name, breedId, age, species, weight, color, ownerId);
  }

  public void UpdateDetails(string name, int breedId, int age, string species, double weight, string color)
  {
    Guard.Against.NullOrWhiteSpace(name, nameof(name));
    Guard.Against.OutOfRange(name.Length, nameof(name), 1, 50);
    Guard.Against.NegativeOrZero(breedId, nameof(breedId));
    Guard.Against.NegativeOrZero(age, nameof(age));
    Guard.Against.NullOrWhiteSpace(species, nameof(species));
    Guard.Against.OutOfRange(species.Length, nameof(species), 1, 50);
    Guard.Against.NegativeOrZero(weight, nameof(weight));
    Guard.Against.OutOfRange(weight, nameof(weight), 0.1, 200);
    Guard.Against.NullOrWhiteSpace(color, nameof(color));
    Guard.Against.OutOfRange(color.Length, nameof(color), 1, 30);

    Name = name;
    BreedId = breedId;
    Age = age;
    Species = species;
    Weight = weight;
    Color = color;
  }

  public void AddMedicalCondition(string medicalCondition)
  {
    Guard.Against.NullOrWhiteSpace(medicalCondition, nameof(medicalCondition));
    Guard.Against.OutOfRange(medicalCondition.Length, nameof(medicalCondition), 1, 500);
    MedicalConditions = string.IsNullOrEmpty(MedicalConditions) ? medicalCondition : $"{MedicalConditions}, {medicalCondition}";
  }

  public void UpdateVaccinationStatus(bool vaccinationStatus)
  {
    VaccinationStatus = vaccinationStatus;
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
}
