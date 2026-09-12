using FurryFriends.Core.BookingAggregate;
using FurryFriends.Core.BookingAggregate.Enums;
using FurryFriends.UseCases.Domain.Bookings.Dto;
using Microsoft.Extensions.Logging;

namespace FurryFriends.UseCases.Domain.Bookings.Command;

public class ConfirmBookingHandler : ICommandHandler<ConfirmBookingCommand, Result<BookingStatusDto>>
{
  private readonly IRepository<Booking> _bookingRepo;
  private readonly ILogger<ConfirmBookingHandler> _logger;

  public ConfirmBookingHandler(IRepository<Booking> bookingRepo, ILogger<ConfirmBookingHandler> logger)
  {
    _bookingRepo = bookingRepo;
    _logger = logger;
  }

  public async Task<Result<BookingStatusDto>> Handle(ConfirmBookingCommand request, CancellationToken cancellationToken)
  {
    _logger.LogInformation("Attempting to confirm booking {BookingId}", request.BookingId);

    var booking = await _bookingRepo.GetByIdAsync(request.BookingId, cancellationToken);
    if (booking == null)
    {
      _logger.LogWarning("Booking {BookingId} not found", request.BookingId);
      return Result.NotFound("Booking not found");
    }

    if (booking.Status != BookingStatus.Pending)
    {
      _logger.LogWarning("Cannot confirm booking {BookingId} in status {Status}", request.BookingId, booking.Status);
      return Result.Error($"Cannot confirm booking in {booking.Status} status");
    }

    try
    {
      booking.Confirm();

      await _bookingRepo.UpdateAsync(booking, cancellationToken);

      _logger.LogInformation("Successfully confirmed booking {BookingId}", request.BookingId);
      return Result.Success(new BookingStatusDto(
          booking.Id,
          booking.Status.ToString(),
          "Booking confirmed successfully"));
    }
    catch (InvalidOperationException ex)
    {
      _logger.LogError(ex, "Failed to confirm booking {BookingId}", request.BookingId);
      return Result.Error(ex.Message);
    }
  }
}
