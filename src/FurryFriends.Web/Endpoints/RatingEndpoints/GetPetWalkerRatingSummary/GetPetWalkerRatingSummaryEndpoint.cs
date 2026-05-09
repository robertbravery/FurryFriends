using FurryFriends.UseCases.Domain.Ratings.GetPetWalkerRatingSummary;
using FurryFriends.Web.Endpoints.Base;
using FurryFriends.Web.Endpoints.RatingEndpoints.GetRatingsForPetWalker;

namespace FurryFriends.Web.Endpoints.RatingEndpoints.GetPetWalkerRatingSummary;

public class GetPetWalkerRatingSummaryEndpoint : BaseEndpoint<GetPetWalkerRatingSummaryRequest, GetPetWalkerRatingSummaryResponse>
{
    public GetPetWalkerRatingSummaryEndpoint(IMediator mediator, ILogger<GetPetWalkerRatingSummaryEndpoint> logger)
        : base(mediator, logger) { }

    protected override string OperationName => "GetPetWalkerRatingSummary";

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

        await HandleResultAsync(
            ct => _mediator.Send(query, ct),
            (PetWalkerRatingSummaryDto summary, CancellationToken ct) =>
            {
                var recentRatings = summary.RecentRatings.Select(r => new GetRatingsForPetWalkerResponse(
                r.Id,
                r.PetWalkerId,
                r.ClientId,
                r.RatingValue,
                r.Comment,
                r.CreatedAt,
                r.UpdatedAt,
                r.ClientName)).ToList();

                return Task.FromResult(new GetPetWalkerRatingSummaryResponse(
                summary.PetWalkerId,
                summary.AverageRating,
                summary.TotalRatings,
                recentRatings));
            },
            cancellationToken);
    }
}
