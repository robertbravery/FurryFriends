namespace FurryFriends.Web.Endpoints.TimeslotEndpoints.WorkingHours;

public class GetWorkingHoursRequest
{
    public const string Route = "/working-hours/{petWalkerId}";

    public Guid PetWalkerId { get; set; }
}
