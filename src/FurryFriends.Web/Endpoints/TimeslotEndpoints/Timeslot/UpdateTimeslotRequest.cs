namespace FurryFriends.Web.Endpoints.TimeslotEndpoints.Timeslot;

public class UpdateTimeslotRequest
{
    public const string Route = "/timeslots/{TimeslotId}";

    public Guid TimeslotId { get; set; }
    public TimeOnly StartTime { get; set; }
    public int DurationInMinutes { get; set; }
}
