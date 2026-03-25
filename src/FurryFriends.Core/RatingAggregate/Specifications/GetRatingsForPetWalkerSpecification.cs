using Ardalis.Specification;
using FurryFriends.Core.RatingAggregate;

namespace FurryFriends.Core.RatingAggregate.Specifications;

public class GetRatingsForPetWalkerSpecification : Specification<Rating>, ISingleResultSpecification<Rating>
{
    public GetRatingsForPetWalkerSpecification(Guid petWalkerId)
    {
        Query.Where(r => r.PetWalkerId == petWalkerId)
            .OrderByDescending(r => r.CreatedDate);
    }
}

public class GetRatingsForPetWalkerWithPaginationSpecification : Specification<Rating>
{
    public GetRatingsForPetWalkerWithPaginationSpecification(Guid petWalkerId, int page, int pageSize)
    {
        Query.Where(r => r.PetWalkerId == petWalkerId)
            .OrderByDescending(r => r.CreatedDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
    }

    public GetRatingsForPetWalkerWithPaginationSpecification(Guid petWalkerId)
    {
        Query.Where(r => r.PetWalkerId == petWalkerId)
            .OrderByDescending(r => r.CreatedDate);
    }
}
