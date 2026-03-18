namespace FurryFriends.UseCases.Timeslots.WorkingHours.Dto;

public record WorkingHoursDto(
    Guid Id,
    Guid PetWalkerId,
    DayOfWeek DayOfWeek,
    TimeOnly StartTime,
    TimeOnly EndTime,
    bool IsActive
);
