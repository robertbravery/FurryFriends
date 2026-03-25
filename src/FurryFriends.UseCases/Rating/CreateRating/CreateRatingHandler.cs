using Ardalis.GuardClauses;
using Ardalis.Result;
using FurryFriends.Core.BookingAggregate;
using FurryFriends.Core.BookingAggregate.Enums;
using FurryFriends.Core.BookingAggregate.Specifications;
using FurryFriends.Core.RatingAggregate;
using FurryFriends.Core.RatingAggregate.Specifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FurryFriends.UseCases.Rating.CreateRating;

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

        _logger.LogInformation("Creating rating for Booking: {BookingId}, Rating: {RatingValue}",
            request.BookingId, request.RatingValue);

        // Fetch the booking to get PetWalkerId and ClientId
        var bookingSpec = new BookingByIdSpecification(request.BookingId);
        var booking = await _bookingRepository.FirstOrDefaultAsync(bookingSpec, cancellationToken);

        if (booking == null)
        {
            _logger.LogWarning("Booking not found: {BookingId}", request.BookingId);
            return Result.NotFound("Booking not found.");
        }

        // Check that booking is completed - ratings only allowed for completed bookings
        if (booking.Status != BookingStatus.Completed)
        {
            _logger.LogWarning("Cannot rate booking that is not completed. Status: {Status}", booking.Status);
            return Result.Error("Ratings can only be submitted for completed bookings.");
        }

        // Check if rating already exists for this booking
        var existingRatingSpec = new GetRatingByBookingIdSpecification(request.BookingId);
        var existingRating = await _ratingRepository.FirstOrDefaultAsync(existingRatingSpec, cancellationToken);

        if (existingRating != null)
        {
            _logger.LogWarning("Rating already exists for Booking: {BookingId}", request.BookingId);
            return Result.Error("A rating has already been submitted for this booking.");
        }

        // Create rating with actual PetWalkerId and ClientId from the booking
        var rating = Core.RatingAggregate.Rating.Create(
            booking.PetWalkerId,
            booking.PetOwnerId,
            request.BookingId,
            request.RatingValue,
            request.Comment);

        await _ratingRepository.AddAsync(rating, cancellationToken);

        _logger.LogInformation("Rating created successfully: {RatingId} for Booking: {BookingId}, PetWalker: {PetWalkerId}, Client: {ClientId}",
            rating.Id, request.BookingId, booking.PetWalkerId, booking.PetOwnerId);

        return Result<Guid>.Success(rating.Id);
    }
}
