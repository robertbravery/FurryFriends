using FurryFriends.Core.BookingAggregate;
using FurryFriends.Core.BookingAggregate.Enums;

namespace FurryFriends.UseCases.Services.BookingService;

public interface IBookingService
{
    /// <summary>
    /// Creates a booking. The price is derived server-side from the pet walker's hourly
    /// rate and the booked duration; it is deliberately not a parameter so that the price
    /// cannot be supplied (or zeroed) by the caller.
    /// </summary>
    Task<Booking> CreateBookingAsync(
        Guid petWalkerId,
        Guid clientId,
        DateTime startTime,
        DateTime endTime);

    Task<Booking> GetBookingAsync(Guid bookingId);
    
    Task<IEnumerable<Booking>> GetPetWalkerBookingsAsync(
        Guid petWalkerId, 
        DateTime startDate, 
        DateTime endDate);
    
    Task<IEnumerable<Booking>> GetClientBookingsAsync(
        Guid clientId, 
        DateTime startDate, 
        DateTime endDate);
    
    Task<Booking> UpdateBookingStatusAsync(
        Guid bookingId, 
        BookingStatus newStatus,
        string? notes = null);
    
    Task<bool> CanBookTimeSlotAsync(
        Guid petWalkerId,
        DateTime startTime,
        DateTime endTime);
    
    Task<IEnumerable<TimeSlot>> GetAvailableTimeSlotsAsync(
        Guid petWalkerId,
        DateTime date);
}

public record TimeSlot(DateTime StartTime, DateTime EndTime);