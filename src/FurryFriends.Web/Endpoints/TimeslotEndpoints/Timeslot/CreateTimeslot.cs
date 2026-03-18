using Ardalis.Result;
using FurryFriends.UseCases.Timeslots.Timeslot;
using Microsoft.Extensions.Logging;

namespace FurryFriends.Web.Endpoints.TimeslotEndpoints.Timeslot;

public class CreateTimeslotEndpoint(IMediator mediator, ILogger<CreateTimeslotEndpoint> logger)
    : Endpoint<CreateTimeslotRequest, Result<CreateTimeslotResponse>>
{
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<CreateTimeslotEndpoint> _logger = logger;

    public override void Configure()
    {
        Post(CreateTimeslotRequest.Route);
        AllowAnonymous();
        Options(x => x.WithName("CreateTimeslot"));
    }

    public override async Task HandleAsync(CreateTimeslotRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Creating timeslot for PetWalker: {PetWalkerId}, Date: {Date}, Time: {StartTime}, Duration: {Duration}",
            request.PetWalkerId,
            request.Date,
            request.StartTime,
            request.DurationInMinutes);

        var command = new CreateTimeslotCommand(
            request.PetWalkerId,
            request.Date,
            request.StartTime,
            request.DurationInMinutes);

        var result = await _mediator.Send(command, CancellationToken.None);

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
        Response = Result.Success(new CreateTimeslotResponse
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
