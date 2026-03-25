using FurryFriends.UseCases.Rating.GetRatingsForPetWalker;

namespace FurryFriends.UseCases.Rating.GetPetWalkerRatingSummary;

public record PetWalkerRatingSummaryDto(
    Guid PetWalkerId,
    double AverageRating,
    int TotalRatings,
    List<RatingDto> RecentRatings);
