namespace FurryFriends.Web.Endpoints.RatingEndpoints.GetRatingsForPetWalker;

public record GetRatingsForPetWalkerResponse(
    Guid Id,
    Guid PetWalkerId,
    Guid ClientId,
    Guid BookingId,
    int RatingValue,
    string? Comment,
    DateTime CreatedDate,
    DateTime? ModifiedDate,
    string? ClientName);
