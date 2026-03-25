namespace FurryFriends.Web.Endpoints.RatingEndpoints.GetPetWalkerRatingSummary;

public class GetPetWalkerRatingSummaryRequest
{
    public const string Route = "/petwalkers/{PetWalkerId}/ratings/summary";

    public Guid PetWalkerId { get; set; }
}
