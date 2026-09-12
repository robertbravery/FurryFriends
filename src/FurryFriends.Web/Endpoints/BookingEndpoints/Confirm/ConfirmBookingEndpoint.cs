using FurryFriends.UseCases.Domain.Bookings.Command;
using FurryFriends.UseCases.Domain.Bookings.Dto;
using FurryFriends.Web.Endpoints.Base;
using FurryFriends.Web.Endpoints.BookingEndpoints.Records;

namespace FurryFriends.Web.Endpoints.BookingEndpoints.Confirm;

public class ConfirmBookingEndpoint : BaseEndpoint<ConfirmBookingRequest, BookingStatusResponse>
{
  public ConfirmBookingEndpoint(IMediator mediator, ILogger<ConfirmBookingEndpoint> logger)
      : base(mediator, logger) { }

  protected override string OperationName => "ConfirmBooking";

  public override void Configure()
  {
    Post(ConfirmBookingRequest.Route);
    AllowAnonymous(); // Requires auth: PetWalker (or Client) for own booking
    Options(o => o.WithName("ConfirmBooking"));

    Summary(s =>
    {
      s.Summary = "Confirm a Booking";
      s.Description = "Confirms a pending booking, moving it to the Confirmed status.";
    });
  }

  public override Task HandleAsync(ConfirmBookingRequest request, CancellationToken cancellationToken)
  {
    return HandleResultAsync(
        ct => _mediator.Send(new ConfirmBookingCommand(request.BookingId), ct),
        (BookingStatusDto result, CancellationToken _) =>
            Task.FromResult(new BookingStatusResponse(result.BookingId, result.Status, result.Message)),
        cancellationToken);
  }
}
