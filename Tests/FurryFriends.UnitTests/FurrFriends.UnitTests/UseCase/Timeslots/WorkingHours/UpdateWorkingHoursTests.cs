using FurryFriends.Core.TimeslotAggregate;
using FurryFriends.UseCases.Timeslots.WorkingHours;
using Microsoft.Extensions.Logging;
using Moq;
using WorkingHoursEntity = FurryFriends.Core.TimeslotAggregate.WorkingHours;

namespace FurryFriends.UnitTests.UseCase.Timeslots.WorkingHours;

public class UpdateWorkingHoursTests
{
    private readonly Mock<IRepository<WorkingHoursEntity>> _mockWorkingHoursRepository;
    private readonly Mock<ILogger<UpdateWorkingHoursHandler>> _mockLogger;
    private readonly UpdateWorkingHoursHandler _handler;
    private readonly CancellationToken _ct;

    public UpdateWorkingHoursTests()
    {
        _mockWorkingHoursRepository = new Mock<IRepository<WorkingHoursEntity>>();
        _mockLogger = new Mock<ILogger<UpdateWorkingHoursHandler>>();
        _handler = new UpdateWorkingHoursHandler(_mockWorkingHoursRepository.Object, _mockLogger.Object);
        _ct = CancellationToken.None;
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenWorkingHoursAreUpdated()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var workingHoursId = Guid.NewGuid();
        
        // Use reflection to set the Id since the constructor generates a new one
        var existingWorkingHours = WorkingHoursEntity.Create(
            petWalkerId,
            DayOfWeek.Monday,
            TimeOnly.FromTimeSpan(TimeSpan.FromHours(9)),
            TimeOnly.FromTimeSpan(TimeSpan.FromHours(17)),
            true).Value;

        var command = new UpdateWorkingHoursCommand(
            existingWorkingHours.Id,
            TimeOnly.FromTimeSpan(TimeSpan.FromHours(8)),
            TimeOnly.FromTimeSpan(TimeSpan.FromHours(18)),
            true);

        _mockWorkingHoursRepository
            .Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<Ardalis.Specification.ISpecification<WorkingHoursEntity>>(), _ct))
            .ReturnsAsync(existingWorkingHours);

        _mockWorkingHoursRepository
            .Setup(repo => repo.ListAsync(It.IsAny<Ardalis.Specification.ISpecification<WorkingHoursEntity>>(), _ct))
            .ReturnsAsync(new List<WorkingHoursEntity>());

        _mockWorkingHoursRepository
            .Setup(repo => repo.DeleteAsync(existingWorkingHours, _ct))
            .Returns(Task.CompletedTask);

        _mockWorkingHoursRepository
            .Setup(repo => repo.AddAsync(It.IsAny<WorkingHoursEntity>(), _ct))
            .ReturnsAsync((WorkingHoursEntity w, CancellationToken ct) => w);

        // Act
        var result = await _handler.Handle(command, _ct);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _mockWorkingHoursRepository.Verify(repo => repo.DeleteAsync(existingWorkingHours, _ct), Times.Once);
        _mockWorkingHoursRepository.Verify(repo => repo.AddAsync(It.IsAny<WorkingHoursEntity>(), _ct), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenWorkingHoursDoNotExist()
    {
        // Arrange
        var workingHoursId = Guid.NewGuid();
        var command = new UpdateWorkingHoursCommand(
            workingHoursId,
            TimeOnly.FromTimeSpan(TimeSpan.FromHours(9)),
            TimeOnly.FromTimeSpan(TimeSpan.FromHours(17)),
            true);

        _mockWorkingHoursRepository
            .Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<Ardalis.Specification.ISpecification<WorkingHoursEntity>>(), _ct))
            .ReturnsAsync((WorkingHoursEntity?)null);

        // Act
        var result = await _handler.Handle(command, _ct);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Working hours not found");

        _mockWorkingHoursRepository.Verify(repo => repo.DeleteAsync(It.IsAny<WorkingHoursEntity>(), _ct), Times.Never);
        _mockWorkingHoursRepository.Verify(repo => repo.AddAsync(It.IsAny<WorkingHoursEntity>(), _ct), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenEndTimeIsBeforeStartTime()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var existingWorkingHours = WorkingHoursEntity.Create(
            petWalkerId,
            DayOfWeek.Monday,
            TimeOnly.FromTimeSpan(TimeSpan.FromHours(9)),
            TimeOnly.FromTimeSpan(TimeSpan.FromHours(17)),
            true).Value;

        var command = new UpdateWorkingHoursCommand(
            existingWorkingHours.Id,
            TimeOnly.FromTimeSpan(TimeSpan.FromHours(17)),
            TimeOnly.FromTimeSpan(TimeSpan.FromHours(9)),
            true);

        _mockWorkingHoursRepository
            .Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<Ardalis.Specification.ISpecification<WorkingHoursEntity>>(), _ct))
            .ReturnsAsync(existingWorkingHours);

        // Act
        var result = await _handler.Handle(command, _ct);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("End time must be after start time");

        _mockWorkingHoursRepository.Verify(repo => repo.DeleteAsync(It.IsAny<WorkingHoursEntity>(), _ct), Times.Never);
        _mockWorkingHoursRepository.Verify(repo => repo.AddAsync(It.IsAny<WorkingHoursEntity>(), _ct), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenRepositoryThrowsException()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var existingWorkingHours = WorkingHoursEntity.Create(
            petWalkerId,
            DayOfWeek.Monday,
            TimeOnly.FromTimeSpan(TimeSpan.FromHours(9)),
            TimeOnly.FromTimeSpan(TimeSpan.FromHours(17)),
            true).Value;

        var command = new UpdateWorkingHoursCommand(
            existingWorkingHours.Id,
            TimeOnly.FromTimeSpan(TimeSpan.FromHours(8)),
            TimeOnly.FromTimeSpan(TimeSpan.FromHours(18)),
            true);

        _mockWorkingHoursRepository
            .Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<Ardalis.Specification.ISpecification<WorkingHoursEntity>>(), _ct))
            .ReturnsAsync(existingWorkingHours);

        _mockWorkingHoursRepository
            .Setup(repo => repo.ListAsync(It.IsAny<Ardalis.Specification.ISpecification<WorkingHoursEntity>>(), _ct))
            .ReturnsAsync(new List<WorkingHoursEntity>());

        _mockWorkingHoursRepository
            .Setup(repo => repo.DeleteAsync(existingWorkingHours, _ct))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        // Act
        var result = await _handler.Handle(command, _ct);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Database error");

        _mockWorkingHoursRepository.Verify(repo => repo.DeleteAsync(existingWorkingHours, _ct), Times.Once);
    }
}
