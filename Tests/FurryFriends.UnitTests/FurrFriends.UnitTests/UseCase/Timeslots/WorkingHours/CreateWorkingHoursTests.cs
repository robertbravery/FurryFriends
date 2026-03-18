using FurryFriends.Core.TimeslotAggregate;
using FurryFriends.UseCases.Timeslots.WorkingHours;
using Microsoft.Extensions.Logging;
using Moq;
using WorkingHoursEntity = FurryFriends.Core.TimeslotAggregate.WorkingHours;

namespace FurryFriends.UnitTests.UseCase.Timeslots.WorkingHours;

public class CreateWorkingHoursTests
{
    private readonly Mock<IRepository<WorkingHoursEntity>> _mockWorkingHoursRepository;
    private readonly Mock<ILogger<CreateWorkingHoursHandler>> _mockLogger;
    private readonly CreateWorkingHoursHandler _handler;
    private readonly CancellationToken _ct;

    public CreateWorkingHoursTests()
    {
        _mockWorkingHoursRepository = new Mock<IRepository<WorkingHoursEntity>>();
        _mockLogger = new Mock<ILogger<CreateWorkingHoursHandler>>();
        _handler = new CreateWorkingHoursHandler(_mockWorkingHoursRepository.Object, _mockLogger.Object);
        _ct = CancellationToken.None;
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenWorkingHoursAreCreated()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var command = new CreateWorkingHoursCommand(
            petWalkerId,
            DayOfWeek.Monday,
            TimeOnly.FromTimeSpan(TimeSpan.FromHours(9)),
            TimeOnly.FromTimeSpan(TimeSpan.FromHours(17)),
            true);

        _mockWorkingHoursRepository
            .Setup(repo => repo.AddAsync(It.IsAny<WorkingHoursEntity>(), _ct))
            .ReturnsAsync((WorkingHoursEntity w, CancellationToken ct) => w);

        _mockWorkingHoursRepository
            .Setup(repo => repo.ListAsync(It.IsAny<Ardalis.Specification.ISpecification<WorkingHoursEntity>>(), _ct))
            .ReturnsAsync(new List<WorkingHoursEntity>());

        // Act
        var result = await _handler.Handle(command, _ct);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().NotBe(Guid.Empty);

        _mockWorkingHoursRepository.Verify(repo => repo.AddAsync(It.IsAny<WorkingHoursEntity>(), _ct), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenEndTimeIsBeforeStartTime()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var command = new CreateWorkingHoursCommand(
            petWalkerId,
            DayOfWeek.Monday,
            TimeOnly.FromTimeSpan(TimeSpan.FromHours(17)),
            TimeOnly.FromTimeSpan(TimeSpan.FromHours(9)),
            true);

        // Act
        var result = await _handler.Handle(command, _ct);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("End time must be after start time");

        _mockWorkingHoursRepository.Verify(repo => repo.AddAsync(It.IsAny<WorkingHoursEntity>(), _ct), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenStartTimeEqualsEndTime()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var time = TimeOnly.FromTimeSpan(TimeSpan.FromHours(9));
        var command = new CreateWorkingHoursCommand(
            petWalkerId,
            DayOfWeek.Monday,
            time,
            time,
            true);

        // Act
        var result = await _handler.Handle(command, _ct);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("End time must be after start time");

        _mockWorkingHoursRepository.Verify(repo => repo.AddAsync(It.IsAny<WorkingHoursEntity>(), _ct), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenPetWalkerIdIsEmpty()
    {
        // Arrange
        var command = new CreateWorkingHoursCommand(
            Guid.Empty,
            DayOfWeek.Monday,
            TimeOnly.FromTimeSpan(TimeSpan.FromHours(9)),
            TimeOnly.FromTimeSpan(TimeSpan.FromHours(17)),
            true);

        // Act
        var result = await _handler.Handle(command, _ct);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("PetWalkerId is required");

        _mockWorkingHoursRepository.Verify(repo => repo.AddAsync(It.IsAny<WorkingHoursEntity>(), _ct), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenRepositoryThrowsException()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var command = new CreateWorkingHoursCommand(
            petWalkerId,
            DayOfWeek.Monday,
            TimeOnly.FromTimeSpan(TimeSpan.FromHours(9)),
            TimeOnly.FromTimeSpan(TimeSpan.FromHours(17)),
            true);

        _mockWorkingHoursRepository
            .Setup(repo => repo.AddAsync(It.IsAny<WorkingHoursEntity>(), _ct))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        _mockWorkingHoursRepository
            .Setup(repo => repo.ListAsync(It.IsAny<Ardalis.Specification.ISpecification<WorkingHoursEntity>>(), _ct))
            .ReturnsAsync(new List<WorkingHoursEntity>());

        // Act
        var result = await _handler.Handle(command, _ct);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Database error");

        _mockWorkingHoursRepository.Verify(repo => repo.AddAsync(It.IsAny<WorkingHoursEntity>(), _ct), Times.Once);
    }
}
