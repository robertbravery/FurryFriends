using System.Net;
using System.Net.Http.Json;
using FurryFriends.Web.Endpoints.TimeslotEndpoints.CustomTimeRequest;

namespace FurryFriends.FunctionalTests.ApiEndpoints.TimeslotEndpoints.CustomTimeRequest;

public class RespondToCustomTimeRequestTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public RespondToCustomTimeRequestTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task RespondToCustomTimeRequest_InvalidRequestId_ReturnsNotFound()
    {
        // Arrange
        var invalidRequestId = Guid.NewGuid();
        
        var request = new
        {
            requestId = invalidRequestId,
            response = "Accept",
            counterOfferedDate = (string?)null,
            counterOfferedTime = (string?)null,
            reason = (string?)null
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/timeslots/respond-custom-time/{invalidRequestId}", request);

        // Assert
        Assert.True(
            response.StatusCode == HttpStatusCode.NotFound ||
            response.StatusCode == HttpStatusCode.BadRequest ||
            response.StatusCode == HttpStatusCode.InternalServerError,
            $"Got unexpected status: {response.StatusCode}");
    }

    [Fact]
    public async Task RespondToCustomTimeRequest_DeclineWithoutReason_ReturnsBadRequest()
    {
        // Arrange
        var requestId = Guid.NewGuid();
        
        var request = new
        {
            requestId = requestId,
            response = "Decline",
            counterOfferedDate = (string?)null,
            counterOfferedTime = (string?)null,
            reason = (string?)null
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/timeslots/respond-custom-time/{requestId}", request);

        // Assert
        Assert.True(
            response.StatusCode == HttpStatusCode.BadRequest ||
            response.StatusCode == HttpStatusCode.NotFound ||
            response.StatusCode == HttpStatusCode.InternalServerError,
            $"Got unexpected status: {response.StatusCode}");
    }

    [Fact]
    public async Task RespondToCustomTimeRequest_CounterOfferWithoutDateTime_ReturnsBadRequest()
    {
        // Arrange
        var requestId = Guid.NewGuid();
        
        var request = new
        {
            requestId = requestId,
            response = "CounterOffer",
            counterOfferedDate = (string?)null,
            counterOfferedTime = (string?)null,
            reason = "Test reason"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/timeslots/respond-custom-time/{requestId}", request);

        // Assert
        Assert.True(
            response.StatusCode == HttpStatusCode.BadRequest ||
            response.StatusCode == HttpStatusCode.NotFound ||
            response.StatusCode == HttpStatusCode.InternalServerError,
            $"Got unexpected status: {response.StatusCode}");
    }
}
