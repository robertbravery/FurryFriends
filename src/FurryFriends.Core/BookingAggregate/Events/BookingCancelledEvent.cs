namespace FurryFriends.Core.BookingAggregate.Events;

public class BookingCancelledEvent : DomainEventBase
{
    public Booking Booking { get; }
  public string Reason { get; }

  public BookingCancelledEvent(Booking booking, string reason)
    {
        Booking = booking;
        Reason = reason;
  }
}
