// Application/Scheduling/Specifications/BookingConflictSpec.cs
using FurryFriends.Core.BookingAggregate;

public class BookingConflictSpec : Specification<Booking>
{
  public BookingConflictSpec(Guid walkerId, DateTime start, DateTime end)
  {
    Query.Where(b => b.PetWalkerId == walkerId &&
                    b.StartTime < end &&
                    b.EndTime > start);
  }
}
