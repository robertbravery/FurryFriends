namespace FurryFriends.BlazorUI.Client.Models.PetWalkers;

/// <summary>
/// Helper class for working with schedules
/// </summary>
public static class ScheduleHelper
{
  /// <summary>
  /// Get all days of the week in order (Monday first)
  /// </summary>
  public static readonly DayOfWeek[] WeekDays =
  {
    DayOfWeek.Monday,
    DayOfWeek.Tuesday,
    DayOfWeek.Wednesday,
    DayOfWeek.Thursday,
    DayOfWeek.Friday,
    DayOfWeek.Saturday,
    DayOfWeek.Sunday
  };

  /// <summary>
  /// Get display name for day of week
  /// </summary>
  public static string GetDayDisplayName(DayOfWeek dayOfWeek)
  {
    return dayOfWeek switch
    {
      DayOfWeek.Monday => "Monday",
      DayOfWeek.Tuesday => "Tuesday",
      DayOfWeek.Wednesday => "Wednesday",
      DayOfWeek.Thursday => "Thursday",
      DayOfWeek.Friday => "Friday",
      DayOfWeek.Saturday => "Saturday",
      DayOfWeek.Sunday => "Sunday",
      _ => dayOfWeek.ToString()
    };
  }

  /// <summary>
  /// Get short display name for day of week
  /// </summary>
  public static string GetDayShortName(DayOfWeek dayOfWeek)
  {
    return dayOfWeek switch
    {
      DayOfWeek.Monday => "Mon",
      DayOfWeek.Tuesday => "Tue",
      DayOfWeek.Wednesday => "Wed",
      DayOfWeek.Thursday => "Thu",
      DayOfWeek.Friday => "Fri",
      DayOfWeek.Saturday => "Sat",
      DayOfWeek.Sunday => "Sun",
      _ => dayOfWeek.ToString().Substring(0, 3)
    };
  }

  /// <summary>
  /// Create a default empty schedule for all days of the week
  /// </summary>
  public static List<ScheduleItemDto> CreateEmptyWeeklySchedule()
  {
    return WeekDays.Select(day => new ScheduleItemDto
    {
      DayOfWeek = day,
      StartTime = TimeOnly.FromDateTime(DateTime.Parse("09:00")), // Default 9 AM
      EndTime = TimeOnly.FromDateTime(DateTime.Parse("17:00")),   // Default 5 PM
      IsActive = false // Not active by default
    }).ToList();
  }

  /// <summary>
  /// Format time span for display
  /// </summary>
  public static string FormatTime(TimeSpan time)
  {
    var hours = time.Hours;
    var minutes = time.Minutes;
    var amPm = hours < 12 ? "AM" : "PM";

    if (hours == 0)
      hours = 12;
    else if (hours > 12)
      hours -= 12;

    return $"{hours}:{minutes:D2} {amPm}";
  }

  /// <summary>
  /// Parse time string to TimeSpan
  /// </summary>
  public static TimeSpan ParseTime(string timeString)
  {
    if (TimeSpan.TryParse(timeString, out var result))
      return result;

    return TimeSpan.Zero;
  }

  /// <summary>
  /// Validate that a schedule item is valid
  /// </summary>
  public static bool IsValidScheduleItem(ScheduleItemDto item)
  {
    return item.StartTime < item.EndTime &&
           item.EndTime <= TimeOnly.FromDateTime(DateTime.Parse("17:00")) &&
           item.StartTime >= TimeOnly.FromDateTime(DateTime.Parse("09:00"));
  }

  /// <summary>
  /// Get time options for dropdowns (every 30 minutes)
  /// </summary>
  public static List<TimeOption> GetTimeOptions()
  {
    var options = new List<TimeOption>();

    for (int hour = 0; hour < 24; hour++)
    {
      for (int minute = 0; minute < 60; minute += 30)
      {
        var time = new TimeSpan(hour, minute, 0);
        options.Add(new TimeOption
        {
          Value = time,
          Display = FormatTime(time)
        });
      }
    }

    return options;
  }
}

/// <summary>
/// Time option for dropdowns
/// </summary>
public class TimeOption
{
  public TimeSpan Value { get; set; }
  public string Display { get; set; } = string.Empty;
}
