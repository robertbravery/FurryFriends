namespace FurryFriends.Web.Endpoints.BookingEndpoints.Cancel;

public class CancelBookingResponse
{
  public bool Success { get; set; }
  public string Message { get; set; } = string.Empty;
  public Guid BookingId { get; set; }

  public CancelBookingResponse(bool success, string message, Guid bookingId)
  {
    Success = success;
    Message = message;
    BookingId = bookingId;
  }
}