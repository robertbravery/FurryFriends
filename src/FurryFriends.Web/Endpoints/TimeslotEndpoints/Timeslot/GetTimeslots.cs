using Ardalis.Result;
using FurryFriends.UseCases.Timeslots.Timeslot;
using Microsoft.Extensions.Logging;

namespace FurryFriends.Web.Endpoints.TimeslotEndpoints.Timeslot;

public class GetTimeslotsEndpoint(IMediator mediator, ILogger<GetTimeslotsEndpoint> logger)
    : Endpoint<GetTimeslotsRequest, Result<GetTimeslotsResponse>>
{
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<GetTimeslotsEndpoint> _logger = logger;

    public override void Configure()
    {
        Get(GetTimeslotsRequest.Route);
        AllowAnonymous();
        Options(x => x.WithName("GetTimeslots"));
    }

    public override async Task HandleAsync(GetTimeslotsRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Getting timeslots for PetWalker: {PetWalkerId}, Date: {Date}",
            request.PetWalkerId,
            request.Date);

        var query = new GetTimeslotsQuery(
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

        var response = new GetTimeslotsResponse
        {
            PetWalkerId = result.Value.PetWalkerId,
            Date = result.Value.Date,
            Timeslots = result.Value.Timeslots.Select(t => new TimeslotResponse
            {
                Id = t.Id,
                PetWalkerId = t.PetWalkerId,
                Date = t.Date,
                StartTime = t.StartTime,
                EndTime = t.EndTime,
                DurationInMinutes = t.DurationInMinutes,
                Status = t.Status
            }).ToList()
        };

        Response = Result.Success(response);
        await SendOkAsync(Response, cancellationToken);
    }
}
