namespace FurryFriends.BlazorUI.Client.Models.Timeslots;

/// <summary>
/// Represents working hours for a petwalker on a specific day
/// </summary>
public class WorkingHoursDto
{
    public Guid WorkingHoursId { get; set; }
    public Guid PetWalkerId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public string DayName { get; set; } = string.Empty;
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// Request to create working hours
/// </summary>
public class CreateWorkingHoursRequest
{
    public Guid PetWalkerId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
}

/// <summary>
/// Request to update working hours
/// </summary>
public class UpdateWorkingHoursRequest
{
    public Guid WorkingHoursId { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// Response containing working hours list
/// </summary>
public class GetWorkingHoursResponse
{
    public Guid PetWalkerId { get; set; }
    public List<WorkingHoursDto> WorkingHours { get; set; } = new();
}
