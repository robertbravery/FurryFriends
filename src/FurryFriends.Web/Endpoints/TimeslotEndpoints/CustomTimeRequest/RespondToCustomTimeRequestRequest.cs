namespace FurryFriends.Web.Endpoints.TimeslotEndpoints.CustomTimeRequest;

public class RespondToCustomTimeRequestRequest
{
    public const string Route = "/timeslots/respond-custom-time/{RequestId}";
    
    public Guid RequestId { get; set; }
    public string Response { get; set; } = string.Empty; // Accept, Decline, CounterOffer
    public DateOnly? CounterOfferedDate { get; set; }
    public TimeOnly? CounterOfferedTime { get; set; }
    public string? Reason { get; set; }
}
