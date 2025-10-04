namespace FurryFriends.Core.BookingAggregate.Specifications;

public sealed class BookingsByPetWalkerSpec : Specification<Booking>
{
  public BookingsByPetWalkerSpec(Guid petWalkerId, DateTime startDate, DateTime endDate)
  {
    Query
        .Where(b => b.PetWalkerId == petWalkerId)
        .Where(b => b.StartTime >= startDate && b.EndTime <= endDate)
        .Include(b => b.PetWalker)
        .OrderBy(b => b.StartTime);
  }
}
