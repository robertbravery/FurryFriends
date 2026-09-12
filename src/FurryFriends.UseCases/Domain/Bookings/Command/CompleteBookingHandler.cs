using FurryFriends.Core.BookingAggregate;
using FurryFriends.Core.BookingAggregate.Enums;
using FurryFriends.UseCases.Domain.Bookings.Dto;
using Microsoft.Extensions.Logging;

namespace FurryFriends.UseCases.Domain.Bookings.Command;

public class CompleteBookingHandler : ICommandHandler<CompleteBookingCommand, Result<BookingStatusDto>>
{
  private readonly IRepository<Booking> _bookingRepo;
  private readonly ILogger<CompleteBookingHandler> _logger;

  public CompleteBookingHandler(IRepository<Booking> bookingRepo, ILogger<CompleteBookingHandler> logger)
  {
    _bookingRepo = bookingRepo;
    _logger = logger;
  }

  public async Task<Result<BookingStatusDto>> Handle(CompleteBookingCommand request, CancellationToken cancellationToken)
  {
    _logger.LogInformation("Attempting to complete booking {BookingId}", request.BookingId);

    var booking = await _bookingRepo.GetByIdAsync(request.BookingId, cancellationToken);
    if (booking == null)
    {
      _logger.LogWarning("Booking {BookingId} not found", request.BookingId);
      return Result.NotFound("Booking not found");
    }

    if (booking.Status != BookingStatus.InProgress)
    {
      _logger.LogWarning("Cannot complete booking {BookingId} in status {Status}", request.BookingId, booking.Status);
      return Result.Error($"Cannot complete booking in {booking.Status} status");
    }

    try
    {
      booking.Complete();

      await _bookingRepo.UpdateAsync(booking, cancellationToken);

      _logger.LogInformation("Successfully completed booking {BookingId}", request.BookingId);
      return Result.Success(new BookingStatusDto(
          booking.Id,
          booking.Status.ToString(),
          "Booking completed successfully"));
    }
    catch (InvalidOperationException ex)
    {
      _logger.LogError(ex, "Failed to complete booking {BookingId}", request.BookingId);
      return Result.Error(ex.Message);
    }
  }
}
