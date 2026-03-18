using FurryFriends.Core.TimeslotAggregate.Specifications;
using FurryFriends.UseCases.Timeslots.CustomTimeRequest;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using CustomTimeRequestEntity = FurryFriends.Core.TimeslotAggregate.CustomTimeRequest;

namespace FurryFriends.UnitTests.UseCase.Timeslots.CustomTimeRequest;

public class RespondToCustomTimeRequestTests
{
    private readonly Mock<IRepository<CustomTimeRequestEntity>> _customTimeRequestRepositoryMock;
    private readonly Mock<IRepository<FurryFriends.Core.BookingAggregate.Booking>> _bookingRepositoryMock;
    private readonly Mock<ILogger<RespondToCustomTimeRequestHandler>> _loggerMock;
    private readonly RespondToCustomTimeRequestHandler _handler;

    public RespondToCustomTimeRequestTests()
    {
        _customTimeRequestRepositoryMock = new Mock<IRepository<CustomTimeRequestEntity>>();
        _bookingRepositoryMock = new Mock<IRepository<FurryFriends.Core.BookingAggregate.Booking>>();
        _loggerMock = new Mock<ILogger<RespondToCustomTimeRequestHandler>>();

        _handler = new RespondToCustomTimeRequestHandler(
            _customTimeRequestRepositoryMock.Object,
            _bookingRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_RequestNotFound_ReturnsError()
    {
        // Arrange
        var requestId = Guid.NewGuid();

        _customTimeRequestRepositoryMock
            .Setup(x => x.FirstOrDefaultAsync(It.IsAny<CustomTimeRequestByIdSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CustomTimeRequestEntity?)null);

        var command = new RespondToCustomTimeRequestCommand(
            requestId,
            CustomTimeRequestResponse.Accept,
            null,
            null,
            null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Errors.First());
    }

    [Fact]
    public async Task Handle_AcceptValidRequest_ReturnsSuccess()
    {
        // Arrange
        var requestId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        var petWalkerId = Guid.NewGuid();
        var requestedDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var preferredStartTime = new TimeOnly(10, 0);
        var duration = 30;
        var clientAddress = "123 Main St, Johannesburg, Gauteng, 2001";

        var customTimeRequest = CustomTimeRequestEntity.Create(
            clientId,
            petWalkerId,
            requestedDate,
            preferredStartTime,
            duration,
            clientAddress).Value;

        _customTimeRequestRepositoryMock
            .Setup(x => x.FirstOrDefaultAsync(It.IsAny<CustomTimeRequestByIdSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(customTimeRequest);

        _customTimeRequestRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<CustomTimeRequestEntity>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _bookingRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<FurryFriends.Core.BookingAggregate.Booking>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((FurryFriends.Core.BookingAggregate.Booking b, CancellationToken _) => b);

        var command = new RespondToCustomTimeRequestCommand(
            requestId,
            CustomTimeRequestResponse.Accept,
            null,
            null,
            null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Accepted", result.Value.Status);
    }

    [Fact]
    public async Task Handle_DeclineValidRequest_ReturnsSuccess()
    {
        // Arrange
        var requestId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        var petWalkerId = Guid.NewGuid();
        var requestedDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var preferredStartTime = new TimeOnly(10, 0);
        var duration = 30;
        var clientAddress = "123 Main St, Johannesburg, Gauteng, 2001";

        var customTimeRequest = CustomTimeRequestEntity.Create(
            clientId,
            petWalkerId,
            requestedDate,
            preferredStartTime,
            duration,
            clientAddress).Value;

        _customTimeRequestRepositoryMock
            .Setup(x => x.FirstOrDefaultAsync(It.IsAny<CustomTimeRequestByIdSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(customTimeRequest);

        _customTimeRequestRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<CustomTimeRequestEntity>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var command = new RespondToCustomTimeRequestCommand(
            requestId,
            CustomTimeRequestResponse.Decline,
            null,
            null,
            "Not available");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Declined", result.Value.Status);
    }

    [Fact]
    public async Task Handle_CounterOfferValidRequest_ReturnsSuccess()
    {
        // Arrange
        var requestId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        var petWalkerId = Guid.NewGuid();
        var requestedDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var preferredStartTime = new TimeOnly(10, 0);
        var duration = 30;
        var clientAddress = "123 Main St, Johannesburg, Gauteng, 2001";

        var customTimeRequest = CustomTimeRequestEntity.Create(
            clientId,
            petWalkerId,
            requestedDate,
            preferredStartTime,
            duration,
            clientAddress).Value;

        _customTimeRequestRepositoryMock
            .Setup(x => x.FirstOrDefaultAsync(It.IsAny<CustomTimeRequestByIdSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(customTimeRequest);

        _customTimeRequestRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<CustomTimeRequestEntity>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var command = new RespondToCustomTimeRequestCommand(
            requestId,
            CustomTimeRequestResponse.CounterOffer,
            DateOnly.FromDateTime(DateTime.Today.AddDays(2)),
            new TimeOnly(14, 0),
            "Better time available");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("CounterOffered", result.Value.Status);
    }
}
