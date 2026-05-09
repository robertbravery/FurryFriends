using Bunit;
using FurryFriends.BlazorUI.Client.Services.Interfaces;
using FurryFriends.BlazorUI.Components.Pages.Ratings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace FurryFriends.UnitTests.BlazorUI;

public class PetWalkerRatingsTests : TestContext
{
  private readonly Mock<IRatingService> _mockRatingService;
  private readonly Mock<ILogger<PetWalkerRatings>> _mockLogger;
  private readonly Guid _petWalkerId;

  public PetWalkerRatingsTests()
  {
    _mockRatingService = new Mock<IRatingService>();
    _mockLogger = new Mock<ILogger<PetWalkerRatings>>();
    _petWalkerId = Guid.NewGuid();

    Services.AddSingleton(_mockRatingService.Object);
    Services.AddSingleton(_mockLogger.Object);
  }

  private static RatingDto CreateRatingDto(int ratingValue, string? clientName = null, string? comment = null)
  {
    return new RatingDto(
      Id: Guid.NewGuid(),
      PetWalkerId: Guid.NewGuid(),
      ClientId: Guid.NewGuid(),
      RatingValue: ratingValue,
      Comment: comment,
      CreatedAt: DateTime.UtcNow,
      UpdatedAt: DateTime.UtcNow,
      ClientName: clientName ?? "Test Client");
  }

  private static PaginatedRatingResponse CreatePaginatedResponse(
    List<RatingDto> items,
    int page = 1,
    int pageSize = 10,
    int totalCount = 0)
  {
    var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
    return new PaginatedRatingResponse(
      Items: items,
      PageNumber: page,
      PageSize: pageSize,
      TotalCount: totalCount,
      TotalPages: totalPages,
      HasPreviousPage: page > 1,
      HasNextPage: page < totalPages);
  }

  [Fact]
  public void ShouldCallServiceOnInitialization()
  {
    // Arrange
    _mockRatingService
      .Setup(s => s.GetRatingsAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>()))
      .ReturnsAsync(CreatePaginatedResponse(new List<RatingDto>(), totalCount: 0));

    // Act
    var cut = RenderComponent<PetWalkerRatings>(parameters => parameters
      .Add(p => p.PetWalkerId, _petWalkerId));

    // Assert - service was called with correct parameters
    _mockRatingService.Verify(s => s.GetRatingsAsync(_petWalkerId, 1, 10), Times.Once);
    Assert.False(cut.Instance.IsLoading);
  }

  [Fact]
  public void ShouldShowEmptyStateWhenNoRatingsExist()
  {
    // Arrange
    _mockRatingService
      .Setup(s => s.GetRatingsAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>()))
      .ReturnsAsync(CreatePaginatedResponse(new List<RatingDto>(), totalCount: 0));

    // Act
    var cut = RenderComponent<PetWalkerRatings>(parameters => parameters
      .Add(p => p.PetWalkerId, _petWalkerId));

    // Assert
    var emptyState = cut.Find(".ratings-empty");
    Assert.NotNull(emptyState);
    Assert.Contains("No ratings yet", emptyState.TextContent);
    Assert.Contains("Be the first to leave a rating for this pet walker.", emptyState.TextContent);
  }

  [Fact]
  public void ShouldShowErrorStateWhenServiceFails()
  {
    // Arrange
    _mockRatingService
      .Setup(s => s.GetRatingsAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>()))
      .ThrowsAsync(new HttpRequestException("Network error"));

    // Act
    var cut = RenderComponent<PetWalkerRatings>(parameters => parameters
      .Add(p => p.PetWalkerId, _petWalkerId));

    // Assert
    var errorAlert = cut.Find(".alert.alert-danger");
    Assert.NotNull(errorAlert);
    Assert.Contains("Failed to load ratings. Please try again later.", errorAlert.TextContent);
  }

  [Fact]
  public void ShouldRenderRatingItemsWhenDataLoads()
  {
    // Arrange
    var ratings = new List<RatingDto>
      {
        CreateRatingDto(5, "Alice", "Excellent service!"),
        CreateRatingDto(3, "Bob", "Average experience"),
      };

    _mockRatingService
      .Setup(s => s.GetRatingsAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>()))
      .ReturnsAsync(CreatePaginatedResponse(ratings, totalCount: 2));

    // Act
    var cut = RenderComponent<PetWalkerRatings>(parameters => parameters
      .Add(p => p.PetWalkerId, _petWalkerId));

    // Assert
    var ratingItems = cut.FindAll(".rating-item");
    Assert.Equal(2, ratingItems.Count);

    // Check first rating
    Assert.Contains("Alice", cut.Markup);
    Assert.Contains("Excellent service!", cut.Markup);

    // Check second rating
    Assert.Contains("Bob", cut.Markup);
    Assert.Contains("Average experience", cut.Markup);
  }

  [Fact]
  public void ShouldDisplayStarsForEachRating()
  {
    // Arrange
    var ratings = new List<RatingDto>
      {
        CreateRatingDto(4, "Charlie"),
      };

    _mockRatingService
      .Setup(s => s.GetRatingsAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>()))
      .ReturnsAsync(CreatePaginatedResponse(ratings, totalCount: 1));

    // Act
    var cut = RenderComponent<PetWalkerRatings>(parameters => parameters
      .Add(p => p.PetWalkerId, _petWalkerId));

    // Assert - each rating has 5 stars with correct filled count
    var filledStars = cut.FindAll(".rating-star.star-filled");
    Assert.Equal(4, filledStars.Count);

    var emptyStars = cut.FindAll(".rating-star.star-empty");
    Assert.Equal(1, emptyStars.Count);
  }

  [Fact]
  public void ShouldHandleRatingWithoutComment()
  {
    // Arrange - rating with null comment
    var ratings = new List<RatingDto>
      {
        CreateRatingDto(5, "Dave", null),
      };

    _mockRatingService
      .Setup(s => s.GetRatingsAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>()))
      .ReturnsAsync(CreatePaginatedResponse(ratings, totalCount: 1));

    // Act
    var cut = RenderComponent<PetWalkerRatings>(parameters => parameters
      .Add(p => p.PetWalkerId, _petWalkerId));

    // Assert - no comment section rendered
    Assert.Empty(cut.FindAll(".rating-comment"));
  }

  [Fact]
  public void ShouldDisplayPaginationControls()
  {
    // Arrange - 25 items on page 1 of 10 per page = 3 pages
    var ratings = Enumerable.Range(1, 10).Select(i =>
      CreateRatingDto(i % 5 + 1, $"User{i}", $"Comment {i}")
    ).ToList();

    _mockRatingService
      .Setup(s => s.GetRatingsAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>()))
      .ReturnsAsync(CreatePaginatedResponse(ratings, page: 1, pageSize: 10, totalCount: 25));

    // Act
    var cut = RenderComponent<PetWalkerRatings>(parameters => parameters
      .Add(p => p.PetWalkerId, _petWalkerId)
      .Add(p => p.DefaultPageSize, 10));

    // Assert
    var pagination = cut.Find(".ratings-pagination");
    Assert.NotNull(pagination);

    // Use TextContent to avoid HTML encoding issues
    Assert.Contains("Page 1 of 3", pagination.TextContent);

    var summaryText = cut.Find(".ratings-summary-text");
    Assert.Contains("Showing", summaryText.TextContent);
    Assert.Contains("1-10", summaryText.TextContent);
    Assert.Contains("25", summaryText.TextContent);

    var buttons = cut.FindAll("button");
    Assert.Equal(2, buttons.Count);

    var prevButton = buttons[0];
    var nextButton = buttons[1];

    Assert.True(prevButton.HasAttribute("disabled")); // disabled on first page
    Assert.False(nextButton.HasAttribute("disabled")); // enabled since has next page
  }

  [Fact]
  public void ShouldNavigateToNextPage()
  {
    // Arrange - page 1 data
    var page1Ratings = Enumerable.Range(1, 10).Select(i =>
      CreateRatingDto(5, $"User{i}")
    ).ToList();

    var page2Ratings = Enumerable.Range(11, 5).Select(i =>
      CreateRatingDto(4, $"User{i}")
    ).ToList();

    var page1Response = CreatePaginatedResponse(page1Ratings, page: 1, pageSize: 10, totalCount: 15);
    var page2Response = CreatePaginatedResponse(page2Ratings, page: 2, pageSize: 10, totalCount: 15);

    _mockRatingService
      .Setup(s => s.GetRatingsAsync(_petWalkerId, 1, 10))
      .ReturnsAsync(page1Response);

    _mockRatingService
      .Setup(s => s.GetRatingsAsync(_petWalkerId, 2, 10))
      .ReturnsAsync(page2Response);

    // Act - render on page 1
    var cut = RenderComponent<PetWalkerRatings>(parameters => parameters
      .Add(p => p.PetWalkerId, _petWalkerId)
      .Add(p => p.DefaultPageSize, 10));

    // Initially shows page 1 users
    Assert.Contains("User1", cut.Markup);
    Assert.DoesNotContain("User11", cut.Markup);

    // Navigate to page 2
    var nextButton = cut.FindAll("button")[1]; // next button
    nextButton.Click();

    // Now shows page 2 users
    Assert.Contains("User11", cut.Markup);
    Assert.Contains("Page 2 of 2", cut.Markup);
  }

  [Fact]
  public void ShouldNavigateToPreviousPage()
  {
    // Arrange
    var page2Ratings = Enumerable.Range(11, 5).Select(i =>
      CreateRatingDto(4, $"User{i}")
    ).ToList();

    // Set up so that PetWalkerRatings initializes on page 2
    // (it initializes to page 1, so we'll call GoToNextPage via the test)
    var page1Ratings = Enumerable.Range(1, 10).Select(i =>
      CreateRatingDto(5, $"User{i}")
    ).ToList();

    var page1Response = CreatePaginatedResponse(page1Ratings, page: 1, pageSize: 10, totalCount: 15);
    var page2Response = CreatePaginatedResponse(page2Ratings, page: 2, pageSize: 10, totalCount: 15);

    _mockRatingService
      .Setup(s => s.GetRatingsAsync(_petWalkerId, 1, 10))
      .ReturnsAsync(page1Response);

    _mockRatingService
      .Setup(s => s.GetRatingsAsync(_petWalkerId, 2, 10))
      .ReturnsAsync(page2Response);

    // Act
    var cut = RenderComponent<PetWalkerRatings>(parameters => parameters
      .Add(p => p.PetWalkerId, _petWalkerId)
      .Add(p => p.DefaultPageSize, 10));

    // Go to page 2
    var nextButton = cut.FindAll("button")[1];
    nextButton.Click();

    // Verify on page 2
    Assert.Contains("User11", cut.Markup);
    Assert.Contains("Page 2 of 2", cut.Markup);

    // Go back to page 1
    var prevButton = cut.FindAll("button")[0];
    prevButton.Click();

    // Verify on page 1
    Assert.Contains("User1", cut.Markup);
    Assert.Contains("Page 1 of 2", cut.Markup);
  }

  [Fact]
  public void ShouldDisplayDefaultHeaderText()
  {
    // Arrange
    _mockRatingService
      .Setup(s => s.GetRatingsAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>()))
      .ReturnsAsync(CreatePaginatedResponse(new List<RatingDto>(), totalCount: 0));

    // Act
    var cut = RenderComponent<PetWalkerRatings>(parameters => parameters
      .Add(p => p.PetWalkerId, _petWalkerId));

    // Assert - use TextContent to avoid HTML encoding of &
    var header = cut.Find("h4");
    Assert.Equal("Ratings & Reviews", header.TextContent);
  }

  [Fact]
  public void ShouldUseCustomHeaderText()
  {
    // Arrange
    _mockRatingService
      .Setup(s => s.GetRatingsAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>()))
      .ReturnsAsync(CreatePaginatedResponse(new List<RatingDto>(), totalCount: 0));

    // Act
    var cut = RenderComponent<PetWalkerRatings>(parameters => parameters
      .Add(p => p.PetWalkerId, _petWalkerId)
      .Add(p => p.HeaderText, "Custom Reviews"));

    // Assert
    // Use TextContent for reliable string comparison
    var header = cut.Find("h4");
    Assert.Equal("Custom Reviews", header.TextContent);
  }
}
