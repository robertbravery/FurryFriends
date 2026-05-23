namespace FurryFriends.Web.Endpoints.RatingEndpoints.Create;

public record CreateRatingResponse(Guid Id, Guid PetWalkerId, int RatingValue, string? Comment);
