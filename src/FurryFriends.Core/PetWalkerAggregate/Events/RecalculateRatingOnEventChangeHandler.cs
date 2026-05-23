using FurryFriends.Core.PetWalkerAggregate.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FurryFriends.Core.PetWalkerAggregate.Events;

public class RecalculateRatingOnEventChangeHandler : INotificationHandler<RatingAddedEvent>,
    INotificationHandler<RatingUpdatedEvent>,
    INotificationHandler<RatingRemovedEvent>
{
    private readonly IRepository<PetWalker> _repository;
    private readonly ILogger<RecalculateRatingOnEventChangeHandler> _logger;
    private readonly IReadRepository<RatingAggregate.Rating> _ratingRepository;

    public RecalculateRatingOnEventChangeHandler(
        IRepository<PetWalker> repository,
        IReadRepository<RatingAggregate.Rating> ratingRepository,
        ILogger<RecalculateRatingOnEventChangeHandler> logger)
    {
        _repository = Guard.Against.Null(repository, nameof(repository));
        _ratingRepository = Guard.Against.Null(ratingRepository, nameof(ratingRepository));
        _logger = Guard.Against.Null(logger, nameof(logger));
    }

    public async Task Handle(RatingAddedEvent notification, CancellationToken cancellationToken)
    {
        await RecalculateRatingAsync(notification.Rating.PetWalkerId, cancellationToken);
    }

    public async Task Handle(RatingUpdatedEvent notification, CancellationToken cancellationToken)
    {
        await RecalculateRatingAsync(notification.Rating.PetWalkerId, cancellationToken);
    }

    public async Task Handle(RatingRemovedEvent notification, CancellationToken cancellationToken)
    {
        await RecalculateRatingAsync(notification.Rating.PetWalkerId, cancellationToken);
    }

    private async Task RecalculateRatingAsync(Guid petWalkerId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Recalculating rating aggregate for PetWalker {PetWalkerId}", petWalkerId);

        var spec = new RatingAggregate.Specifications.GetActiveRatingsForPetWalkerSpecification(petWalkerId);
        var ratings = await _ratingRepository.ListAsync(spec, cancellationToken);

        if (ratings.Count == 0)
        {
            var petWalker = await _repository.GetByIdAsync(petWalkerId, cancellationToken);
            if (petWalker is not null)
            {
                petWalker.UpdateRatingAggregate(null, 0);
                await _repository.UpdateAsync(petWalker, cancellationToken);
                _logger.LogInformation(
                    "Set PetWalker {PetWalkerId} AverageRating to null, TotalRatingsCount to 0 (no active ratings)",
                    petWalkerId);
            }
            return;
        }

        double average = Math.Round(ratings.Average(r => r.RatingValue), 1, MidpointRounding.AwayFromZero);
        int totalCount = ratings.Count;

        var petWalkerToUpdate = await _repository.GetByIdAsync(petWalkerId, cancellationToken);
        if (petWalkerToUpdate is not null)
        {
            petWalkerToUpdate.UpdateRatingAggregate(average, totalCount);
            await _repository.UpdateAsync(petWalkerToUpdate, cancellationToken);
            _logger.LogInformation(
                "Updated PetWalker {PetWalkerId} — AverageRating: {Average}, TotalRatingsCount: {Count}",
                petWalkerId, average, totalCount);
        }
    }
}