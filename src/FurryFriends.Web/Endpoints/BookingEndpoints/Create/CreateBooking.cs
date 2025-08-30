using FurryFriends.UseCases.Domain.Bookings.Dto;

namespace FurryFriends.Web.Endpoints.BookingEndpoints.Create;

public class CreateBooking(IMediator Mediator)
  : Endpoint<CreateBookingRequest, Result<CreateBookingResponse>>
{
  public override void Configure()
  {
    Post(CreateBookingRequest.Route);
    AllowAnonymous();
    Options(o => o.WithName("CreateBooking_" + Guid.NewGuid().ToString())); // Ensure unique name

    Summary(s =>
    {
      s.Summary = "Create a new Booking";
      s.Description = "Creates a new Booking for Petwalker and a pet owner with the selected Pet.";

    });
  }

  public override async Task HandleAsync(CreateBookingRequest request, CancellationToken cancellationToken)
  {
    var bookingCommand = new CreateBookingCommand(
      request.PetWalkerId,
      request.PetOwnerId,
      request.StartDate,
      request.EndDate
    );
    var result = await Mediator.Send(bookingCommand, cancellationToken);
    if (result == null)
    {
      await HandleResultErrorsAsync(result, cancellationToken);
      return;
    }
    Response = new CreateBookingResponse(result.Value.Id, result.Value.Start, result.Value.Start);
  }

  private async Task HandleResultErrorsAsync(Result<BookingDto>? result, CancellationToken cancellationToken)
  {
    if (result?.ValidationErrors?.Any() == true)
    {
      foreach (var error in result.ValidationErrors)
      {
        AddError(error.ErrorMessage);
      }
    }

    if (result?.Errors?.Any() == true)
    {
      foreach (var error in result.Errors)
      {
        AddError(error);
      }
    }

    await SendErrorsAsync(result!.IsSuccess ? StatusCodes.Status500InternalServerError : StatusCodes.Status400BadRequest, cancellationToken);

  }
}
