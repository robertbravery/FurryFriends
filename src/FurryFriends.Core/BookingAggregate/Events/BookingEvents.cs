using Ardalis.SharedKernel;

namespace FurryFriends.Core.BookingAggregate.Events;

public class BookingCreatedEvent : DomainEventBase
{
    public Booking Booking { get; }

    public BookingCreatedEvent(Booking booking)
    {
        Booking = booking;
    }
}

public class BookingConfirmedEvent : DomainEventBase
{
    public Booking Booking { get; }

    public BookingConfirmedEvent(Booking booking)
    {
        Booking = booking;
    }
}

public class BookingStartedEvent : DomainEventBase
{
    public Booking Booking { get; }

    public BookingStartedEvent(Booking booking)
    {
        Booking = booking;
    }
}

public class BookingCompletedEvent : DomainEventBase
{
    public Booking Booking { get; }

    public BookingCompletedEvent(Booking booking)
    {
        Booking = booking;
    }
}

public class BookingCancelledEvent : DomainEventBase
{
    public Booking Booking { get; }

    public BookingCancelledEvent(Booking booking)
    {
        Booking = booking;
    }
}

public class BookingNoShowEvent : DomainEventBase
{
    public Booking Booking { get; }

    public BookingNoShowEvent(Booking booking)
    {
        Booking = booking;
    }
}