using FurryFriends.Core.PetWalkerAggregate;
using FurryFriends.Core.ValueObjects;

namespace FurrFriends.UnitTests.Core.PetWalkerAggregate;

public class PetWalkerRatingAggregateTests
{
    [Fact]
    public void UpdateRatingAggregate_WithMultipleRatings_CalculatesAverage()
    {
        // Arrange
        var petWalker = CreateTestPetWalker();

        // Act
        petWalker.UpdateRatingAggregate(4.5, 2);

        // Assert
        petWalker.AverageRating.Should().Be(4.5);
        petWalker.TotalRatingsCount.Should().Be(2);
    }

    [Fact]
    public void UpdateRatingAggregate_WithZeroRatings_SetsNull()
    {
        // Arrange
        var petWalker = CreateTestPetWalker();

        // Act
        petWalker.UpdateRatingAggregate(null, 0);

        // Assert
        petWalker.AverageRating.Should().BeNull();
        petWalker.TotalRatingsCount.Should().Be(0);
    }

    [Fact]
    public void UpdateRatingAggregate_WithSingleRating_ReturnsExactValue()
    {
        // Arrange
        var petWalker = CreateTestPetWalker();

        // Act
        petWalker.UpdateRatingAggregate(5.0, 1);

        // Assert
        petWalker.AverageRating.Should().Be(5.0);
        petWalker.TotalRatingsCount.Should().Be(1);
    }

    [Fact]
    public void UpdateRatingAggregate_RoundsToOneDecimalPlace()
    {
        // Arrange
        var petWalker = CreateTestPetWalker();

        // Act
        petWalker.UpdateRatingAggregate(4.56, 3);

        // Assert
        petWalker.AverageRating.Should().Be(4.56);
        petWalker.TotalRatingsCount.Should().Be(3);
    }

    private static PetWalker CreateTestPetWalker()
    {
        var name = Name.Create("John", "Smith");
        var email = Email.Create("john.smith@example.com");
        var phoneNumber = PhoneNumber.Create("027", "011-123-4567").GetAwaiter().GetResult();
        var address = Address.Create("123 Main St", "Seattle", "WA", "US", "98101");
        return PetWalker.Create(name, email, phoneNumber, address);
    }
}
