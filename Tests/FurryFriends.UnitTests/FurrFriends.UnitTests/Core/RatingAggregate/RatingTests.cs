using FurryFriends.Core.RatingAggregate;

namespace FurrFriends.UnitTests.Core.RatingAggregate;

public class RatingTests
{
    [Fact]
    public void Create_WithValidParameters_ReturnsRating()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        var ratingValue = 5;
        var comment = "Great service!";

        // Act
        var rating = Rating.Create(petWalkerId, clientId, ratingValue, comment);

        // Assert
        rating.Should().NotBeNull();
        rating.PetWalkerId.Should().Be(petWalkerId);
        rating.ClientId.Should().Be(clientId);
        rating.RatingValue.Should().Be(ratingValue);
        rating.Comment.Should().Be(comment);
        rating.Status.Should().Be(RatingStatus.Active);
        rating.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        rating.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void Create_WithValidRatingValues_1To5_Succeeds(int ratingValue)
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var clientId = Guid.NewGuid();

        // Act
        var rating = Rating.Create(petWalkerId, clientId, ratingValue, "Test");

        // Assert
        rating.RatingValue.Should().Be(ratingValue);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(6)]
    [InlineData(100)]
    public void Create_WithInvalidRatingValue_Throws(int invalidRatingValue)
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var clientId = Guid.NewGuid();

        // Act
        var act = () => Rating.Create(petWalkerId, clientId, invalidRatingValue, "Test");

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Create_WithEmptyPetWalkerId_Throws()
    {
        // Arrange
        var clientId = Guid.NewGuid();

        // Act
        var act = () => Rating.Create(Guid.Empty, clientId, 5, "Test");

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithEmptyClientId_Throws()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();

        // Act
        var act = () => Rating.Create(petWalkerId, Guid.Empty, 5, "Test");

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void CanEdit_WhenNewlyCreated_ReturnsTrue()
    {
        // Arrange
        var rating = Rating.Create(Guid.NewGuid(), Guid.NewGuid(), 5, "Test");

        // Act
        var canEdit = rating.CanEdit();

        // Assert
        canEdit.Should().BeTrue();
    }

    [Fact]
    public void CanEdit_After24Hours_ReturnsFalse()
    {
        // Arrange
        var rating = Rating.Create(Guid.NewGuid(), Guid.NewGuid(), 5, "Test");

        // Use reflection to set CreatedAt to 25 hours ago
        var createdDate = DateTime.UtcNow.AddHours(-25);
        rating.GetType().GetProperty("CreatedAt")!.SetValue(rating, createdDate);

        // Act
        var canEdit = rating.CanEdit();

        // Assert
        canEdit.Should().BeFalse();
    }

    [Fact]
    public void UpdateRating_Within24Hours_Succeeds()
    {
        // Arrange
        var rating = Rating.Create(Guid.NewGuid(), Guid.NewGuid(), 5, "Original");

        // Act
        var result = rating.UpdateRating(4, "Updated comment");

        // Assert
        result.IsSuccess.Should().BeTrue();
        rating.RatingValue.Should().Be(4);
        rating.Comment.Should().Be("Updated comment");
        rating.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void UpdateRating_After24Hours_ReturnsError()
    {
        // Arrange
        var rating = Rating.Create(Guid.NewGuid(), Guid.NewGuid(), 5, "Test");

        // Use reflection to set CreatedAt to 25 hours ago
        var createdDate = DateTime.UtcNow.AddHours(-25);
        rating.GetType().GetProperty("CreatedAt")!.SetValue(rating, createdDate);

        // Act
        var result = rating.UpdateRating(3, "Updated");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("24 hours"));
    }

    [Fact]
    public void UpdateRating_WithModeratedStatus_ReturnsError()
    {
        // Arrange
        var rating = Rating.Create(Guid.NewGuid(), Guid.NewGuid(), 5, "Test");
        rating.SetStatus(RatingStatus.Moderated);

        // Act
        var result = rating.UpdateRating(3, "Updated");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("moderation"));
    }

    [Fact]
    public void Remove_Within24Hours_Succeeds()
    {
        // Arrange
        var rating = Rating.Create(Guid.NewGuid(), Guid.NewGuid(), 5, "Test");

        // Act
        var result = rating.Remove();

        // Assert
        result.IsSuccess.Should().BeTrue();
        rating.Status.Should().Be(RatingStatus.Removed);
    }

    [Fact]
    public void Remove_After24Hours_ReturnsError()
    {
        // Arrange
        var rating = Rating.Create(Guid.NewGuid(), Guid.NewGuid(), 5, "Test");

        // Use reflection to set CreatedAt to 25 hours ago
        var createdDate = DateTime.UtcNow.AddHours(-25);
        rating.GetType().GetProperty("CreatedAt")!.SetValue(rating, createdDate);

        // Act
        var result = rating.Remove();

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("24 hours"));
    }

    [Fact]
    public void Remove_WithModeratedStatus_ReturnsError()
    {
        // Arrange
        var rating = Rating.Create(Guid.NewGuid(), Guid.NewGuid(), 5, "Test");
        rating.SetStatus(RatingStatus.Moderated);

        // Act
        var result = rating.Remove();

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("moderation"));
    }

    [Fact]
    public void UpdateRating_WithInvalidValue_Throws()
    {
        // Arrange
        var rating = Rating.Create(Guid.NewGuid(), Guid.NewGuid(), 5, "Test");

        // Act
        var act = () => rating.UpdateRating(0, "Test");

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
