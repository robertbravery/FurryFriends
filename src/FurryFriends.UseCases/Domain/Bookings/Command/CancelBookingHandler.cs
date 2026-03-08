using FurryFriends.Core.BookingAggregate;
using FurryFriends.Core.BookingAggregate.Enums;
using Microsoft.Extensions.Logging;

namespace FurryFriends.UseCases.Domain.Bookings.Command;

public class CancelBookingHandler : ICommandHandler<CancelBookingCommand, Result>
{
  private readonly IRepository<Booking> _bookingRepo;
  private readonly ILogger<CancelBookingHandler> _logger;

  public CancelBookingHandler(IRepository<Booking> bookingRepo, ILogger<CancelBookingHandler> logger)
  {
    _bookingRepo = bookingRepo;
    _logger = logger;
  }

  public async Task<Result> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
  {
    _logger.LogInformation("Attempting to cancel booking {BookingId}", request.BookingId);

    var booking = await _bookingRepo.GetByIdAsync(request.BookingId, cancellationToken);
    if (booking == null)
    {
      _logger.LogWarning("Booking {BookingId} not found", request.BookingId);
      return Result.NotFound("Booking not found");
    }

    // Validate booking can be cancelled
    if (booking.Status == BookingStatus.Completed)
    {
      _logger.LogWarning("Cannot cancel completed booking {BookingId}", request.BookingId);
      return Result.Error("Cannot cancel a completed booking");
    }

    if (booking.Status == BookingStatus.Cancelled)
    {
      _logger.LogWarning("Booking {BookingId} is already cancelled", request.BookingId);
      return Result.Error("Booking is already cancelled");
    }

    if (booking.Status == BookingStatus.InProgress)
    {
      _logger.LogWarning("Cannot cancel booking {BookingId} in progress", request.BookingId);
      return Result.Error("Cannot cancel a booking that is in progress");
    }

    try
    {
      // Create cancellation record
      var cancellation = Cancellation.Create(
        booking.Id,
        request.Reason,
        request.CancelledBy,
        request.AdditionalNotes);

      // Cancel the booking with cancellation details
      booking.CancelWithCancellation(cancellation);

      await _bookingRepo.UpdateAsync(booking, cancellationToken);

      _logger.LogInformation("Successfully cancelled booking {BookingId}", request.BookingId);
      return Result.Success();
    }
    catch (InvalidOperationException ex)
    {
      _logger.LogError(ex, "Failed to cancel booking {BookingId}", request.BookingId);
      return Result.Error(ex.Message);
    }
  }
}