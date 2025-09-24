
namespace FurryFriends.Web.Endpoints.BookingEndpoints.GetPetWalkerBookings;

public class GetPetWalkerBookingsRequest
{
  public const string Route = "/bookings/petwalker/{PetWalkerId:guid}";

  public Guid PetWalkerId { get; set; }

  [FromQuery]
  public DateTime StartDate { get; set; }

  [FromQuery]
  public DateTime EndDate { get; set; }
}
