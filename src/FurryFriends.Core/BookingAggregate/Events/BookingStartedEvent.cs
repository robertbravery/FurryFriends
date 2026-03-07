namespace FurryFriends.Core.BookingAggregate.Events;

public class BookingStartedEvent : DomainEventBase
{
    public Booking Booking { get; }

    public BookingStartedEvent(Booking booking)
    {
        Booking = booking;
    }
}
