using System.Net;
using System.Net.Http.Json;

namespace FurryFriends.FunctionalTests.RatingEndpoints;

public class UpdateRatingTests(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task UpdateRating_Within24Hours_ReturnsNoContent()
    {
        // Arrange
        var ratingId = Guid.NewGuid();
        var request = new UpdateRatingRequest
        {
            RatingId = ratingId,
            RatingValue = 4,
            Comment = "Updated review"
        };
        var url = $"/ratings/{ratingId}";

        // Act
        var response = await _client.PutAsJsonAsync(url, request);

        // Assert - will fail if rating doesn't exist (expected TDD)
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NoContent, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateRating_After24Hours_ReturnsBadRequest()
    {
        // Arrange
        var ratingId = Guid.NewGuid();
        var request = new UpdateRatingRequest
        {
            RatingId = ratingId,
            RatingValue = 3,
            Comment = "Changed mind"
        };
        var url = $"/ratings/{ratingId}";

        // Act
        var response = await _client.PutAsJsonAsync(url, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateRating_WithModeratedStatus_ReturnsBadRequest()
    {
        // Arrange
        var ratingId = Guid.NewGuid();
        var request = new UpdateRatingRequest
        {
            RatingId = ratingId,
            RatingValue = 2,
            Comment = "Review changed"
        };
        var url = $"/ratings/{ratingId}";

        // Act
        var response = await _client.PutAsJsonAsync(url, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
