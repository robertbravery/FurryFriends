using System.Net;
using System.Net.Http.Json;

namespace FurryFriends.FunctionalTests.RatingEndpoints;

public class GetRatingSummaryTests(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetSummary_WithRatings_ReturnsAverageAndCount()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var url = $"/petwalkers/{petWalkerId}/ratings/summary";

        // Act
        var response = await _client.GetAsync(url);
        var result = await response.Content.ReadFromJsonAsync<Result<GetPetWalkerRatingSummaryResponse>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task GetSummary_PetWalkerNotFound_ReturnsNotFound()
    {
        // Arrange
        var petWalkerId = Guid.Empty;
        var url = $"/petwalkers/{petWalkerId}/ratings/summary";

        // Act
        var response = await _client.GetAsync(url);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
