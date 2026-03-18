using System.Net;
using System.Net.Http.Json;
using Ardalis.Result;
using FluentAssertions;
using FurryFriends.Core.Enums;
using FurryFriends.Web.Endpoints.TimeslotEndpoints.Timeslot;

namespace FurryFriends.FunctionalTests.ApiEndpoints.TimeslotEndpoints.Timeslot;

public class UpdateTimeslotTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private const string URL = "/timeslots";

    public UpdateTimeslotTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task UpdateTimeslot_ValidRequest_ReturnsSuccess()
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

        // Now update the timeslot
        var updateRequest = new
        {
            timeslotId = timeslotId,
            startTime = "10:00",
            durationInMinutes = 30
        };

        // Act
        var response = await _client.PutAsJsonAsync($"{URL}/{timeslotId}", updateRequest);
        var result = await response.Content.ReadFromJsonAsync<Result<UpdateTimeslotResponse>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(timeslotId);
        result.Value.StartTime.Should().Be(TimeOnly.Parse("10:00"));
        result.Value.DurationInMinutes.Should().Be(30);
    }

    [Fact]
    public async Task UpdateTimeslot_InvalidDuration_ReturnsBadRequest()
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

        // Now try to update with invalid duration
        var updateRequest = new
        {
            timeslotId = timeslotId,
            startTime = "10:00",
            durationInMinutes = 20 // Invalid - below 30
        };

        // Act
        var response = await _client.PutAsJsonAsync($"{URL}/{timeslotId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateTimeslot_TimeslotNotFound_ReturnsNotFound()
    {
        // Arrange
        var timeslotId = Guid.NewGuid();

        var updateRequest = new
        {
            timeslotId = timeslotId,
            startTime = "10:00",
            durationInMinutes = 30
        };

        // Act
        var response = await _client.PutAsJsonAsync($"{URL}/{timeslotId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateTimeslot_BookedTimeslot_ReturnsBadRequest()
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

        // Now try to update the booked timeslot
        var updateRequest = new
        {
            timeslotId = timeslotId,
            startTime = "10:00",
            durationInMinutes = 30
        };

        // Act
        var response = await _client.PutAsJsonAsync($"{URL}/{timeslotId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
