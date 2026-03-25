namespace FurryFriends.Web.Endpoints.RatingEndpoints.Create;

public record CreateRatingResponse(Guid Id, Guid BookingId, int RatingValue, string? Comment);
