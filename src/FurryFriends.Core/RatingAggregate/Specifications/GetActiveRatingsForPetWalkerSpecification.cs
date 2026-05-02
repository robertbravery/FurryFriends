using Ardalis.Specification;
using FurryFriends.Core.RatingAggregate;

namespace FurryFriends.Core.RatingAggregate.Specifications;

public class GetActiveRatingsForPetWalkerSpecification : Specification<Rating>
{
    public GetActiveRatingsForPetWalkerSpecification(Guid petWalkerId)
    {
        Query
            .Where(r => r.PetWalkerId == petWalkerId)
            .Where(r => r.Status == RatingStatus.Active)
            .OrderByDescending(r => r.CreatedAt);
    }
}