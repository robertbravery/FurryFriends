using Ardalis.GuardClauses;
using FurryFriends.Core.BookingAggregate;
using FurryFriends.Core.BookingAggregate.Specifications;
using FurryFriends.Core.RatingAggregate.Specifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FurryFriends.UseCases.Domain.Ratings.CreateRating;

public class CreateRatingHandler : IRequestHandler<CreateRatingCommand, Result<Guid>>
{
    private readonly IRepository<Core.RatingAggregate.Rating> _ratingRepository;
    private readonly IRepository<Booking> _bookingRepository;
    private readonly ILogger<CreateRatingHandler> _logger;

    public CreateRatingHandler(
        IRepository<Core.RatingAggregate.Rating> ratingRepository,
        IRepository<Booking> bookingRepository,
        ILogger<CreateRatingHandler> logger)
    {
        _ratingRepository = ratingRepository;
        _bookingRepository = bookingRepository;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(CreateRatingCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        _logger.LogInformation(
            "Creating rating for PetWalker: {PetWalkerId} by Client: {ClientId}, Rating: {RatingValue}",
            request.PetWalkerId, request.ClientId, request.RatingValue);

        // Verify client has at least one completed booking with this petwalker
        var completedBookingsSpec = new CountCompletedBookingsForClientPetWalkerSpecification(
            request.ClientId, request.PetWalkerId);
        var completedCount = await _bookingRepository.CountAsync(completedBookingsSpec, cancellationToken);

        if (completedCount == 0)
        {
            _logger.LogWarning(
                "Client {ClientId} has no completed bookings with PetWalker {PetWalkerId}",
                request.ClientId, request.PetWalkerId);
            return Result.Error("You must have at least one completed booking with this petwalker to submit a rating.");
        }

        // Check if client already has an active rating for this petwalker (ratings ≤ completed bookings rule)
        var existingRatingSpec = new GetActiveRatingsForPetWalkerSpecification(request.PetWalkerId);
        var allActiveRatings = await _ratingRepository.ListAsync(existingRatingSpec, cancellationToken);
        var existingRating = allActiveRatings.FirstOrDefault(r => r.ClientId == request.ClientId);

        if (existingRating != null)
        {
            // Client already has a rating — new rating replaces the previous one (ratings ≤ bookings constraint)
            _logger.LogInformation(
                "Replacing existing rating {ExistingRatingId} for PetWalker {PetWalkerId} by Client {ClientId}",
                existingRating.Id, request.PetWalkerId, request.ClientId);

            existingRating.UpdateRating(request.RatingValue, request.Comment);
            await _ratingRepository.UpdateAsync(existingRating, cancellationToken);

            _logger.LogInformation(
                "Rating updated successfully: {RatingId} for PetWalker: {PetWalkerId}, Client: {ClientId}",
                existingRating.Id, request.PetWalkerId, request.ClientId);

            return Result<Guid>.Success(existingRating.Id);
        }

        // Check ratings ≤ completed bookings constraint
        var clientRatingsOnPetWalker = allActiveRatings.Count(r => r.ClientId == request.ClientId);
        if (clientRatingsOnPetWalker >= completedCount)
        {
            _logger.LogWarning(
                "Client {ClientId} has reached the maximum number of ratings ({RatingsCount}) for PetWalker {PetWalkerId} (completed bookings: {CompletedCount})",
                request.ClientId, clientRatingsOnPetWalker, request.PetWalkerId, completedCount);
            return Result.Error("You have reached the maximum number of ratings allowed for this petwalker.");
        }

        // Create rating
        var rating = Core.RatingAggregate.Rating.Create(
            request.PetWalkerId,
            request.ClientId,
            request.RatingValue,
            request.Comment);

        await _ratingRepository.AddAsync(rating, cancellationToken);

        _logger.LogInformation(
            "Rating created successfully: {RatingId} for PetWalker: {PetWalkerId}, Client: {ClientId}",
            rating.Id, request.PetWalkerId, request.ClientId);

        return Result<Guid>.Success(rating.Id);
    }
}
