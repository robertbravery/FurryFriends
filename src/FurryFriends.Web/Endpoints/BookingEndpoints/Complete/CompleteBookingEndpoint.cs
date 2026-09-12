using FurryFriends.UseCases.Domain.Bookings.Command;
using FurryFriends.UseCases.Domain.Bookings.Dto;
using FurryFriends.Web.Endpoints.Base;
using FurryFriends.Web.Endpoints.BookingEndpoints.Records;

namespace FurryFriends.Web.Endpoints.BookingEndpoints.Complete;

public class CompleteBookingEndpoint : BaseEndpoint<CompleteBookingRequest, BookingStatusResponse>
{
  public CompleteBookingEndpoint(IMediator mediator, ILogger<CompleteBookingEndpoint> logger)
      : base(mediator, logger) { }

  protected override string OperationName => "CompleteBooking";

  public override void Configure()
  {
    Post(CompleteBookingRequest.Route);
    AllowAnonymous(); // Requires auth: PetWalker (or Client) for own booking
    Options(o => o.WithName("CompleteBooking"));

    Summary(s =>
    {
      s.Summary = "Complete a Booking";
      s.Description = "Completes an in-progress booking, moving it to the Completed status.";
    });
  }

  public override Task HandleAsync(CompleteBookingRequest request, CancellationToken cancellationToken)
  {
    return HandleResultAsync(
        ct => _mediator.Send(new CompleteBookingCommand(request.BookingId), ct),
        (BookingStatusDto result, CancellationToken _) =>
            Task.FromResult(new BookingStatusResponse(result.BookingId, result.Status, result.Message)),
        cancellationToken);
  }
}
