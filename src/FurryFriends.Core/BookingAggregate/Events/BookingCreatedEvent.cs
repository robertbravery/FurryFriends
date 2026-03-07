namespace FurryFriends.Core.BookingAggregate.Events;

public class BookingCreatedEvent : DomainEventBase
{
    public Booking Booking { get; }

    public BookingCreatedEvent(Booking booking)
    {
        Booking = booking;
    }
}
