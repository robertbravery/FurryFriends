using Microsoft.AspNetCore.Mvc;
using FromQueryAttribute = FastEndpoints.FromQueryAttribute;

namespace FurryFriends.Web.Endpoints.BookingEndpoints.GetAvailableSlots;

public class GetAvailableSlotsRequest
{
  public const string Route = "/petwalker/{PetWalkerId:guid}/available-slots";

  [FromRoute]
  public Guid PetWalkerId { get; set; }

  [FromQuery]
  public DateTime Date { get; set; }
}
