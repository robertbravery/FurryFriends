namespace FurryFriends.BlazorUI.Client.Models.Bookings;

/// <summary>
/// Represents an available time slot for booking
/// </summary>
public class AvailableSlotDto
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsAvailable { get; set; }
    public decimal Price { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Response DTO for available slots query
/// </summary>
public class AvailableSlotsResponseDto
{
    public Guid PetWalkerId { get; set; }
    public DateTime Date { get; set; }
    public List<AvailableSlotDto> AvailableSlots { get; set; } = new();
}
