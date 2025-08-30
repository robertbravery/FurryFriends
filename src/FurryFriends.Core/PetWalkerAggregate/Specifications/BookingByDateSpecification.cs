namespace FurryFriends.Core.PetWalkerAggregate.Specifications;

public class BookingByDateSpecification : Specification<Booking>
{
  public BookingByDateSpecification(Guid walkerId, DateTime date)
  {
    var startOfDay = date.Date;
    var endOfDay = startOfDay.AddDays(1);

    Query.Where(b => b.PetWalkerId == walkerId &&
                     b.Start >= startOfDay &&
                     b.Start < endOfDay);
  }
}


