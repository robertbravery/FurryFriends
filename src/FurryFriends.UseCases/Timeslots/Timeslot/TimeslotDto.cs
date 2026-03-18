namespace FurryFriends.UseCases.Timeslots.Timeslot;

public record TimeslotDto(
    Guid Id,
    Guid PetWalkerId,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime,
    int DurationInMinutes,
    Core.Enums.TimeslotStatus Status);
