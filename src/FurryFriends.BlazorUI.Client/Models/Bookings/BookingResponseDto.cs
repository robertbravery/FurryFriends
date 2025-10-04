namespace FurryFriends.BlazorUI.Client.Models.Bookings;

/// <summary>
/// Response DTO for booking creation
/// </summary>
public class BookingResponseDto
{
    public Guid BookingId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool Success { get; set; }
}
