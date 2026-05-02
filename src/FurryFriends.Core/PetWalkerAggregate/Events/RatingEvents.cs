using Ardalis.SharedKernel;
using FurryFriends.Core.RatingAggregate;

namespace FurryFriends.Core.PetWalkerAggregate.Events;

public class RatingAddedEvent : DomainEventBase
{
    public Rating Rating { get; }

    public RatingAddedEvent(Rating rating)
    {
        Rating = rating;
    }
}

public class RatingUpdatedEvent : DomainEventBase
{
    public Rating Rating { get; }

    public RatingUpdatedEvent(Rating rating)
    {
        Rating = rating;
    }
}

public class RatingRemovedEvent : DomainEventBase
{
    public Rating Rating { get; }

    public RatingRemovedEvent(Rating rating)
    {
        Rating = rating;
    }
}