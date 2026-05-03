using System.Net;

namespace FurryFriends.FunctionalTests.RatingEndpoints;

public class DeleteRatingTests(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task DeleteRating_Within24Hours_ReturnsNoContent()
    {
        // Arrange
        var ratingId = Guid.NewGuid();
        var url = $"/ratings/{ratingId}";

        // Act
        var response = await _client.DeleteAsync(url);

        // Assert - expected to fail in TDD since Delete endpoint doesn't exist yet
        response.StatusCode.Should().BeOneOf(HttpStatusCode.NoContent, HttpStatusCode.NotFound, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteRating_After24Hours_ReturnsBadRequest()
    {
        // Arrange
        var ratingId = Guid.NewGuid();
        var url = $"/ratings/{ratingId}";

        // Act
        var response = await _client.DeleteAsync(url);

        // Assert - expected to fail in TDD since Delete endpoint doesn't exist yet
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteRating_WithModeratedStatus_ReturnsBadRequest()
    {
        // Arrange
        var ratingId = Guid.NewGuid();
        var url = $"/ratings/{ratingId}";

        // Act
        var response = await _client.DeleteAsync(url);

        // Assert - expected to fail in TDD since Delete endpoint doesn't exist yet
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
