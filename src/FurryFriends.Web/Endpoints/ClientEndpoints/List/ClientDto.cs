namespace FurryFriends.Web.Endpoints.ClientEndpoints.List;

public record ClientDto(
    Guid Id, 
    string Name, 
    string EmailAddress, 
    string City,
    int TotalPets,
    Dictionary<string, int> PetsBySpecies);
