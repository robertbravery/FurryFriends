namespace FurryFriends.Web.Endpoints.RatingEndpoints.Update;

public record UpdateRatingResponse(Guid Id, int? RatingValue, string? Comment);
