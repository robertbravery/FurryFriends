using FurryFriends.Core.Enums;
using FurryFriends.Core.TimeslotAggregate;

namespace FurryFriends.Core.TimeslotAggregate.Specifications;

public sealed class PendingCustomTimeRequestsByPetWalkerSpec : Specification<CustomTimeRequest>
{
    public PendingCustomTimeRequestsByPetWalkerSpec(Guid petWalkerId)
    {
        Query
            .Where(r => r.PetWalkerId == petWalkerId)
            .Where(r => r.Status == CustomTimeRequestStatus.Pending);
    }
}

public sealed class CustomTimeRequestsByClientSpec : Specification<CustomTimeRequest>
{
    public CustomTimeRequestsByClientSpec(Guid clientId)
    {
        Query
            .Where(r => r.ClientId == clientId);
    }
}

public sealed class PendingCustomTimeRequestByClientAndPetWalkerSpec : Specification<CustomTimeRequest>
{
    public PendingCustomTimeRequestByClientAndPetWalkerSpec(Guid clientId, Guid petWalkerId)
    {
        Query
            .Where(r => r.ClientId == clientId)
            .Where(r => r.PetWalkerId == petWalkerId)
            .Where(r => r.Status == CustomTimeRequestStatus.Pending);
    }
}

public sealed class CustomTimeRequestByIdSpec : Specification<CustomTimeRequest>
{
    public CustomTimeRequestByIdSpec(Guid id)
    {
        Query
            .Where(r => r.Id == id);
    }
}