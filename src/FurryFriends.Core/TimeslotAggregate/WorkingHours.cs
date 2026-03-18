using FurryFriends.Core.Common;

namespace FurryFriends.Core.TimeslotAggregate;

public class WorkingHours : AuditableEntity<Guid>
{
    public Guid PetWalkerId { get; private set; }
    public DayOfWeek DayOfWeek { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }
    public bool IsActive { get; private set; }

    internal WorkingHours() { } // Required by EF Core

    private WorkingHours(
        Guid petWalkerId,
        DayOfWeek dayOfWeek,
        TimeOnly startTime,
        TimeOnly endTime,
        bool isActive = true)
    {
        Id = Guid.NewGuid();
        PetWalkerId = petWalkerId;
        DayOfWeek = dayOfWeek;
        StartTime = startTime;
        EndTime = endTime;
        IsActive = isActive;
    }

    public static Result<WorkingHours> Create(
        Guid petWalkerId,
        DayOfWeek dayOfWeek,
        TimeOnly startTime,
        TimeOnly endTime,
        bool isActive = true)
    {
        Guard.Against.Default(petWalkerId, nameof(petWalkerId));
        Guard.Against.OutOfRange(endTime, nameof(endTime), startTime.AddMinutes(1), TimeOnly.MaxValue);

        return Result.Success(new WorkingHours(petWalkerId, dayOfWeek, startTime, endTime, isActive));
    }

    public void SetActive(bool isActive)
    {
        IsActive = isActive;
        UpdatedAt = DateTime.Now;
    }

    public void SetInactive()
    {
        IsActive = false;
        UpdatedAt = DateTime.Now;
    }
}