namespace FurryFriends.Web.Endpoints.TimeslotEndpoints.Booking;

public class BookTimeslotResponse
{
    public Guid BookingId { get; set; }
    public Guid TimeslotId { get; set; }
    public Guid PetWalkerId { get; set; }
    public Guid ClientId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string ClientAddress { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool HasTravelBuffer { get; set; }
    public int? TravelBufferMinutes { get; set; }
}
