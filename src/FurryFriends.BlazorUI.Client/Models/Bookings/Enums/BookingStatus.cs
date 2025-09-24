namespace FurryFriends.BlazorUI.Client.Models.Bookings.Enums;

/// <summary>
/// Represents the status of a booking
/// </summary>
public enum BookingStatus
{
    Pending = 0,
    Confirmed = 1,
    InProgress = 2,
    Completed = 3,
    Cancelled = 4,
    NoShow = 5
}
