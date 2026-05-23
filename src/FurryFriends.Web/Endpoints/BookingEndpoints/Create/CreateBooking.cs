using FurryFriends.UseCases.Domain.Bookings.Dto;
using FurryFriends.Web.Endpoints.Base;

namespace FurryFriends.Web.Endpoints.BookingEndpoints.Create;

public class CreateBookingEndpoint : BaseEndpoint<CreateBookingRequest, CreateBookingResponse>
{
  public CreateBookingEndpoint(IMediator mediator, ILogger<CreateBookingEndpoint> logger)
      : base(mediator, logger) { }

  protected override string OperationName => "CreateBooking";

  public override void Configure()
  {
    Post(CreateBookingRequest.Route);
    AllowAnonymous();
    Options(o => o.WithName("CreateBooking"));

    Summary(s =>
    {
      s.Summary = "Create a new Booking";
      s.Description = "Creates a new Booking for Petwalker and a pet owner with the selected Pet.";
    });
  }

  public override async Task HandleAsync(CreateBookingRequest request, CancellationToken cancellationToken)
  {
    _logger.LogInformation(
        "Creating booking for PetWalker: {PetWalkerId}, PetOwner: {PetOwnerId}",
        request.PetWalkerId,
        request.PetOwnerId);

    var bookingCommand = new CreateBookingCommand(
        request.PetWalkerId,
        request.PetOwnerId,
        request.StartDate,
        request.EndDate
    );

    await HandleResultAsync(
        ct => _mediator.Send(bookingCommand, ct),
        (BookingDto result, CancellationToken ct) =>
            Task.FromResult(new CreateBookingResponse(result.Id, result.Start, result.Start)),
        cancellationToken);
  }
}
