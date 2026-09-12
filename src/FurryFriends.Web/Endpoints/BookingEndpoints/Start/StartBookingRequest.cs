namespace FurryFriends.Web.Endpoints.BookingEndpoints.Start;

public class StartBookingRequest
{
  public const string Route = "/bookings/{bookingId}/start";

  public Guid BookingId { get; set; }
}
