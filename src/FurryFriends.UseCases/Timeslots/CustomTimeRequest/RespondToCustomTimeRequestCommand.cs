using Ardalis.Result;

namespace FurryFriends.UseCases.Timeslots.CustomTimeRequest;

public enum CustomTimeRequestResponse
{
    Accept,
    Decline,
    CounterOffer
}

/// <summary>
/// Command to respond to a custom time request (accept/decline/counter-offer)
/// </summary>
public record RespondToCustomTimeRequestCommand(
    Guid RequestId,
    CustomTimeRequestResponse Response,
    DateOnly? CounterOfferedDate,
    TimeOnly? CounterOfferedTime,
    string? Reason
) : ICommand<Result<CustomTimeRequestDto>>;
