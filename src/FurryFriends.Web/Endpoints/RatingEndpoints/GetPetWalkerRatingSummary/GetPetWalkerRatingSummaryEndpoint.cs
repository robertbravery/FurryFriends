using Ardalis.Result;
using FurryFriends.UseCases.Rating.GetPetWalkerRatingSummary;
using FurryFriends.Web.Endpoints.RatingEndpoints.GetPetWalkerRatingSummary;
using FurryFriends.Web.Endpoints.RatingEndpoints.GetRatingsForPetWalker;

namespace FurryFriends.Web.Endpoints.RatingEndpoints.GetPetWalkerRatingSummary;

public class GetPetWalkerRatingSummaryEndpoint(IMediator mediator, ILogger<GetPetWalkerRatingSummaryEndpoint> logger)
    : Endpoint<GetPetWalkerRatingSummaryRequest, Result<GetPetWalkerRatingSummaryResponse>>
{
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<GetPetWalkerRatingSummaryEndpoint> _logger = logger;

    public override void Configure()
    {
        Get(GetPetWalkerRatingSummaryRequest.Route);
        AllowAnonymous();
        Options(x => x.WithName("GetPetWalkerRatingSummary"));

        Summary(s =>
        {
            s.Summary = "Get PetWalker rating summary";
            s.Description = "Returns the average rating and total ratings count for a PetWalker";
            s.Response<Result<GetPetWalkerRatingSummaryResponse>>(200, "Summary retrieved successfully");
        });
    }

    public override async Task HandleAsync(GetPetWalkerRatingSummaryRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Retrieving rating summary for PetWalker: {PetWalkerId}",
            request.PetWalkerId);

        var query = new GetPetWalkerRatingSummaryQuery(request.PetWalkerId);

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

        var summary = result.Value;
        var recentRatings = summary.RecentRatings.Select(r => new GetRatingsForPetWalkerResponse(
            r.Id,
            r.PetWalkerId,
            r.ClientId,
            r.BookingId,
            r.RatingValue,
            r.Comment,
            r.CreatedDate,
            r.ModifiedDate,
            r.ClientName)).ToList();

        Response = Result.Success(new GetPetWalkerRatingSummaryResponse(
            summary.PetWalkerId,
            summary.AverageRating,
            summary.TotalRatings,
            recentRatings));
    }
}
