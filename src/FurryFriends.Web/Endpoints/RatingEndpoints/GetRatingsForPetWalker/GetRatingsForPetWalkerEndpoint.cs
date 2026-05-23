using FurryFriends.UseCases.Domain.Ratings.GetRatingsForPetWalker;
using FurryFriends.Web.Endpoints.Base;

namespace FurryFriends.Web.Endpoints.RatingEndpoints.GetRatingsForPetWalker;

public class GetRatingsForPetWalkerEndpoint : BaseEndpoint<GetRatingsForPetWalkerRequest, List<GetRatingsForPetWalkerResponse>>
{
    public GetRatingsForPetWalkerEndpoint(IMediator mediator, ILogger<GetRatingsForPetWalkerEndpoint> logger)
        : base(mediator, logger) { }

    protected override string OperationName => "GetRatingsForPetWalker";

    public override void Configure()
    {
        Get(GetRatingsForPetWalkerRequest.Route);
        AllowAnonymous();
        Options(x => x.WithName("GetRatingsForPetWalker"));

        Summary(s =>
        {
            s.Summary = "Get ratings for a PetWalker";
            s.Description = "Returns a paginated list of ratings for a specific PetWalker";
            s.Response<Result<List<GetRatingsForPetWalkerResponse>>>(200, "Ratings retrieved successfully");
        });
    }

    public override async Task HandleAsync(GetRatingsForPetWalkerRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Retrieving ratings for PetWalker: {PetWalkerId}, Page: {Page}, PageSize: {PageSize}",
            request.PetWalkerId,
            request.Page,
            request.PageSize);

        var query = new GetRatingsForPetWalkerQuery(
            request.PetWalkerId,
            request.Page,
            request.PageSize);

        await HandleResultAsync(
            ct => _mediator.Send(query, ct),
            (List<RatingDto> ratings, CancellationToken ct) =>
            {
                var response = ratings.Select(r => new GetRatingsForPetWalkerResponse(
                r.Id,
                r.PetWalkerId,
                r.ClientId,
                r.RatingValue,
                r.Comment,
                r.CreatedAt,
                r.UpdatedAt,
                r.ClientName)).ToList();

                return Task.FromResult(response);
            },
            cancellationToken);
    }
}
