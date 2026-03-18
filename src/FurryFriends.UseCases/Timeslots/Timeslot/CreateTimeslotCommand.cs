using Ardalis.Result;
using FurryFriends.UseCases.Timeslots.Timeslot;

namespace FurryFriends.UseCases.Timeslots.Timeslot;

public record CreateTimeslotCommand(
    Guid PetWalkerId,
    DateOnly Date,
    TimeOnly StartTime,
    int DurationInMinutes
) : ICommand<Result<TimeslotDto>>;
