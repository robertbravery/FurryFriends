public class Schedule : ValueObject
{
  public DayOfWeek DayOfWeek { get; }
  public TimeOnly StartTime { get; }
  public TimeOnly EndTime { get; }

  private Schedule() { } // EF Core needs this

  public Schedule(DayOfWeek dayOfWeek, TimeOnly startTime, TimeOnly endTime)
  {
    Guard.Against.OutOfRange((int)dayOfWeek, nameof(dayOfWeek), 0, 6);
    Guard.Against.OutOfRange(startTime, nameof(startTime), TimeOnly.MinValue, TimeOnly.MaxValue);
    Guard.Against.OutOfRange(endTime, nameof(endTime), TimeOnly.MinValue, TimeOnly.MaxValue);
    Guard.Against.InvalidInput(endTime, nameof(endTime), t => t > startTime, "End time must be after start time");

    DayOfWeek = dayOfWeek;
    StartTime = startTime;
    EndTime = endTime;
  }

  protected override IEnumerable<object> GetEqualityComponents()
  {
    yield return DayOfWeek;
    yield return StartTime;
    yield return EndTime;
  }
}
