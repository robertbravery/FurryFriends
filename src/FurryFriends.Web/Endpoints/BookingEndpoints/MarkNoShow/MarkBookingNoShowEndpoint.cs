using FurryFriends.UseCases.Domain.Bookings.Command;
using FurryFriends.UseCases.Domain.Bookings.Dto;
using FurryFriends.Web.Endpoints.Base;
using FurryFriends.Web.Endpoints.BookingEndpoints.Records;

namespace FurryFriends.Web.Endpoints.BookingEndpoints.MarkNoShow;

public class MarkBookingNoShowEndpoint : BaseEndpoint<MarkBookingNoShowRequest, BookingStatusResponse>
{
  public MarkBookingNoShowEndpoint(IMediator mediator, ILogger<MarkBookingNoShowEndpoint> logger)
      : base(mediator, logger) { }

  protected override string OperationName => "MarkBookingNoShow";

  public override void Configure()
  {
    Post(MarkBookingNoShowRequest.Route);
    AllowAnonymous(); // Requires auth: PetWalker (or Client) for own booking
    Options(o => o.WithName("MarkBookingNoShow"));

    Summary(s =>
    {
      s.Summary = "Mark a Booking as No-Show";
      s.Description = "Marks a confirmed booking as a no-show, moving it to the NoShow status.";
    });
  }

  public override Task HandleAsync(MarkBookingNoShowRequest request, CancellationToken cancellationToken)
  {
    return HandleResultAsync(
        ct => _mediator.Send(new MarkBookingNoShowCommand(request.BookingId), ct),
        (BookingStatusDto result, CancellationToken _) =>
            Task.FromResult(new BookingStatusResponse(result.BookingId, result.Status, result.Message)),
        cancellationToken);
  }
}
