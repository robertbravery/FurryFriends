using FurryFriends.Web.Endpoints.RatingEndpoints.GetRatingsForPetWalker;

namespace FurryFriends.Web.Endpoints.RatingEndpoints.GetPetWalkerRatingSummary;

public record GetPetWalkerRatingSummaryResponse(
    Guid PetWalkerId,
    double AverageRating,
    int TotalRatings,
    List<GetRatingsForPetWalkerResponse> RecentRatings);
