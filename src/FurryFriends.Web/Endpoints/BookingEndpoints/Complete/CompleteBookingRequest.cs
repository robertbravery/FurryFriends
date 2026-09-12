namespace FurryFriends.Web.Endpoints.BookingEndpoints.Complete;

public class CompleteBookingRequest
{
  public const string Route = "/bookings/{bookingId}/complete";

  public Guid BookingId { get; set; }
}
