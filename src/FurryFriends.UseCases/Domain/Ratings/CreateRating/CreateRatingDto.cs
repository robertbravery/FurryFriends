namespace FurryFriends.UseCases.Domain.Ratings.CreateRating;

public record CreateRatingDto(Guid BookingId, int RatingValue, string? Comment);
