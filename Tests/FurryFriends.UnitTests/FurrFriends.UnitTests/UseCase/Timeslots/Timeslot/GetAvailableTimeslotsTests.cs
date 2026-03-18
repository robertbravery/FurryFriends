using FurryFriends.Core.Enums;
using FurryFriends.Core.PetWalkerAggregate;
using FurryFriends.Core.PetWalkerAggregate.Specifications;
using FurryFriends.Core.TimeslotAggregate;
using FurryFriends.Core.TimeslotAggregate.Specifications;
using FurryFriends.Core.ValueObjects;
using FurryFriends.UseCases.Timeslots.Timeslot;
using FurryFriends.UnitTests.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using TimeslotEntity = FurryFriends.Core.TimeslotAggregate.Timeslot;

namespace FurryFriends.UnitTests.UseCase.Timeslots.Timeslot;

public class GetAvailableTimeslotsTests
{
    private readonly Mock<IRepository<TimeslotEntity>> _timeslotRepositoryMock;
    private readonly Mock<IRepository<PetWalker>> _petWalkerRepositoryMock;
    private readonly Mock<ILogger<GetAvailableTimeslotsHandler>> _loggerMock;
    private readonly GetAvailableTimeslotsHandler _handler;
    private readonly CancellationToken _ct;

    public GetAvailableTimeslotsTests()
    {
        _timeslotRepositoryMock = new Mock<IRepository<TimeslotEntity>>();
        _petWalkerRepositoryMock = new Mock<IRepository<PetWalker>>();
        _loggerMock = new Mock<ILogger<GetAvailableTimeslotsHandler>>();
        _handler = new GetAvailableTimeslotsHandler(
            _timeslotRepositoryMock.Object,
            _petWalkerRepositoryMock.Object,
            _loggerMock.Object);
        _ct = CancellationToken.None;
    }

    private PetWalker CreateTestPetWalker()
    {
        var phoneNumberResult = PhoneNumber.Create("027", "011-123-4567").Result.Value;
        return PetWalker.Create(
            Name.Create("John", "Smith"),
            Email.Create("john.smith@example.com"),
            phoneNumberResult,
            Address.Create("123 Main St", "Seattle", "WA", "US", "98101"));
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenAvailableTimeslotsExist()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Today.AddDays(1));

        var query = new GetAvailableTimeslotsQuery(petWalkerId, date);

        // Mock pet walker exists
        var petWalker = CreateTestPetWalker();
        petWalker.Id = petWalkerId;

        _petWalkerRepositoryMock
            .Setup(x => x.FirstOrDefaultAsync(It.IsAny<GetPetWalkerByIdSpecification>(), _ct))
            .ReturnsAsync(petWalker);

        // Mock available timeslots - use future date to avoid validation issues
        var futureDate = DateOnly.FromDateTime(DateTime.Today.AddDays(7));
        var timeslots = new List<TimeslotEntity>
        {
            TimeslotEntity.Create(petWalkerId, futureDate, new TimeOnly(9, 0), 30, TimeslotStatus.Available).Value,
            TimeslotEntity.Create(petWalkerId, futureDate, new TimeOnly(10, 0), 30, TimeslotStatus.Available).Value
        };

        _timeslotRepositoryMock
            .Setup(x => x.ListAsync(It.IsAny<AvailableTimeslotsByPetWalkerAndDateSpec>(), _ct))
            .ReturnsAsync(timeslots);

        // Act
        var result = await _handler.Handle(query, _ct);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.PetWalkerId.Should().Be(petWalkerId);
        result.Value.Date.Should().Be(date);
        result.Value.Timeslots.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccessWithEmptyList_WhenNoAvailableTimeslotsExist()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Today.AddDays(1));

        var query = new GetAvailableTimeslotsQuery(petWalkerId, date);

        // Mock pet walker exists
        var petWalker = CreateTestPetWalker();
        petWalker.Id = petWalkerId;

        _petWalkerRepositoryMock
            .Setup(x => x.FirstOrDefaultAsync(It.IsAny<GetPetWalkerByIdSpecification>(), _ct))
            .ReturnsAsync(petWalker);

        _timeslotRepositoryMock
            .Setup(x => x.ListAsync(It.IsAny<AvailableTimeslotsByPetWalkerAndDateSpec>(), _ct))
            .ReturnsAsync(new List<TimeslotEntity>());

        // Act
        var result = await _handler.Handle(query, _ct);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Timeslots.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenPetWalkerDoesNotExist()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Today.AddDays(1));

        var query = new GetAvailableTimeslotsQuery(petWalkerId, date);

        _petWalkerRepositoryMock
            .Setup(x => x.FirstOrDefaultAsync(It.IsAny<GetPetWalkerByIdSpecification>(), _ct))
            .ReturnsAsync((PetWalker?)null);

        // Act
        var result = await _handler.Handle(query, _ct);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(Ardalis.Result.ResultStatus.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldFilterOutPastTimeslots_WhenDateIsToday()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var today = DateOnly.FromDateTime(DateTime.Today);

        var query = new GetAvailableTimeslotsQuery(petWalkerId, today);

        // Mock pet walker exists
        var petWalker = CreateTestPetWalker();
        petWalker.Id = petWalkerId;

        _petWalkerRepositoryMock
            .Setup(x => x.FirstOrDefaultAsync(It.IsAny<GetPetWalkerByIdSpecification>(), _ct))
            .ReturnsAsync(petWalker);

        // One past timeslot and one future timeslot - use a valid future date
        var futureDate = DateOnly.FromDateTime(DateTime.Today.AddDays(7));
        var timeslots = new List<TimeslotEntity>
        {
            TimeslotEntity.Create(petWalkerId, futureDate, new TimeOnly(8, 0), 30, TimeslotStatus.Available).Value,  // Past time
            TimeslotEntity.Create(petWalkerId, futureDate, new TimeOnly(23, 0), 30, TimeslotStatus.Available).Value   // Future time
        };

        _timeslotRepositoryMock
            .Setup(x => x.ListAsync(It.IsAny<AvailableTimeslotsByPetWalkerAndDateSpec>(), _ct))
            .ReturnsAsync(timeslots);

        // Act
        var result = await _handler.Handle(query, _ct);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        // Should only return the future timeslot (23:00 is in the future)
        result.Value.Timeslots.Should().HaveCount(1);
        result.Value.Timeslots[0].StartTime.Should().Be(new TimeOnly(23, 0));
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenRepositoryThrowsException()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Today.AddDays(1));

        var query = new GetAvailableTimeslotsQuery(petWalkerId, date);

        _petWalkerRepositoryMock
            .Setup(x => x.FirstOrDefaultAsync(It.IsAny<GetPetWalkerByIdSpecification>(), _ct))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        // Act
        var result = await _handler.Handle(query, _ct);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(Ardalis.Result.ResultStatus.Error);
    }
}
