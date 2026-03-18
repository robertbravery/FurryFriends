using FurryFriends.Core.TimeslotAggregate;
using FurryFriends.UseCases.Timeslots.WorkingHours;
using Microsoft.Extensions.Logging;
using Moq;
using WorkingHoursEntity = FurryFriends.Core.TimeslotAggregate.WorkingHours;

namespace FurryFriends.UnitTests.UseCase.Timeslots.WorkingHours;

public class DeleteWorkingHoursTests
{
    private readonly Mock<IRepository<WorkingHoursEntity>> _mockWorkingHoursRepository;
    private readonly Mock<ILogger<DeleteWorkingHoursHandler>> _mockLogger;
    private readonly DeleteWorkingHoursHandler _handler;
    private readonly CancellationToken _ct;

    public DeleteWorkingHoursTests()
    {
        _mockWorkingHoursRepository = new Mock<IRepository<WorkingHoursEntity>>();
        _mockLogger = new Mock<ILogger<DeleteWorkingHoursHandler>>();
        _handler = new DeleteWorkingHoursHandler(_mockWorkingHoursRepository.Object, _mockLogger.Object);
        _ct = CancellationToken.None;
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenWorkingHoursAreDeleted()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var workingHours = WorkingHoursEntity.Create(
            petWalkerId,
            DayOfWeek.Monday,
            TimeOnly.FromTimeSpan(TimeSpan.FromHours(9)),
            TimeOnly.FromTimeSpan(TimeSpan.FromHours(17)),
            true).Value;

        var command = new DeleteWorkingHoursCommand(workingHours.Id);

        _mockWorkingHoursRepository
            .Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<Ardalis.Specification.ISpecification<WorkingHoursEntity>>(), _ct))
            .ReturnsAsync(workingHours);

        _mockWorkingHoursRepository
            .Setup(repo => repo.DeleteAsync(workingHours, _ct))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, _ct);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();

        _mockWorkingHoursRepository.Verify(repo => repo.FirstOrDefaultAsync(It.IsAny<Ardalis.Specification.ISpecification<WorkingHoursEntity>>(), _ct), Times.Once);
        _mockWorkingHoursRepository.Verify(repo => repo.DeleteAsync(workingHours, _ct), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenWorkingHoursDoNotExist()
    {
        // Arrange
        var workingHoursId = Guid.NewGuid();
        var command = new DeleteWorkingHoursCommand(workingHoursId);

        _mockWorkingHoursRepository
            .Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<Ardalis.Specification.ISpecification<WorkingHoursEntity>>(), _ct))
            .ReturnsAsync((WorkingHoursEntity?)null);

        // Act
        var result = await _handler.Handle(command, _ct);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Working hours not found");

        _mockWorkingHoursRepository.Verify(repo => repo.FirstOrDefaultAsync(It.IsAny<Ardalis.Specification.ISpecification<WorkingHoursEntity>>(), _ct), Times.Once);
        _mockWorkingHoursRepository.Verify(repo => repo.DeleteAsync(It.IsAny<WorkingHoursEntity>(), _ct), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenRepositoryThrowsException()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var workingHours = WorkingHoursEntity.Create(
            petWalkerId,
            DayOfWeek.Monday,
            TimeOnly.FromTimeSpan(TimeSpan.FromHours(9)),
            TimeOnly.FromTimeSpan(TimeSpan.FromHours(17)),
            true).Value;

        var command = new DeleteWorkingHoursCommand(workingHours.Id);

        _mockWorkingHoursRepository
            .Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<Ardalis.Specification.ISpecification<WorkingHoursEntity>>(), _ct))
            .ReturnsAsync(workingHours);

        _mockWorkingHoursRepository
            .Setup(repo => repo.DeleteAsync(workingHours, _ct))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        // Act
        var result = await _handler.Handle(command, _ct);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Database error");

        _mockWorkingHoursRepository.Verify(repo => repo.FirstOrDefaultAsync(It.IsAny<Ardalis.Specification.ISpecification<WorkingHoursEntity>>(), _ct), Times.Once);
        _mockWorkingHoursRepository.Verify(repo => repo.DeleteAsync(workingHours, _ct), Times.Once);
    }
}
