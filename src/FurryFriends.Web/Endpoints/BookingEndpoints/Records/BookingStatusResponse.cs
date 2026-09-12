namespace FurryFriends.Web.Endpoints.BookingEndpoints.Records;

/// <summary>
/// Shared response for booking lifecycle transitions (confirm, start, complete, no-show).
/// </summary>
public class BookingStatusResponse(Guid bookingId, string status, string message)
{
  public Guid BookingId { get; set; } = bookingId;
  public string Status { get; set; } = status;
  public string Message { get; set; } = message;
}
