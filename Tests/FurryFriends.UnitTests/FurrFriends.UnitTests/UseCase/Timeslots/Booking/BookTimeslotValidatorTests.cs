using FurryFriends.UseCases.Timeslots.Booking;
using FluentValidation.TestHelper;
using Xunit;

namespace FurryFriends.UseCases.Timeslots.Booking.Tests;

public class BookTimeslotValidatorTests
{
    private readonly BookTimeslotValidator _validator;

    public BookTimeslotValidatorTests()
    {
        _validator = new BookTimeslotValidator();
    }

    [Fact]
    public void Validate_ValidCommand_PassesValidation()
    {
        // Arrange
        var command = new BookTimeslotCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "123 Main St, Johannesburg, Gauteng, 2001",
            new List<Guid> { Guid.NewGuid() });

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_MissingTimeslotId_FailsValidation()
    {
        // Arrange
        var command = new BookTimeslotCommand(
            Guid.Empty,
            Guid.NewGuid(),
            "123 Main St, Johannesburg, Gauteng, 2001",
            new List<Guid> { Guid.NewGuid() });

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TimeslotId);
    }

    [Fact]
    public void Validate_MissingClientId_FailsValidation()
    {
        // Arrange
        var command = new BookTimeslotCommand(
            Guid.NewGuid(),
            Guid.Empty,
            "123 Main St, Johannesburg, Gauteng, 2001",
            new List<Guid> { Guid.NewGuid() });

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ClientId);
    }

    [Fact]
    public void Validate_MissingClientAddress_FailsValidation()
    {
        // Arrange
        var command = new BookTimeslotCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "",
            new List<Guid> { Guid.NewGuid() });

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ClientAddress);
    }

    [Fact]
    public void Validate_ClientAddressTooLong_FailsValidation()
    {
        // Arrange
        var longAddress = new string('A', 501);
        var command = new BookTimeslotCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            longAddress,
            new List<Guid> { Guid.NewGuid() });

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ClientAddress);
    }

    [Fact]
    public void Validate_EmptyPetIds_FailsValidation()
    {
        // Arrange
        var command = new BookTimeslotCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "123 Main St, Johannesburg, Gauteng, 2001",
            new List<Guid>());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PetIds);
    }

    [Fact]
    public void Validate_TooManyPets_FailsValidation()
    {
        // Arrange
        var pets = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
        var command = new BookTimeslotCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "123 Main St, Johannesburg, Gauteng, 2001",
            pets);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PetIds);
    }
}
