using FurryFriends.BlazorUI.Client.Models.Common;

namespace FurryFriends.BlazorUI.Client.Models.PetWalkers;

/// <summary>
/// Response DTO for getting PetWalker schedules
/// </summary>
public class GetScheduleResponseDto
{
  /// <summary>
  /// PetWalker ID
  /// </summary>
  public Guid PetWalkerId { get; set; }
  
  /// <summary>
  /// List of schedule items
  /// </summary>
  public List<ScheduleItemDto> Schedules { get; set; } = new List<ScheduleItemDto>();
  
  /// <summary>
  /// Whether the PetWalker has any schedules set
  /// </summary>
  public bool HasSchedules => Schedules.Any();
  
  /// <summary>
  /// Get schedules grouped by day of week for easy display
  /// </summary>
  public Dictionary<DayOfWeek, List<ScheduleItemDto>> SchedulesByDay =>
    Schedules.GroupBy(s => s.DayOfWeek)
             .ToDictionary(g => g.Key, g => g.ToList());
  
  /// <summary>
  /// Get schedule for a specific day
  /// </summary>
  public List<ScheduleItemDto> GetSchedulesForDay(DayOfWeek dayOfWeek)
  {
    return Schedules.Where(s => s.DayOfWeek == dayOfWeek).ToList();
  }
  
  /// <summary>
  /// Check if PetWalker is available on a specific day
  /// </summary>
  public bool IsAvailableOnDay(DayOfWeek dayOfWeek)
  {
    return Schedules.Any(s => s.DayOfWeek == dayOfWeek && s.IsActive);
  }
}
