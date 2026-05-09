namespace FurryFriends.UseCases.Domain.Ratings.CreateRating;

public record CreateRatingDto(Guid PetWalkerId, int RatingValue, string? Comment);
