namespace FurryFriends.BlazorUI.Client.Models.PetWalkers;

/// <summary>
/// Represents a single schedule item for a specific day of the week
/// </summary>
public class ScheduleItemDto
{
  /// <summary>
  /// Day of the week (0 = Sunday, 1 = Monday, etc.)
  /// </summary>
  public DayOfWeek DayOfWeek { get; set; }

  /// <summary>
  /// Start time for availability on this day
  /// </summary>
  public TimeOnly StartTime { get; set; }

  /// <summary>
  /// End time for availability on this day
  /// </summary>
  public TimeOnly EndTime { get; set; }

  /// <summary>
  /// Whether this schedule item is active/enabled
  /// </summary>
  public bool IsActive { get; set; } = true;

  /// <summary>
  /// Display name for the day of week
  /// </summary>
  public string DayName => DayOfWeek.ToString();

  /// <summary>
  /// Formatted start time for display (e.g., "9:00 AM")
  /// </summary>
  public string StartTimeFormatted => StartTime.ToString(@"h:mm") + (StartTime.Hour < 12 ? " AM" : " PM");

  /// <summary>
  /// Formatted end time for display (e.g., "5:00 PM")
  /// </summary>
  public string EndTimeFormatted => EndTime.ToString(@"h:mm") + (EndTime.Hour < 12 ? " AM" : " PM");

  /// <summary>
  /// Time range display (e.g., "9:00 AM - 5:00 PM")
  /// </summary>
  public string TimeRangeFormatted => $"{StartTimeFormatted} - {EndTimeFormatted}";

  /// <summary>
  /// Default constructor
  /// </summary>
  public ScheduleItemDto()
  {
  }

  /// <summary>
  /// Constructor with parameters
  /// </summary>
  public ScheduleItemDto(DayOfWeek dayOfWeek, TimeOnly startTime, TimeOnly endTime)
  {
    DayOfWeek = dayOfWeek;
    StartTime = startTime;
    EndTime = endTime;
  }

  /// <summary>
  /// Validates that the schedule item is valid
  /// </summary>
  public bool IsValid()
  {
    return StartTime < EndTime && EndTime <= TimeOnly.FromDateTime(DateTime.Parse("17:00")); ;
  }
}
