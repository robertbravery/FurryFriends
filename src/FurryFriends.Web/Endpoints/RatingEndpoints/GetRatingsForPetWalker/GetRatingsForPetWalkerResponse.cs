namespace FurryFriends.Web.Endpoints.RatingEndpoints.GetRatingsForPetWalker;

public record GetRatingsForPetWalkerResponse(
    Guid Id,
    Guid PetWalkerId,
    Guid ClientId,
    int RatingValue,
    string? Comment,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    string? ClientName);