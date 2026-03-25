namespace FurryFriends.UseCases.Rating.CreateRating;

public record CreateRatingDto(Guid BookingId, int RatingValue, string? Comment);
