using Ardalis.Specification;

namespace FurryFriends.Core.BookingAggregate.Specifications;

public sealed class BookingByIdSpecification : Specification<Booking>, ISingleResultSpecification<Booking>
{
    public BookingByIdSpecification(Guid bookingId)
    {
        Query
            .Where(b => b.Id == bookingId)
            .Include(b => b.PetWalker)
            .Include(b => b.PetOwner);
    }
}
