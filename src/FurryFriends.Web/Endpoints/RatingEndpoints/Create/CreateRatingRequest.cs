namespace FurryFriends.Web.Endpoints.RatingEndpoints.Create;

public class CreateRatingRequest
{
    public const string Route = "/ratings";

    public Guid BookingId { get; set; }
    public int RatingValue { get; set; }
    public string? Comment { get; set; }
}
