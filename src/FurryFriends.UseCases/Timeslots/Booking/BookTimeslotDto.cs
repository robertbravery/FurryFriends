namespace FurryFriends.UseCases.Timeslots.Booking;

/// <summary>
/// DTO for booking a timeslot response
/// </summary>
public record BookTimeslotDto(
    Guid BookingId,
    Guid TimeslotId,
    Guid PetWalkerId,
    Guid ClientId,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime,
    string ClientAddress,
    string Status,
    bool HasTravelBuffer,
    int? TravelBufferMinutes
);
