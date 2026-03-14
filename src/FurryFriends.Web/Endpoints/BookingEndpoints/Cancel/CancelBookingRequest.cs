using FurryFriends.Core.BookingAggregate.Enums;

namespace FurryFriends.Web.Endpoints.BookingEndpoints.Cancel;

public class CancelBookingRequest
{
  public const string Route = "/api/bookings/{BookingId}/cancel";

  public Guid BookingId { get; set; }

  public CancellationReason Reason { get; set; }

  public CancelledBy CancelledBy { get; set; }

  public string? AdditionalNotes { get; set; }
}