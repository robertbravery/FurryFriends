

public record UpdatePetInfoCommand(
    Guid ClientId,
    Guid PetId,
    string Name,
    int Age,
    double Weight,
    string Color,
    string? DietaryRestrictions,
    string? FavoriteActivities) : ICommand<Result>;