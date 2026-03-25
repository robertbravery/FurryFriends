using Ardalis.Result;
using FurryFriends.UseCases.Rating.GetRatingsForPetWalker;
using FurryFriends.Web.Endpoints.Base;
using FurryFriends.Web.Endpoints.RatingEndpoints.GetRatingsForPetWalker;

namespace FurryFriends.Web.Endpoints.RatingEndpoints.GetRatingsForPetWalker;

public class GetRatingsForPetWalkerEndpoint(IMediator mediator, ILogger<GetRatingsForPetWalkerEndpoint> logger)
    : Endpoint<GetRatingsForPetWalkerRequest, Result<List<GetRatingsForPetWalkerResponse>>>
{
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<GetRatingsForPetWalkerEndpoint> _logger = logger;

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

        var ratings = result.Value.Select(r => new GetRatingsForPetWalkerResponse(
            r.Id,
            r.PetWalkerId,
            r.ClientId,
            r.BookingId,
            r.RatingValue,
            r.Comment,
            r.CreatedDate,
            r.ModifiedDate,
            r.ClientName)).ToList();

        Response = Result.Success(ratings);
    }
}
