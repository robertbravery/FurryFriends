namespace FurryFriends.Web.Endpoints.RatingEndpoints.Delete;

public class DeleteRatingRequest
{
    public const string Route = "/ratings/{RatingId}";

    public Guid RatingId { get; set; }

    /// <summary>
    /// ClientId is used to verify ownership of the rating.
    /// Will be populated from the authenticated user context when auth is implemented.
    /// </summary>
    public Guid ClientId { get; set; }
}
