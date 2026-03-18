using Bogus;
using FurryFriends.Core.Enums;
using FurryFriends.Core.TimeslotAggregate;
using FurryFriends.Core.TimeslotAggregate.Specifications;
using FurryFriends.UseCases.Timeslots.Timeslot;
using Microsoft.Extensions.Logging;
using Moq;
using TimeslotEntity = FurryFriends.Core.TimeslotAggregate.Timeslot;

namespace FurryFriends.UnitTests.UseCases.Timeslots;

public class DeleteTimeslotTests
{
    private readonly Faker _faker;
    private readonly Mock<IRepository<TimeslotEntity>> _timeslotRepositoryMock;
    private readonly Mock<ILogger<DeleteTimeslotHandler>> _loggerMock;
    private readonly DeleteTimeslotHandler _handler;

    public DeleteTimeslotTests()
    {
        _faker = new Faker();
        _timeslotRepositoryMock = new Mock<IRepository<TimeslotEntity>>();
        _loggerMock = new Mock<ILogger<DeleteTimeslotHandler>>();
        _handler = new DeleteTimeslotHandler(
            _timeslotRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_DeletesAvailableTimeslotSuccessfully()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var timeslotId = Guid.NewGuid();

        var command = new DeleteTimeslotCommand(timeslotId);

        // Mock existing timeslot that is Available
        var existingTimeslot = TimeslotEntity.Create(
            petWalkerId,
            date,
            new TimeOnly(9, 0),
            30,
            TimeslotStatus.Available).Value;
        existingTimeslot.Id = timeslotId;

        _timeslotRepositoryMock
            .Setup(x => x.FirstOrDefaultAsync(It.IsAny<TimeslotByIdSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTimeslot);

        // Mock delete
        _timeslotRepositoryMock
            .Setup(x => x.DeleteAsync(It.IsAny<TimeslotEntity>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ValidRequest_DeletesCancelledTimeslotSuccessfully()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var timeslotId = Guid.NewGuid();

        var command = new DeleteTimeslotCommand(timeslotId);

        // Mock existing timeslot that is Cancelled
        var existingTimeslot = TimeslotEntity.Create(
            petWalkerId,
            date,
            new TimeOnly(9, 0),
            30,
            TimeslotStatus.Cancelled).Value;
        existingTimeslot.Id = timeslotId;

        _timeslotRepositoryMock
            .Setup(x => x.FirstOrDefaultAsync(It.IsAny<TimeslotByIdSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTimeslot);

        // Mock delete
        _timeslotRepositoryMock
            .Setup(x => x.DeleteAsync(It.IsAny<TimeslotEntity>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_TimeslotNotFound_ReturnsError()
    {
        // Arrange
        var timeslotId = Guid.NewGuid();

        var command = new DeleteTimeslotCommand(timeslotId);

        _timeslotRepositoryMock
            .Setup(x => x.FirstOrDefaultAsync(It.IsAny<TimeslotByIdSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TimeslotEntity?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(Ardalis.Result.ResultStatus.NotFound);
    }

    [Fact]
    public async Task Handle_BookedTimeslot_ReturnsError()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var timeslotId = Guid.NewGuid();

        var command = new DeleteTimeslotCommand(timeslotId);

        // Mock existing timeslot that is Booked (cannot be deleted)
        var existingTimeslot = TimeslotEntity.Create(
            petWalkerId,
            date,
            new TimeOnly(9, 0),
            30,
            TimeslotStatus.Booked).Value;
        existingTimeslot.Id = timeslotId;

        _timeslotRepositoryMock
            .Setup(x => x.FirstOrDefaultAsync(It.IsAny<TimeslotByIdSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTimeslot);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Only Available or Cancelled timeslots can be deleted.");
    }

    [Fact]
    public async Task Handle_UnavailableTimeslot_ReturnsError()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var timeslotId = Guid.NewGuid();

        var command = new DeleteTimeslotCommand(timeslotId);

        // Mock existing timeslot that is Unavailable (cannot be deleted)
        var existingTimeslot = TimeslotEntity.Create(
            petWalkerId,
            date,
            new TimeOnly(9, 0),
            30,
            TimeslotStatus.Unavailable).Value;
        existingTimeslot.Id = timeslotId;

        _timeslotRepositoryMock
            .Setup(x => x.FirstOrDefaultAsync(It.IsAny<TimeslotByIdSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTimeslot);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Only Available or Cancelled timeslots can be deleted.");
    }
}
