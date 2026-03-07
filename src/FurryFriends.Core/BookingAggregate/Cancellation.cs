using FurryFriends.Core.Common;
using System;

namespace FurryFriends.Core.BookingAggregate;

public class Cancellation : BaseEntity<Guid>
{
    public Guid BookingId { get; private set; }
    public DateTime CancellationDate { get; private set; }
    public string Reason { get; private set; } = default!;
    public Guid CancelledBy { get; private set; }

    private Cancellation() { } // Required for EF Core

    public Cancellation(Guid bookingId, Guid cancelledBy, string reason) : base(Guid.NewGuid())
    {   
        BookingId = bookingId;
        CancellationDate = DateTime.UtcNow;
        CancelledBy = cancelledBy;
        Reason = reason;
    }
}
