using Ardalis.Result;
using FurryFriends.UseCases.Timeslots.WorkingHours;
using Microsoft.Extensions.Logging;

namespace FurryFriends.Web.Endpoints.TimeslotEndpoints.WorkingHours;

public class DeleteWorkingHoursEndpoint(IMediator mediator, ILogger<DeleteWorkingHoursEndpoint> logger)
    : Endpoint<DeleteWorkingHoursRequest, Result<DeleteWorkingHoursResponse>>
{
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<DeleteWorkingHoursEndpoint> _logger = logger;

    public override void Configure()
    {
        Delete(DeleteWorkingHoursRequest.Route);
        AllowAnonymous();
        Options(x => x.WithName("DeleteWorkingHours"));
    }

    public override async Task HandleAsync(DeleteWorkingHoursRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting working hours: {Id}", request.Id);

        var command = new DeleteWorkingHoursCommand(request.Id);

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

        Response = Result.Success(new DeleteWorkingHoursResponse(result.Value));
    }
}
