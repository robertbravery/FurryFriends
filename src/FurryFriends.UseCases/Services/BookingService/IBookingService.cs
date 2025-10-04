using FurryFriends.Core.BookingAggregate;
using FurryFriends.Core.BookingAggregate.Enums;

namespace FurryFriends.UseCases.Services.BookingService;

public interface IBookingService
{
    Task<Booking> CreateBookingAsync(
        Guid petWalkerId,
        Guid clientId,
        DateTime startTime,
        DateTime endTime,
        decimal price);

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