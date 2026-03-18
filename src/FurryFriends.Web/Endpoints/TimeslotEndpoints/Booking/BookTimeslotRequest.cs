namespace FurryFriends.Web.Endpoints.TimeslotEndpoints.Booking;

public class BookTimeslotRequest
{
    public const string Route = "/timeslots/{TIMESLOTID}/book";

    public Guid TimeslotId { get; set; }
    public Guid ClientId { get; set; }
    public string ClientAddress { get; set; } = string.Empty;
    public List<Guid> PetIds { get; set; } = new();
}
