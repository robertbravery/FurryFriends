namespace FurryFriends.Web.Endpoints.TimeslotEndpoints.WorkingHours;

public class CreateWorkingHoursRequest
{
    public const string Route = "/working-hours";

    public Guid PetWalkerId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public bool IsActive { get; set; } = true;
}
