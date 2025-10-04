namespace FurryFriends.BlazorUI.Client.Models.PetWalkers;

/// <summary>
/// Request DTO for setting PetWalker schedules
/// Matches the API contract for the SetSchedule endpoint
/// </summary>
public class SetScheduleRequestDto
{
  /// <summary>
  /// PetWalker ID
  /// </summary>
  public Guid PetWalkerId { get; set; }

  /// <summary>
  /// List of schedule items to set
  /// </summary>
  public List<ScheduleCommandBodyDto> Schedules { get; set; } = new List<ScheduleCommandBodyDto>();
}

/// <summary>
/// Individual schedule command body that matches the API SetScheduleCommandBody
/// </summary>
public class ScheduleCommandBodyDto
{
  /// <summary>
  /// Day of the week
  /// </summary>
  public DayOfWeek DayOfWeek { get; set; }

  /// <summary>
  /// Start time
  /// </summary>
  public TimeOnly StartTime { get; set; }

  /// <summary>
  /// End time
  /// </summary>
  public TimeOnly EndTime { get; set; }

  /// <summary>
  /// Default constructor
  /// </summary>
  public ScheduleCommandBodyDto()
  {
  }

  /// <summary>
  /// Constructor with parameters
  /// </summary>
  public ScheduleCommandBodyDto(DayOfWeek dayOfWeek, TimeOnly startTime, TimeOnly endTime)
  {
    DayOfWeek = dayOfWeek;
    StartTime = startTime;
    EndTime = endTime;
  }

  /// <summary>
  /// Create from ScheduleItemDto
  /// </summary>
  public static ScheduleCommandBodyDto FromScheduleItem(ScheduleItemDto scheduleItem)
  {
    return new ScheduleCommandBodyDto(scheduleItem.DayOfWeek, scheduleItem.StartTime, scheduleItem.EndTime);
  }
}
