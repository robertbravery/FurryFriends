using FurryFriends.Core.Enums;

namespace FurryFriends.Web.Endpoints.TimeslotEndpoints.Timeslot;

public class GetTimeslotsResponse
{
    public Guid PetWalkerId { get; set; }
    public DateOnly? Date { get; set; }
    public List<TimeslotResponse> Timeslots { get; set; } = new();
}

public class TimeslotResponse
{
    public Guid Id { get; set; }
    public Guid PetWalkerId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int DurationInMinutes { get; set; }
    public TimeslotStatus Status { get; set; }
}
