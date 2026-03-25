using Ardalis.GuardClauses;
using Ardalis.Result;
using FurryFriends.Core.RatingAggregate;
using FurryFriends.Core.RatingAggregate.Specifications;
using FurryFriends.UseCases.Rating.GetRatingsForPetWalker;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FurryFriends.UseCases.Rating.GetPetWalkerRatingSummary;

public class GetPetWalkerRatingSummaryHandler : IRequestHandler<GetPetWalkerRatingSummaryQuery, Result<PetWalkerRatingSummaryDto>>
{
    private readonly IRepository<Core.RatingAggregate.Rating> _repository;
    private readonly ILogger<GetPetWalkerRatingSummaryHandler> _logger;

    public GetPetWalkerRatingSummaryHandler(IRepository<Core.RatingAggregate.Rating> repository, ILogger<GetPetWalkerRatingSummaryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<PetWalkerRatingSummaryDto>> Handle(GetPetWalkerRatingSummaryQuery request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        _logger.LogInformation("Retrieving rating summary for PetWalker: {PetWalkerId}", request.PetWalkerId);

        // Get all ratings for the petwalker
        var allRatingsSpec = new GetRatingsForPetWalkerWithPaginationSpecification(request.PetWalkerId);
        var allRatings = await _repository.ListAsync(allRatingsSpec, cancellationToken);

        if (allRatings == null || allRatings.Count == 0)
        {
            _logger.LogInformation("No ratings found for PetWalker: {PetWalkerId}", request.PetWalkerId);
            return Result<PetWalkerRatingSummaryDto>.Success(new PetWalkerRatingSummaryDto(
                request.PetWalkerId,
                0,
                0,
                new List<RatingDto>()));
        }

        // Calculate average rating
        var averageRating = allRatings.Average(r => r.RatingValue);
        var totalRatings = allRatings.Count;

        // Get recent ratings (last 10)
        var recentRatings = allRatings
            .OrderByDescending(r => r.CreatedDate)
            .Take(10)
            .Select(r => new RatingDto(
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

        var summary = new PetWalkerRatingSummaryDto(
            request.PetWalkerId,
            Math.Round(averageRating, 2),
            totalRatings,
            recentRatings);

        _logger.LogInformation("Rating summary retrieved for PetWalker: {PetWalkerId}, Average: {AverageRating}, Total: {TotalRatings}",
            request.PetWalkerId, summary.AverageRating, summary.TotalRatings);

        return Result<PetWalkerRatingSummaryDto>.Success(summary);
    }
}
