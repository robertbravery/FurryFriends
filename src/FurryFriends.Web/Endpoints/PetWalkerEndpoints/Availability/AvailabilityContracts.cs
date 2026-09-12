namespace FurryFriends.Web.Endpoints.PetWalkerEndpoints.Availability;

public static class AvailabilityRoutes
{
  public const string DateExceptions = "/petwalker/{PetWalkerId:guid}/date-exceptions";
  public const string DateExceptionByDate = "/petwalker/{PetWalkerId:guid}/date-exceptions/{Date}";
  public const string Settings = "/petwalker/{PetWalkerId:guid}/availability-settings";
}

public class GetDateExceptionsRequest
{
  public Guid PetWalkerId { get; set; }
}

public class SetDateExceptionRequest
{
  public Guid PetWalkerId { get; set; }

  /// <summary>The date being overridden.</summary>
  public DateTime Date { get; set; }

  /// <summary>True to block the whole day; false to replace the hours for that day.</summary>
  public bool IsUnavailable { get; set; }

  /// <summary>Replacement start time, required when <see cref="IsUnavailable"/> is false.</summary>
  public TimeOnly? StartTime { get; set; }

  /// <summary>Replacement end time, required when <see cref="IsUnavailable"/> is false.</summary>
  public TimeOnly? EndTime { get; set; }

  public string? Reason { get; set; }
}

public class DeleteDateExceptionRequest
{
  public Guid PetWalkerId { get; set; }
  public DateTime Date { get; set; }
}

public class UpdateAvailabilitySettingsRequest
{
  public Guid PetWalkerId { get; set; }
  public int SlotDurationMinutes { get; set; }
  public int InterBookingGapMinutes { get; set; }
  public int DailyPetWalkLimit { get; set; }
}

public class DateExceptionResponse
{
  public Guid Id { get; set; }
  public Guid PetWalkerId { get; set; }
  public DateOnly Date { get; set; }
  public bool IsUnavailable { get; set; }
  public TimeOnly? StartTime { get; set; }
  public TimeOnly? EndTime { get; set; }
  public string? Reason { get; set; }
}

public class AvailabilitySettingsResponse
{
  public Guid PetWalkerId { get; set; }
  public int SlotDurationMinutes { get; set; }
  public int InterBookingGapMinutes { get; set; }
  public int DailyPetWalkLimit { get; set; }
}
