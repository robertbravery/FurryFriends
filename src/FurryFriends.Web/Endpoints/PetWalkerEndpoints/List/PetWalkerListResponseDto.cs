namespace FurryFriends.Web.Endpoints.PetWalkerEndpoints.List;

public record PetWalkerListResponseDto
(
    Guid Id, string Name, string EmailAddress, string City, string Location
);
