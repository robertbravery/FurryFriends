using FluentAssertions;
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
        var bookingId = Guid.NewGuid();
        var ratingValue = 5;
        var comment = "Great service!";

        // Act
        var rating = Rating.Create(petWalkerId, clientId, bookingId, ratingValue, comment);

        // Assert
        rating.Should().NotBeNull();
        rating.PetWalkerId.Should().Be(petWalkerId);
        rating.ClientId.Should().Be(clientId);
        rating.BookingId.Should().Be(bookingId);
        rating.RatingValue.Should().Be(ratingValue);
        rating.Comment.Should().Be(comment);
        rating.CreatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        rating.ModifiedDate.Should().BeNull();
    }

    [Fact]
    public void Create_WithNullComment_Succeeds()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        var bookingId = Guid.NewGuid();
        var ratingValue = 4;

        // Act
        var rating = Rating.Create(petWalkerId, clientId, bookingId, ratingValue, null);

        // Assert
        rating.Should().NotBeNull();
        rating.Comment.Should().BeNull();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void Create_WithValidRatingValues_Succeeds(int ratingValue)
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        var bookingId = Guid.NewGuid();

        // Act
        var rating = Rating.Create(petWalkerId, clientId, bookingId, ratingValue, "Test");

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
        var bookingId = Guid.NewGuid();

        // Act
        var act = () => Rating.Create(petWalkerId, clientId, bookingId, invalidRatingValue, "Test");

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Create_WithEmptyPetWalkerId_Throws()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var bookingId = Guid.NewGuid();

        // Act
        var act = () => Rating.Create(Guid.Empty, clientId, bookingId, 5, "Test");

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Create_WithEmptyClientId_Throws()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var bookingId = Guid.NewGuid();

        // Act
        var act = () => Rating.Create(petWalkerId, Guid.Empty, bookingId, 5, "Test");

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Create_WithEmptyBookingId_Throws()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var clientId = Guid.NewGuid();

        // Act
        var act = () => Rating.Create(petWalkerId, clientId, Guid.Empty, 5, "Test");

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CanUpdate_WhenNewlyCreated_ReturnsTrue()
    {
        // Arrange
        var rating = Rating.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 5, "Test");

        // Act
        var canUpdate = rating.CanUpdate();

        // Assert
        canUpdate.Should().BeTrue();
    }

    [Fact]
    public void CanUpdate_After7Days_ReturnsFalse()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        var bookingId = Guid.NewGuid();
        
        // Use reflection to set CreatedDate to 8 days ago
        var rating = Rating.Create(petWalkerId, clientId, bookingId, 5, "Test");
        var createdDate = DateTime.UtcNow.AddDays(-8);
        rating.GetType().GetProperty("CreatedDate")!.SetValue(rating, createdDate);

        // Act
        var canUpdate = rating.CanUpdate();

        // Assert
        canUpdate.Should().BeFalse();
    }

    [Fact]
    public void CanUpdate_AfterAlreadyUpdated_ReturnsFalse()
    {
        // Arrange
        var rating = Rating.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 5, "Test");
        
        // First update
        rating.UpdateRatingValue(4);
        
        // Act - try to update again
        var canUpdate = rating.CanUpdate();

        // Assert
        canUpdate.Should().BeFalse();
    }

    [Fact]
    public void UpdateRatingValue_Within7Days_Succeeds()
    {
        // Arrange
        var rating = Rating.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 5, "Original");

        // Act
        rating.UpdateRatingValue(4);

        // Assert
        rating.RatingValue.Should().Be(4);
        rating.ModifiedDate.Should().NotBeNull();
        rating.ModifiedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void UpdateRatingValue_After7Days_Throws()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        var bookingId = Guid.NewGuid();
        var rating = Rating.Create(petWalkerId, clientId, bookingId, 5, "Test");
        
        // Use reflection to set CreatedDate to 8 days ago
        var createdDate = DateTime.UtcNow.AddDays(-8);
        rating.GetType().GetProperty("CreatedDate")!.SetValue(rating, createdDate);

        // Act
        var act = () => rating.UpdateRatingValue(3);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*cannot be updated*");
    }

    [Fact]
    public void UpdateRatingValue_WithInvalidValue_Throws()
    {
        // Arrange
        var rating = Rating.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 5, "Test");

        // Act
        var act = () => rating.UpdateRatingValue(0);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void UpdateComment_Within7Days_Succeeds()
    {
        // Arrange
        var rating = Rating.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 5, "Original");

        // Act
        rating.UpdateComment("Updated comment");

        // Assert
        rating.Comment.Should().Be("Updated comment");
        rating.ModifiedDate.Should().NotBeNull();
    }

    [Fact]
    public void UpdateComment_After7Days_Throws()
    {
        // Arrange
        var rating = Rating.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 5, "Test");
        
        // Use reflection to set CreatedDate to 8 days ago
        var createdDate = DateTime.UtcNow.AddDays(-8);
        rating.GetType().GetProperty("CreatedDate")!.SetValue(rating, createdDate);

        // Act
        var act = () => rating.UpdateComment("New comment");

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*cannot be updated*");
    }

    [Fact]
    public void UpdateComment_AfterAlreadyUpdated_Throws()
    {
        // Arrange
        var rating = Rating.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 5, "Original");
        
        // First update
        rating.UpdateComment("First update");
        
        // Act
        var act = () => rating.UpdateComment("Second update");

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*cannot be updated*");
    }
}
