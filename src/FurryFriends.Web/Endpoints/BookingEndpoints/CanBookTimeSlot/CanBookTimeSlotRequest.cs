

namespace FurryFriends.Web.Endpoints.BookingEndpoints.CanBookTimeSlot;

public class CanBookTimeSlotRequest
{
  public const string Route = "/petwalker/{PetWalkerId:guid}/can-book";

  [Microsoft.AspNetCore.Mvc.FromRoute]
  public Guid PetWalkerId { get; set; }

  // Bound from the query string by convention; see GetClientBookingsRequest for why
  // [FromQuery] is intentionally omitted on scalar values.
  public DateTime StartTime { get; set; }

  public DateTime EndTime { get; set; }
}
