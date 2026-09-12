using FurryFriends.Core.BookingAggregate;
using FurryFriends.Core.BookingAggregate.Enums;
using FurryFriends.UseCases.Domain.Bookings.Dto;
using Microsoft.Extensions.Logging;

namespace FurryFriends.UseCases.Domain.Bookings.Command;

public class StartBookingHandler : ICommandHandler<StartBookingCommand, Result<BookingStatusDto>>
{
  private readonly IRepository<Booking> _bookingRepo;
  private readonly ILogger<StartBookingHandler> _logger;

  public StartBookingHandler(IRepository<Booking> bookingRepo, ILogger<StartBookingHandler> logger)
  {
    _bookingRepo = bookingRepo;
    _logger = logger;
  }

  public async Task<Result<BookingStatusDto>> Handle(StartBookingCommand request, CancellationToken cancellationToken)
  {
    _logger.LogInformation("Attempting to start booking {BookingId}", request.BookingId);

    var booking = await _bookingRepo.GetByIdAsync(request.BookingId, cancellationToken);
    if (booking == null)
    {
      _logger.LogWarning("Booking {BookingId} not found", request.BookingId);
      return Result.NotFound("Booking not found");
    }

    if (booking.Status != BookingStatus.Confirmed)
    {
      _logger.LogWarning("Cannot start booking {BookingId} in status {Status}", request.BookingId, booking.Status);
      return Result.Error($"Cannot start booking in {booking.Status} status");
    }

    try
    {
      booking.BeginWalk();

      await _bookingRepo.UpdateAsync(booking, cancellationToken);

      _logger.LogInformation("Successfully started booking {BookingId}", request.BookingId);
      return Result.Success(new BookingStatusDto(
          booking.Id,
          booking.Status.ToString(),
          "Booking started successfully"));
    }
    catch (InvalidOperationException ex)
    {
      _logger.LogError(ex, "Failed to start booking {BookingId}", request.BookingId);
      return Result.Error(ex.Message);
    }
  }
}
