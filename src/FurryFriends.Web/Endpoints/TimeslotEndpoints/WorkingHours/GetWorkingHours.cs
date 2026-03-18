using Ardalis.Result;
using FurryFriends.UseCases.Timeslots.WorkingHours;
using FurryFriends.UseCases.Timeslots.WorkingHours.Dto;
using Microsoft.Extensions.Logging;

namespace FurryFriends.Web.Endpoints.TimeslotEndpoints.WorkingHours;

public class GetWorkingHoursEndpoint(IMediator mediator, ILogger<GetWorkingHoursEndpoint> logger)
    : Endpoint<GetWorkingHoursRequest, Result<GetWorkingHoursResponse>>
{
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<GetWorkingHoursEndpoint> _logger = logger;

    public override void Configure()
    {
        Get(GetWorkingHoursRequest.Route);
        AllowAnonymous();
        Options(x => x.WithName("GetWorkingHours"));
    }

    public override async Task HandleAsync(GetWorkingHoursRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting working hours for PetWalker: {PetWalkerId}", request.PetWalkerId);

        var query = new GetWorkingHoursQuery(request.PetWalkerId);

        var result = await _mediator.Send(query, cancellationToken);

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

        var workingHoursDtos = result.Value.Select(w => new WorkingHoursDto(
            w.Id,
            w.PetWalkerId,
            w.DayOfWeek,
            w.StartTime,
            w.EndTime,
            w.IsActive)).ToList();

        Response = Result.Success(new GetWorkingHoursResponse(workingHoursDtos));
    }
}
