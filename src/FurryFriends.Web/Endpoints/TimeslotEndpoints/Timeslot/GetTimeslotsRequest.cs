namespace FurryFriends.Web.Endpoints.TimeslotEndpoints.Timeslot;

public class GetTimeslotsRequest
{
    public const string Route = "/timeslots";

    public Guid PetWalkerId { get; set; }
    public DateOnly? Date { get; set; }
}
