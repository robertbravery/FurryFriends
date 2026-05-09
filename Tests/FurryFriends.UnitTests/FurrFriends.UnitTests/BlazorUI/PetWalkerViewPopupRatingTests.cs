using Bunit;
using FurryFriends.BlazorUI.Client.Models.Common;
using FurryFriends.BlazorUI.Client.Models.PetWalkers;
using FurryFriends.BlazorUI.Client.Pages.PetWalkers;
using FurryFriends.BlazorUI.Client.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Moq;

namespace FurryFriends.UnitTests.BlazorUI;

public class PetWalkerViewPopupRatingTests : TestContext
{
    private readonly Mock<IPetWalkerService> _mockPetWalkerService;
    private readonly Mock<IScheduleService> _mockScheduleService;
    private readonly Mock<IRatingService> _mockRatingService;
    private readonly Mock<IClientContextService> _mockClientContextService;
    private readonly Mock<IJSRuntime> _mockJsRuntime;
    private readonly Mock<ILogger<PetWalkerViewPopup>> _mockLogger;
    private readonly Guid _clientId;
    private readonly Guid _petWalkerId;
    private readonly string _petWalkerEmail;

    public PetWalkerViewPopupRatingTests()
    {
        _mockPetWalkerService = new Mock<IPetWalkerService>();
        _mockScheduleService = new Mock<IScheduleService>();
        _mockRatingService = new Mock<IRatingService>();
        _mockClientContextService = new Mock<IClientContextService>();
        _mockJsRuntime = new Mock<IJSRuntime>();
        _mockLogger = new Mock<ILogger<PetWalkerViewPopup>>();

        _clientId = Guid.NewGuid();
        _petWalkerId = Guid.NewGuid();
        _petWalkerEmail = "walker@example.com";

        // Default client context returns a known client ID
        _mockClientContextService
            .Setup(s => s.GetCurrentClientIdAsync())
            .ReturnsAsync(_clientId);

        Services.AddSingleton(_mockPetWalkerService.Object);
        Services.AddSingleton(_mockScheduleService.Object);
        Services.AddSingleton(_mockRatingService.Object);
        Services.AddSingleton(_mockClientContextService.Object);
        Services.AddSingleton(_mockJsRuntime.Object);
        Services.AddSingleton(_mockLogger.Object);
    }

    private static PetWalkerDetailDto CreatePetWalkerDetailDto(Guid id, string email)
    {
        return new PetWalkerDetailDto
        {
            Id = id,
            Name = "John Doe",
            EmailAddress = email,
            Biography = "Experienced pet walker",
            HourlyRate = 25.00m,
            YearsOfExperience = 3,
            DailyPetWalkLimit = 5,
            IsVerified = true,
            HasInsurance = true,
            HasFirstAidCertification = false,
            City = "TestCity",
            State = "TS",
            Country = "Testland",
            ZipCode = "12345",
            Gender = "Male",
            PhoneNumber = "555-1234",
            Schedules = new List<ScheduleItemDto>()
        };
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

    private void SetupDefaultMocks(PetWalkerDetailDto? petWalker = null)
    {
        var dto = petWalker ?? CreatePetWalkerDetailDto(_petWalkerId, _petWalkerEmail);

        _mockPetWalkerService
            .Setup(s => s.GetPetWalkerDetailsByEmailAsync(_petWalkerEmail))
            .ReturnsAsync(new ApiResponse<PetWalkerDetailDto>
            {
                Success = true,
                Data = dto,
                Message = "Success"
            });

        _mockScheduleService
            .Setup(s => s.GetScheduleAsync(_petWalkerId))
            .ReturnsAsync(new ApiResponse<GetScheduleResponseDto>
            {
                Success = true,
                Data = new GetScheduleResponseDto
                {
                    PetWalkerId = _petWalkerId,
                    Schedules = new List<ScheduleItemDto>()
                }
            });

        _mockRatingService
            .Setup(s => s.GetRatingSummaryAsync(_petWalkerId))
            .ReturnsAsync(new RatingSummaryDto(
                PetWalkerId: _petWalkerId,
                AverageRating: 4.5,
                TotalRatings: 10,
                RecentRatings: new List<RatingDto>()));

        _mockRatingService
            .Setup(s => s.GetRatingsAsync(_petWalkerId, 1, 10))
            .ReturnsAsync(CreatePaginatedResponse(new List<RatingDto>(), totalCount: 0));
    }

    [Fact]
    public void ShouldResolveClientIdFromContextOnInitialization()
    {
        // Arrange
        SetupDefaultMocks();

        // Act
        var cut = RenderComponent<PetWalkerViewPopup>(parameters => parameters
            .Add(p => p.PetWalkerEmail, _petWalkerEmail));

        // Assert - client context was called to resolve the ID
        _mockClientContextService.Verify(s => s.GetCurrentClientIdAsync(), Times.AtLeastOnce);
    }

    [Fact]
    public void ShouldDisplayRatingSummaryInHeader()
    {
        // Arrange
        SetupDefaultMocks();

        // Act
        var cut = RenderComponent<PetWalkerViewPopup>(parameters => parameters
            .Add(p => p.PetWalkerEmail, _petWalkerEmail));

        // Assert - rating summary is displayed in the rating-count span
        var ratingCount = cut.Find(".rating-count");
        Assert.Contains("10 reviews", ratingCount.TextContent);
    }

    [Fact]
    public void ShouldShowRatingsEmptyStateWhenNoRatingsExist()
    {
        // Arrange
        SetupDefaultMocks();

        // Act
        var cut = RenderComponent<PetWalkerViewPopup>(parameters => parameters
            .Add(p => p.PetWalkerEmail, _petWalkerEmail));

        // Assert - empty ratings state is shown
        var emptyState = cut.Find(".ratings-empty");
        Assert.NotNull(emptyState);
        Assert.Contains("No ratings yet", emptyState.TextContent);
        Assert.Contains("Be the first to leave a rating", emptyState.TextContent);
    }

    [Fact]
    public void ShouldShowRatingsErrorStateWhenServiceFails()
    {
        // Arrange
        var dto = CreatePetWalkerDetailDto(_petWalkerId, _petWalkerEmail);

        _mockPetWalkerService
            .Setup(s => s.GetPetWalkerDetailsByEmailAsync(_petWalkerEmail))
            .ReturnsAsync(new ApiResponse<PetWalkerDetailDto>
            {
                Success = true,
                Data = dto
            });

        _mockScheduleService
            .Setup(s => s.GetScheduleAsync(_petWalkerId))
            .ReturnsAsync(new ApiResponse<GetScheduleResponseDto>
            {
                Success = true,
                Data = new GetScheduleResponseDto { PetWalkerId = _petWalkerId }
            });

        _mockRatingService
            .Setup(s => s.GetRatingSummaryAsync(_petWalkerId))
            .ReturnsAsync(new RatingSummaryDto(
                PetWalkerId: _petWalkerId,
                AverageRating: 0,
                TotalRatings: 0,
                RecentRatings: new List<RatingDto>()));

        _mockRatingService
            .Setup(s => s.GetRatingsAsync(_petWalkerId, 1, 10))
            .ThrowsAsync(new HttpRequestException("Network error"));

        // Act
        var cut = RenderComponent<PetWalkerViewPopup>(parameters => parameters
            .Add(p => p.PetWalkerEmail, _petWalkerEmail));

        // Assert - error state
        var errorAlert = cut.Find(".alert.alert-danger");
        Assert.NotNull(errorAlert);
        Assert.Contains("Failed to load ratings", errorAlert.TextContent);
    }

    [Fact]
    public void ShouldRenderRatingItemsWhenDataLoads()
    {
        // Arrange
        var dto = CreatePetWalkerDetailDto(_petWalkerId, _petWalkerEmail);
        var ratings = new List<RatingDto>
        {
            CreateRatingDto(5, "Alice", "Excellent service!"),
            CreateRatingDto(3, "Bob", "Average experience"),
        };

        _mockPetWalkerService
            .Setup(s => s.GetPetWalkerDetailsByEmailAsync(_petWalkerEmail))
            .ReturnsAsync(new ApiResponse<PetWalkerDetailDto> { Success = true, Data = dto });

        _mockScheduleService
            .Setup(s => s.GetScheduleAsync(_petWalkerId))
            .ReturnsAsync(new ApiResponse<GetScheduleResponseDto>
            {
                Success = true,
                Data = new GetScheduleResponseDto { PetWalkerId = _petWalkerId }
            });

        _mockRatingService
            .Setup(s => s.GetRatingSummaryAsync(_petWalkerId))
            .ReturnsAsync(new RatingSummaryDto(
                PetWalkerId: _petWalkerId,
                AverageRating: 4.0,
                TotalRatings: 2,
                RecentRatings: new List<RatingDto>()));

        _mockRatingService
            .Setup(s => s.GetRatingsAsync(_petWalkerId, 1, 10))
            .ReturnsAsync(CreatePaginatedResponse(ratings, totalCount: 2));

        // Act
        var cut = RenderComponent<PetWalkerViewPopup>(parameters => parameters
            .Add(p => p.PetWalkerEmail, _petWalkerEmail));

        // Assert
        var ratingItems = cut.FindAll(".rating-item");
        Assert.Equal(2, ratingItems.Count);

        Assert.Contains("Alice", cut.Markup);
        Assert.Contains("Excellent service!", cut.Markup);
        Assert.Contains("Bob", cut.Markup);
        Assert.Contains("Average experience", cut.Markup);
    }

    [Fact]
    public void ShouldDisplayStarsForEachRating()
    {
        // Arrange
        var dto = CreatePetWalkerDetailDto(_petWalkerId, _petWalkerEmail);
        var ratings = new List<RatingDto>
        {
            CreateRatingDto(4, "Charlie"),
        };

        _mockPetWalkerService
            .Setup(s => s.GetPetWalkerDetailsByEmailAsync(_petWalkerEmail))
            .ReturnsAsync(new ApiResponse<PetWalkerDetailDto> { Success = true, Data = dto });

        _mockScheduleService
            .Setup(s => s.GetScheduleAsync(_petWalkerId))
            .ReturnsAsync(new ApiResponse<GetScheduleResponseDto>
            {
                Success = true,
                Data = new GetScheduleResponseDto { PetWalkerId = _petWalkerId }
            });

        _mockRatingService
            .Setup(s => s.GetRatingSummaryAsync(_petWalkerId))
            .ReturnsAsync(new RatingSummaryDto(
                PetWalkerId: _petWalkerId,
                AverageRating: 4.0,
                TotalRatings: 1,
                RecentRatings: new List<RatingDto>()));

        _mockRatingService
            .Setup(s => s.GetRatingsAsync(_petWalkerId, 1, 10))
            .ReturnsAsync(CreatePaginatedResponse(ratings, totalCount: 1));

        // Act
        var cut = RenderComponent<PetWalkerViewPopup>(parameters => parameters
            .Add(p => p.PetWalkerEmail, _petWalkerEmail));

        // Assert
        var filledStars = cut.FindAll(".rating-star.star-filled");
        Assert.Equal(4, filledStars.Count);

        var emptyStars = cut.FindAll(".rating-star.star-empty");
        Assert.Single(emptyStars);
    }

    [Fact]
    public void ShouldHandleRatingWithoutComment()
    {
        // Arrange
        var dto = CreatePetWalkerDetailDto(_petWalkerId, _petWalkerEmail);
        var ratings = new List<RatingDto>
        {
            CreateRatingDto(5, "Dave", null),
        };

        _mockPetWalkerService
            .Setup(s => s.GetPetWalkerDetailsByEmailAsync(_petWalkerEmail))
            .ReturnsAsync(new ApiResponse<PetWalkerDetailDto> { Success = true, Data = dto });

        _mockScheduleService
            .Setup(s => s.GetScheduleAsync(_petWalkerId))
            .ReturnsAsync(new ApiResponse<GetScheduleResponseDto>
            {
                Success = true,
                Data = new GetScheduleResponseDto { PetWalkerId = _petWalkerId }
            });

        _mockRatingService
            .Setup(s => s.GetRatingSummaryAsync(_petWalkerId))
            .ReturnsAsync(new RatingSummaryDto(
                PetWalkerId: _petWalkerId,
                AverageRating: 5.0,
                TotalRatings: 1,
                RecentRatings: new List<RatingDto>()));

        _mockRatingService
            .Setup(s => s.GetRatingsAsync(_petWalkerId, 1, 10))
            .ReturnsAsync(CreatePaginatedResponse(ratings, totalCount: 1));

        // Act
        var cut = RenderComponent<PetWalkerViewPopup>(parameters => parameters
            .Add(p => p.PetWalkerEmail, _petWalkerEmail));

        // Assert - no comment section rendered
        Assert.Empty(cut.FindAll(".rating-comment"));
    }

    [Fact]
    public void ShouldDisplayPaginationControls()
    {
        // Arrange - 25 items on page 1 of 10 per page = 3 pages
        var dto = CreatePetWalkerDetailDto(_petWalkerId, _petWalkerEmail);
        var ratings = Enumerable.Range(1, 10).Select(i =>
            CreateRatingDto(i % 5 + 1, $"User{i}", $"Comment {i}")
        ).ToList();

        _mockPetWalkerService
            .Setup(s => s.GetPetWalkerDetailsByEmailAsync(_petWalkerEmail))
            .ReturnsAsync(new ApiResponse<PetWalkerDetailDto> { Success = true, Data = dto });

        _mockScheduleService
            .Setup(s => s.GetScheduleAsync(_petWalkerId))
            .ReturnsAsync(new ApiResponse<GetScheduleResponseDto>
            {
                Success = true,
                Data = new GetScheduleResponseDto { PetWalkerId = _petWalkerId }
            });

        _mockRatingService
            .Setup(s => s.GetRatingSummaryAsync(_petWalkerId))
            .ReturnsAsync(new RatingSummaryDto(
                PetWalkerId: _petWalkerId,
                AverageRating: 3.0,
                TotalRatings: 25,
                RecentRatings: new List<RatingDto>()));

        _mockRatingService
            .Setup(s => s.GetRatingsAsync(_petWalkerId, 1, 10))
            .ReturnsAsync(CreatePaginatedResponse(ratings, page: 1, pageSize: 10, totalCount: 25));

        // Act
        var cut = RenderComponent<PetWalkerViewPopup>(parameters => parameters
            .Add(p => p.PetWalkerEmail, _petWalkerEmail));

        // Assert
        var pagination = cut.Find(".ratings-pagination");
        Assert.NotNull(pagination);
        Assert.Contains("Page 1 of 3", pagination.TextContent);

        var summaryText = cut.Find(".ratings-summary-text");
        Assert.Contains("Showing", summaryText.TextContent);
        // The range format has whitespace around the dash due to Razor line breaks, so check parts individually
        Assert.Contains("of 25 ratings", summaryText.TextContent);

        var buttons = cut.FindAll(".ratings-pagination button");
        Assert.Equal(2, buttons.Count);

        var prevButton = buttons[0];
        var nextButton = buttons[1];

        Assert.True(prevButton.HasAttribute("disabled"));
        Assert.False(nextButton.HasAttribute("disabled"));
    }

    [Fact]
    public void ShouldNavigateToNextPage()
    {
        // Arrange
        var dto = CreatePetWalkerDetailDto(_petWalkerId, _petWalkerEmail);
        var page1Ratings = Enumerable.Range(1, 10).Select(i =>
            CreateRatingDto(5, $"User{i}")
        ).ToList();
        var page2Ratings = Enumerable.Range(11, 5).Select(i =>
            CreateRatingDto(4, $"User{i}")
        ).ToList();

        var page1Response = CreatePaginatedResponse(page1Ratings, page: 1, pageSize: 10, totalCount: 15);
        var page2Response = CreatePaginatedResponse(page2Ratings, page: 2, pageSize: 10, totalCount: 15);

        _mockPetWalkerService
            .Setup(s => s.GetPetWalkerDetailsByEmailAsync(_petWalkerEmail))
            .ReturnsAsync(new ApiResponse<PetWalkerDetailDto> { Success = true, Data = dto });

        _mockScheduleService
            .Setup(s => s.GetScheduleAsync(_petWalkerId))
            .ReturnsAsync(new ApiResponse<GetScheduleResponseDto>
            {
                Success = true,
                Data = new GetScheduleResponseDto { PetWalkerId = _petWalkerId }
            });

        _mockRatingService
            .Setup(s => s.GetRatingSummaryAsync(_petWalkerId))
            .ReturnsAsync(new RatingSummaryDto(
                PetWalkerId: _petWalkerId,
                AverageRating: 4.0,
                TotalRatings: 15,
                RecentRatings: new List<RatingDto>()));

        _mockRatingService
            .Setup(s => s.GetRatingsAsync(_petWalkerId, 1, 10))
            .ReturnsAsync(page1Response);
        _mockRatingService
            .Setup(s => s.GetRatingsAsync(_petWalkerId, 2, 10))
            .ReturnsAsync(page2Response);

        // Act
        var cut = RenderComponent<PetWalkerViewPopup>(parameters => parameters
            .Add(p => p.PetWalkerEmail, _petWalkerEmail));

        // Initially shows page 1 users
        Assert.Contains("User1", cut.Markup);
        Assert.DoesNotContain("User11", cut.Markup);

        // Navigate to page 2
        var nextButton = cut.FindAll(".ratings-pagination button")[1];
        nextButton.Click();

        // Now shows page 2 users
        Assert.Contains("User11", cut.Markup);
        Assert.Contains("Page 2 of 2", cut.Markup);
    }

    [Fact]
    public void ShouldNavigateToPreviousPage()
    {
        // Arrange
        var dto = CreatePetWalkerDetailDto(_petWalkerId, _petWalkerEmail);
        var page1Ratings = Enumerable.Range(1, 10).Select(i =>
            CreateRatingDto(5, $"User{i}")
        ).ToList();
        var page2Ratings = Enumerable.Range(11, 5).Select(i =>
            CreateRatingDto(4, $"User{i}")
        ).ToList();

        var page1Response = CreatePaginatedResponse(page1Ratings, page: 1, pageSize: 10, totalCount: 15);
        var page2Response = CreatePaginatedResponse(page2Ratings, page: 2, pageSize: 10, totalCount: 15);

        _mockPetWalkerService
            .Setup(s => s.GetPetWalkerDetailsByEmailAsync(_petWalkerEmail))
            .ReturnsAsync(new ApiResponse<PetWalkerDetailDto> { Success = true, Data = dto });

        _mockScheduleService
            .Setup(s => s.GetScheduleAsync(_petWalkerId))
            .ReturnsAsync(new ApiResponse<GetScheduleResponseDto>
            {
                Success = true,
                Data = new GetScheduleResponseDto { PetWalkerId = _petWalkerId }
            });

        _mockRatingService
            .Setup(s => s.GetRatingSummaryAsync(_petWalkerId))
            .ReturnsAsync(new RatingSummaryDto(
                PetWalkerId: _petWalkerId,
                AverageRating: 4.0,
                TotalRatings: 15,
                RecentRatings: new List<RatingDto>()));

        _mockRatingService
            .Setup(s => s.GetRatingsAsync(_petWalkerId, 1, 10))
            .ReturnsAsync(page1Response);
        _mockRatingService
            .Setup(s => s.GetRatingsAsync(_petWalkerId, 2, 10))
            .ReturnsAsync(page2Response);

        // Act
        var cut = RenderComponent<PetWalkerViewPopup>(parameters => parameters
            .Add(p => p.PetWalkerEmail, _petWalkerEmail));

        // Go to page 2
        var nextButton = cut.FindAll(".ratings-pagination button")[1];
        nextButton.Click();

        Assert.Contains("User11", cut.Markup);
        Assert.Contains("Page 2 of 2", cut.Markup);

        // Go back to page 1
        var prevButton = cut.FindAll(".ratings-pagination button")[0];
        prevButton.Click();

        Assert.Contains("User1", cut.Markup);
        Assert.Contains("Page 1 of 2", cut.Markup);
    }

    [Fact]
    public async Task ShouldRefreshRatingsWhenRatingSubmitted()
    {
        // Arrange
        var dto = CreatePetWalkerDetailDto(_petWalkerId, _petWalkerEmail);

        _mockPetWalkerService
            .Setup(s => s.GetPetWalkerDetailsByEmailAsync(_petWalkerEmail))
            .ReturnsAsync(new ApiResponse<PetWalkerDetailDto> { Success = true, Data = dto });

        _mockScheduleService
            .Setup(s => s.GetScheduleAsync(_petWalkerId))
            .ReturnsAsync(new ApiResponse<GetScheduleResponseDto>
            {
                Success = true,
                Data = new GetScheduleResponseDto { PetWalkerId = _petWalkerId }
            });

        _mockRatingService
            .Setup(s => s.GetRatingSummaryAsync(_petWalkerId))
            .ReturnsAsync(new RatingSummaryDto(
                PetWalkerId: _petWalkerId,
                AverageRating: 0,
                TotalRatings: 0,
                RecentRatings: new List<RatingDto>()));

        _mockRatingService
            .Setup(s => s.GetRatingsAsync(_petWalkerId, 1, 10))
            .ReturnsAsync(CreatePaginatedResponse(new List<RatingDto>(), totalCount: 0));

        var cut = RenderComponent<PetWalkerViewPopup>(parameters => parameters
            .Add(p => p.PetWalkerEmail, _petWalkerEmail));

        // Reset call counts after initialization
        _mockRatingService.Invocations.Clear();

        // Act - simulate rating submitted via the bUnit dispatcher
        await cut.InvokeAsync(() => cut.Instance.HandleRatingSubmitted());

        // Assert - rating summary and list were refreshed
        _mockRatingService.Verify(s => s.GetRatingSummaryAsync(_petWalkerId), Times.Once);
        _mockRatingService.Verify(s => s.GetRatingsAsync(_petWalkerId, 1, 10), Times.Once);
    }
}
