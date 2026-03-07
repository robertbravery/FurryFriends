using Ardalis.SharedKernel;

namespace FurryFriends.Core.BookingAggregate.Events;

public class BookingNoShowEvent : DomainEventBase
{
    public Booking Booking { get; }

    public BookingNoShowEvent(Booking booking)
    {
        Booking = booking;
    }
}