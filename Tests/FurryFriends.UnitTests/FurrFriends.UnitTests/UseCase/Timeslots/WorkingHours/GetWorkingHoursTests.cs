using FurryFriends.Core.TimeslotAggregate;
using FurryFriends.UseCases.Timeslots.WorkingHours;
using Microsoft.Extensions.Logging;
using Moq;
using WorkingHoursEntity = FurryFriends.Core.TimeslotAggregate.WorkingHours;

namespace FurryFriends.UnitTests.UseCase.Timeslots.WorkingHours;

public class GetWorkingHoursTests
{
    private readonly Mock<IRepository<WorkingHoursEntity>> _mockWorkingHoursRepository;
    private readonly Mock<ILogger<GetWorkingHoursHandler>> _mockLogger;
    private readonly GetWorkingHoursHandler _handler;
    private readonly CancellationToken _ct;

    public GetWorkingHoursTests()
    {
        _mockWorkingHoursRepository = new Mock<IRepository<WorkingHoursEntity>>();
        _mockLogger = new Mock<ILogger<GetWorkingHoursHandler>>();
        _handler = new GetWorkingHoursHandler(_mockWorkingHoursRepository.Object, _mockLogger.Object);
        _ct = CancellationToken.None;
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenWorkingHoursExist()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var query = new GetWorkingHoursQuery(petWalkerId);

        var workingHours = new List<WorkingHoursEntity>
        {
            WorkingHoursEntity.Create(
                petWalkerId,
                DayOfWeek.Monday,
                TimeOnly.FromTimeSpan(TimeSpan.FromHours(9)),
                TimeOnly.FromTimeSpan(TimeSpan.FromHours(17)),
                true).Value,
            WorkingHoursEntity.Create(
                petWalkerId,
                DayOfWeek.Tuesday,
                TimeOnly.FromTimeSpan(TimeSpan.FromHours(9)),
                TimeOnly.FromTimeSpan(TimeSpan.FromHours(17)),
                true).Value
        };

        _mockWorkingHoursRepository
            .Setup(repo => repo.ListAsync(It.IsAny<Ardalis.Specification.ISpecification<WorkingHoursEntity>>(), _ct))
            .ReturnsAsync(workingHours);

        // Act
        var result = await _handler.Handle(query, _ct);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);

        _mockWorkingHoursRepository.Verify(repo => repo.ListAsync(It.IsAny<Ardalis.Specification.ISpecification<WorkingHoursEntity>>(), _ct), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccessWithEmptyList_WhenNoWorkingHoursExist()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var query = new GetWorkingHoursQuery(petWalkerId);

        _mockWorkingHoursRepository
            .Setup(repo => repo.ListAsync(It.IsAny<Ardalis.Specification.ISpecification<WorkingHoursEntity>>(), _ct))
            .ReturnsAsync(new List<WorkingHoursEntity>());

        // Act
        var result = await _handler.Handle(query, _ct);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();

        _mockWorkingHoursRepository.Verify(repo => repo.ListAsync(It.IsAny<Ardalis.Specification.ISpecification<WorkingHoursEntity>>(), _ct), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenRepositoryThrowsException()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var query = new GetWorkingHoursQuery(petWalkerId);

        _mockWorkingHoursRepository
            .Setup(repo => repo.ListAsync(It.IsAny<Ardalis.Specification.ISpecification<WorkingHoursEntity>>(), _ct))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        // Act
        var result = await _handler.Handle(query, _ct);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Database error");

        _mockWorkingHoursRepository.Verify(repo => repo.ListAsync(It.IsAny<Ardalis.Specification.ISpecification<WorkingHoursEntity>>(), _ct), Times.Once);
    }
}
