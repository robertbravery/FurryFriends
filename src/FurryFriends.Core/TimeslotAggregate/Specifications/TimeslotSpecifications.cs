using FurryFriends.Core.Enums;
using FurryFriends.Core.TimeslotAggregate;

namespace FurryFriends.Core.TimeslotAggregate.Specifications;

public sealed class AvailableTimeslotsByPetWalkerAndDateSpec : Specification<Timeslot>
{
    public AvailableTimeslotsByPetWalkerAndDateSpec(Guid petWalkerId, DateOnly date)
    {
        Query
            .Where(t => t.PetWalkerId == petWalkerId)
            .Where(t => t.Date == date)
            .Where(t => t.Status == TimeslotStatus.Available)
            .OrderBy(t => t.StartTime);
    }
}

public sealed class OverlappingTimeslotsSpec : Specification<Timeslot>
{
    public OverlappingTimeslotsSpec(Guid petWalkerId, DateOnly date, TimeOnly startTime, TimeOnly endTime)
    {
        Query
            .Where(t => t.PetWalkerId == petWalkerId)
            .Where(t => t.Date == date)
            .Where(t => t.Status != TimeslotStatus.Cancelled)
            .Where(t => t.StartTime < endTime && t.EndTime > startTime);
    }
}

public sealed class TimeslotsByPetWalkerAndDateSpec : Specification<Timeslot>
{
    public TimeslotsByPetWalkerAndDateSpec(Guid petWalkerId, DateOnly date)
    {
        Query
            .Where(t => t.PetWalkerId == petWalkerId)
            .Where(t => t.Date == date);
    }
}

public sealed class TimeslotByIdSpec : Specification<Timeslot>
{
    public TimeslotByIdSpec(Guid timeslotId)
    {
        Query
            .Where(t => t.Id == timeslotId);
    }
}

public sealed class TimeslotsByPetWalkerSpec : Specification<Timeslot>
{
    public TimeslotsByPetWalkerSpec(Guid petWalkerId)
    {
        Query
            .Where(t => t.PetWalkerId == petWalkerId)
            .OrderBy(t => t.Date)
            .ThenBy(t => t.StartTime);
    }
}

public sealed class TimeslotsDuringBufferSpec : Specification<Timeslot>
{
    public TimeslotsDuringBufferSpec(Guid petWalkerId, DateOnly date, TimeOnly startTime, int bufferMinutes)
    {
        var bufferEndTime = startTime.AddMinutes(bufferMinutes);
        Query
            .Where(t => t.PetWalkerId == petWalkerId)
            .Where(t => t.Date == date)
            .Where(t => t.Status == TimeslotStatus.Available)
            .Where(t => t.StartTime < bufferEndTime && t.EndTime > startTime);
    }
}