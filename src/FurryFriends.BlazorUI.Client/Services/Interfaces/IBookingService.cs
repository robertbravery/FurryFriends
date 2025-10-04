using FurryFriends.BlazorUI.Client.Models.Bookings;
using FurryFriends.BlazorUI.Client.Models.Common;

namespace FurryFriends.BlazorUI.Client.Services.Interfaces;

/// <summary>
/// Service interface for managing bookings
/// </summary>
public interface IBookingService
{
    /// <summary>
    /// Create a new booking
    /// </summary>
    /// <param name="bookingRequest">Booking request details</param>
    /// <returns>API response with booking creation result</returns>
    Task<ApiResponse<BookingResponseDto>> CreateBookingAsync(BookingRequestDto bookingRequest);

    /// <summary>
    /// Get available time slots for a PetWalker on a specific date
    /// </summary>
    /// <param name="petWalkerId">PetWalker ID</param>
    /// <param name="date">Date to check availability</param>
    /// <returns>API response with available slots</returns>
    Task<ApiResponse<AvailableSlotsResponseDto>> GetAvailableSlotsAsync(Guid petWalkerId, DateTime date);

    /// <summary>
    /// Check if a specific time slot can be booked
    /// </summary>
    /// <param name="petWalkerId">PetWalker ID</param>
    /// <param name="startTime">Start time of the slot</param>
    /// <param name="endTime">End time of the slot</param>
    /// <returns>API response indicating if the slot is available</returns>
    Task<ApiResponse<bool>> CanBookTimeSlotAsync(Guid petWalkerId, DateTime startTime, DateTime endTime);

    /// <summary>
    /// Get bookings for a specific client
    /// </summary>
    /// <param name="clientId">Client ID</param>
    /// <param name="startDate">Start date range</param>
    /// <param name="endDate">End date range</param>
    /// <returns>API response with client bookings</returns>
    Task<ApiResponse<List<BookingDto>>> GetClientBookingsAsync(Guid clientId, DateTime startDate, DateTime endDate);

    /// <summary>
    /// Get bookings for a specific PetWalker
    /// </summary>
    /// <param name="petWalkerId">PetWalker ID</param>
    /// <param name="startDate">Start date range</param>
    /// <param name="endDate">End date range</param>
    /// <returns>API response with PetWalker bookings</returns>
    Task<ApiResponse<List<BookingDto>>> GetPetWalkerBookingsAsync(Guid petWalkerId, DateTime startDate, DateTime endDate);

    /// <summary>
    /// Get available PetWalkers for booking selection (legacy method for backward compatibility)
    /// </summary>
    /// <param name="serviceArea">Optional service area filter</param>
    /// <returns>API response with available PetWalkers</returns>
    Task<ApiResponse<List<PetWalkerSummaryDto>>> GetAvailablePetWalkersAsync(string? serviceArea = null);

    /// <summary>
    /// Get available PetWalkers for booking selection with pagination, sorting, and filtering
    /// </summary>
    /// <param name="request">Request parameters including pagination, sorting, and filtering options</param>
    /// <returns>API response with paginated PetWalkers</returns>
    Task<ApiResponse<PaginatedPetWalkersResponse>> GetAvailablePetWalkersAsync(GetAvailablePetWalkersRequest request);
}
