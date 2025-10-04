using Ardalis.GuardClauses;
using FurryFriends.Core.PetWalkerAggregate;

namespace FurryFriends.Core.BookingAggregate.Validation;

public static class BookingValidationRules
{
    public static void ValidateBookingSchedule(this IGuardClause guardClause, 
        DateTime startTime, 
        DateTime endTime, 
        PetWalker petWalker,
        string parameterName)
    {
        // Check if booking falls on a day the pet walker is available
        var dayOfWeek = startTime.DayOfWeek;
        var availableSchedules = petWalker.Schedules
            .Where(s => s.DayOfWeek == dayOfWeek)
            .ToList();
        
        if (!availableSchedules.Any())
        {
            throw new ArgumentException($"Pet walker is not available on {dayOfWeek}", parameterName);
        }

        var bookingStartTime = TimeOnly.FromDateTime(startTime);
        var bookingEndTime = TimeOnly.FromDateTime(endTime);

        // Check if booking time falls within any of pet walker's schedules for that day
        var isWithinSchedule = availableSchedules.Any(schedule =>
            bookingStartTime >= schedule.StartTime && 
            bookingEndTime <= schedule.EndTime);

        if (!isWithinSchedule)
        {
            var scheduleRanges = string.Join(", ", availableSchedules.Select(s => 
                $"{s.StartTime} - {s.EndTime}"));
            
            throw new ArgumentException(
                $"Booking time {bookingStartTime} - {bookingEndTime} is outside pet walker's available hours: {scheduleRanges}", 
                parameterName);
        }
    }

    public static void ValidateNoOverlappingBookings(this IGuardClause guardClause,
        DateTime startTime,
        DateTime endTime,
        IEnumerable<Booking> existingBookings,
        string parameterName)
    {
        var hasOverlap = existingBookings.Any(booking => 
            booking.StartTime < endTime && booking.EndTime > startTime);

        if (hasOverlap)
        {
            throw new ArgumentException("This time slot overlaps with an existing booking", parameterName);
        }
    }

    public static void ValidateDailyBookingLimit(this IGuardClause guardClause,
        DateTime bookingDate,
        PetWalker petWalker,
        IEnumerable<Booking> existingBookings,
        string parameterName)
    {
        var bookingsOnDate = existingBookings.Count(b => 
            b.StartTime.Date == bookingDate.Date);

        if (bookingsOnDate >= petWalker.DailyPetWalkLimit)
        {
            throw new ArgumentException(
                $"Pet walker has reached their daily booking limit of {petWalker.DailyPetWalkLimit} walks", 
                parameterName);
        }
    }

    public static void ValidateServiceArea(this IGuardClause guardClause,
        string bookingLocation,
        PetWalker petWalker,
        string parameterName)
    {
        // Add service area validation logic here once location handling is implemented
        throw new NotImplementedException("Service area validation not yet implemented");
    }
}