using FurryFriends.Core.BookingAggregate.Enums;

namespace FurryFriends.Core.BookingAggregate.Specifications;

public sealed class OverlappingBookingsSpec : Specification<Booking>
{
  public OverlappingBookingsSpec(Guid petWalkerId, DateTime startTime, DateTime endTime)
  {
    Query
        .Where(b => b.PetWalkerId == petWalkerId)
        .Where(b => b.Status != BookingStatus.Cancelled && b.Status != BookingStatus.Completed)
        .Where(b => b.StartTime < endTime && b.EndTime > startTime);
  }
}
