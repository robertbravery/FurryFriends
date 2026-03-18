using Ardalis.Result;
using FurryFriends.UseCases.Timeslots.WorkingHours.Dto;

namespace FurryFriends.UseCases.Timeslots.WorkingHours;

public record UpdateWorkingHoursCommand(
    Guid Id,
    TimeOnly StartTime,
    TimeOnly EndTime,
    bool IsActive
) : ICommand<Result<WorkingHoursDto>>;
