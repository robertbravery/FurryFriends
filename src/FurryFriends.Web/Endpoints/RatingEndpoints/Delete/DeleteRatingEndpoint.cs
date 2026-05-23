using FurryFriends.UseCases.Domain.Ratings.DeleteRating;
using FurryFriends.Web.Endpoints.Base;

namespace FurryFriends.Web.Endpoints.RatingEndpoints.Delete;

public class DeleteRatingEndpoint : BaseEndpoint<DeleteRatingRequest, DeleteRatingResponse>
{
    public DeleteRatingEndpoint(IMediator mediator, ILogger<DeleteRatingEndpoint> logger)
        : base(mediator, logger) { }

    protected override string OperationName => "DeleteRating";

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

        await HandleResultAsync(
            ct => _mediator.Send(command, ct),
            cancellationToken);
    }
}
