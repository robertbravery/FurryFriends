using Ardalis.Result;
using FurryFriends.UseCases.Timeslots.WorkingHours;
using FurryFriends.UseCases.Timeslots.WorkingHours.Dto;
using Microsoft.Extensions.Logging;

namespace FurryFriends.Web.Endpoints.TimeslotEndpoints.WorkingHours;

public class CreateWorkingHoursEndpoint(IMediator mediator, ILogger<CreateWorkingHoursEndpoint> logger)
    : Endpoint<CreateWorkingHoursRequest, Result<CreateWorkingHoursResponse>>
{
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<CreateWorkingHoursEndpoint> _logger = logger;

    public override void Configure()
    {
        Post(CreateWorkingHoursRequest.Route);
        AllowAnonymous();
        Options(x => x.WithName("CreateWorkingHours_" + Guid.NewGuid().ToString()));
    }

    public override async Task HandleAsync(CreateWorkingHoursRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Creating working hours for PetWalker: {PetWalkerId}, Day: {DayOfWeek}, Time: {StartTime} - {EndTime}",
            request.PetWalkerId,
            request.DayOfWeek,
            request.StartTime,
            request.EndTime);

        var command = new CreateWorkingHoursCommand(
            request.PetWalkerId,
            request.DayOfWeek,
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
        Response = Result.Success(new CreateWorkingHoursResponse(
            workingHoursDto.Id,
            workingHoursDto.PetWalkerId,
            workingHoursDto.DayOfWeek,
            workingHoursDto.StartTime,
            workingHoursDto.EndTime,
            workingHoursDto.IsActive));
    }
}
