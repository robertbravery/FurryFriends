using FurryFriends.Core.Common;
using FurryFriends.Core.Enums;

namespace FurryFriends.Core.TimeslotAggregate;

public class Timeslot : AuditableEntity<Guid>
{
    public Guid PetWalkerId { get; private set; }
    public DateOnly Date { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }
    public int DurationInMinutes { get; private set; }
    public TimeslotStatus Status { get; private set; }

    internal Timeslot() { } // Required by EF Core

    private Timeslot(
        Guid petWalkerId,
        DateOnly date,
        TimeOnly startTime,
        int durationInMinutes,
        TimeslotStatus status)
    {
        Id = Guid.NewGuid();
        PetWalkerId = petWalkerId;
        Date = date;
        StartTime = startTime;
        DurationInMinutes = durationInMinutes;
        EndTime = startTime.AddMinutes(durationInMinutes);
        Status = status;
    }

    public static Result<Timeslot> Create(
        Guid petWalkerId,
        DateOnly date,
        TimeOnly startTime,
        int durationInMinutes,
        TimeslotStatus status)
    {
        Guard.Against.Default(petWalkerId, nameof(petWalkerId));
        Guard.Against.OutOfRange(durationInMinutes, nameof(durationInMinutes), 30, 45);
        Guard.Against.OutOfRange(date, nameof(date), DateOnly.FromDateTime(DateTime.Today), DateOnly.MaxValue);

        return Result.Success(new Timeslot(petWalkerId, date, startTime, durationInMinutes, status));
    }

    public void UpdateStatus(TimeslotStatus newStatus)
    {
        Status = newStatus;
        UpdatedAt = DateTime.Now;
    }

    public void Book()
    {
        Status = TimeslotStatus.Booked;
        UpdatedAt = DateTime.Now;
    }

    public void Cancel()
    {
        Status = TimeslotStatus.Cancelled;
        UpdatedAt = DateTime.Now;
    }

    public void MakeUnavailable()
    {
        Status = TimeslotStatus.Unavailable;
        UpdatedAt = DateTime.Now;
    }

    public void UpdateStartTimeAndDuration(TimeOnly newStartTime, int newDurationInMinutes)
    {
        StartTime = newStartTime;
        DurationInMinutes = newDurationInMinutes;
        EndTime = newStartTime.AddMinutes(newDurationInMinutes);
        UpdatedAt = DateTime.Now;
    }
}