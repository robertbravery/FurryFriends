namespace FurryFriends.Web.Endpoints.BookingEndpoints.MarkNoShow;

public class MarkBookingNoShowRequest
{
  public const string Route = "/bookings/{bookingId}/no-show";

  public Guid BookingId { get; set; }
}
