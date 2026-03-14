namespace FurryFriends.BlazorUI.Client.Models.Bookings;

/// <summary>
/// Request DTO for cancelling a booking
/// </summary>
public class CancelBookingRequestDto
{
    /// <summary>
    /// Reason for cancellation
    /// </summary>
    public CancellationReason Reason { get; set; }

    /// <summary>
    /// Who initiated the cancellation
    /// </summary>
    public CancelledBy CancelledBy { get; set; }

    /// <summary>
    /// Optional additional notes for cancellation
    /// </summary>
    public string? AdditionalNotes { get; set; }
}

/// <summary>
/// Cancellation reason options
/// </summary>
public enum CancellationReason
{
    ClientRequest,
    PetWalkerRequest,
    Other
}

/// <summary>
/// Who cancelled the booking
/// </summary>
public enum CancelledBy
{
    Client,
    PetWalker
}