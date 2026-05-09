using System.Net;
using System.Net.Http.Json;
using FurryFriends.Web.Endpoints.RatingEndpoints.Create;

namespace FurryFriends.FunctionalTests.RatingEndpoints;

public class CreateRatingTests(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private const string URL = "/ratings";

    [Fact]
    public async Task CreateRating_WithoutCompletedBooking_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateRatingRequest
        {
            PetWalkerId = Guid.NewGuid(),
            ClientId = Guid.NewGuid(),
            RatingValue = 3,
            Comment = "Average"
        };

        // Act
        var response = await _client.PostAsJsonAsync(URL, request);

        // Assert - no completed bookings exist, so eligibility check fails
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateRating_WithInvalidRatingValue_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateRatingRequest
        {
            PetWalkerId = Guid.NewGuid(),
            ClientId = Guid.NewGuid(),
            RatingValue = 0,
            Comment = "Invalid"
        };

        // Act
        var response = await _client.PostAsJsonAsync(URL, request);

        // Assert - validation should catch the invalid rating value
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateRating_WithMissingPetWalkerId_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateRatingRequest
        {
            PetWalkerId = Guid.Empty,
            ClientId = Guid.NewGuid(),
            RatingValue = 5,
            Comment = "Great"
        };

        // Act
        var response = await _client.PostAsJsonAsync(URL, request);

        // Assert - validation should catch empty PetWalkerId
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateRating_WithMissingClientId_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateRatingRequest
        {
            PetWalkerId = Guid.NewGuid(),
            ClientId = Guid.Empty,
            RatingValue = 5,
            Comment = "Great"
        };

        // Act
        var response = await _client.PostAsJsonAsync(URL, request);

        // Assert - validation should catch empty ClientId
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
