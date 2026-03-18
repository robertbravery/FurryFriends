using FurryFriends.UseCases.Timeslots.Timeslot;
using FluentValidation.TestHelper;

namespace FurryFriends.UnitTests.UseCases.Timeslots;

public class CreateTimeslotValidatorTests
{
    private readonly CreateTimeslotValidator _validator;

    public CreateTimeslotValidatorTests()
    {
        _validator = new CreateTimeslotValidator();
    }

    [Fact]
    public void Validate_ValidRequest_PassesValidation()
    {
        // Arrange
        var command = new CreateTimeslotCommand(
            Guid.NewGuid(),
            DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            new TimeOnly(9, 0),
            30);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_InvalidPetWalkerId_FailsValidation()
    {
        // Arrange
        var command = new CreateTimeslotCommand(
            Guid.Empty,
            DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            new TimeOnly(9, 0),
            30);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PetWalkerId);
    }

    [Fact]
    public void Validate_PastDate_FailsValidation()
    {
        // Arrange
        var command = new CreateTimeslotCommand(
            Guid.NewGuid(),
            DateOnly.FromDateTime(DateTime.Today.AddDays(-1)),
            new TimeOnly(9, 0),
            30);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Date);
    }

    [Fact]
    public void Validate_DurationBelow30_FailsValidation()
    {
        // Arrange
        var command = new CreateTimeslotCommand(
            Guid.NewGuid(),
            DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            new TimeOnly(9, 0),
            15);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DurationInMinutes);
    }

    [Fact]
    public void Validate_DurationAbove45_FailsValidation()
    {
        // Arrange
        var command = new CreateTimeslotCommand(
            Guid.NewGuid(),
            DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            new TimeOnly(9, 0),
            60);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DurationInMinutes);
    }

    [Theory]
    [InlineData(30)]
    [InlineData(35)]
    [InlineData(40)]
    [InlineData(45)]
    public void Validate_ValidDurations_PassesValidation(int duration)
    {
        // Arrange
        var command = new CreateTimeslotCommand(
            Guid.NewGuid(),
            DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            new TimeOnly(9, 0),
            duration);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(29)]
    [InlineData(46)]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_InvalidDurations_FailsValidation(int duration)
    {
        // Arrange
        var command = new CreateTimeslotCommand(
            Guid.NewGuid(),
            DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            new TimeOnly(9, 0),
            duration);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DurationInMinutes);
    }

    [Fact]
    public void Validate_ValidStartTime_PassesValidation()
    {
        // Arrange
        var command = new CreateTimeslotCommand(
            Guid.NewGuid(),
            DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            new TimeOnly(8, 0),
            30);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_MidnightStartTime_PassesValidation()
    {
        // Arrange
        var command = new CreateTimeslotCommand(
            Guid.NewGuid(),
            DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            new TimeOnly(0, 0),
            30);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
