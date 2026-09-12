namespace FurryFriends.BlazorUI.Client.Models.Bookings;

/// <summary>
/// Response returned by the booking lifecycle endpoints (confirm, start, complete, no-show).
/// </summary>
public class BookingStatusResponseDto
{
    /// <summary>
    /// ID of the booking that was transitioned
    /// </summary>
    public Guid BookingId { get; set; }

    /// <summary>
    /// The booking's status after the transition (e.g. "Confirmed").
    /// Kept as a string because the API returns the enum name.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Human readable message describing the outcome
    /// </summary>
    public string Message { get; set; } = string.Empty;
}
