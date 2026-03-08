using FurryFriends.UseCases.Domain.Bookings.Command;

namespace FurryFriends.Web.Endpoints.BookingEndpoints.Cancel;

public class CancelBooking(IMediator Mediator)
  : Endpoint<CancelBookingRequest, Result<CancelBookingResponse>>
{
  public override void Configure()
  {
    Post(CancelBookingRequest.Route);
    AllowAnonymous();
    Options(o => o.WithName("CancelBooking_" + Guid.NewGuid().ToString())); // Ensure unique name

    Summary(s =>
    {
      s.Summary = "Cancel a Booking";
      s.Description = "Cancels an existing booking with a specified reason and cancellation details.";
    });
  }

  public override async Task HandleAsync(CancelBookingRequest request, CancellationToken cancellationToken)
  {
    var cancelCommand = new CancelBookingCommand(
      request.BookingId,
      request.Reason,
      request.CancelledBy,
      request.AdditionalNotes
    );

    var result = await Mediator.Send(cancelCommand, cancellationToken);

    if (result.IsSuccess)
    {
      Response = Result<CancelBookingResponse>.Success(
        new CancelBookingResponse(true, "Booking cancelled successfully", request.BookingId));
      return;
    }

    // Handle errors
    await HandleResultErrorsAsync(result, cancellationToken);
  }

  private async Task HandleResultErrorsAsync(Result result, CancellationToken cancellationToken)
  {
    if (result.ValidationErrors?.Any() == true)
    {
      foreach (var error in result.ValidationErrors)
      {
        AddError(error.ErrorMessage);
      }
    }

    if (result.Errors?.Any() == true)
    {
      foreach (var error in result.Errors)
      {
        AddError(error);
      }
    }

    await SendErrorsAsync(result.IsSuccess ? StatusCodes.Status500InternalServerError : StatusCodes.Status400BadRequest, cancellationToken);
  }
}