using Ardalis.Result;
using FurryFriends.UseCases.Timeslots.Timeslot;
using Microsoft.Extensions.Logging;

namespace FurryFriends.Web.Endpoints.TimeslotEndpoints.Timeslot;

public class GetAvailableTimeslotsEndpoint(IMediator mediator, ILogger<GetAvailableTimeslotsEndpoint> logger)
    : Endpoint<GetAvailableTimeslotsRequest, Result<GetAvailableTimeslotsResponse>>
{
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<GetAvailableTimeslotsEndpoint> _logger = logger;

    public override void Configure()
    {
        Get(GetAvailableTimeslotsRequest.Route);
        AllowAnonymous();
        Options(x => x.WithName("GetAvailableTimeslots"));
    }

    public override async Task HandleAsync(GetAvailableTimeslotsRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Getting available timeslots for PetWalker: {PetWalkerId}, Date: {Date}",
            request.PetWalkerId,
            request.Date);

        var query = new GetAvailableTimeslotsQuery(
            request.PetWalkerId,
            request.Date);

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

        var response = new GetAvailableTimeslotsResponse
        {
            PetWalkerId = result.Value.PetWalkerId,
            Date = result.Value.Date,
            Timeslots = result.Value.Timeslots.Select(t => new AvailableTimeslotResponse
            {
                TimeslotId = t.TimeslotId,
                StartTime = t.StartTime,
                EndTime = t.EndTime,
                DurationInMinutes = t.DurationInMinutes
            }).ToList(),
            HasTravelBufferWarning = result.Value.HasTravelBufferWarning,
            TravelBufferMessage = result.Value.TravelBufferMessage
        };

        Response = Result.Success(response);
        await SendOkAsync(Response, cancellationToken);
    }
}
