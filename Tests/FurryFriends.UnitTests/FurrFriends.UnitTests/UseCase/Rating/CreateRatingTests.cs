using Ardalis.Result;
using FurryFriends.Core.BookingAggregate;
using FurryFriends.Core.BookingAggregate.Enums;
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
    public async Task Handle_WithValidBooking_ReturnsSuccess()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var petWalkerId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        var command = new CreateRatingCommand(bookingId, 5, "Great service!");

        var booking = CreateCompletedBooking(bookingId, petWalkerId, clientId);
        _bookingRepository.FirstOrDefaultAsync(Arg.Any<Ardalis.Specification.ISpecification<Booking>>())
            .Returns(booking);
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
    public async Task Handle_WithNonExistentBooking_ReturnsNotFound()
    {
        // Arrange
        var command = new CreateRatingCommand(Guid.NewGuid(), 5, "Test");
        _bookingRepository.FirstOrDefaultAsync(Arg.Any<Ardalis.Specification.ISpecification<Booking>>())
            .Returns((Booking?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.Should().Be(ResultStatus.NotFound);
        result.Errors.Should().Contain(e => e.Contains("Booking not found"));
    }

    [Fact]
    public async Task Handle_WithNonCompletedBooking_ReturnsError()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var petWalkerId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        var command = new CreateRatingCommand(bookingId, 5, "Test");

        var booking = CreateBookingWithStatus(bookingId, petWalkerId, clientId, BookingStatus.Confirmed);
        _bookingRepository.FirstOrDefaultAsync(Arg.Any<Ardalis.Specification.ISpecification<Booking>>())
            .Returns(booking);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("completed bookings"));
    }

    [Fact]
    public async Task Handle_WithExistingActiveRating_ReturnsError()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var petWalkerId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        var command = new CreateRatingCommand(bookingId, 5, "Test");

        var booking = CreateCompletedBooking(bookingId, petWalkerId, clientId);
        _bookingRepository.FirstOrDefaultAsync(Arg.Any<Ardalis.Specification.ISpecification<Booking>>())
            .Returns(booking);

        var existingRating = RatingEntity.Create(petWalkerId, clientId, 4, "Previous");
        _ratingRepository.ListAsync(Arg.Any<Ardalis.Specification.ISpecification<RatingEntity>>())
            .Returns(new List<RatingEntity> { existingRating });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("already been submitted"));
    }

    private static Booking CreateCompletedBooking(Guid bookingId, Guid petWalkerId, Guid clientId)
    {
        return CreateBookingWithStatus(bookingId, petWalkerId, clientId, BookingStatus.Completed);
    }

    private static Booking CreateBookingWithStatus(Guid bookingId, Guid petWalkerId, Guid clientId, BookingStatus status)
    {
        // Use internal parameterless constructor (accessible via InternalsVisibleTo)
        var booking = new Booking();
        booking.Id = bookingId;

        // Use reflection to set properties with private setters
        var type = typeof(Booking);
        type.GetProperty(nameof(Booking.PetWalkerId))?.SetValue(booking, petWalkerId);
        type.GetProperty(nameof(Booking.PetOwnerId))?.SetValue(booking, clientId);
        type.GetProperty(nameof(Booking.Status))?.SetValue(booking, status);

        return booking;
    }
}
