
namespace FurryFriends.Web.Endpoints.BookingEndpoints.GetPetWalkerBookings;

public class GetPetWalkerBookingsRequest
{
  public const string Route = "/bookings/petwalker/{PetWalkerId:guid}";

  public Guid PetWalkerId { get; set; }

  // Bound from the query string by convention. Deliberately NOT decorated with
  // [FromQuery]: that attribute marks a property as a complex query object, which
  // routes the value through the object-binding path. Plain scalars bind fine
  // without it (see ListRequestBase / ListPetWalkerRequest for the same pattern).
  public DateTime StartDate { get; set; }

  public DateTime EndDate { get; set; }
}
