

namespace FurryFriends.Web.Endpoints.BookingEndpoints.GetClientBookings;

public class GetClientBookingsRequest
{
  public const string Route = "/bookings/client/{ClientId:guid}";

  public Guid ClientId { get; set; }

  [FromQuery]
  public DateTime StartDate { get; set; }

  [FromQuery]
  public DateTime EndDate { get; set; }
}
