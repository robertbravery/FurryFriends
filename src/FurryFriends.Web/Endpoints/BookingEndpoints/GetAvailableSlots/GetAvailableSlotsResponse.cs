namespace FurryFriends.Web.Endpoints.BookingEndpoints.GetAvailableSlots;

/// <summary>
/// Response for available slots query
/// </summary>
public class GetAvailableSlotsResponse
{
    public Guid PetWalkerId { get; set; }
    public DateTime Date { get; set; }
    public List<AvailableSlotResponse> AvailableSlots { get; set; } = new();
}

/// <summary>
/// Individual available slot information
/// </summary>
public class AvailableSlotResponse
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsAvailable { get; set; }
    public decimal Price { get; set; }
    public string? Notes { get; set; }
}
