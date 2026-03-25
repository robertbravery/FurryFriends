using Ardalis.Specification;
using FurryFriends.Core.RatingAggregate;

namespace FurryFriends.Core.RatingAggregate.Specifications;

public class GetPetWalkerRatingSummarySpecification : Specification<Rating>
{
    public GetPetWalkerRatingSummarySpecification(Guid petWalkerId)
    {
        Query.Where(r => r.PetWalkerId == petWalkerId)
            .OrderByDescending(r => r.CreatedDate)
            .Take(10);
    }
}

public class CountPetWalkerRatingsSpecification : Specification<Rating>
{
    public CountPetWalkerRatingsSpecification(Guid petWalkerId)
    {
        Query.Where(r => r.PetWalkerId == petWalkerId);
    }
}
