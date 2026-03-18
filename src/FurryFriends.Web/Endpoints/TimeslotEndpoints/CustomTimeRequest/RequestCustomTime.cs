using Ardalis.Result;
using FurryFriends.UseCases.Timeslots.CustomTimeRequest;
using Microsoft.Extensions.Logging;

namespace FurryFriends.Web.Endpoints.TimeslotEndpoints.CustomTimeRequest;

public class RequestCustomTimeEndpoint : Endpoint<RequestCustomTimeRequest, Result<RequestCustomTimeResponse>>
{
    private readonly IMediator _mediator;
    private readonly ILogger<RequestCustomTimeEndpoint> _logger;

    public RequestCustomTimeEndpoint(IMediator mediator, ILogger<RequestCustomTimeEndpoint> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public override void Configure()
    {
        Post(RequestCustomTimeRequest.Route);
        AllowAnonymous();
        Options(x => x.WithName("RequestCustomTime"));
    }

    public override async Task HandleAsync(RequestCustomTimeRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Requesting custom time for petwalker: {PetWalkerId} from client: {ClientId}",
            request.PetWalkerId,
            request.ClientId);

        var command = new RequestCustomTimeCommand(
            request.PetWalkerId,
            request.ClientId,
            request.RequestedDate,
            request.PreferredStartTime,
            request.PreferredDurationMinutes,
            request.ClientAddress,
            request.PetIds);

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            await HandleResultErrorsAsync(result, cancellationToken);
            return;
        }

        var dto = result.Value;
        Response = Result.Success(new RequestCustomTimeResponse
        {
            RequestId = dto.RequestId,
            PetWalkerId = dto.PetWalkerId,
            ClientId = dto.ClientId,
            RequestedDate = dto.RequestedDate,
            PreferredStartTime = dto.PreferredStartTime,
            PreferredEndTime = dto.PreferredEndTime,
            PreferredDurationMinutes = dto.PreferredDurationMinutes,
            ClientAddress = dto.ClientAddress,
            Status = dto.Status,
            CreatedAt = dto.CreatedAt
        });
    }

    private async Task HandleResultErrorsAsync(Result<CustomTimeRequestDto> result, CancellationToken cancellationToken)
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
        
        // Check for duplicate pending request (conflict)
        if (result.Errors?.Any(e => e.Contains("pending custom time request")) == true)
        {
            statusCode = StatusCodes.Status409Conflict;
        }

        await SendErrorsAsync(statusCode, cancellationToken);
    }
}
