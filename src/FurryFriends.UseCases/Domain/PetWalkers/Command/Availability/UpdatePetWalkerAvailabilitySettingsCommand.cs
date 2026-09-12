using FurryFriends.UseCases.Domain.PetWalkers.Dto;

namespace FurryFriends.UseCases.Domain.PetWalkers.Command.Availability;

/// <summary>
/// Updates the walker's derived-availability settings: slot length, gap between
/// bookings, and the daily walk cap.
/// </summary>
public record UpdatePetWalkerAvailabilitySettingsCommand(
    Guid PetWalkerId,
    int SlotDurationMinutes,
    int InterBookingGapMinutes,
    int DailyPetWalkLimit) : ICommand<Result<PetWalkerAvailabilitySettingsDto>>;
