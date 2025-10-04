

namespace FurryFriends.Web.Endpoints.BookingEndpoints.CanBookTimeSlot;

public class CanBookTimeSlotRequest
{
  public const string Route = "/petwalker/{PetWalkerId:guid}/can-book";

  [Microsoft.AspNetCore.Mvc.FromRoute]
  public Guid PetWalkerId { get; set; }

  [FromQuery]
  public DateTime StartTime { get; set; }

  [FromQuery]
  public DateTime EndTime { get; set; }
}
