using FurryFriends.BlazorUI.Client.Models.Common;
using FurryFriends.BlazorUI.Client.Models.Timeslots;

namespace FurryFriends.BlazorUI.Client.Services.Interfaces;

/// <summary>
/// Service interface for managing timeslots
/// </summary>
public interface ITimeslotService
{
    #region Working Hours

    /// <summary>
    /// Create working hours for a petwalker
    /// </summary>
    Task<ApiResponse<WorkingHoursDto>> CreateWorkingHoursAsync(CreateWorkingHoursRequest request);

    /// <summary>
    /// Get working hours for a petwalker
    /// </summary>
    Task<ApiResponse<GetWorkingHoursResponse>> GetWorkingHoursAsync(Guid petWalkerId, DayOfWeek? dayOfWeek = null);

    /// <summary>
    /// Update working hours
    /// </summary>
    Task<ApiResponse<WorkingHoursDto>> UpdateWorkingHoursAsync(UpdateWorkingHoursRequest request);

    /// <summary>
    /// Delete working hours
    /// </summary>
    Task<ApiResponse<bool>> DeleteWorkingHoursAsync(Guid workingHoursId);

    #endregion

    #region Timeslots

    /// <summary>
    /// Create a new timeslot
    /// </summary>
    Task<ApiResponse<CreateTimeslotResponse>> CreateTimeslotAsync(CreateTimeslotRequest request);

    /// <summary>
    /// Get timeslots for a petwalker
    /// </summary>
    Task<ApiResponse<List<TimeslotDto>>> GetTimeslotsAsync(GetTimeslotsRequest request);

    /// <summary>
    /// Get available timeslots for a petwalker on a specific date
    /// </summary>
    Task<ApiResponse<GetAvailableTimeslotsResponse>> GetAvailableTimeslotsAsync(Guid petWalkerId, DateTime date);

    /// <summary>
    /// Update a timeslot
    /// </summary>
    Task<ApiResponse<TimeslotDto>> UpdateTimeslotAsync(UpdateTimeslotRequest request);

    /// <summary>
    /// Delete a timeslot
    /// </summary>
    Task<ApiResponse<bool>> DeleteTimeslotAsync(Guid timeslotId);

    /// <summary>
    /// Book a timeslot
    /// </summary>
    Task<ApiResponse<TimeslotBookingResponseDto>> BookTimeslotAsync(Guid timeslotId, Guid clientId, List<Guid> petIds);

    #endregion

    #region Custom Time Requests

    /// <summary>
    /// Request a custom time slot
    /// </summary>
    Task<ApiResponse<RequestCustomTimeResponse>> RequestCustomTimeAsync(RequestCustomTimeRequest request);

    /// <summary>
    /// Get custom time requests for a petwalker
    /// </summary>
    Task<ApiResponse<List<CustomTimeRequestDto>>> GetPetWalkerCustomTimeRequestsAsync(Guid petWalkerId, string? status = null);

    /// <summary>
    /// Get custom time requests for a client
    /// </summary>
    Task<ApiResponse<List<CustomTimeRequestDto>>> GetClientCustomTimeRequestsAsync(Guid clientId, string? status = null);

    /// <summary>
    /// Respond to a custom time request (accept/decline/counter-offer)
    /// </summary>
    Task<ApiResponse<RespondToCustomTimeRequestResponse>> RespondToCustomTimeRequestAsync(RespondToCustomTimeRequestRequest request);

    #endregion
}

/// <summary>
/// Booking response DTO for timeslot booking
/// </summary>
public class TimeslotBookingResponseDto
{
    public Guid BookingId { get; set; }
    public Guid TimeslotId { get; set; }
    public Guid ClientId { get; set; }
    public Guid PetWalkerId { get; set; }
    public DateTime BookingTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
}
