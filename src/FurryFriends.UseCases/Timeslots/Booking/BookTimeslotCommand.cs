namespace FurryFriends.UseCases.Timeslots.Booking;

/// <summary>
/// Command to book a timeslot
/// </summary>
public record BookTimeslotCommand(
    Guid TimeslotId,
    Guid ClientId,
    string ClientAddress,
    List<Guid> PetIds
) : ICommand<Result<BookTimeslotDto>>;
