namespace FurryFriends.Web.Endpoints.RatingEndpoints.Update;

public class UpdateRatingRequest
{
    public const string Route = "/ratings/{RatingId}";

    public Guid RatingId { get; set; }

    public int? RatingValue { get; set; }
    public string? Comment { get; set; }
}
