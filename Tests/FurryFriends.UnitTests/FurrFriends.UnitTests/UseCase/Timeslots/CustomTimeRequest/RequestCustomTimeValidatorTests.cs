using FurryFriends.UseCases.Timeslots.CustomTimeRequest;
using FluentValidation.TestHelper;
using Xunit;

namespace FurryFriends.UnitTests.UseCase.Timeslots.CustomTimeRequest;

public class RequestCustomTimeValidatorTests
{
    private readonly RequestCustomTimeValidator _validator;

    public RequestCustomTimeValidatorTests()
    {
        _validator = new RequestCustomTimeValidator();
    }

    [Fact]
    public void Validate_EmptyPetWalkerId_ReturnsError()
    {
        // Arrange
        var command = new RequestCustomTimeCommand(
            Guid.Empty,
            Guid.NewGuid(),
            DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            new TimeOnly(10, 0),
            30,
            "123 Main St",
            new List<Guid> { Guid.NewGuid() });

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PetWalkerId);
    }

    [Fact]
    public void Validate_EmptyClientId_ReturnsError()
    {
        // Arrange
        var command = new RequestCustomTimeCommand(
            Guid.NewGuid(),
            Guid.Empty,
            DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            new TimeOnly(10, 0),
            30,
            "123 Main St",
            new List<Guid> { Guid.NewGuid() });

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ClientId);
    }

    [Fact]
    public void Validate_PastDate_ReturnsError()
    {
        // Arrange
        var command = new RequestCustomTimeCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateOnly.FromDateTime(DateTime.Today.AddDays(-1)),
            new TimeOnly(10, 0),
            30,
            "123 Main St",
            new List<Guid> { Guid.NewGuid() });

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RequestedDate);
    }

    [Fact]
    public void Validate_DurationLessThan30_ReturnsError()
    {
        // Arrange
        var command = new RequestCustomTimeCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            new TimeOnly(10, 0),
            15,
            "123 Main St",
            new List<Guid> { Guid.NewGuid() });

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PreferredDurationMinutes);
    }

    [Fact]
    public void Validate_DurationGreaterThan45_ReturnsError()
    {
        // Arrange
        var command = new RequestCustomTimeCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            new TimeOnly(10, 0),
            60,
            "123 Main St",
            new List<Guid> { Guid.NewGuid() });

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PreferredDurationMinutes);
    }

    [Fact]
    public void Validate_EmptyPetIds_ReturnsError()
    {
        // Arrange
        var command = new RequestCustomTimeCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            new TimeOnly(10, 0),
            30,
            "123 Main St",
            new List<Guid>());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PetIds);
    }

    [Fact]
    public void Validate_ValidRequest_Passes()
    {
        // Arrange
        var command = new RequestCustomTimeCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            new TimeOnly(10, 0),
            30,
            "123 Main St, Johannesburg, Gauteng, 2001",
            new List<Guid> { Guid.NewGuid() });

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
