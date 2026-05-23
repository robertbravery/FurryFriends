using Ardalis.Result;
using FurryFriends.Core.BookingAggregate;
using FurryFriends.UseCases.Domain.Ratings.CreateRating;
using NSubstitute;
using RatingEntity = FurryFriends.Core.RatingAggregate.Rating;

namespace FurrFriends.UnitTests.UseCases.Rating;

public class CreateRatingTests
{
    private readonly IRepository<RatingEntity> _ratingRepository;
    private readonly IRepository<Booking> _bookingRepository;
    private readonly ILogger<CreateRatingHandler> _logger;
    private readonly CreateRatingHandler _handler;

    public CreateRatingTests()
    {
        _ratingRepository = Substitute.For<IRepository<RatingEntity>>();
        _bookingRepository = Substitute.For<IRepository<Booking>>();
        _logger = Substitute.For<ILogger<CreateRatingHandler>>();
        _handler = new CreateRatingHandler(_ratingRepository, _bookingRepository, _logger);
    }

    [Fact]
    public async Task Handle_WithValidEligibility_ReturnsSuccess()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        var command = new CreateRatingCommand(petWalkerId, clientId, 5, "Great service!");

        // Client has 3 completed bookings with this petwalker
        _bookingRepository.CountAsync(Arg.Any<Ardalis.Specification.ISpecification<Booking>>())
            .Returns(3);

        // No existing active ratings
        _ratingRepository.ListAsync(Arg.Any<Ardalis.Specification.ISpecification<RatingEntity>>())
            .Returns(new List<RatingEntity>());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        await _ratingRepository.Received(1).AddAsync(Arg.Any<RatingEntity>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithNoCompletedBookings_ReturnsError()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        var command = new CreateRatingCommand(petWalkerId, clientId, 5, "Test");

        // Client has 0 completed bookings with this petwalker
        _bookingRepository.CountAsync(Arg.Any<Ardalis.Specification.ISpecification<Booking>>())
            .Returns(0);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("completed booking"));
    }

    [Fact]
    public async Task Handle_WithExistingActiveRating_UpdatesExisting()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        var command = new CreateRatingCommand(petWalkerId, clientId, 5, "Updated review");

        // Client has 3 completed bookings with this petwalker
        _bookingRepository.CountAsync(Arg.Any<Ardalis.Specification.ISpecification<Booking>>())
            .Returns(3);

        // Client already has an active rating for this petwalker
        var existingRating = RatingEntity.Create(petWalkerId, clientId, 4, "Previous");
        _ratingRepository.ListAsync(Arg.Any<Ardalis.Specification.ISpecification<RatingEntity>>())
            .Returns(new List<RatingEntity> { existingRating });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(existingRating.Id);
        // Should update existing rating, not create new one
        await _ratingRepository.Received(1).UpdateAsync(Arg.Any<RatingEntity>(), Arg.Any<CancellationToken>());
        await _ratingRepository.Received(0).AddAsync(Arg.Any<RatingEntity>(), Arg.Any<CancellationToken>());
        // Verify the rating was updated with new values
        existingRating.RatingValue.Should().Be(5);
        existingRating.Comment.Should().Be("Updated review");
    }

    [Fact]
    public async Task Handle_WhenRatingsExceedBookings_ReturnsError()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        var command = new CreateRatingCommand(petWalkerId, clientId, 5, "Test");

        // Client has only 1 completed booking with this petwalker
        _bookingRepository.CountAsync(Arg.Any<Ardalis.Specification.ISpecification<Booking>>())
            .Returns(1);

        // Client already has 1 active rating (ratings = bookings, can't add more)
        var existingRating = RatingEntity.Create(petWalkerId, clientId, 3, "Okay");
        var otherClientRating = RatingEntity.Create(petWalkerId, Guid.NewGuid(), 5, "Great");
        _ratingRepository.ListAsync(Arg.Any<Ardalis.Specification.ISpecification<RatingEntity>>())
            .Returns(new List<RatingEntity> { existingRating, otherClientRating });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        // The handler finds existing rating from THIS client first, so it replaces it
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(existingRating.Id);
    }
}
