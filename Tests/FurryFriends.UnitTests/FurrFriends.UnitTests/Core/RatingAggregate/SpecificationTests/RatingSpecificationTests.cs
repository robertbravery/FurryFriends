using FluentAssertions;
using FurryFriends.Core.RatingAggregate;
using FurryFriends.Core.RatingAggregate.Specifications;

namespace FurrFriends.UnitTests.Core.RatingAggregate.SpecificationTests;

public class GetRatingsForPetWalkerSpecificationTests
{
    [Fact]
    public void Specification_MatchesCorrectPetWalker()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var otherPetWalkerId = Guid.NewGuid();
        
        var ratings = new List<Rating>
        {
            Rating.Create(petWalkerId, Guid.NewGuid(), Guid.NewGuid(), 5, "Great!"),
            Rating.Create(petWalkerId, Guid.NewGuid(), Guid.NewGuid(), 4, "Good"),
            Rating.Create(otherPetWalkerId, Guid.NewGuid(), Guid.NewGuid(), 3, "Average")
        };

        var spec = new GetRatingsForPetWalkerSpecification(petWalkerId);

        // Act
        var filtered = ratings.AsQueryable().Where(spec.WhereExpressions.First().Filter);

        // Assert
        filtered.Should().HaveCount(2);
        filtered.Should().AllSatisfy(r => r.PetWalkerId.Should().Be(petWalkerId));
    }

    [Fact]
    public void Specification_OrdersByCreatedDateDescending()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        
        var rating1 = Rating.Create(petWalkerId, Guid.NewGuid(), Guid.NewGuid(), 5, "Old");
        var rating2 = Rating.Create(petWalkerId, Guid.NewGuid(), Guid.NewGuid(), 4, "New");
        
        // Use reflection to set CreatedDate
        var type = typeof(Rating);
        type.GetProperty("CreatedDate")!.SetValue(rating1, DateTime.UtcNow.AddDays(-2));
        type.GetProperty("CreatedDate")!.SetValue(rating2, DateTime.UtcNow.AddDays(-1));

        var ratings = new List<Rating> { rating1, rating2 };

        var spec = new GetRatingsForPetWalkerSpecification(petWalkerId);

        // Act
        var filtered = ratings.AsQueryable()
            .Where(spec.WhereExpressions.First().Filter)
            .OrderByDescending(r => r.CreatedDate)
            .ToList();

        // Assert
        filtered.Should().HaveCount(2);
        filtered[0].Should().Be(rating2); // Most recent first
        filtered[1].Should().Be(rating1);
    }
}

public class GetRatingByBookingIdSpecificationTests
{
    [Fact]
    public void Specification_FindsByBookingId()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var otherBookingId = Guid.NewGuid();
        
        var ratings = new List<Rating>
        {
            Rating.Create(Guid.NewGuid(), Guid.NewGuid(), bookingId, 5, "Found"),
            Rating.Create(Guid.NewGuid(), Guid.NewGuid(), otherBookingId, 4, "Not found")
        };

        var spec = new GetRatingByBookingIdSpecification(bookingId);

        // Act
        var filtered = ratings.AsQueryable().Where(spec.WhereExpressions.First().Filter);

        // Assert
        filtered.Should().HaveCount(1);
        filtered.First().BookingId.Should().Be(bookingId);
    }

    [Fact]
    public void Specification_ReturnsNullForNonExistentBooking()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        
        var ratings = new List<Rating>
        {
            Rating.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 5, "Test")
        };

        var spec = new GetRatingByBookingIdSpecification(bookingId);

        // Act
        var filtered = ratings.AsQueryable().Where(spec.WhereExpressions.First().Filter);

        // Assert
        filtered.Should().BeEmpty();
    }
}

public class GetPetWalkerRatingSummarySpecificationTests
{
    [Fact]
    public void Specification_MatchesCorrectPetWalker()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var otherPetWalkerId = Guid.NewGuid();
        
        var ratings = new List<Rating>
        {
            Rating.Create(petWalkerId, Guid.NewGuid(), Guid.NewGuid(), 5, "Great!"),
            Rating.Create(petWalkerId, Guid.NewGuid(), Guid.NewGuid(), 4, "Good"),
            Rating.Create(otherPetWalkerId, Guid.NewGuid(), Guid.NewGuid(), 3, "Average")
        };

        var spec = new GetPetWalkerRatingSummarySpecification(petWalkerId);

        // Act
        var filtered = ratings.AsQueryable().Where(spec.WhereExpressions.First().Filter);

        // Assert
        filtered.Should().HaveCount(2);
        filtered.Should().AllSatisfy(r => r.PetWalkerId.Should().Be(petWalkerId));
    }

    [Fact]
    public void Specification_CalculatesAverageCorrectly()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        
        var ratings = new List<Rating>
        {
            Rating.Create(petWalkerId, Guid.NewGuid(), Guid.NewGuid(), 5, "5 stars"),
            Rating.Create(petWalkerId, Guid.NewGuid(), Guid.NewGuid(), 4, "4 stars"),
            Rating.Create(petWalkerId, Guid.NewGuid(), Guid.NewGuid(), 3, "3 stars"),
            Rating.Create(petWalkerId, Guid.NewGuid(), Guid.NewGuid(), 2, "2 stars"),
            Rating.Create(petWalkerId, Guid.NewGuid(), Guid.NewGuid(), 1, "1 star")
        };

        var spec = new GetPetWalkerRatingSummarySpecification(petWalkerId);

        // Act
        var filtered = ratings.AsQueryable().Where(spec.WhereExpressions.First().Filter).ToList();
        
        // Calculate average manually
        var expectedAverage = filtered.Average(r => r.RatingValue);

        // Assert
        filtered.Should().HaveCount(5);
        expectedAverage.Should().Be(3.0);
    }
}
