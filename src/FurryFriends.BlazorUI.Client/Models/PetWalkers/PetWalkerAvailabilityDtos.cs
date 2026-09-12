namespace FurryFriends.BlazorUI.Client.Models.PetWalkers;

/// <summary>
/// A date-specific override of a walker's weekly availability.
/// </summary>
public class PetWalkerDateExceptionDto
{
  public Guid Id { get; set; }
  public Guid PetWalkerId { get; set; }
  public DateOnly Date { get; set; }

  /// <summary>True when the walker is unavailable for the whole day.</summary>
  public bool IsUnavailable { get; set; }

  public TimeOnly? StartTime { get; set; }
  public TimeOnly? EndTime { get; set; }
  public string? Reason { get; set; }

  /// <summary>Display helper, e.g. "Unavailable all day" or "10:00 - 14:00".</summary>
  public string HoursDisplay => IsUnavailable
      ? "Unavailable all day"
      : $"{StartTime?.ToString("HH:mm")} - {EndTime?.ToString("HH:mm")}";
}

/// <summary>
/// Settings that drive how bookable slots are derived from the weekly pattern.
/// </summary>
public class AvailabilitySettingsDto
{
  public Guid PetWalkerId { get; set; }
  public int SlotDurationMinutes { get; set; } = 60;
  public int InterBookingGapMinutes { get; set; }
  public int DailyPetWalkLimit { get; set; } = 8;
}
