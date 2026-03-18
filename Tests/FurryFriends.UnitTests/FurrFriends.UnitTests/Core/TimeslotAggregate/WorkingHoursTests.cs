using Bogus;
using FurryFriends.Core.Entities;
using FurryFriends.Core.Enums;

namespace FurryFriends.UnitTests.Core.TimeslotAggregate;

public class WorkingHoursTests
{
    private readonly Faker _faker;

    public WorkingHoursTests()
    {
        _faker = new Faker();
    }

    [Fact]
    public void Create_WorkingHours_WithValidData_ReturnsValidWorkingHours()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var dayOfWeek = DayOfWeek.Monday;
        var startTime = new TimeOnly(8, 0);
        var endTime = new TimeOnly(18, 0);

        // Act
        var result = WorkingHours.Create(petWalkerId, dayOfWeek, startTime, endTime);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeOfType<WorkingHours>();
        result.Value.PetWalkerId.Should().Be(petWalkerId);
        result.Value.DayOfWeek.Should().Be(dayOfWeek);
        result.Value.StartTime.Should().Be(startTime);
        result.Value.EndTime.Should().Be(endTime);
    }

    [Fact]
    public void Create_WorkingHours_IsActiveDefaultValue_IsTrue()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var dayOfWeek = DayOfWeek.Monday;
        var startTime = new TimeOnly(8, 0);
        var endTime = new TimeOnly(18, 0);

        // Act
        var result = WorkingHours.Create(petWalkerId, dayOfWeek, startTime, endTime);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Create_WorkingHours_WithIsActiveFalse_ReturnsValidWorkingHours()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var dayOfWeek = DayOfWeek.Monday;
        var startTime = new TimeOnly(8, 0);
        var endTime = new TimeOnly(18, 0);

        // Act
        var result = WorkingHours.Create(petWalkerId, dayOfWeek, startTime, endTime, isActive: false);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Create_WorkingHours_EndTimeGreaterThanStartTime_ReturnsValidWorkingHours()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var dayOfWeek = DayOfWeek.Monday;
        var startTime = new TimeOnly(9, 0);
        var endTime = new TimeOnly(17, 0);

        // Act
        var result = WorkingHours.Create(petWalkerId, dayOfWeek, startTime, endTime);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Create_WorkingHours_EndTimeEqualsStartTime_ReturnsError()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var dayOfWeek = DayOfWeek.Monday;
        var startTime = new TimeOnly(9, 0);
        var endTime = new TimeOnly(9, 0);

        // Act
        var result = WorkingHours.Create(petWalkerId, dayOfWeek, startTime, endTime);

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void Create_WorkingHours_EndTimeLessThanStartTime_ReturnsError()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var dayOfWeek = DayOfWeek.Monday;
        var startTime = new TimeOnly(18, 0);
        var endTime = new TimeOnly(9, 0);

        // Act
        var result = WorkingHours.Create(petWalkerId, dayOfWeek, startTime, endTime);

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void Create_WorkingHours_ForAllDaysOfWeek_ReturnsValidWorkingHours()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var startTime = new TimeOnly(8, 0);
        var endTime = new TimeOnly(18, 0);

        // Act & Assert - Test all days of the week
        foreach (DayOfWeek day in Enum.GetValues<DayOfWeek>())
        {
            var result = WorkingHours.Create(petWalkerId, day, startTime, endTime);
            result.IsSuccess.Should().BeTrue($"WorkingHours should be creatable for {day}");
        }
    }

    [Fact]
    public void Create_WorkingHours_WithFullDay_ReturnsValidWorkingHours()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var dayOfWeek = DayOfWeek.Wednesday;
        var startTime = new TimeOnly(6, 0);
        var endTime = new TimeOnly(22, 0);

        // Act
        var result = WorkingHours.Create(petWalkerId, dayOfWeek, startTime, endTime);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void SetActive_WorkingHours_ChangesIsActive()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var dayOfWeek = DayOfWeek.Monday;
        var startTime = new TimeOnly(8, 0);
        var endTime = new TimeOnly(18, 0);

        var workingHours = WorkingHours.Create(petWalkerId, dayOfWeek, startTime, endTime).Value;

        // Act
        workingHours.SetActive(false);

        // Assert
        workingHours.IsActive.Should().BeFalse();
    }

    [Fact]
    public void SetInactive_WorkingHours_ChangesIsActiveToFalse()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var dayOfWeek = DayOfWeek.Monday;
        var startTime = new TimeOnly(8, 0);
        var endTime = new TimeOnly(18, 0);

        var workingHours = WorkingHours.Create(petWalkerId, dayOfWeek, startTime, endTime).Value;

        // Act
        workingHours.SetInactive();

        // Assert
        workingHours.IsActive.Should().BeFalse();
    }

    [Fact]
    public void SetActive_WorkingHours_ChangesIsActiveToTrue()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var dayOfWeek = DayOfWeek.Monday;
        var startTime = new TimeOnly(8, 0);
        var endTime = new TimeOnly(18, 0);

        var workingHours = WorkingHours.Create(petWalkerId, dayOfWeek, startTime, endTime, isActive: false).Value;

        // Act
        workingHours.SetActive(true);

        // Assert
        workingHours.IsActive.Should().BeTrue();
    }
}
