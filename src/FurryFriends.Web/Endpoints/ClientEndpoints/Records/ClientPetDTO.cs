namespace FurryFriends.Web.Endpoints.ClientEndpoints.Records;

public record PetRecord(
    Guid Id,
    string Name,
    string Species,
    string Breed,
    int Age,
    double? Weight,
    string? SpecialNeeds,
    string? MedicalConditions,
    bool IsActive
);
