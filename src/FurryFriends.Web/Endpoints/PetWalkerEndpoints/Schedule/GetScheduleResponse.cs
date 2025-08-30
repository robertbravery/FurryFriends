namespace FurryFriends.Web.Endpoints.PetWalkerEndpoints.Schedule;

/// <summary>
/// Response for getting PetWalker schedules
/// </summary>
public class GetScheduleResponse
{
  /// <summary>
  /// PetWalker ID
  /// </summary>
  public Guid PetWalkerId { get; set; }

  /// <summary>
  /// List of schedule items
  /// </summary>
  public List<ScheduleItemResponse> Schedules { get; set; } = new List<ScheduleItemResponse>();
}

/// <summary>
/// Individual schedule item in the response
/// </summary>
public class ScheduleItemResponse
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
}
