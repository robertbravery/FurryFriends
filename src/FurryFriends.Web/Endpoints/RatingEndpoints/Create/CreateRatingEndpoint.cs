using FurryFriends.UseCases.Domain.Ratings.CreateRating;
using FurryFriends.Web.Endpoints.Base;

namespace FurryFriends.Web.Endpoints.RatingEndpoints.Create;

public class CreateRatingEndpoint : BaseEndpoint<CreateRatingRequest, CreateRatingResponse>
{
    public CreateRatingEndpoint(IMediator mediator, ILogger<CreateRatingEndpoint> logger)
        : base(mediator, logger) { }

    protected override string OperationName => "CreateRating";

    public override void Configure()
    {
        Post(CreateRatingRequest.Route);
        AllowAnonymous();
        Options(x => x.WithName("CreateRating"));
    }

    public override async Task HandleAsync(CreateRatingRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Creating rating for PetWalker: {PetWalkerId} by Client: {ClientId}, Rating: {RatingValue}",
            request.PetWalkerId,
            request.ClientId,
            request.RatingValue);

        var command = new CreateRatingCommand(
            request.PetWalkerId,
            request.ClientId,
            request.RatingValue,
            request.Comment);

        await HandleResultAsync(
            ct => _mediator.Send(command, ct),
            (Guid ratingId, CancellationToken ct) =>
                Task.FromResult(new CreateRatingResponse(
                    ratingId,
                    request.PetWalkerId,
                    request.RatingValue,
                    request.Comment)),
            cancellationToken);
    }
}
