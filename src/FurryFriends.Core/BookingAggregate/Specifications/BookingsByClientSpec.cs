namespace FurryFriends.Core.BookingAggregate.Specifications;

public sealed class BookingsByClientSpec : Specification<Booking>
{
    public BookingsByClientSpec(Guid clientId, DateTime startDate, DateTime endDate)
    {
        Query
            .Where(b => b.PetOwnerId == clientId)
            .Where(b => b.StartTime >= startDate && b.EndTime <= endDate)
            .Include(b => b.PetWalker)
            .OrderBy(b => b.StartTime);
    }
}
