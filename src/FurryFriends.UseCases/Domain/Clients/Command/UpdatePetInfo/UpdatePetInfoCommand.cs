

public record UpdatePetInfoCommand(
    Guid ClientId,
    Guid PetId,
    string Name,
    int Age,
    double Weight,
    string Color,
    string? MedicalHistory,
    bool IsVaccinated,
    string? FavoriteActivities,
    string? DietaryRestrictions,
    string? SpecialNeeds,
    string? Photo) : ICommand<Result>;
