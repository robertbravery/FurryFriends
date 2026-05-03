namespace FurryFriends.UseCases.Domain.Ratings.GetRatingsForPetWalker;

public record RatingDto(
    Guid Id,
    Guid PetWalkerId,
    Guid ClientId,
    int RatingValue,
    string? Comment,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    string? ClientName);
