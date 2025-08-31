using FurryFriends.Core.BookingAggregate.Enums;

namespace FurryFriends.Core.BookingAggregate.Specifications;

public sealed class ActiveBookingsByPetWalkerOnDateSpec : Specification<Booking>
{
  public ActiveBookingsByPetWalkerOnDateSpec(Guid petWalkerId, DateTime date)
  {
    Query
        .Where(b => b.PetWalkerId == petWalkerId)
        .Where(b => b.StartTime.Date == date.Date)
        .Where(b => b.Status == BookingStatus.Confirmed ||
                   b.Status == BookingStatus.Pending ||
                   b.Status == BookingStatus.InProgress)
        .Include(b => b.PetWalker)
        .OrderBy(b => b.StartTime);
  }
}
