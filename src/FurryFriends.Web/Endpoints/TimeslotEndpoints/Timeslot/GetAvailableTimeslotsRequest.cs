namespace FurryFriends.Web.Endpoints.TimeslotEndpoints.Timeslot;

public class GetAvailableTimeslotsRequest
{
    public const string Route = "/timeslots/available";

    public Guid PetWalkerId { get; set; }
    public DateOnly Date { get; set; }
}
