// Application/Scheduling/Specifications/BookingConflictSpec.cs
public class BookingConflictSpec : Specification<Booking>
{
  public BookingConflictSpec(Guid walkerId, DateTime start, DateTime end)
  {
    Query.Where(b => b.PetWalkerId == walkerId &&
                    b.Start < end &&
                    b.End > start);
  }
}
