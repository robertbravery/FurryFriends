using FurryFriends.Core.Enums;

namespace FurryFriends.Web.Endpoints.TimeslotEndpoints.Timeslot;

public class UpdateTimeslotResponse
{
    public Guid Id { get; set; }
    public Guid PetWalkerId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int DurationInMinutes { get; set; }
    public TimeslotStatus Status { get; set; }
}
