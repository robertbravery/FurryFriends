using Microsoft.AspNetCore.Mvc;

namespace FurryFriends.Web.Endpoints.BookingEndpoints.GetAvailableSlots;

public class GetAvailableSlotsRequest
{
  public const string Route = "/petwalker/{PetWalkerId:guid}/available-slots";

  [FromRoute]
  public Guid PetWalkerId { get; set; }

  [FastEndpoints.FromQuery]
  public DateTime Date { get; set; }
}
