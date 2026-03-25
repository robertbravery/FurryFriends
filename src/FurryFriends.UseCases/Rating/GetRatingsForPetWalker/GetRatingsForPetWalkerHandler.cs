using Ardalis.GuardClauses;
using Ardalis.Result;
using FurryFriends.Core.RatingAggregate;
using FurryFriends.Core.RatingAggregate.Specifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FurryFriends.UseCases.Rating.GetRatingsForPetWalker;

public class GetRatingsForPetWalkerHandler : IRequestHandler<GetRatingsForPetWalkerQuery, Result<List<RatingDto>>>
{
    private readonly IRepository<Core.RatingAggregate.Rating> _repository;
    private readonly ILogger<GetRatingsForPetWalkerHandler> _logger;

    public GetRatingsForPetWalkerHandler(IRepository<Core.RatingAggregate.Rating> repository, ILogger<GetRatingsForPetWalkerHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<List<RatingDto>>> Handle(GetRatingsForPetWalkerQuery request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        _logger.LogInformation("Retrieving ratings for PetWalker: {PetWalkerId}, Page: {Page}, PageSize: {PageSize}",
            request.PetWalkerId, request.Page, request.PageSize);

        var spec = new GetRatingsForPetWalkerWithPaginationSpecification(request.PetWalkerId, request.Page, request.PageSize);
        var ratings = await _repository.ListAsync(spec, cancellationToken);

        var dtos = ratings.Select(r => new RatingDto(
            r.Id,
            r.PetWalkerId,
            r.ClientId,
            r.BookingId,
            r.RatingValue,
            r.Comment,
            r.CreatedDate,
            r.ModifiedDate,
            null // ClientName - would need to be fetched from Client aggregate
        )).ToList();

        _logger.LogInformation("Retrieved {Count} ratings for PetWalker: {PetWalkerId}",
            dtos.Count, request.PetWalkerId);

        return Result<List<RatingDto>>.Success(dtos);
    }
}
