using FurryFriends.UseCases.Domain.Bookings.Dto;

namespace FurryFriends.UseCases.Domain.Bookings.Command;

public record StartBookingCommand(Guid BookingId) : ICommand<Result<BookingStatusDto>>;
