namespace FurryFriends.UseCases.Timeslots.Timeslot;

public record UpdateTimeslotCommand(
    Guid TimeslotId,
    TimeOnly StartTime,
    int DurationInMinutes
) : ICommand<Result<TimeslotDto>>;
