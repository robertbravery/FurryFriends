namespace FurryFriends.Web.Endpoints.BookingEndpoints.GetPetWalkerBookings;

/// <summary>
/// Response DTO for PetWalker booking information
/// </summary>
public class PetWalkerBookingResponse
{
    public Guid Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
