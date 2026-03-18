using Ardalis.Result;
using FurryFriends.UseCases.Timeslots.Booking;
using Microsoft.Extensions.Logging;

namespace FurryFriends.Web.Endpoints.TimeslotEndpoints.Booking;

public class BookTimeslotEndpoint : Endpoint<BookTimeslotRequest, Result<BookTimeslotResponse>>
{
    private readonly IMediator _mediator;
    private readonly ILogger<BookTimeslotEndpoint> _logger;

    public BookTimeslotEndpoint(IMediator mediator, ILogger<BookTimeslotEndpoint> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public override void Configure()
    {
        Post(BookTimeslotRequest.Route);
        AllowAnonymous();
        Options(x => x.WithName("BookTimeslot"));
    }

    public override async Task HandleAsync(BookTimeslotRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Booking timeslot: {TimeslotId} for client: {ClientId}",
            request.TimeslotId,
            request.ClientId);

        var command = new BookTimeslotCommand(
            request.TimeslotId,
            request.ClientId,
            request.ClientAddress,
            request.PetIds);

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            await HandleResultErrorsAsync(result, cancellationToken);
            return;
        }

        var bookingDto = result.Value;
        Response = Result.Success(new BookTimeslotResponse
        {
            BookingId = bookingDto.BookingId,
            TimeslotId = bookingDto.TimeslotId,
            PetWalkerId = bookingDto.PetWalkerId,
            ClientId = bookingDto.ClientId,
            Date = bookingDto.Date,
            StartTime = bookingDto.StartTime,
            EndTime = bookingDto.EndTime,
            ClientAddress = bookingDto.ClientAddress,
            Status = bookingDto.Status,
            HasTravelBuffer = bookingDto.HasTravelBuffer,
            TravelBufferMinutes = bookingDto.TravelBufferMinutes
        });
    }

    private async Task HandleResultErrorsAsync(Result<BookTimeslotDto> result, CancellationToken cancellationToken)
    {
        if (result.Status == ResultStatus.NotFound)
        {
            await SendNotFoundAsync(cancellationToken);
            return;
        }

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

        var statusCode = result.IsSuccess ? StatusCodes.Status500InternalServerError : StatusCodes.Status400BadRequest;
        
        // Check for conflict (already booked)
        if (result.Errors?.Any(e => e.Contains("no longer available")) == true)
        {
            statusCode = StatusCodes.Status409Conflict;
        }

        await SendErrorsAsync(statusCode, cancellationToken);
    }
}
