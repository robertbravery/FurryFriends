using Ardalis.GuardClauses;
using FurryFriends.Core.PetWalkerAggregate;

namespace FurryFriends.Core.BookingAggregate.Validation;

public static class GuardClauseExtensions
{
    public static void BookingSchedule(this IGuardClause guard, 
        DateTime startTime, 
        DateTime endTime, 
        PetWalker petWalker,
        string parameterName)
    {
        BookingValidationRules.ValidateBookingSchedule(guard, startTime, endTime, petWalker, parameterName);
    }

    public static void NoOverlappingBookings(this IGuardClause guard,
        DateTime startTime,
        DateTime endTime,
        IEnumerable<Booking> existingBookings,
        string parameterName)
    {
        BookingValidationRules.ValidateNoOverlappingBookings(guard, startTime, endTime, existingBookings, parameterName);
    }

    public static void DailyBookingLimit(this IGuardClause guard,
        DateTime startTime,
        PetWalker petWalker,
        IEnumerable<Booking> existingBookings,
        string parameterName)
    {
        BookingValidationRules.ValidateDailyBookingLimit(guard, startTime, petWalker, existingBookings, parameterName);
    }
}