using Microsoft.AspNetCore.Mvc;
using FromQueryAttribute = FastEndpoints.FromQueryAttribute;

namespace FurryFriends.Web.Endpoints.BookingEndpoints.GetAvailableSlots;

public class GetAvailableSlotsRequest
{
  public const string Route = "/petwalker/{PetWalkerId:guid}/available-slots";

  [FromRoute]
  public Guid PetWalkerId { get; set; }

  // Bound from the query string by convention. Deliberately NOT decorated with
  // [FromQuery]: FastEndpoints requires that attribute only for complex types and throws
  // at startup-binding time otherwise -
  // "The property [...] has to be a complex type in order to work with the [FromQuery] attribute."
  // Scalars bind from the query string automatically.
  public DateTime Date { get; set; }
}
