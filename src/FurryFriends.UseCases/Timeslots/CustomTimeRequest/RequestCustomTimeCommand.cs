using Ardalis.Result;

namespace FurryFriends.UseCases.Timeslots.CustomTimeRequest;

/// <summary>
/// Command to request a custom time when preset slots are unavailable
/// </summary>
public record RequestCustomTimeCommand(
    Guid PetWalkerId,
    Guid ClientId,
    DateOnly RequestedDate,
    TimeOnly PreferredStartTime,
    int PreferredDurationMinutes,
    string ClientAddress,
    List<Guid> PetIds
) : ICommand<Result<CustomTimeRequestDto>>;
