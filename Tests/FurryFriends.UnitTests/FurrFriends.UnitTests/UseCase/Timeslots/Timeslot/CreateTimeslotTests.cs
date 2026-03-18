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

public class CreateTimeslotTests
{
    private readonly Faker _faker;
    private readonly Mock<IRepository<TimeslotEntity>> _timeslotRepositoryMock;
    private readonly Mock<IRepository<PetWalker>> _petWalkerRepositoryMock;
    private readonly Mock<ILogger<CreateTimeslotHandler>> _loggerMock;
    private readonly CreateTimeslotHandler _handler;

    public CreateTimeslotTests()
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
        // Create a test PetWalker using the static factory method
        var phoneNumberResult = await PhoneNumber.Create("027", "011-123-4567");
        var phoneNumber = phoneNumberResult.Value;
        
        var petWalker = PetWalker.Create(
            Name.Create("John", "Doe"),
            Email.Create("john@example.com"),
            phoneNumber,
            Address.Create("123 Main St", "Johannesburg", "Gauteng", "2001", "South Africa"));
        
        // Add a schedule
        var schedule = new Schedule(dayOfWeek, startTime, endTime);
        petWalker.AddSchedule(schedule);
        
        return petWalker;
    }

    [Fact]
    public async Task Handle_ValidRequest_CreatesTimeslotSuccessfully()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var startTime = new TimeOnly(9, 0);
        var durationInMinutes = 30;

        var command = new CreateTimeslotCommand(
            petWalkerId,
            date,
            startTime,
            durationInMinutes);

        // Mock pet walker with schedule that covers the requested time
        var petWalkerWithSchedule = await CreatePetWalkerWithScheduleAsync(petWalkerId, date.DayOfWeek, new TimeOnly(8, 0), new TimeOnly(18, 0));

        _petWalkerRepositoryMock
            .Setup(x => x.FirstOrDefaultAsync(It.IsAny<GetPetWalkerByIdSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(petWalkerWithSchedule);

        // Mock no overlapping timeslots
        _timeslotRepositoryMock
            .Setup(x => x.ListAsync(It.IsAny<OverlappingTimeslotsSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TimeslotEntity>());

        // Mock add
        TimeslotEntity? addedTimeslot = null;
        _timeslotRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<TimeslotEntity>(), It.IsAny<CancellationToken>()))
            .Callback<TimeslotEntity, CancellationToken>((t, _) => addedTimeslot = t)
            .ReturnsAsync((TimeslotEntity t, CancellationToken _) => t);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.PetWalkerId.Should().Be(petWalkerId);
        result.Value.Date.Should().Be(date);
        result.Value.StartTime.Should().Be(startTime);
        result.Value.DurationInMinutes.Should().Be(durationInMinutes);
    }

    [Fact]
    public async Task Handle_DurationBelow30_ReturnsError()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var startTime = new TimeOnly(9, 0);
        var durationInMinutes = 15; // Invalid - below 30

        var command = new CreateTimeslotCommand(
            petWalkerId,
            date,
            startTime,
            durationInMinutes);

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
        var startTime = new TimeOnly(9, 0);
        var durationInMinutes = 60; // Invalid - above 45

        var command = new CreateTimeslotCommand(
            petWalkerId,
            date,
            startTime,
            durationInMinutes);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("DurationInMinutes must be between 30 and 45 minutes.");
    }

    [Fact]
    public async Task Handle_TimeslotBeforeWorkingHours_ReturnsError()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var startTime = new TimeOnly(7, 0); // Before working hours (8:00)
        var durationInMinutes = 30;

        var command = new CreateTimeslotCommand(
            petWalkerId,
            date,
            startTime,
            durationInMinutes);

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
    public async Task Handle_TimeslotAfterWorkingHours_ReturnsError()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var startTime = new TimeOnly(17, 30); // After working hours (ends at 18:00)
        var durationInMinutes = 30;

        var command = new CreateTimeslotCommand(
            petWalkerId,
            date,
            startTime,
            durationInMinutes);

        // Mock pet walker with schedule that ends at 18:00
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
        var startTime = new TimeOnly(10, 0);
        var durationInMinutes = 30;

        var command = new CreateTimeslotCommand(
            petWalkerId,
            date,
            startTime,
            durationInMinutes);

        // Mock pet walker with schedule that covers the requested time
        var petWalkerWithSchedule = await CreatePetWalkerWithScheduleAsync(petWalkerId, date.DayOfWeek, new TimeOnly(8, 0), new TimeOnly(18, 0));

        _petWalkerRepositoryMock
            .Setup(x => x.FirstOrDefaultAsync(It.IsAny<GetPetWalkerByIdSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(petWalkerWithSchedule);

        // Mock overlapping timeslot exists
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
    public async Task Handle_NoWorkingHoursForDay_ReturnsError()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var startTime = new TimeOnly(9, 0);
        var durationInMinutes = 30;

        var command = new CreateTimeslotCommand(
            petWalkerId,
            date,
            startTime,
            durationInMinutes);

        // Mock pet walker with NO schedule for this day
        var differentDay = date.DayOfWeek == DayOfWeek.Monday ? DayOfWeek.Tuesday : DayOfWeek.Monday;
        var petWalkerWithoutSchedule = await CreatePetWalkerWithScheduleAsync(petWalkerId, differentDay, new TimeOnly(8, 0), new TimeOnly(18, 0));

        _petWalkerRepositoryMock
            .Setup(x => x.FirstOrDefaultAsync(It.IsAny<GetPetWalkerByIdSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(petWalkerWithoutSchedule);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Pet walker does not have working hours set for this day.");
    }

    [Fact]
    public async Task Handle_ValidDuration45_CreatesTimeslotSuccessfully()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var startTime = new TimeOnly(9, 0);
        var durationInMinutes = 45;

        var command = new CreateTimeslotCommand(
            petWalkerId,
            date,
            startTime,
            durationInMinutes);

        // Mock pet walker with schedule that covers the requested time
        var petWalkerWithSchedule = await CreatePetWalkerWithScheduleAsync(petWalkerId, date.DayOfWeek, new TimeOnly(8, 0), new TimeOnly(18, 0));

        _petWalkerRepositoryMock
            .Setup(x => x.FirstOrDefaultAsync(It.IsAny<GetPetWalkerByIdSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(petWalkerWithSchedule);

        // Mock no overlapping timeslots
        _timeslotRepositoryMock
            .Setup(x => x.ListAsync(It.IsAny<OverlappingTimeslotsSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TimeslotEntity>());

        // Mock add
        _timeslotRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<TimeslotEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TimeslotEntity t, CancellationToken _) => t);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.DurationInMinutes.Should().Be(45);
    }

    [Fact]
    public async Task Handle_TimeslotExactlyAtWorkingHoursStart_CreatesSuccessfully()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var startTime = new TimeOnly(8, 0); // Exactly at working hours start
        var durationInMinutes = 30;

        var command = new CreateTimeslotCommand(
            petWalkerId,
            date,
            startTime,
            durationInMinutes);

        // Mock pet walker with schedule that starts at 8:00
        var petWalkerWithSchedule = await CreatePetWalkerWithScheduleAsync(petWalkerId, date.DayOfWeek, new TimeOnly(8, 0), new TimeOnly(18, 0));

        _petWalkerRepositoryMock
            .Setup(x => x.FirstOrDefaultAsync(It.IsAny<GetPetWalkerByIdSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(petWalkerWithSchedule);

        // Mock no overlapping timeslots
        _timeslotRepositoryMock
            .Setup(x => x.ListAsync(It.IsAny<OverlappingTimeslotsSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TimeslotEntity>());

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
    public async Task Handle_TimeslotExactlyAtWorkingHoursEnd_CreatesSuccessfully()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var startTime = new TimeOnly(17, 30); // 30 min before end (ends at 18:00)
        var durationInMinutes = 30;

        var command = new CreateTimeslotCommand(
            petWalkerId,
            date,
            startTime,
            durationInMinutes);

        // Mock pet walker with schedule that ends at 18:00
        var petWalkerWithSchedule = await CreatePetWalkerWithScheduleAsync(petWalkerId, date.DayOfWeek, new TimeOnly(8, 0), new TimeOnly(18, 0));

        _petWalkerRepositoryMock
            .Setup(x => x.FirstOrDefaultAsync(It.IsAny<GetPetWalkerByIdSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(petWalkerWithSchedule);

        // Mock no overlapping timeslots
        _timeslotRepositoryMock
            .Setup(x => x.ListAsync(It.IsAny<OverlappingTimeslotsSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TimeslotEntity>());

        // Mock add
        _timeslotRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<TimeslotEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TimeslotEntity t, CancellationToken _) => t);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}
