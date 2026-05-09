using FurryFriends.UseCases.Domain.Ratings.UpdateRating;
using FurryFriends.Web.Endpoints.Base;

namespace FurryFriends.Web.Endpoints.RatingEndpoints.Update;

public class UpdateRatingEndpoint : BaseEndpoint<UpdateRatingRequest, UpdateRatingResponse>
{
    public UpdateRatingEndpoint(IMediator mediator, ILogger<UpdateRatingEndpoint> logger)
        : base(mediator, logger) { }

    protected override string OperationName => "UpdateRating";

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

        await HandleResultAsync(
            ct => _mediator.Send(command, ct),
            (Guid ratingId, CancellationToken ct) =>
                Task.FromResult(new UpdateRatingResponse(
                    ratingId,
                    request.RatingValue,
                    request.Comment)),
            cancellationToken);
    }
}
