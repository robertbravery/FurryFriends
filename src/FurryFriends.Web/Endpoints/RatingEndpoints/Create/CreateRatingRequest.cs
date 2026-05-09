namespace FurryFriends.Web.Endpoints.RatingEndpoints.Create;

public class CreateRatingRequest
{
    public const string Route = "/ratings";

    public Guid PetWalkerId { get; set; }
    public Guid ClientId { get; set; }
    public int RatingValue { get; set; }
    public string? Comment { get; set; }
}
