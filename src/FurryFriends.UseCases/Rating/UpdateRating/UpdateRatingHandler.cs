using Ardalis.GuardClauses;
using Ardalis.Result;
using FurryFriends.Core.RatingAggregate;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FurryFriends.UseCases.Rating.UpdateRating;

public class UpdateRatingHandler : IRequestHandler<UpdateRatingCommand, Result<Guid>>
{
    private readonly IRepository<Core.RatingAggregate.Rating> _repository;
    private readonly ILogger<UpdateRatingHandler> _logger;

    public UpdateRatingHandler(IRepository<Core.RatingAggregate.Rating> repository, ILogger<UpdateRatingHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(UpdateRatingCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        _logger.LogInformation("Updating rating: {RatingId}", request.RatingId);

        var rating = await _repository.GetByIdAsync(request.RatingId, cancellationToken);

        if (rating == null)
        {
            _logger.LogWarning("Rating not found: {RatingId}", request.RatingId);
            return Result<Guid>.Error("Rating not found.");
        }

        if (!rating.CanUpdate())
        {
            _logger.LogWarning("Rating cannot be updated: {RatingId}", request.RatingId);
            return Result<Guid>.Error("Rating cannot be updated. Either 7 days have passed or it has already been updated.");
        }

        if (request.RatingValue.HasValue)
        {
            rating.UpdateRatingValue(request.RatingValue.Value);
            _logger.LogInformation("Rating value updated to {RatingValue} for Rating: {RatingId}",
                request.RatingValue.Value, request.RatingId);
        }

        if (request.Comment != null)
        {
            rating.UpdateComment(request.Comment);
            _logger.LogInformation("Comment updated for Rating: {RatingId}", request.RatingId);
        }

        await _repository.UpdateAsync(rating, cancellationToken);

        _logger.LogInformation("Rating updated successfully: {RatingId}", request.RatingId);

        return Result<Guid>.Success(rating.Id);
    }
}
