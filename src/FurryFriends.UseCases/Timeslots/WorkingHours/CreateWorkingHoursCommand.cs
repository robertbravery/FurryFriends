using Ardalis.Result;
using FurryFriends.UseCases.Timeslots.WorkingHours.Dto;

namespace FurryFriends.UseCases.Timeslots.WorkingHours;

public record CreateWorkingHoursCommand(
    Guid PetWalkerId,
    DayOfWeek DayOfWeek,
    TimeOnly StartTime,
    TimeOnly EndTime,
    bool IsActive = true
) : ICommand<Result<WorkingHoursDto>>;
