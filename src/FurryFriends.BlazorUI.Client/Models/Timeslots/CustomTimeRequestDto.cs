namespace FurryFriends.BlazorUI.Client.Models.Timeslots;

/// <summary>
/// Status of a custom time request
/// </summary>
public enum CustomTimeRequestStatus
{
    Pending,
    Accepted,
    Declined,
    CounterOffered,
    Expired
}

/// <summary>
/// Represents a custom time request from a client
/// </summary>
public class CustomTimeRequestDto
{
    public Guid RequestId { get; set; }
    public Guid PetWalkerId { get; set; }
    public string PetWalkerName { get; set; } = string.Empty;
    public Guid ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public DateOnly RequestedDate { get; set; }
    public TimeOnly PreferredStartTime { get; set; }
    public TimeOnly PreferredEndTime { get; set; }
    public int PreferredDurationMinutes { get; set; }
    public string ClientAddress { get; set; } = string.Empty;
    public List<Guid> PetIds { get; set; } = new();
    public List<string> PetNames { get; set; } = new();
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; }
    public DateTime? RespondedAt { get; set; }
    
    // Counter-offer fields
    public DateOnly? CounterOfferedDate { get; set; }
    public TimeOnly? CounterOfferedStartTime { get; set; }
    public int? CounterOfferedDurationMinutes { get; set; }
}

/// <summary>
/// Request to submit a custom time request
/// </summary>
public class RequestCustomTimeRequest
{
    public Guid PetWalkerId { get; set; }
    public Guid ClientId { get; set; }
    public DateOnly RequestedDate { get; set; }
    public TimeOnly PreferredStartTime { get; set; }
    public int PreferredDurationMinutes { get; set; } = 30;
    public string ClientAddress { get; set; } = string.Empty;
    public List<Guid> PetIds { get; set; } = new();
}

/// <summary>
/// Response for custom time request creation
/// </summary>
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
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Request to respond to a custom time request (accept/decline/counter-offer)
/// </summary>
public class RespondToCustomTimeRequestRequest
{
    public Guid RequestId { get; set; }
    public string Action { get; set; } = string.Empty; // "Accept", "Decline", "CounterOffer"
    public DateOnly? CounterOfferedDate { get; set; }
    public TimeOnly? CounterOfferedStartTime { get; set; }
    public int? CounterOfferedDurationMinutes { get; set; }
}

/// <summary>
/// Response for responding to custom time request
/// </summary>
public class RespondToCustomTimeRequestResponse
{
    public Guid RequestId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime RespondedAt { get; set; }
    public string? Message { get; set; }
}
