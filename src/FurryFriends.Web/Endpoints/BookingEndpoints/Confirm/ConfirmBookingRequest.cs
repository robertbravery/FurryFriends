namespace FurryFriends.Web.Endpoints.BookingEndpoints.Confirm;

public class ConfirmBookingRequest
{
  public const string Route = "/bookings/{bookingId}/confirm";

  public Guid BookingId { get; set; }
}
