using Bogus;
using FurryFriends.Core.Enums;
using FurryFriends.Core.PetWalkerAggregate;
using FurryFriends.Core.PetWalkerAggregate.Specifications;
using FurryFriends.Core.TimeslotAggregate;
using FurryFriends.Core.TimeslotAggregate.Specifications;
using FurryFriends.Core.ValueObjects;
using FurryFriends.UseCases.Timeslots.Timeslot;
using Microsoft.Extensions.Logging;
using Moq;
using TimeslotEntity = FurryFriends.Core.TimeslotAggregate.Timeslot;
using WorkingHoursEntity = FurryFriends.Core.TimeslotAggregate.WorkingHours;

namespace FurryFriends.UnitTests.UseCases.Timeslots;

public class UpdateTimeslotTests
{
    private readonly Faker _faker;
    private readonly Mock<IRepository<TimeslotEntity>> _timeslotRepositoryMock;
    private readonly Mock<IRepository<PetWalker>> _petWalkerRepositoryMock;
    private readonly Mock<ILogger<UpdateTimeslotHandler>> _loggerMock;
    private readonly UpdateTimeslotHandler _handler;

    public UpdateTimeslotTests()
    {
        _faker = new Faker();
        _timeslotRepositoryMock = new Mock<IRepository<TimeslotEntity>>();
        _petWalkerRepositoryMock = new Mock<IRepository<PetWalker>>();
        _loggerMock = new Mock<ILogger<UpdateTimeslotHandler>>();
        _handler = new UpdateTimeslotHandler(
            _timeslotRepositoryMock.Object,
            _petWalkerRepositoryMock.Object,
            _loggerMock.Object);
    }

    private async Task<PetWalker> CreatePetWalkerWithScheduleAsync(Guid petWalkerId, DayOfWeek dayOfWeek, TimeOnly startTime, TimeOnly endTime)
    {
        var phoneNumberResult = await PhoneNumber.Create("027", "011-123-4567");
        var phoneNumber = phoneNumberResult.Value;
        
        var petWalker = PetWalker.Create(
            Name.Create("John", "Doe"),
            Email.Create("john@example.com"),
            phoneNumber,
            Address.Create("123 Main St", "Johannesburg", "Gauteng", "2001", "South Africa"));
        
        var schedule = new Schedule(dayOfWeek, startTime, endTime);
        petWalker.AddSchedule(schedule);
        
        return petWalker;
    }

    [Fact]
    public async Task Handle_ValidRequest_UpdatesTimeslotSuccessfully()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var timeslotId = Guid.NewGuid();
        var startTime = new TimeOnly(9, 0);
        var durationInMinutes = 30;

        var command = new UpdateTimeslotCommand(
            timeslotId,
            startTime,
            durationInMinutes);

        // Mock existing timeslot that is Available
        var existingTimeslot = TimeslotEntity.Create(
            petWalkerId,
            date,
            new TimeOnly(10, 0),
            30,
            TimeslotStatus.Available).Value;
        existingTimeslot.Id = timeslotId;

        _timeslotRepositoryMock
            .Setup(x => x.FirstOrDefaultAsync(It.IsAny<TimeslotByIdSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTimeslot);

        // Mock pet walker with schedule that covers the requested time
        var petWalkerWithSchedule = await CreatePetWalkerWithScheduleAsync(petWalkerId, date.DayOfWeek, new TimeOnly(8, 0), new TimeOnly(18, 0));

        _petWalkerRepositoryMock
            .Setup(x => x.FirstOrDefaultAsync(It.IsAny<GetPetWalkerByIdSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(petWalkerWithSchedule);

        // Mock no overlapping timeslots
        _timeslotRepositoryMock
            .Setup(x => x.ListAsync(It.IsAny<OverlappingTimeslotsSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TimeslotEntity>());

        // Mock update
        _timeslotRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<TimeslotEntity>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.StartTime.Should().Be(startTime);
        result.Value.DurationInMinutes.Should().Be(durationInMinutes);
    }

    [Fact]
    public async Task Handle_TimeslotNotAvailable_ReturnsError()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var timeslotId = Guid.NewGuid();
        var startTime = new TimeOnly(9, 0);
        var durationInMinutes = 30;

        var command = new UpdateTimeslotCommand(
            timeslotId,
            startTime,
            durationInMinutes);

        // Mock existing timeslot that is Booked (not Available)
        var existingTimeslot = TimeslotEntity.Create(
            petWalkerId,
            date,
            new TimeOnly(10, 0),
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
        result.Errors.Should().Contain("Only Available timeslots can be updated.");
    }

    [Fact]
    public async Task Handle_TimeslotNotFound_ReturnsError()
    {
        // Arrange
        var timeslotId = Guid.NewGuid();
        var startTime = new TimeOnly(9, 0);
        var durationInMinutes = 30;

        var command = new UpdateTimeslotCommand(
            timeslotId,
            startTime,
            durationInMinutes);

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
    public async Task Handle_TimeslotBeforeWorkingHours_ReturnsError()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var timeslotId = Guid.NewGuid();
        var startTime = new TimeOnly(7, 0); // Before working hours (8:00)
        var durationInMinutes = 30;

        var command = new UpdateTimeslotCommand(
            timeslotId,
            startTime,
            durationInMinutes);

        // Mock existing timeslot that is Available
        var existingTimeslot = TimeslotEntity.Create(
            petWalkerId,
            date,
            new TimeOnly(10, 0),
            30,
            TimeslotStatus.Available).Value;
        existingTimeslot.Id = timeslotId;

        _timeslotRepositoryMock
            .Setup(x => x.FirstOrDefaultAsync(It.IsAny<TimeslotByIdSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTimeslot);

        // Mock pet walker with schedule that starts at 8:00
        var petWalkerWithSchedule = await CreatePetWalkerWithScheduleAsync(petWalkerId, date.DayOfWeek, new TimeOnly(8, 0), new TimeOnly(18, 0));

        _petWalkerRepositoryMock
            .Setup(x => x.FirstOrDefaultAsync(It.IsAny<GetPetWalkerByIdSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(petWalkerWithSchedule);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Timeslot must be within working hours.");
    }

    [Fact]
    public async Task Handle_OverlappingTimeslot_ReturnsError()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var timeslotId = Guid.NewGuid();
        var startTime = new TimeOnly(10, 0);
        var durationInMinutes = 30;

        var command = new UpdateTimeslotCommand(
            timeslotId,
            startTime,
            durationInMinutes);

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

        // Mock pet walker with schedule that covers the requested time
        var petWalkerWithSchedule = await CreatePetWalkerWithScheduleAsync(petWalkerId, date.DayOfWeek, new TimeOnly(8, 0), new TimeOnly(18, 0));

        _petWalkerRepositoryMock
            .Setup(x => x.FirstOrDefaultAsync(It.IsAny<GetPetWalkerByIdSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(petWalkerWithSchedule);

        // Mock overlapping timeslot exists (different ID)
        var overlappingTimeslot = TimeslotEntity.Create(
            petWalkerId,
            date,
            new TimeOnly(9, 30),
            30,
            TimeslotStatus.Available).Value;
        overlappingTimeslot.Id = Guid.NewGuid();

        _timeslotRepositoryMock
            .Setup(x => x.ListAsync(It.IsAny<OverlappingTimeslotsSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TimeslotEntity> { overlappingTimeslot });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Timeslot overlaps with an existing timeslot.");
    }

    [Fact]
    public async Task Handle_DurationBelow30_ReturnsError()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var timeslotId = Guid.NewGuid();
        var startTime = new TimeOnly(9, 0);
        var durationInMinutes = 15; // Invalid - below 30

        var command = new UpdateTimeslotCommand(
            timeslotId,
            startTime,
            durationInMinutes);

        // Mock existing timeslot that is Available
        var existingTimeslot = TimeslotEntity.Create(
            petWalkerId,
            date,
            new TimeOnly(10, 0),
            30,
            TimeslotStatus.Available).Value;
        existingTimeslot.Id = timeslotId;

        _timeslotRepositoryMock
            .Setup(x => x.FirstOrDefaultAsync(It.IsAny<TimeslotByIdSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTimeslot);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("DurationInMinutes must be between 30 and 45 minutes.");
    }

    [Fact]
    public async Task Handle_DurationAbove45_ReturnsError()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var timeslotId = Guid.NewGuid();
        var startTime = new TimeOnly(9, 0);
        var durationInMinutes = 60; // Invalid - above 45

        var command = new UpdateTimeslotCommand(
            timeslotId,
            startTime,
            durationInMinutes);

        // Mock existing timeslot that is Available
        var existingTimeslot = TimeslotEntity.Create(
            petWalkerId,
            date,
            new TimeOnly(10, 0),
            30,
            TimeslotStatus.Available).Value;
        existingTimeslot.Id = timeslotId;

        _timeslotRepositoryMock
            .Setup(x => x.FirstOrDefaultAsync(It.IsAny<TimeslotByIdSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTimeslot);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("DurationInMinutes must be between 30 and 45 minutes.");
    }
}
