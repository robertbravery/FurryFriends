using Ardalis.Result;
using FurryFriends.UseCases.Timeslots.WorkingHours;
using FurryFriends.UseCases.Timeslots.WorkingHours.Dto;
using Microsoft.Extensions.Logging;

namespace FurryFriends.Web.Endpoints.TimeslotEndpoints.WorkingHours;

public class UpdateWorkingHoursEndpoint(IMediator mediator, ILogger<UpdateWorkingHoursEndpoint> logger)
    : Endpoint<UpdateWorkingHoursRequest, Result<UpdateWorkingHoursResponse>>
{
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<UpdateWorkingHoursEndpoint> _logger = logger;

    public override void Configure()
    {
        Put(UpdateWorkingHoursRequest.Route);
        AllowAnonymous();
        Options(x => x.WithName("UpdateWorkingHours"));
    }

    public override async Task HandleAsync(UpdateWorkingHoursRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Updating working hours: {Id}, Time: {StartTime} - {EndTime}",
            request.Id,
            request.StartTime,
            request.EndTime);

        var command = new UpdateWorkingHoursCommand(
            request.Id,
            request.StartTime,
            request.EndTime,
            request.IsActive);

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

        var workingHoursDto = result.Value;
        Response = Result.Success(new UpdateWorkingHoursResponse(
            workingHoursDto.Id,
            workingHoursDto.PetWalkerId,
            workingHoursDto.DayOfWeek,
            workingHoursDto.StartTime,
            workingHoursDto.EndTime,
            workingHoursDto.IsActive));
    }
}
