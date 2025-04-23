using Ardalis.GuardClauses;

namespace FurryFriends.Core.ClientAggregate;

public partial class Client
{
  public Result UpdatePetInfo(Guid petId, string name, int age, double weight,
      string color, string? medicalHistory, bool isVaccinated, string? favoriteActivities, 
      string? dietaryRestrictions, string? specialNeeds, string? photo, int breedId = 0)
  {
    var pet = Pets.FirstOrDefault(p => p.Id == petId);
    if (pet == null)
      return Result.Error("Pet not found in this client's pets");

    pet.UpdateGeneralInfo(name, age, weight, color, medicalHistory, isVaccinated, 
        favoriteActivities, dietaryRestrictions, specialNeeds, photo, breedId);
    return Result.Success();
  }
}
