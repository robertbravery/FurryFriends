using Bunit;
using FurryFriends.BlazorUI.Client.Services.Interfaces;
using FurryFriends.BlazorUI.Components.Common;

namespace FurryFriends.UnitTests.BlazorUI;

public class RatingDisplayTests : TestContext
{
    [Fact]
    public void ShouldShowEmptyStateWhenSummaryIsNull()
    {
        // Arrange & Act
        var cut = RenderComponent<RatingDisplay>(parameters => parameters
          .Add(p => p.RatingSummary, null)
          .Add(p => p.MaxStars, 5)
          .Add(p => p.ShowNumericValue, true));

        // Assert
        cut.MarkupMatches("<div class=\"rating-display\"><div class=\"rating-empty\"><span class=\"rating-empty-text\">No ratings yet</span></div></div>");
        Assert.Contains("No ratings yet", cut.Markup);
    }

    [Fact]
    public void ShouldRenderCorrectNumberOfFilledStars()
    {
        // Arrange
        var summary = new RatingSummaryDto(
          PetWalkerId: Guid.NewGuid(),
          AverageRating: 4.2,
          TotalRatings: 10,
          RecentRatings: new List<RatingDto>());

        // Act
        var cut = RenderComponent<RatingDisplay>(parameters => parameters
          .Add(p => p.RatingSummary, summary)
          .Add(p => p.MaxStars, 5)
          .Add(p => p.ShowNumericValue, true));

        // Assert
        var allStars = cut.FindAll(".rating-star");
        Assert.Equal(5, allStars.Count);

        var filledStars = cut.FindAll(".star-filled");
        Assert.Equal(4, filledStars.Count); // 4.2 rounded = 4

        var emptyStars = cut.FindAll(".star-empty");
        Assert.Equal(1, emptyStars.Count);
    }

    [Fact]
    public void ShouldDisplayNumericAverageAndCount()
    {
        // Arrange
        var summary = new RatingSummaryDto(
          PetWalkerId: Guid.NewGuid(),
          AverageRating: 3.5,
          TotalRatings: 7,
          RecentRatings: new List<RatingDto>());

        // Act
        var cut = RenderComponent<RatingDisplay>(parameters => parameters
          .Add(p => p.RatingSummary, summary)
          .Add(p => p.ShowNumericValue, true));

        // Assert
        Assert.Contains("3.5", cut.Markup);
        Assert.Contains("7 ratings", cut.Markup);
    }

    [Fact]
    public void ShouldShowSingularRatingWhenCountIsOne()
    {
        // Arrange
        var summary = new RatingSummaryDto(
          PetWalkerId: Guid.NewGuid(),
          AverageRating: 5.0,
          TotalRatings: 1,
          RecentRatings: new List<RatingDto>());

        // Act
        var cut = RenderComponent<RatingDisplay>(parameters => parameters
          .Add(p => p.RatingSummary, summary)
          .Add(p => p.ShowNumericValue, true));

        // Assert
        Assert.Contains("1 rating", cut.Markup);
        Assert.DoesNotContain("1 ratings", cut.Markup);
    }

    [Fact]
    public void ShouldHideNumericValueWhenShowNumericValueIsFalse()
    {
        // Arrange
        var summary = new RatingSummaryDto(
          PetWalkerId: Guid.NewGuid(),
          AverageRating: 4.5,
          TotalRatings: 15,
          RecentRatings: new List<RatingDto>());

        // Act
        var cut = RenderComponent<RatingDisplay>(parameters => parameters
          .Add(p => p.RatingSummary, summary)
          .Add(p => p.ShowNumericValue, false));

        // Assert - average should NOT appear
        Assert.DoesNotContain("4.5", cut.Markup);
        // Count should still appear
        Assert.Contains("15 ratings", cut.Markup);
    }

    [Fact]
    public void ShouldHandleZeroRatingsSummary()
    {
        // Arrange
        var summary = new RatingSummaryDto(
          PetWalkerId: Guid.NewGuid(),
          AverageRating: 0.0,
          TotalRatings: 0,
          RecentRatings: new List<RatingDto>());

        // Act
        var cut = RenderComponent<RatingDisplay>(parameters => parameters
          .Add(p => p.RatingSummary, summary)
          .Add(p => p.MaxStars, 5)
          .Add(p => p.ShowNumericValue, true));

        // Assert
        var filledStars = cut.FindAll(".star-filled");
        Assert.Equal(0, filledStars.Count); // 0.0 rounded = 0

        var emptyStars = cut.FindAll(".star-empty");
        Assert.Equal(5, emptyStars.Count);

        Assert.Contains("0.0", cut.Markup);
        Assert.Contains("0 ratings", cut.Markup);
    }

    [Fact]
    public void ShouldRoundUpCorrectly()
    {
        // Arrange - 4.5 rounds to 5 with MidpointRounding.AwayFromZero
        var summary = new RatingSummaryDto(
          PetWalkerId: Guid.NewGuid(),
          AverageRating: 4.5,
          TotalRatings: 20,
          RecentRatings: new List<RatingDto>());

        // Act
        var cut = RenderComponent<RatingDisplay>(parameters => parameters
          .Add(p => p.RatingSummary, summary)
          .Add(p => p.MaxStars, 5));

        // Assert
        var filledStars = cut.FindAll(".star-filled");
        Assert.Equal(5, filledStars.Count);
    }

    [Fact]
    public void ShouldRespectMaxStarsParameter()
    {
        // Arrange
        var summary = new RatingSummaryDto(
          PetWalkerId: Guid.NewGuid(),
          AverageRating: 3.0,
          TotalRatings: 5,
          RecentRatings: new List<RatingDto>());

        // Act
        var cut = RenderComponent<RatingDisplay>(parameters => parameters
          .Add(p => p.RatingSummary, summary)
          .Add(p => p.MaxStars, 3));

        // Assert
        var allStars = cut.FindAll(".rating-star");
        Assert.Equal(3, allStars.Count);

        var filledStars = cut.FindAll(".star-filled");
        Assert.Equal(3, filledStars.Count);
    }
}
