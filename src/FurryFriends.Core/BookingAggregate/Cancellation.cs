using FurryFriends.Core.BookingAggregate.Enums;
using FurryFriends.Core.Common;

namespace FurryFriends.Core.BookingAggregate;

public class Cancellation : AuditableEntity<Guid>
{
  public Guid BookingId { get; private set; }
  public DateTime CancellationDate { get; private set; }
  public CancellationReason Reason { get; private set; }
  public CancelledBy CancelledBy { get; private set; }
  public string? AdditionalNotes { get; private set; }

  public virtual Booking Booking { get; private set; } = default!;

  internal Cancellation() { } // Required by EF Core

  private Cancellation(Guid bookingId, CancellationReason reason, CancelledBy cancelledBy, string? additionalNotes)
  {
    Id = Guid.NewGuid();
    BookingId = bookingId;
    CancellationDate = DateTime.UtcNow;
    Reason = reason;
    CancelledBy = cancelledBy;
    AdditionalNotes = additionalNotes;
  }

  public static Cancellation Create(
    Guid bookingId,
    CancellationReason reason,
    CancelledBy cancelledBy,
    string? additionalNotes = null)
  {
    Guard.Against.Default(bookingId, nameof(bookingId));
    
    if (!Enum.IsDefined(typeof(CancellationReason), reason))
    {
      throw new ArgumentException($"Invalid cancellation reason: {reason}", nameof(reason));
    }
    
    if (!Enum.IsDefined(typeof(CancelledBy), cancelledBy))
    {
      throw new ArgumentException($"Invalid cancelled by value: {cancelledBy}", nameof(cancelledBy));
    }

    return new Cancellation(bookingId, reason, cancelledBy, additionalNotes);
  }
}