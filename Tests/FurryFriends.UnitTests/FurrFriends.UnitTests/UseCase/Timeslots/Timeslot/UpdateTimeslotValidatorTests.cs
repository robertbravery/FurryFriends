using FurryFriends.UseCases.Timeslots.Timeslot;
using FluentValidation.TestHelper;

namespace FurryFriends.UnitTests.UseCases.Timeslots;

public class UpdateTimeslotValidatorTests
{
    private readonly UpdateTimeslotValidator _validator;

    public UpdateTimeslotValidatorTests()
    {
        _validator = new UpdateTimeslotValidator();
    }

    [Fact]
    public void Should_HaveError_When_TimeslotIdIsEmpty()
    {
        // Arrange
        var command = new UpdateTimeslotCommand(
            Guid.Empty,
            new TimeOnly(9, 0),
            30);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TimeslotId)
            .WithErrorMessage("TimeslotId is required.");
    }

    [Fact]
    public void Should_HaveError_When_StartTimeIsDefault()
    {
        // Arrange
        var command = new UpdateTimeslotCommand(
            Guid.NewGuid(),
            default,
            30);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.StartTime)
            .WithErrorMessage("StartTime is required.");
    }

    [Fact]
    public void Should_HaveError_When_DurationBelow30()
    {
        // Arrange
        var command = new UpdateTimeslotCommand(
            Guid.NewGuid(),
            new TimeOnly(9, 0),
            15);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DurationInMinutes)
            .WithErrorMessage("DurationInMinutes must be between 30 and 45 minutes.");
    }

    [Fact]
    public void Should_HaveError_When_DurationAbove45()
    {
        // Arrange
        var command = new UpdateTimeslotCommand(
            Guid.NewGuid(),
            new TimeOnly(9, 0),
            60);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DurationInMinutes)
            .WithErrorMessage("DurationInMinutes must be between 30 and 45 minutes.");
    }

    [Fact]
    public void Should_NotHaveError_When_ValidCommand()
    {
        // Arrange
        var command = new UpdateTimeslotCommand(
            Guid.NewGuid(),
            new TimeOnly(9, 0),
            30);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_NotHaveError_When_DurationIs30()
    {
        // Arrange
        var command = new UpdateTimeslotCommand(
            Guid.NewGuid(),
            new TimeOnly(9, 0),
            30);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.DurationInMinutes);
    }

    [Fact]
    public void Should_NotHaveError_When_DurationIs45()
    {
        // Arrange
        var command = new UpdateTimeslotCommand(
            Guid.NewGuid(),
            new TimeOnly(9, 0),
            45);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.DurationInMinutes);
    }
}
