namespace FurryFriends.Web.Endpoints.TimeslotEndpoints.WorkingHours;

public class UpdateWorkingHoursRequest
{
    public const string Route = "/working-hours/{id}";

    public Guid Id { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public bool IsActive { get; set; } = true;
}
