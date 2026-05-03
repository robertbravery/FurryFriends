using FurryFriends.UseCases.Domain.Ratings.DeleteRating;

namespace FurryFriends.Web.Endpoints.RatingEndpoints.Delete;

public class DeleteRatingEndpoint(IMediator mediator, ILogger<DeleteRatingEndpoint> logger)
    : Endpoint<DeleteRatingRequest, Result<DeleteRatingResponse>>
{
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<DeleteRatingEndpoint> _logger = logger;

    public override void Configure()
    {
        Delete(DeleteRatingRequest.Route);
        AllowAnonymous();
        Options(x => x.WithName("DeleteRating"));

        Summary(s =>
        {
            s.Summary = "Delete a rating";
            s.Description = "Deletes a rating within the 24-hour editing window";
            s.Response<Result<DeleteRatingResponse>>(200, "Rating deleted successfully");
        });
    }

    public override async Task HandleAsync(DeleteRatingRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Deleting rating: {RatingId}",
            request.RatingId);

        var command = new DeleteRatingCommand(
            request.RatingId,
            request.ClientId);

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

            if (result.Status == ResultStatus.Invalid)
            {
                await SendErrorsAsync(StatusCodes.Status400BadRequest, cancellationToken);
                return;
            }

            Response = Result.Error();
            await SendErrorsAsync(StatusCodes.Status400BadRequest, cancellationToken);
            return;
        }

        await SendNoContentAsync(cancellationToken);
    }
}
