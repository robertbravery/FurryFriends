// Application/Scheduling/Commands/CreateBookingCommand.cs
using FurryFriends.UseCases.Domain.Bookings.Dto;
using Mediator;

public record CreateBookingCommand(
    Guid PetWalkerId,
    Guid PetOwnerId,
    DateTime Start,
    DateTime End
) : ICommand<Result<BookingDto>>;
