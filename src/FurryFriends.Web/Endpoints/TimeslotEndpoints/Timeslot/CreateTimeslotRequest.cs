namespace FurryFriends.Web.Endpoints.TimeslotEndpoints.Timeslot;

public class CreateTimeslotRequest
{
    public const string Route = "/timeslots";

    public Guid PetWalkerId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public int DurationInMinutes { get; set; }
}
