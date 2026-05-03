using System.Net;
using System.Net.Http.Json;

namespace FurryFriends.FunctionalTests.RatingEndpoints;

public class CreateRatingTests(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private const string URL = "/ratings";

    [Fact]
    public async Task CreateRating_WithValidEligibility_ReturnsCreated()
    {
        // Arrange
        var request = new CreateRatingRequest
        {
            BookingId = Guid.NewGuid(),
            RatingValue = 5,
            Comment = "Great service!"
        };

        // Act
        var response = await _client.PostAsJsonAsync(URL, request);
        var result = await response.Content.ReadFromJsonAsync<Result<CreateRatingResponse>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.Value.Should().NotBeNull();
        result.Value.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateRating_WithoutCompletedBooking_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateRatingRequest
        {
            BookingId = Guid.NewGuid(),
            RatingValue = 3,
            Comment = "Average"
        };

        // Act
        var response = await _client.PostAsJsonAsync(URL, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateRating_ExceedingBookingLimit_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateRatingRequest
        {
            BookingId = Guid.NewGuid(),
            RatingValue = 1,
            Comment = "Poor"
        };

        // Act
        var response = await _client.PostAsJsonAsync(URL, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
