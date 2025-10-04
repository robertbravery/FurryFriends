using FurryFriends.BlazorUI.Client.Models.Common;
using FurryFriends.BlazorUI.Client.Models.PetWalkers;

namespace FurryFriends.BlazorUI.Client.Services.Interfaces;

/// <summary>
/// Service interface for managing PetWalker schedules
/// </summary>
public interface IScheduleService
{
  /// <summary>
  /// Get schedules for a specific PetWalker
  /// </summary>
  /// <param name="petWalkerId">PetWalker ID</param>
  /// <returns>API response containing schedule information</returns>
  Task<ApiResponse<GetScheduleResponseDto>> GetScheduleAsync(Guid petWalkerId);
  
  /// <summary>
  /// Set/update schedules for a specific PetWalker
  /// </summary>
  /// <param name="petWalkerId">PetWalker ID</param>
  /// <param name="schedules">List of schedule items to set</param>
  /// <returns>API response indicating success or failure</returns>
  Task<ApiResponse<bool>> SetScheduleAsync(Guid petWalkerId, List<ScheduleItemDto> schedules);
  
  /// <summary>
  /// Clear all schedules for a specific PetWalker
  /// </summary>
  /// <param name="petWalkerId">PetWalker ID</param>
  /// <returns>API response indicating success or failure</returns>
  Task<ApiResponse<bool>> ClearScheduleAsync(Guid petWalkerId);
}
