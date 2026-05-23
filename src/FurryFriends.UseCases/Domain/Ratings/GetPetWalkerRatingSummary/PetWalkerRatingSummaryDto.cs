using FurryFriends.UseCases.Domain.Ratings.GetRatingsForPetWalker;

namespace FurryFriends.UseCases.Domain.Ratings.GetPetWalkerRatingSummary;

public record PetWalkerRatingSummaryDto(
    Guid PetWalkerId,
    double AverageRating,
    int TotalRatings,
    List<RatingDto> RecentRatings);
