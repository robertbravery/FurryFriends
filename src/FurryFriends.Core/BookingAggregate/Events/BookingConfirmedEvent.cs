namespace FurryFriends.Core.BookingAggregate.Events;

public class BookingConfirmedEvent : DomainEventBase
{
    public Booking Booking { get; }

    public BookingConfirmedEvent(Booking booking)
    {
        Booking = booking;
    }
}
