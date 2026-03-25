using Ardalis.Specification;
using FurryFriends.Core.RatingAggregate;

namespace FurryFriends.Core.RatingAggregate.Specifications;

public class GetRatingByBookingIdSpecification : Specification<Rating>, ISingleResultSpecification<Rating>
{
    public GetRatingByBookingIdSpecification(Guid bookingId)
    {
        Query.Where(r => r.BookingId == bookingId);
    }
}
