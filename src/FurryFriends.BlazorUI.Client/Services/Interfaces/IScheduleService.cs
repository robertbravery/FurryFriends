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

  /// <summary>
  /// Get the date-specific availability overrides for a PetWalker
  /// </summary>
  Task<ApiResponse<List<PetWalkerDateExceptionDto>>> GetDateExceptionsAsync(Guid petWalkerId);

  /// <summary>
  /// Block a whole day, or replace the hours for that day, without touching the weekly pattern.
  /// Upserts: at most one exception per date.
  /// </summary>
  Task<ApiResponse<PetWalkerDateExceptionDto>> SetDateExceptionAsync(
      Guid petWalkerId,
      DateTime date,
      bool isUnavailable,
      TimeOnly? startTime,
      TimeOnly? endTime,
      string? reason);

  /// <summary>
  /// Remove the override for a date, reverting to the weekly pattern.
  /// </summary>
  Task<ApiResponse<bool>> DeleteDateExceptionAsync(Guid petWalkerId, DateTime date);

  /// <summary>
  /// Get the settings used to derive bookable slots.
  /// </summary>
  Task<ApiResponse<AvailabilitySettingsDto>> GetAvailabilitySettingsAsync(Guid petWalkerId);

  /// <summary>
  /// Update slot length, inter-booking gap, and the daily walk cap.
  /// </summary>
  Task<ApiResponse<AvailabilitySettingsDto>> UpdateAvailabilitySettingsAsync(
      Guid petWalkerId,
      int slotDurationMinutes,
      int interBookingGapMinutes,
      int dailyPetWalkLimit);
}
