using Ardalis.Result;
using FurryFriends.UseCases.Timeslots.Timeslot;
using Microsoft.Extensions.Logging;

namespace FurryFriends.Web.Endpoints.TimeslotEndpoints.Timeslot;

public class UpdateTimeslotEndpoint(IMediator mediator, ILogger<UpdateTimeslotEndpoint> logger)
    : Endpoint<UpdateTimeslotRequest, Result<UpdateTimeslotResponse>>
{
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<UpdateTimeslotEndpoint> _logger = logger;

    public override void Configure()
    {
        Put(UpdateTimeslotRequest.Route);
        AllowAnonymous();
        Options(x => x.WithName("UpdateTimeslot"));
    }

    public override async Task HandleAsync(UpdateTimeslotRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Updating timeslot: {TimeslotId}, StartTime: {StartTime}, Duration: {Duration}",
            request.TimeslotId,
            request.StartTime,
            request.DurationInMinutes);

        var command = new UpdateTimeslotCommand(
            request.TimeslotId,
            request.StartTime,
            request.DurationInMinutes);

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            foreach (var error in result.Errors)
            {
                AddError(error);
            }

            if (result.Status == ResultStatus.NotFound)
            {
                await SendNotFoundAsync(cancellationToken);
                return;
            }

            Response = Result.Error();
            await SendErrorsAsync(StatusCodes.Status400BadRequest, cancellationToken);
            return;
        }

        var timeslotDto = result.Value;
        Response = Result.Success(new UpdateTimeslotResponse
        {
            Id = timeslotDto.Id,
            PetWalkerId = timeslotDto.PetWalkerId,
            Date = timeslotDto.Date,
            StartTime = timeslotDto.StartTime,
            EndTime = timeslotDto.EndTime,
            DurationInMinutes = timeslotDto.DurationInMinutes,
            Status = timeslotDto.Status
        });
    }
}
