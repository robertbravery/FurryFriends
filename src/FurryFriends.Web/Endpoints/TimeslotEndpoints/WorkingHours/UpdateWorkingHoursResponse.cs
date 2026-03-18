namespace FurryFriends.Web.Endpoints.TimeslotEndpoints.WorkingHours;

public record UpdateWorkingHoursResponse(
    Guid Id,
    Guid PetWalkerId,
    DayOfWeek DayOfWeek,
    TimeOnly StartTime,
    TimeOnly EndTime,
    bool IsActive);
