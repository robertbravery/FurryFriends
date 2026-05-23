using Ardalis.GuardClauses;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FurryFriends.UseCases.Domain.Ratings.DeleteRating;

public class DeleteRatingHandler : IRequestHandler<DeleteRatingCommand, Result>
{
    private readonly IRepository<Core.RatingAggregate.Rating> _repository;
    private readonly ILogger<DeleteRatingHandler> _logger;

    public DeleteRatingHandler(
        IRepository<Core.RatingAggregate.Rating> repository,
        ILogger<DeleteRatingHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteRatingCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        _logger.LogInformation("Deleting rating: {RatingId}", request.RatingId);

        var rating = await _repository.GetByIdAsync(request.RatingId, cancellationToken);

        if (rating == null)
        {
            _logger.LogWarning("Rating not found: {RatingId}", request.RatingId);
            return Result.NotFound("Rating not found.");
        }

        // Verify the client owns this rating
        if (rating.ClientId != request.ClientId)
        {
            _logger.LogWarning("Client {ClientId} does not own rating {RatingId}", request.ClientId, request.RatingId);
            return Result.Error("Rating not found.");
        }

        // Use domain method that checks 24h window and status, sets status to Removed
        var removeResult = rating.Remove();

        if (!removeResult.IsSuccess)
        {
            _logger.LogWarning("Rating deletion failed: {RatingId}, Reason: {Error}",
                request.RatingId, string.Join("; ", removeResult.Errors));
            return Result.Error(string.Join("; ", removeResult.Errors));
        }

        await _repository.UpdateAsync(rating, cancellationToken);

        _logger.LogInformation("Rating deleted successfully: {RatingId}", request.RatingId);

        return Result.Success();
    }
}
