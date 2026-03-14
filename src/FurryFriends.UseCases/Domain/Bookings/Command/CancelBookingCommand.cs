using FurryFriends.Core.BookingAggregate.Enums;

namespace FurryFriends.UseCases.Domain.Bookings.Command;

public record CancelBookingCommand(
    Guid BookingId,
    CancellationReason Reason,
    CancelledBy CancelledBy,
    string? AdditionalNotes = null
) : ICommand<Result>;