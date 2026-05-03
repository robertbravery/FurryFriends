using Ardalis.GuardClauses;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FurryFriends.UseCases.Domain.Ratings.UpdateRating;

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

        // Use domain method that checks 24h window and status
        var updateResult = rating.UpdateRating(
            request.RatingValue ?? rating.RatingValue,
            request.Comment ?? rating.Comment);

        if (!updateResult.IsSuccess)
        {
            _logger.LogWarning("Rating update failed: {RatingId}, Reason: {Error}", 
                request.RatingId, string.Join("; ", updateResult.Errors));
            return Result<Guid>.Error(string.Join("; ", updateResult.Errors));
        }

        await _repository.UpdateAsync(rating, cancellationToken);

        _logger.LogInformation("Rating updated successfully: {RatingId}", request.RatingId);

        return Result<Guid>.Success(rating.Id);
    }
}
