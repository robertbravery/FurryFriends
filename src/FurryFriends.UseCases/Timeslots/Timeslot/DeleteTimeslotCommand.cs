using Ardalis.Result;

namespace FurryFriends.UseCases.Timeslots.Timeslot;

public record DeleteTimeslotCommand(
    Guid TimeslotId
) : ICommand<Result<bool>>;
