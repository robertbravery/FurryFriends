using FurryFriends.Core.BookingAggregate;
using FurryFriends.Core.BookingAggregate.Enums;
using FurryFriends.UseCases.Domain.Bookings.Dto;
using Microsoft.Extensions.Logging;

namespace FurryFriends.UseCases.Domain.Bookings.Command;

public class MarkBookingNoShowHandler : ICommandHandler<MarkBookingNoShowCommand, Result<BookingStatusDto>>
{
  private readonly IRepository<Booking> _bookingRepo;
  private readonly ILogger<MarkBookingNoShowHandler> _logger;

  public MarkBookingNoShowHandler(IRepository<Booking> bookingRepo, ILogger<MarkBookingNoShowHandler> logger)
  {
    _bookingRepo = bookingRepo;
    _logger = logger;
  }

  public async Task<Result<BookingStatusDto>> Handle(MarkBookingNoShowCommand request, CancellationToken cancellationToken)
  {
    _logger.LogInformation("Attempting to mark booking {BookingId} as no-show", request.BookingId);

    var booking = await _bookingRepo.GetByIdAsync(request.BookingId, cancellationToken);
    if (booking == null)
    {
      _logger.LogWarning("Booking {BookingId} not found", request.BookingId);
      return Result.NotFound("Booking not found");
    }

    if (booking.Status != BookingStatus.Confirmed)
    {
      _logger.LogWarning("Cannot mark booking {BookingId} as no-show in status {Status}", request.BookingId, booking.Status);
      return Result.Error($"Cannot mark booking as no-show in {booking.Status} status");
    }

    try
    {
      booking.MarkAsNoShow();

      await _bookingRepo.UpdateAsync(booking, cancellationToken);

      _logger.LogInformation("Successfully marked booking {BookingId} as no-show", request.BookingId);
      return Result.Success(new BookingStatusDto(
          booking.Id,
          booking.Status.ToString(),
          "Booking marked as no-show successfully"));
    }
    catch (InvalidOperationException ex)
    {
      _logger.LogError(ex, "Failed to mark booking {BookingId} as no-show", request.BookingId);
      return Result.Error(ex.Message);
    }
  }
}
