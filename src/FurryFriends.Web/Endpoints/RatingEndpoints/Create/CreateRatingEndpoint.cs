using Ardalis.Result;
using FurryFriends.UseCases.Rating.CreateRating;
using FurryFriends.Web.Endpoints.RatingEndpoints;

namespace FurryFriends.Web.Endpoints.RatingEndpoints.Create;

public class CreateRatingEndpoint(IMediator mediator, ILogger<CreateRatingEndpoint> logger)
    : Endpoint<CreateRatingRequest, Result<CreateRatingResponse>>
{
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<CreateRatingEndpoint> _logger = logger;

    public override void Configure()
    {
        Post(CreateRatingRequest.Route);
        AllowAnonymous();
        Options(x => x.WithName("CreateRating"));
    }

    public override async Task HandleAsync(CreateRatingRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Creating rating for Booking: {BookingId}, Rating: {RatingValue}",
            request.BookingId,
            request.RatingValue);

        var command = new CreateRatingCommand(
            request.BookingId,
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
        Response = Result.Success(new CreateRatingResponse(
            ratingId,
            request.BookingId,
            request.RatingValue,
            request.Comment));
    }
}
