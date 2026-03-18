// Application/Scheduling/Specifications/BookingByDateSpec.cs
using FurryFriends.Core.BookingAggregate;

public class BookingByDateSpec : Specification<Booking>
{
  public BookingByDateSpec(Guid walkerId, DateTime date)
  {
    var startOfDay = date.Date;
    var endOfDay = startOfDay.AddDays(1);

    Query.Where(b => b.PetWalkerId == walkerId &&
                     b.StartTime >= startOfDay &&
                     b.StartTime < endOfDay);
  }

  public BookingByDateSpec(Guid walkerId, DateTime startOfDay, DateTime endOfDay)
  {
    Query.Where(b => b.PetWalkerId == walkerId &&
                     b.StartTime >= startOfDay &&
                     b.StartTime < endOfDay);
  }
}
