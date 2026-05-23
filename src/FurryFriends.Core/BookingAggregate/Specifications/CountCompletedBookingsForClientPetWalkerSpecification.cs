using Ardalis.Specification;
using FurryFriends.Core.BookingAggregate;
using FurryFriends.Core.BookingAggregate.Enums;

namespace FurryFriends.Core.BookingAggregate.Specifications;

public class CountCompletedBookingsForClientPetWalkerSpecification : Specification<Booking>
{
    public CountCompletedBookingsForClientPetWalkerSpecification(Guid clientId, Guid petWalkerId)
    {
        Query
            .Where(b => b.PetOwnerId == clientId)
            .Where(b => b.PetWalkerId == petWalkerId)
            .Where(b => b.Status == BookingStatus.Completed);
    }
}