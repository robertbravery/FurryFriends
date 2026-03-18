namespace FurryFriends.Web.Endpoints.TimeslotEndpoints.CustomTimeRequest;

public class RequestCustomTimeResponse
{
    public Guid RequestId { get; set; }
    public Guid PetWalkerId { get; set; }
    public Guid ClientId { get; set; }
    public DateOnly RequestedDate { get; set; }
    public TimeOnly PreferredStartTime { get; set; }
    public TimeOnly PreferredEndTime { get; set; }
    public int PreferredDurationMinutes { get; set; }
    public string ClientAddress { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
