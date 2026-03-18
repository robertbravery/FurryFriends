using Ardalis.Result;
using FurryFriends.UseCases.Timeslots.CustomTimeRequest;
using Microsoft.Extensions.Logging;

namespace FurryFriends.Web.Endpoints.TimeslotEndpoints.CustomTimeRequest;

public class RespondToCustomTimeRequestEndpoint : Endpoint<RespondToCustomTimeRequestRequest, Result<RespondToCustomTimeRequestResponse>>
{
    private readonly IMediator _mediator;
    private readonly ILogger<RespondToCustomTimeRequestEndpoint> _logger;

    public RespondToCustomTimeRequestEndpoint(IMediator mediator, ILogger<RespondToCustomTimeRequestEndpoint> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public override void Configure()
    {
        Put(RespondToCustomTimeRequestRequest.Route);
        AllowAnonymous();
        Options(x => x.WithName("RespondToCustomTimeRequest"));
    }

    public override async Task HandleAsync(RespondToCustomTimeRequestRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Responding to custom time request: {RequestId} with response: {Response}",
            request.RequestId,
            request.Response);

        var responseEnum = Enum.Parse<CustomTimeRequestResponse>(request.Response, ignoreCase: true);

        var command = new RespondToCustomTimeRequestCommand(
            request.RequestId,
            responseEnum,
            request.CounterOfferedDate,
            request.CounterOfferedTime,
            request.Reason);

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            await HandleResultErrorsAsync(result, cancellationToken);
            return;
        }

        var dto = result.Value;
        Response = Result.Success(new RespondToCustomTimeRequestResponse
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
        
        await SendErrorsAsync(statusCode, cancellationToken);
    }
}
