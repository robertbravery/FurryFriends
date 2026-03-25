namespace FurryFriends.Web.Endpoints.RatingEndpoints.GetRatingsForPetWalker;

public class GetRatingsForPetWalkerRequest
{
    public const string Route = "/petwalkers/{PetWalkerId}/ratings";

    public Guid PetWalkerId { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;
}
