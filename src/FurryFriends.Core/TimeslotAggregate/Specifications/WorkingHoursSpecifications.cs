using FurryFriends.Core.TimeslotAggregate;

namespace FurryFriends.Core.TimeslotAggregate.Specifications;

public sealed class WorkingHoursByPetWalkerAndDaySpec : Specification<WorkingHours>
{
    public WorkingHoursByPetWalkerAndDaySpec(Guid petWalkerId, DayOfWeek dayOfWeek)
    {
        Query
            .Where(w => w.PetWalkerId == petWalkerId)
            .Where(w => w.DayOfWeek == dayOfWeek);
    }
}

public sealed class WorkingHoursByPetWalkerSpec : Specification<WorkingHours>
{
    public WorkingHoursByPetWalkerSpec(Guid petWalkerId)
    {
        Query
            .Where(w => w.PetWalkerId == petWalkerId)
            .OrderBy(w => w.DayOfWeek)
            .ThenBy(w => w.StartTime);
    }
}

public sealed class WorkingHoursByIdSpec : Specification<WorkingHours>
{
    public WorkingHoursByIdSpec(Guid id)
    {
        Query
            .Where(w => w.Id == id);
    }
}

public sealed class WorkingHoursByIdWithPetWalkerSpec : Specification<WorkingHours>
{
    public WorkingHoursByIdWithPetWalkerSpec(Guid id)
    {
        Query
            .Where(w => w.Id == id);
    }
}