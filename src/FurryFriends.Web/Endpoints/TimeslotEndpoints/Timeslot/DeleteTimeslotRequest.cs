namespace FurryFriends.Web.Endpoints.TimeslotEndpoints.Timeslot;

public class DeleteTimeslotRequest
{
    public const string Route = "/timeslots/{TimeslotId}";

    public Guid TimeslotId { get; set; }
}
