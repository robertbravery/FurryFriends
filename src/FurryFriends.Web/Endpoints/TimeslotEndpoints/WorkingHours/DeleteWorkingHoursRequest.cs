namespace FurryFriends.Web.Endpoints.TimeslotEndpoints.WorkingHours;

public class DeleteWorkingHoursRequest
{
    public const string Route = "/working-hours/{id}";

    public Guid Id { get; set; }
}
