namespace FurryFriends.Core.BookingAggregate.Events;

public class BookingCompletedEvent : DomainEventBase
{
    public Booking Booking { get; }

    public BookingCompletedEvent(Booking booking)
    {
        Booking = booking;
    }
}
