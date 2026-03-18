namespace FurryFriends.Web.Endpoints.TimeslotEndpoints.WorkingHours;

public record CreateWorkingHoursResponse(
    Guid Id,
    Guid PetWalkerId,
    DayOfWeek DayOfWeek,
    TimeOnly StartTime,
    TimeOnly EndTime,
    bool IsActive);
