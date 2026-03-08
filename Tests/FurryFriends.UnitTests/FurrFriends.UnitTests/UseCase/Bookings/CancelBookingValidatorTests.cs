using FurryFriends.Core.BookingAggregate.Enums;
using FurryFriends.UseCases.Domain.Bookings.Command;
using FluentValidation.TestHelper;

namespace FurryFriends.UnitTests.UseCase.Bookings;

public class CancelBookingValidatorTests
{
  private readonly CancelBookingValidator _validator;

  public CancelBookingValidatorTests()
  {
    _validator = new CancelBookingValidator();
  }

  [Fact]
  public void Should_HaveError_WhenBookingIdIsEmpty()
  {
    // Arrange
    var command = new CancelBookingCommand(
      Guid.Empty,
      CancellationReason.ClientRequest,
      CancelledBy.Client);

    // Act
    var result = _validator.TestValidate(command);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.BookingId)
        .WithErrorMessage("Booking ID is required");
  }

  [Fact]
  public void Should_NotHaveError_WhenBookingIdIsValid()
  {
    // Arrange
    var command = new CancelBookingCommand(
      Guid.NewGuid(),
      CancellationReason.ClientRequest,
      CancelledBy.Client);

    // Act
    var result = _validator.TestValidate(command);

    // Assert
    result.ShouldNotHaveValidationErrorFor(x => x.BookingId);
  }

  [Fact]
  public void Should_HaveError_WhenReasonIsInvalid()
  {
    // Arrange
    var command = new CancelBookingCommand(
      Guid.NewGuid(),
      (CancellationReason)999, // Invalid enum value
      CancelledBy.Client);

    // Act
    var result = _validator.TestValidate(command);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Reason)
        .WithErrorMessage("Reason must be a valid cancellation reason");
  }

  [Fact]
  public void Should_NotHaveError_WhenReasonIsValid()
  {
    // Arrange
    var command = new CancelBookingCommand(
      Guid.NewGuid(),
      CancellationReason.ClientRequest,
      CancelledBy.Client);

    // Act
    var result = _validator.TestValidate(command);

    // Assert
    result.ShouldNotHaveValidationErrorFor(x => x.Reason);
  }

  [Fact]
  public void Should_HaveError_WhenCancelledByIsInvalid()
  {
    // Arrange
    var command = new CancelBookingCommand(
      Guid.NewGuid(),
      CancellationReason.ClientRequest,
      (CancelledBy)999); // Invalid enum value

    // Act
    var result = _validator.TestValidate(command);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.CancelledBy)
        .WithErrorMessage("CancelledBy must be either Client or PetWalker");
  }

  [Fact]
  public void Should_NotHaveError_WhenCancelledByIsValid()
  {
    // Arrange
    var command = new CancelBookingCommand(
      Guid.NewGuid(),
      CancellationReason.ClientRequest,
      CancelledBy.Client);

    // Act
    var result = _validator.TestValidate(command);

    // Assert
    result.ShouldNotHaveValidationErrorFor(x => x.CancelledBy);
  }

  [Fact]
  public void Should_HaveError_WhenAdditionalNotesExceedsMaximumLength()
  {
    // Arrange
    var longNotes = new string('a', 501); // 501 characters
    var command = new CancelBookingCommand(
      Guid.NewGuid(),
      CancellationReason.ClientRequest,
      CancelledBy.Client,
      longNotes);

    // Act
    var result = _validator.TestValidate(command);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.AdditionalNotes)
        .WithErrorMessage("Additional notes must not exceed 500 characters");
  }

  [Fact]
  public void Should_NotHaveError_WhenAdditionalNotesIsWithinLimit()
  {
    // Arrange
    var validNotes = new string('a', 500); // 500 characters
    var command = new CancelBookingCommand(
      Guid.NewGuid(),
      CancellationReason.ClientRequest,
      CancelledBy.Client,
      validNotes);

    // Act
    var result = _validator.TestValidate(command);

    // Assert
    result.ShouldNotHaveValidationErrorFor(x => x.AdditionalNotes);
  }

  [Fact]
  public void Should_NotHaveError_WhenAdditionalNotesIsNull()
  {
    // Arrange
    var command = new CancelBookingCommand(
      Guid.NewGuid(),
      CancellationReason.ClientRequest,
      CancelledBy.Client,
      null);

    // Act
    var result = _validator.TestValidate(command);

    // Assert
    result.ShouldNotHaveValidationErrorFor(x => x.AdditionalNotes);
  }

  [Fact]
  public void Should_NotHaveError_WhenAdditionalNotesIsEmpty()
  {
    // Arrange
    var command = new CancelBookingCommand(
      Guid.NewGuid(),
      CancellationReason.ClientRequest,
      CancelledBy.Client,
      string.Empty);

    // Act
    var result = _validator.TestValidate(command);

    // Assert
    result.ShouldNotHaveValidationErrorFor(x => x.AdditionalNotes);
  }
}