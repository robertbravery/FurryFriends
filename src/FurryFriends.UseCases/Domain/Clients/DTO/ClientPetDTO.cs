namespace FurryFriends.UseCases.Domain.Clients.DTO;

public record ClientPetDto(
    Guid Id,
    string Name,
    // string Species,
    string Breed,
    int Age,
    double? Weight,
    bool IsSterilized,
    bool IsVaccinated,
    string? SpecialNeeds,
    string? MedicalConditions,
    bool IsActive
);
