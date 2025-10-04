namespace FurryFriends.Web.Endpoints.BookingEndpoints.GetClientBookings;

/// <summary>
/// Response DTO for client booking information
/// </summary>
public class ClientBookingResponse
{
    public Guid Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
