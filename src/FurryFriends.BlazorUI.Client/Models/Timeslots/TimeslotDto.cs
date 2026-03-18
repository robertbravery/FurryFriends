namespace FurryFriends.BlazorUI.Client.Models.Timeslots;

/// <summary>
/// Represents a timeslot created by a petwalker
/// </summary>
public class TimeslotDto
{
    public Guid TimeslotId { get; set; }
    public Guid PetWalkerId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int DurationInMinutes { get; set; }
    public string Status { get; set; } = "Available";
}

/// <summary>
/// Request to create a new timeslot
/// </summary>
public class CreateTimeslotRequest
{
    public Guid PetWalkerId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public int DurationInMinutes { get; set; } = 30;
}

/// <summary>
/// Request to update a timeslot
/// </summary>
public class UpdateTimeslotRequest
{
    public Guid TimeslotId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public int DurationInMinutes { get; set; }
}

/// <summary>
/// Response for timeslot creation
/// </summary>
public class CreateTimeslotResponse
{
    public Guid TimeslotId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int DurationInMinutes { get; set; }
    public string Status { get; set; } = "Available";
}

/// <summary>
/// Available timeslot for clients to book
/// </summary>
public class AvailableTimeslotDto
{
    public Guid TimeslotId { get; set; }
    public Guid PetWalkerId { get; set; }
    public string PetWalkerName { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int DurationInMinutes { get; set; }
    public decimal Price { get; set; }
    public string Status { get; set; } = "Available";
}

/// <summary>
/// Response containing available timeslots
/// </summary>
public class GetAvailableTimeslotsResponse
{
    public Guid PetWalkerId { get; set; }
    public DateTime Date { get; set; }
    public List<AvailableTimeslotDto> AvailableTimeslots { get; set; } = new();
}

/// <summary>
/// Request to get timeslots
/// </summary>
public class GetTimeslotsRequest
{
    public Guid? PetWalkerId { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? Status { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
