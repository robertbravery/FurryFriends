using FurryFriends.Core.Common;

namespace FurryFriends.Core.PetWalkerAggregate;

/// <summary>
/// A date-specific override of a PetWalker's weekly availability - either a full-day
/// block (holiday, sickness) or replacement hours for that single date.
/// The weekly schedule remains the standing pattern; this only describes exceptions.
/// </summary>
public class PetWalkerDateException : AuditableEntity<Guid>
{
  public Guid PetWalkerId { get; private set; }
  public DateOnly Date { get; private set; }

  /// <summary>When true the walker is unavailable for the whole day.</summary>
  public bool IsUnavailable { get; private set; }

  /// <summary>Replacement start time when <see cref="IsUnavailable"/> is false.</summary>
  public TimeOnly? StartTime { get; private set; }

  /// <summary>Replacement end time when <see cref="IsUnavailable"/> is false.</summary>
  public TimeOnly? EndTime { get; private set; }

  public string? Reason { get; private set; }

  internal PetWalkerDateException() { } // Required by EF Core

  private PetWalkerDateException(
      Guid petWalkerId,
      DateOnly date,
      bool isUnavailable,
      TimeOnly? startTime,
      TimeOnly? endTime,
      string? reason)
  {
    Id = Guid.NewGuid();
    PetWalkerId = petWalkerId;
    Date = date;
    IsUnavailable = isUnavailable;
    StartTime = startTime;
    EndTime = endTime;
    Reason = reason;
  }

  /// <summary>
  /// Blocks the walker for the whole of <paramref name="date"/>.
  /// </summary>
  public static Result<PetWalkerDateException> CreateUnavailable(
      Guid petWalkerId,
      DateOnly date,
      string? reason = null)
  {
    var validation = ValidateCommon(petWalkerId, date, reason);
    if (!validation.IsSuccess)
    {
      return validation;
    }

    return Result.Success(new PetWalkerDateException(petWalkerId, date, true, null, null, reason));
  }

  /// <summary>
  /// Replaces the walker's weekly hours for <paramref name="date"/> with a single window.
  /// </summary>
  public static Result<PetWalkerDateException> CreateCustomHours(
      Guid petWalkerId,
      DateOnly date,
      TimeOnly startTime,
      TimeOnly endTime,
      string? reason = null)
  {
    var validation = ValidateCommon(petWalkerId, date, reason);
    if (!validation.IsSuccess)
    {
      return validation;
    }

    if (endTime <= startTime)
    {
      return Result.Error("End time must be after start time");
    }

    return Result.Success(new PetWalkerDateException(petWalkerId, date, false, startTime, endTime, reason));
  }

  private static Result ValidateCommon(Guid petWalkerId, DateOnly date, string? reason)
  {
    if (petWalkerId == Guid.Empty)
    {
      return Result.Error("PetWalker ID is required");
    }

    if (date == default)
    {
      return Result.Error("Date is required");
    }

    if (reason is { Length: > 200 })
    {
      return Result.Error("Reason must not exceed 200 characters");
    }

    return Result.Success();
  }
}
