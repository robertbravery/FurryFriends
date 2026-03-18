using FurryFriends.UseCases.Timeslots.WorkingHours;
using FluentValidation.TestHelper;

namespace FurryFriends.UnitTests.UseCase.Timeslots.WorkingHours;

public class CreateWorkingHoursValidatorTests
{
    private readonly CreateWorkingHoursValidator _validator;

    public CreateWorkingHoursValidatorTests()
    {
        _validator = new CreateWorkingHoursValidator();
    }

    [Fact]
    public void Should_HaveError_WhenPetWalkerIdIsEmpty()
    {
        // Arrange
        var command = new CreateWorkingHoursCommand(
            Guid.Empty,
            DayOfWeek.Monday,
            TimeOnly.FromTimeSpan(TimeSpan.FromHours(9)),
            TimeOnly.FromTimeSpan(TimeSpan.FromHours(17)),
            true);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PetWalkerId)
            .WithErrorMessage("PetWalkerId is required");
    }

    [Fact]
    public void Should_NotHaveError_WhenPetWalkerIdIsValid()
    {
        // Arrange
        var command = new CreateWorkingHoursCommand(
            Guid.NewGuid(),
            DayOfWeek.Monday,
            TimeOnly.FromTimeSpan(TimeSpan.FromHours(9)),
            TimeOnly.FromTimeSpan(TimeSpan.FromHours(17)),
            true);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PetWalkerId);
    }

    [Fact]
    public void Should_HaveError_WhenDayOfWeekIsInvalid()
    {
        // Arrange
        var command = new CreateWorkingHoursCommand(
            Guid.NewGuid(),
            (DayOfWeek)999, // Invalid enum value
            TimeOnly.FromTimeSpan(TimeSpan.FromHours(9)),
            TimeOnly.FromTimeSpan(TimeSpan.FromHours(17)),
            true);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DayOfWeek)
            .WithErrorMessage("Day of week is required");
    }

    [Fact]
    public void Should_NotHaveError_WhenDayOfWeekIsValid()
    {
        // Arrange
        var command = new CreateWorkingHoursCommand(
            Guid.NewGuid(),
            DayOfWeek.Monday,
            TimeOnly.FromTimeSpan(TimeSpan.FromHours(9)),
            TimeOnly.FromTimeSpan(TimeSpan.FromHours(17)),
            true);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.DayOfWeek);
    }

    [Fact]
    public void Should_HaveError_WhenEndTimeIsBeforeStartTime()
    {
        // Arrange
        var command = new CreateWorkingHoursCommand(
            Guid.NewGuid(),
            DayOfWeek.Monday,
            TimeOnly.FromTimeSpan(TimeSpan.FromHours(17)),
            TimeOnly.FromTimeSpan(TimeSpan.FromHours(9)),
            true);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("End time must be after start time");
    }

    [Fact]
    public void Should_NotHaveError_WhenEndTimeIsAfterStartTime()
    {
        // Arrange
        var command = new CreateWorkingHoursCommand(
            Guid.NewGuid(),
            DayOfWeek.Monday,
            TimeOnly.FromTimeSpan(TimeSpan.FromHours(9)),
            TimeOnly.FromTimeSpan(TimeSpan.FromHours(17)),
            true);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x);
    }
}
