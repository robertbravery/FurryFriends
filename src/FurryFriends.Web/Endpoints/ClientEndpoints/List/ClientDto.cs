namespace FurryFriends.Web.Endpoints.ClientEndpoints.List;

public record ClientDto(
    Guid Id,
    string Name,
    string Email,
    string City,
    int Pets,
    Dictionary<string, int> PetsBySpecies);
