using System.Net;
using System.Net.Http.Json;
using Ardalis.Result;
using FluentAssertions;
using FurryFriends.Core.Enums;

namespace FurryFriends.FunctionalTests.ApiEndpoints.TimeslotEndpoints.Timeslot;

public class DeleteTimeslotTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private const string URL = "/timeslots";

    public DeleteTimeslotTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task DeleteTimeslot_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        
        // First create working hours
        var workingHoursRequest = new
        {
            petWalkerId = petWalkerId,
            dayOfWeek = date.DayOfWeek,
            startTime = "08:00",
            endTime = "18:00",
            isActive = true
        };
        await _client.PostAsJsonAsync("/working-hours", workingHoursRequest);

        // Create a timeslot first
        var createRequest = new
        {
            petWalkerId = petWalkerId,
            date = date.ToString("yyyy-MM-dd"),
            startTime = "09:00",
            durationInMinutes = 30
        };
        var createResponse = await _client.PostAsJsonAsync(URL, createRequest);
        var createResult = await createResponse.Content.ReadFromJsonAsync<Result<CreateTimeslotResponse>>();
        var timeslotId = createResult!.Value.Id;

        // Act
        var response = await _client.DeleteAsync($"{URL}/{timeslotId}");
        var result = await response.Content.ReadFromJsonAsync<Result<bool>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteTimeslot_TimeslotNotFound_ReturnsNotFound()
    {
        // Arrange
        var timeslotId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"{URL}/{timeslotId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteTimeslot_BookedTimeslot_ReturnsBadRequest()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        
        // First create working hours
        var workingHoursRequest = new
        {
            petWalkerId = petWalkerId,
            dayOfWeek = date.DayOfWeek,
            startTime = "08:00",
            endTime = "18:00",
            isActive = true
        };
        await _client.PostAsJsonAsync("/working-hours", workingHoursRequest);

        // Create a timeslot first
        var createRequest = new
        {
            petWalkerId = petWalkerId,
            date = date.ToString("yyyy-MM-dd"),
            startTime = "09:00",
            durationInMinutes = 30
        };
        var createResponse = await _client.PostAsJsonAsync(URL, createRequest);
        var createResult = await createResponse.Content.ReadFromJsonAsync<Result<CreateTimeslotResponse>>();
        var timeslotId = createResult!.Value.Id;

        // Book the timeslot first
        var bookRequest = new
        {
            timeslotId = timeslotId,
            clientId = Guid.NewGuid(),
            dogId = Guid.NewGuid()
        };
        await _client.PostAsJsonAsync("/bookings", bookRequest);

        // Now try to delete the booked timeslot
        var response = await _client.DeleteAsync($"{URL}/{timeslotId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
