namespace FurryFriends.BlazorUI.Client.Models.Bookings;

/// <summary>
/// Response DTO for booking cancellation
/// </summary>
public class CancelBookingResponseDto
{
    /// <summary>
    /// Whether the cancellation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Response message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// ID of the cancelled booking
    /// </summary>
    public Guid BookingId { get; set; }
}