using FurryFriends.UseCases.Domain.Bookings.Command;
using FurryFriends.UseCases.Domain.Bookings.Dto;
using FurryFriends.Web.Endpoints.Base;
using FurryFriends.Web.Endpoints.BookingEndpoints.Records;

namespace FurryFriends.Web.Endpoints.BookingEndpoints.Start;

public class StartBookingEndpoint : BaseEndpoint<StartBookingRequest, BookingStatusResponse>
{
  public StartBookingEndpoint(IMediator mediator, ILogger<StartBookingEndpoint> logger)
      : base(mediator, logger) { }

  protected override string OperationName => "StartBooking";

  public override void Configure()
  {
    Post(StartBookingRequest.Route);
    AllowAnonymous(); // Requires auth: PetWalker (or Client) for own booking
    Options(o => o.WithName("StartBooking"));

    Summary(s =>
    {
      s.Summary = "Start a Booking";
      s.Description = "Starts a confirmed booking, moving it to the InProgress status.";
    });
  }

  public override Task HandleAsync(StartBookingRequest request, CancellationToken cancellationToken)
  {
    return HandleResultAsync(
        ct => _mediator.Send(new StartBookingCommand(request.BookingId), ct),
        (BookingStatusDto result, CancellationToken _) =>
            Task.FromResult(new BookingStatusResponse(result.BookingId, result.Status, result.Message)),
        cancellationToken);
  }
}
