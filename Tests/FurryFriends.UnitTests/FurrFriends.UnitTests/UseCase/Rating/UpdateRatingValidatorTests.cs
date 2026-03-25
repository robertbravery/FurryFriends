using FluentAssertions;
using FluentValidation.TestHelper;
using FurryFriends.UseCases.Rating.UpdateRating;

namespace FurrFriends.UnitTests.UseCase.Rating;

public class UpdateRatingValidatorTests
{
    private readonly UpdateRatingValidator _validator;

    public UpdateRatingValidatorTests()
    {
        _validator = new UpdateRatingValidator();
    }

    [Fact]
    public void Should_NotHaveError_WhenValidRequest()
    {
        // Arrange
        var request = new UpdateRatingCommand(
            Guid.NewGuid(),
            4,
            "Updated comment"
        );

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.RatingId);
        result.ShouldNotHaveValidationErrorFor(x => x.RatingValue);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void Should_NotHaveError_WhenValidRatingValues(int ratingValue)
    {
        // Arrange
        var request = new UpdateRatingCommand(
            Guid.NewGuid(),
            ratingValue,
            "Test"
        );

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.RatingValue);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(6)]
    [InlineData(100)]
    public void Should_HaveError_WhenInvalidRatingValue(int invalidRatingValue)
    {
        // Arrange
        var request = new UpdateRatingCommand(
            Guid.NewGuid(),
            invalidRatingValue,
            "Test"
        );

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RatingValue);
    }

    [Fact]
    public void Should_HaveError_WhenRatingIdIsEmpty()
    {
        // Arrange
        var request = new UpdateRatingCommand(
            Guid.Empty,
            5,
            "Test"
        );

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RatingId);
    }

    [Fact]
    public void Should_NotHaveError_WhenRatingValueIsNull()
    {
        // Arrange
        var request = new UpdateRatingCommand(
            Guid.NewGuid(),
            null,
            "Updated comment"
        );

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.RatingValue);
    }

    [Fact]
    public void Should_NotHaveError_WhenCommentIsNull()
    {
        // Arrange
        var request = new UpdateRatingCommand(
            Guid.NewGuid(),
            5,
            null
        );

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Comment);
    }

    [Fact]
    public void Should_NotHaveError_WhenCommentIsEmpty()
    {
        // Arrange
        var request = new UpdateRatingCommand(
            Guid.NewGuid(),
            5,
            string.Empty
        );

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Comment);
    }

    [Fact]
    public void Should_NotHaveError_WhenCommentIsAtMaxLength()
    {
        // Arrange
        var longComment = new string('a', 1000);
        var request = new UpdateRatingCommand(
            Guid.NewGuid(),
            5,
            longComment
        );

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Comment);
    }

    [Fact]
    public void Should_HaveError_WhenCommentExceedsMaxLength()
    {
        // Arrange
        var tooLongComment = new string('a', 1001);
        var request = new UpdateRatingCommand(
            Guid.NewGuid(),
            5,
            tooLongComment
        );

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Comment);
    }

    [Fact]
    public void Should_NotHaveError_WhenBothRatingValueAndCommentAreNull()
    {
        // Arrange
        var request = new UpdateRatingCommand(
            Guid.NewGuid(),
            null,
            null
        );

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.RatingValue);
        result.ShouldNotHaveValidationErrorFor(x => x.Comment);
    }
}
