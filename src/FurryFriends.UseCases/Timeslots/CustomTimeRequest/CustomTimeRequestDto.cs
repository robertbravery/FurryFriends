namespace FurryFriends.UseCases.Timeslots.CustomTimeRequest;

public record CustomTimeRequestDto(
    Guid RequestId,
    Guid PetWalkerId,
    Guid ClientId,
    DateOnly RequestedDate,
    TimeOnly PreferredStartTime,
    TimeOnly PreferredEndTime,
    int PreferredDurationMinutes,
    string ClientAddress,
    string Status,
    DateTime CreatedAt);
