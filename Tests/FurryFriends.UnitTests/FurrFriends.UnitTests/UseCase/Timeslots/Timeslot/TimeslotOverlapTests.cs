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

public class TimeslotOverlapTests
{
    private readonly Faker _faker;
    private readonly Mock<IRepository<TimeslotEntity>> _timeslotRepositoryMock;
    private readonly Mock<IRepository<PetWalker>> _petWalkerRepositoryMock;
    private readonly Mock<ILogger<CreateTimeslotHandler>> _loggerMock;
    private readonly CreateTimeslotHandler _handler;

    public TimeslotOverlapTests()
    {
        _faker = new Faker();
        _timeslotRepositoryMock = new Mock<IRepository<TimeslotEntity>>();
        _petWalkerRepositoryMock = new Mock<IRepository<PetWalker>>();
        _loggerMock = new Mock<ILogger<CreateTimeslotHandler>>();
        _handler = new CreateTimeslotHandler(
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

    private async Task SetupPetWalkerSchedule(Guid petWalkerId, DayOfWeek dayOfWeek)
    {
        var petWalkerWithSchedule = await CreatePetWalkerWithScheduleAsync(petWalkerId, dayOfWeek, new TimeOnly(8, 0), new TimeOnly(18, 0));

        _petWalkerRepositoryMock
            .Setup(x => x.FirstOrDefaultAsync(It.IsAny<GetPetWalkerByIdSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(petWalkerWithSchedule);
    }

    [Fact]
    public async Task Handle_ExactSameTimeslot_ReturnsError()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var startTime = new TimeOnly(10, 0);
        var durationInMinutes = 30;

        var command = new CreateTimeslotCommand(
            petWalkerId,
            date,
            startTime,
            durationInMinutes);

        await SetupPetWalkerSchedule(petWalkerId, date.DayOfWeek);

        // Mock existing timeslot with exact same time
        var existingTimeslot = TimeslotEntity.Create(
            petWalkerId,
            date,
            startTime,
            durationInMinutes,
            TimeslotStatus.Available).Value;

        _timeslotRepositoryMock
            .Setup(x => x.ListAsync(It.IsAny<OverlappingTimeslotsSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TimeslotEntity> { existingTimeslot });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Timeslot overlaps with an existing timeslot.");
    }

    [Fact]
    public async Task Handle_NewTimeslotStartsInsideExisting_ReturnsError()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var startTime = new TimeOnly(9, 45); // Starts inside existing (9:30-10:00)
        var durationInMinutes = 30;

        var command = new CreateTimeslotCommand(
            petWalkerId,
            date,
            startTime,
            durationInMinutes);

        await SetupPetWalkerSchedule(petWalkerId, date.DayOfWeek);

        // Mock existing timeslot from 9:30 to 10:00
        var existingTimeslot = TimeslotEntity.Create(
            petWalkerId,
            date,
            new TimeOnly(9, 30),
            30,
            TimeslotStatus.Available).Value;

        _timeslotRepositoryMock
            .Setup(x => x.ListAsync(It.IsAny<OverlappingTimeslotsSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TimeslotEntity> { existingTimeslot });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Timeslot overlaps with an existing timeslot.");
    }

    [Fact]
    public async Task Handle_NewTimeslotEndsInsideExisting_ReturnsError()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var startTime = new TimeOnly(9, 0);
        var durationInMinutes = 45; // Ends at 9:45, overlapping with 9:30-10:00

        var command = new CreateTimeslotCommand(
            petWalkerId,
            date,
            startTime,
            durationInMinutes);

        await SetupPetWalkerSchedule(petWalkerId, date.DayOfWeek);

        // Mock existing timeslot from 9:30 to 10:00
        var existingTimeslot = TimeslotEntity.Create(
            petWalkerId,
            date,
            new TimeOnly(9, 30),
            30,
            TimeslotStatus.Available).Value;

        _timeslotRepositoryMock
            .Setup(x => x.ListAsync(It.IsAny<OverlappingTimeslotsSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TimeslotEntity> { existingTimeslot });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Timeslot overlaps with an existing timeslot.");
    }

    [Fact]
    public async Task Handle_NewTimeslotCompletelyContainsExisting_ReturnsError()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var startTime = new TimeOnly(9, 0);
        var durationInMinutes = 45; // 9:00-9:45, contains 9:15-9:45

        var command = new CreateTimeslotCommand(
            petWalkerId,
            date,
            startTime,
            durationInMinutes);

        await SetupPetWalkerSchedule(petWalkerId, date.DayOfWeek);

        // Mock existing timeslot from 9:15 to 9:45
        var existingTimeslot = TimeslotEntity.Create(
            petWalkerId,
            date,
            new TimeOnly(9, 15),
            30,
            TimeslotStatus.Available).Value;

        _timeslotRepositoryMock
            .Setup(x => x.ListAsync(It.IsAny<OverlappingTimeslotsSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TimeslotEntity> { existingTimeslot });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Timeslot overlaps with an existing timeslot.");
    }

    [Fact]
    public async Task Handle_NewTimeslotImmediatelyAfterExisting_CreatesSuccessfully()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var startTime = new TimeOnly(10, 0); // Immediately after existing (9:30-10:00)
        var durationInMinutes = 30;

        var command = new CreateTimeslotCommand(
            petWalkerId,
            date,
            startTime,
            durationInMinutes);

        await SetupPetWalkerSchedule(petWalkerId, date.DayOfWeek);

        // Mock existing timeslot from 9:30 to 10:00
        var existingTimeslot = TimeslotEntity.Create(
            petWalkerId,
            date,
            new TimeOnly(9, 30),
            30,
            TimeslotStatus.Available).Value;

        _timeslotRepositoryMock
            .Setup(x => x.ListAsync(It.IsAny<OverlappingTimeslotsSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TimeslotEntity> { existingTimeslot });

        // Mock add
        _timeslotRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<TimeslotEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TimeslotEntity t, CancellationToken _) => t);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_NewTimeslotImmediatelyBeforeExisting_CreatesSuccessfully()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var startTime = new TimeOnly(9, 0); // Immediately before existing (9:30-10:00)
        var durationInMinutes = 30;

        var command = new CreateTimeslotCommand(
            petWalkerId,
            date,
            startTime,
            durationInMinutes);

        await SetupPetWalkerSchedule(petWalkerId, date.DayOfWeek);

        // Mock existing timeslot from 9:30 to 10:00
        var existingTimeslot = TimeslotEntity.Create(
            petWalkerId,
            date,
            new TimeOnly(9, 30),
            30,
            TimeslotStatus.Available).Value;

        _timeslotRepositoryMock
            .Setup(x => x.ListAsync(It.IsAny<OverlappingTimeslotsSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TimeslotEntity> { existingTimeslot });

        // Mock add
        _timeslotRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<TimeslotEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TimeslotEntity t, CancellationToken _) => t);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_OverlappingWithCancelledTimeslot_CreatesSuccessfully()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var startTime = new TimeOnly(10, 0);
        var durationInMinutes = 30;

        var command = new CreateTimeslotCommand(
            petWalkerId,
            date,
            startTime,
            durationInMinutes);

        await SetupPetWalkerSchedule(petWalkerId, date.DayOfWeek);

        // Mock cancelled timeslot (should not block new timeslot)
        var cancelledTimeslot = TimeslotEntity.Create(
            petWalkerId,
            date,
            new TimeOnly(9, 30),
            30,
            TimeslotStatus.Cancelled).Value;

        _timeslotRepositoryMock
            .Setup(x => x.ListAsync(It.IsAny<OverlappingTimeslotsSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TimeslotEntity> { cancelledTimeslot });

        // Mock add
        _timeslotRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<TimeslotEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TimeslotEntity t, CancellationToken _) => t);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_MultipleOverlappingTimeslots_ReturnsError()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var startTime = new TimeOnly(10, 0);
        var durationInMinutes = 30;

        var command = new CreateTimeslotCommand(
            petWalkerId,
            date,
            startTime,
            durationInMinutes);

        await SetupPetWalkerSchedule(petWalkerId, date.DayOfWeek);

        // Mock multiple existing timeslots
        var existingTimeslot1 = TimeslotEntity.Create(
            petWalkerId,
            date,
            new TimeOnly(9, 30),
            30,
            TimeslotStatus.Available).Value;

        var existingTimeslot2 = TimeslotEntity.Create(
            petWalkerId,
            date,
            new TimeOnly(10, 30),
            30,
            TimeslotStatus.Available).Value;

        _timeslotRepositoryMock
            .Setup(x => x.ListAsync(It.IsAny<OverlappingTimeslotsSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TimeslotEntity> { existingTimeslot1, existingTimeslot2 });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Timeslot overlaps with an existing timeslot.");
    }
}
