using Ardalis.Result;
using FurryFriends.UseCases.Rating.UpdateRating;
using FurryFriends.Web.Endpoints.RatingEndpoints.Update;

namespace FurryFriends.Web.Endpoints.RatingEndpoints.Update;

public class UpdateRatingEndpoint(IMediator mediator, ILogger<UpdateRatingEndpoint> logger)
    : Endpoint<UpdateRatingRequest, Result<UpdateRatingResponse>>
{
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<UpdateRatingEndpoint> _logger = logger;

    public override void Configure()
    {
        Put(UpdateRatingRequest.Route);
        AllowAnonymous();
        Options(x => x.WithName("UpdateRating"));
    }

    public override async Task HandleAsync(UpdateRatingRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Updating rating: {RatingId}, RatingValue: {RatingValue}",
            request.RatingId,
            request.RatingValue);

        var command = new UpdateRatingCommand(
            request.RatingId,
            request.RatingValue,
            request.Comment);

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

        var ratingId = result.Value;
        Response = Result.Success(new UpdateRatingResponse(
            ratingId,
            request.RatingValue,
            request.Comment));
    }
}
