namespace FurryFriends.BlazorUI.Client.Models.Bookings;

/// <summary>
/// A single booking as returned by the client/pet walker booking list endpoints.
/// The API returns status as a string, so it is deliberately not mapped to an enum here.
/// </summary>
public class BookingListItemDto
{
    public Guid Id { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    /// <summary>
    /// Booking status as reported by the API (Pending, Confirmed, InProgress, Completed, Cancelled, NoShow).
    /// </summary>
    public string Status { get; set; } = string.Empty;

    public decimal Price { get; set; }

    /// <summary>
    /// Duration of the booking, derived from the start and end times.
    /// </summary>
    public TimeSpan Duration => EndTime - StartTime;

    /// <summary>
    /// Duration formatted for display, e.g. "1h 30m".
    /// </summary>
    public string DurationDisplay
    {
        get
        {
            var duration = Duration;
            if (duration <= TimeSpan.Zero)
            {
                return "-";
            }

            var hours = (int)duration.TotalHours;
            var minutes = duration.Minutes;

            if (hours == 0)
            {
                return $"{minutes}m";
            }

            return minutes == 0 ? $"{hours}h" : $"{hours}h {minutes}m";
        }
    }
}
