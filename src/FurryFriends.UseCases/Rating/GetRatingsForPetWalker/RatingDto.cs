namespace FurryFriends.UseCases.Rating.GetRatingsForPetWalker;

public record RatingDto(
    Guid Id,
    Guid PetWalkerId,
    Guid ClientId,
    Guid BookingId,
    int RatingValue,
    string? Comment,
    DateTime CreatedDate,
    DateTime? ModifiedDate,
    string? ClientName);
