namespace FurryFriends.Web.Endpoints.TimeslotEndpoints.CustomTimeRequest;

public class RequestCustomTimeRequest
{
    public const string Route = "/timeslots/request-custom";
    
    public Guid PetWalkerId { get; set; }
    public Guid ClientId { get; set; }
    public DateOnly RequestedDate { get; set; }
    public TimeOnly PreferredStartTime { get; set; }
    public int PreferredDurationMinutes { get; set; }
    public string ClientAddress { get; set; } = string.Empty;
    public List<Guid> PetIds { get; set; } = new();
}
