using Bogus;
using FurryFriends.Core.Entities;
using FurryFriends.Core.Enums;
using FurryFriends.Core.ValueObjects;

namespace FurryFriends.UnitTests.Core.TimeslotAggregate;

public class TimeslotTests
{
    private readonly Faker _faker;

    public TimeslotTests()
    {
        _faker = new Faker();
    }

    [Fact]
    public void Create_Timeslot_WithValidData_ReturnsValidTimeslot()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var startTime = new TimeOnly(9, 0);
        var durationInMinutes = 30;
        var endTime = new TimeOnly(9, 30);

        // Act
        var result = Timeslot.Create(petWalkerId, date, startTime, durationInMinutes, TimeslotStatus.Available);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeOfType<Timeslot>();
        result.Value.PetWalkerId.Should().Be(petWalkerId);
        result.Value.Date.Should().Be(date);
        result.Value.StartTime.Should().Be(startTime);
        result.Value.DurationInMinutes.Should().Be(durationInMinutes);
        result.Value.EndTime.Should().Be(endTime);
        result.Value.Status.Should().Be(TimeslotStatus.Available);
    }

    [Fact]
    public void Create_Timeslot_StartTimePlusDuration_EqualsEndTime()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var startTime = new TimeOnly(10, 0);
        var durationInMinutes = 45;

        // Act
        var result = Timeslot.Create(petWalkerId, date, startTime, durationInMinutes, TimeslotStatus.Available);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.EndTime.Should().Be(new TimeOnly(10, 45));
    }

    [Fact]
    public void Create_Timeslot_WithDuration30Minutes_ReturnsValidTimeslot()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var startTime = new TimeOnly(9, 0);
        var durationInMinutes = 30;

        // Act
        var result = Timeslot.Create(petWalkerId, date, startTime, durationInMinutes, TimeslotStatus.Available);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.DurationInMinutes.Should().Be(30);
    }

    [Fact]
    public void Create_Timeslot_WithDuration45Minutes_ReturnsValidTimeslot()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var startTime = new TimeOnly(9, 0);
        var durationInMinutes = 45;

        // Act
        var result = Timeslot.Create(petWalkerId, date, startTime, durationInMinutes, TimeslotStatus.Available);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.DurationInMinutes.Should().Be(45);
    }

    [Fact]
    public void Create_Timeslot_WithDurationLessThan30_ReturnsError()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var startTime = new TimeOnly(9, 0);
        var durationInMinutes = 29;

        // Act
        var result = Timeslot.Create(petWalkerId, date, startTime, durationInMinutes, TimeslotStatus.Available);

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void Create_Timeslot_WithDurationGreaterThan45_ReturnsError()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var startTime = new TimeOnly(9, 0);
        var durationInMinutes = 46;

        // Act
        var result = Timeslot.Create(petWalkerId, date, startTime, durationInMinutes, TimeslotStatus.Available);

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void Create_Timeslot_WithPastDate_ReturnsError()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));
        var startTime = new TimeOnly(9, 0);
        var durationInMinutes = 30;

        // Act
        var result = Timeslot.Create(petWalkerId, date, startTime, durationInMinutes, TimeslotStatus.Available);

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void Create_Timeslot_WithTodayDate_ReturnsValidTimeslot()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Today);
        var startTime = new TimeOnly(DateTime.Now.Hour + 1, 0);
        var durationInMinutes = 30;

        // Act
        var result = Timeslot.Create(petWalkerId, date, startTime, durationInMinutes, TimeslotStatus.Available);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Create_Timeslot_WithFutureDate_ReturnsValidTimeslot()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Today.AddDays(7));
        var startTime = new TimeOnly(9, 0);
        var durationInMinutes = 30;

        // Act
        var result = Timeslot.Create(petWalkerId, date, startTime, durationInMinutes, TimeslotStatus.Available);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Create_Timeslot_WithBookedStatus_ReturnsValidTimeslot()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var startTime = new TimeOnly(9, 0);
        var durationInMinutes = 30;

        // Act
        var result = Timeslot.Create(petWalkerId, date, startTime, durationInMinutes, TimeslotStatus.Booked);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(TimeslotStatus.Booked);
    }

    [Fact]
    public void Create_Timeslot_WithUnavailableStatus_ReturnsValidTimeslot()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var startTime = new TimeOnly(9, 0);
        var durationInMinutes = 30;

        // Act
        var result = Timeslot.Create(petWalkerId, date, startTime, durationInMinutes, TimeslotStatus.Unavailable);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(TimeslotStatus.Unavailable);
    }

    [Fact]
    public void Create_Timeslot_WithCancelledStatus_ReturnsValidTimeslot()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var startTime = new TimeOnly(9, 0);
        var durationInMinutes = 30;

        // Act
        var result = Timeslot.Create(petWalkerId, date, startTime, durationInMinutes, TimeslotStatus.Cancelled);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(TimeslotStatus.Cancelled);
    }

    [Fact]
    public void UpdateStatus_Timeslot_ChangesStatus()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var startTime = new TimeOnly(9, 0);
        var durationInMinutes = 30;

        var timeslot = Timeslot.Create(petWalkerId, date, startTime, durationInMinutes, TimeslotStatus.Available).Value;

        // Act
        timeslot.UpdateStatus(TimeslotStatus.Booked);

        // Assert
        timeslot.Status.Should().Be(TimeslotStatus.Booked);
    }

    [Fact]
    public void Book_Timeslot_ChangesStatusToBooked()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var startTime = new TimeOnly(9, 0);
        var durationInMinutes = 30;

        var timeslot = Timeslot.Create(petWalkerId, date, startTime, durationInMinutes, TimeslotStatus.Available).Value;

        // Act
        timeslot.Book();

        // Assert
        timeslot.Status.Should().Be(TimeslotStatus.Booked);
    }

    [Fact]
    public void Cancel_Timeslot_ChangesStatusToCancelled()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var startTime = new TimeOnly(9, 0);
        var durationInMinutes = 30;

        var timeslot = Timeslot.Create(petWalkerId, date, startTime, durationInMinutes, TimeslotStatus.Available).Value;

        // Act
        timeslot.Cancel();

        // Assert
        timeslot.Status.Should().Be(TimeslotStatus.Cancelled);
    }

    [Fact]
    public void MakeUnavailable_Timeslot_ChangesStatusToUnavailable()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var startTime = new TimeOnly(9, 0);
        var durationInMinutes = 30;

        var timeslot = Timeslot.Create(petWalkerId, date, startTime, durationInMinutes, TimeslotStatus.Available).Value;

        // Act
        timeslot.MakeUnavailable();

        // Assert
        timeslot.Status.Should().Be(TimeslotStatus.Unavailable);
    }
}
