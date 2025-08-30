namespace FurryFriends.Web.Endpoints.BookingEndpoints.Create;

public class CreateBookingRequest
{
  public const string Route = "/Bookings";

  public DateTime StartDate { get; set; }
  public DateTime EndDate { get; set; }
  public int PetId { get; set; }
  public Guid PetWalkerId { get; set; }
  public Guid PetOwnerId { get; set; }
}
