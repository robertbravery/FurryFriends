using FurryFriends.UseCases.Domain.Bookings.Dto;

namespace FurryFriends.UseCases.Domain.Bookings.Command;

public record CompleteBookingCommand(Guid BookingId) : ICommand<Result<BookingStatusDto>>;
