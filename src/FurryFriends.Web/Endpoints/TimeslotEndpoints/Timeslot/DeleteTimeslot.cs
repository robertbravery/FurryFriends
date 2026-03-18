using Ardalis.Result;
using FurryFriends.UseCases.Timeslots.Timeslot;
using Microsoft.Extensions.Logging;

namespace FurryFriends.Web.Endpoints.TimeslotEndpoints.Timeslot;

public class DeleteTimeslotEndpoint(IMediator mediator, ILogger<DeleteTimeslotEndpoint> logger)
    : Endpoint<DeleteTimeslotRequest, Result<bool>>
{
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<DeleteTimeslotEndpoint> _logger = logger;

    public override void Configure()
    {
        Delete(DeleteTimeslotRequest.Route);
        AllowAnonymous();
        Options(x => x.WithName("DeleteTimeslot"));
    }

    public override async Task HandleAsync(DeleteTimeslotRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Deleting timeslot: {TimeslotId}",
            request.TimeslotId);

        var command = new DeleteTimeslotCommand(request.TimeslotId);

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

        Response = Result.Success(true);
        await SendOkAsync(Response, cancellationToken);
    }
}
