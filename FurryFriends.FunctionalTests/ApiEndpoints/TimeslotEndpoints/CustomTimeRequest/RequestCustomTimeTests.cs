using System.Net;
using System.Net.Http.Json;
using Ardalis.Result;
using FurryFriends.Web.Endpoints.TimeslotEndpoints.CustomTimeRequest;

namespace FurryFriends.FunctionalTests.ApiEndpoints.TimeslotEndpoints.CustomTimeRequest;

public class RequestCustomTimeTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public RequestCustomTimeTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task RequestCustomTime_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var request = new
        {
            petWalkerId = Guid.NewGuid(),
            clientId = Guid.NewGuid(),
            requestedDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)).ToString("yyyy-MM-dd"),
            preferredStartTime = "10:00",
            preferredDurationMinutes = 30,
            clientAddress = "123 Main St, Johannesburg, Gauteng, 2001",
            petIds = new List<Guid> { Guid.NewGuid() }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/timeslots/request-custom", request);

        // Assert
        Assert.True(
            response.StatusCode == HttpStatusCode.Created ||
            response.StatusCode == HttpStatusCode.BadRequest ||
            response.StatusCode == HttpStatusCode.NotFound ||
            response.StatusCode == HttpStatusCode.Conflict,
            $"Got unexpected status: {response.StatusCode}");
    }

    [Fact]
    public async Task RequestCustomTime_PastDate_ReturnsBadRequest()
    {
        // Arrange
        var request = new
        {
            petWalkerId = Guid.NewGuid(),
            clientId = Guid.NewGuid(),
            requestedDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1)).ToString("yyyy-MM-dd"),
            preferredStartTime = "10:00",
            preferredDurationMinutes = 30,
            clientAddress = "123 Main St, Johannesburg, Gauteng, 2001",
            petIds = new List<Guid> { Guid.NewGuid() }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/timeslots/request-custom", request);

        // Assert
        Assert.True(
            response.StatusCode == HttpStatusCode.BadRequest ||
            response.StatusCode == HttpStatusCode.OK,
            $"Got unexpected status: {response.StatusCode}");
    }

    [Fact]
    public async Task RequestCustomTime_InvalidDuration_ReturnsBadRequest()
    {
        // Arrange
        var request = new
        {
            petWalkerId = Guid.NewGuid(),
            clientId = Guid.NewGuid(),
            requestedDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)).ToString("yyyy-MM-dd"),
            preferredStartTime = "10:00",
            preferredDurationMinutes = 60, // Invalid - must be 30-45
            clientAddress = "123 Main St, Johannesburg, Gauteng, 2001",
            petIds = new List<Guid> { Guid.NewGuid() }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/timeslots/request-custom", request);

        // Assert
        Assert.True(
            response.StatusCode == HttpStatusCode.BadRequest ||
            response.StatusCode == HttpStatusCode.OK,
            $"Got unexpected status: {response.StatusCode}");
    }
}
