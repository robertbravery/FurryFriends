using FurryFriends.Core.PetWalkerAggregate;

namespace FurryFriends.UseCases.Domain.PetWalkers.Dto;

/// <summary>
/// A date-specific override of a walker's weekly availability.
/// </summary>
public record PetWalkerDateExceptionDto(
    Guid Id,
    Guid PetWalkerId,
    DateOnly Date,
    bool IsUnavailable,
    TimeOnly? StartTime,
    TimeOnly? EndTime,
    string? Reason);

/// <summary>
/// The walker's derived-availability settings.
/// </summary>
public record PetWalkerAvailabilitySettingsDto(
    Guid PetWalkerId,
    int SlotDurationMinutes,
    int InterBookingGapMinutes,
    int DailyPetWalkLimit);

public static class PetWalkerDateExceptionMapping
{
  public static PetWalkerDateExceptionDto ToDto(this PetWalkerDateException exception) =>
      new(
          exception.Id,
          exception.PetWalkerId,
          exception.Date,
          exception.IsUnavailable,
          exception.StartTime,
          exception.EndTime,
          exception.Reason);
}
