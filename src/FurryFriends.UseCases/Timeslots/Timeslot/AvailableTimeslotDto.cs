namespace FurryFriends.UseCases.Timeslots.Timeslot;

public record AvailableTimeslotDto(
    Guid TimeslotId,
    TimeOnly StartTime,
    TimeOnly EndTime,
    int DurationInMinutes);
