using FurryFriends.UseCases.Domain.Bookings.Command;
using FurryFriends.Web.Endpoints.Base;

namespace FurryFriends.Web.Endpoints.BookingEndpoints.Cancel;

public class CancelBookingEndpoint : BaseEndpoint<CancelBookingRequest, CancelBookingResponse>
{
  public CancelBookingEndpoint(IMediator mediator, ILogger<CancelBookingEndpoint> logger)
      : base(mediator, logger) { }

  protected override string OperationName => "CancelBooking";

  public override void Configure()
  {
    Post(CancelBookingRequest.Route);
    AllowAnonymous();
    Options(o => o.WithName("CancelBooking"));

    Summary(s =>
    {
      s.Summary = "Cancel a Booking";
      s.Description = "Cancels an existing booking with a specified reason and cancellation details.";
    });
  }

  public override async Task HandleAsync(CancelBookingRequest request, CancellationToken cancellationToken)
  {
    _logger.LogInformation(
        "Cancelling booking {BookingId}, Reason: {Reason}",
        request.BookingId,
        request.Reason);

    var cancelCommand = new CancelBookingCommand(
        request.BookingId,
        request.Reason,
        request.CancelledBy,
        request.AdditionalNotes
    );

    var result = await _mediator.Send(cancelCommand, cancellationToken);

    if (result.IsSuccess)
    {
      Response = Result<CancelBookingResponse>.Success(
          new CancelBookingResponse(true, "Booking cancelled successfully", request.BookingId));
      return;
    }

    AddResultErrors(result);

    await SendErrorsAsync(StatusCodes.Status400BadRequest, cancellationToken);
  }
}
