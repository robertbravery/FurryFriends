namespace FurryFriends.Web.Endpoints.TimeslotEndpoints.Timeslot;

public class GetAvailableTimeslotsResponse
{
    public Guid PetWalkerId { get; set; }
    public DateOnly Date { get; set; }
    public List<AvailableTimeslotResponse> Timeslots { get; set; } = new();
    public bool HasTravelBufferWarning { get; set; }
    public string? TravelBufferMessage { get; set; }
}

public class AvailableTimeslotResponse
{
    public Guid TimeslotId { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int DurationInMinutes { get; set; }
}
