using System.Net;
using System.Net.Http.Json;
using Ardalis.Result;
using FluentAssertions;
using FurryFriends.Core.Enums;
using FurryFriends.Web.Endpoints.TimeslotEndpoints.Timeslot;

namespace FurryFriends.FunctionalTests.ApiEndpoints.TimeslotEndpoints.Timeslot;

public class CreateTimeslotTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private const string URL = "/timeslots";

    public CreateTimeslotTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateTimeslot_ValidRequest_ReturnsSuccess()
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

        var request = new
        {
            petWalkerId = petWalkerId,
            date = date.ToString("yyyy-MM-dd"),
            startTime = "09:00",
            durationInMinutes = 30
        };

        // Act
        var response = await _client.PostAsJsonAsync(URL, request);
        var result = await response.Content.ReadFromJsonAsync<Result<CreateTimeslotResponse>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.Value.Should().NotBeNull();
        result.Value.Id.Should().NotBeEmpty();
        result.Value.PetWalkerId.Should().Be(petWalkerId);
        result.Value.Date.Should().Be(date);
        result.Value.StartTime.Should().Be(TimeOnly.Parse("09:00"));
        result.Value.DurationInMinutes.Should().Be(30);
        result.Value.Status.Should().Be(TimeslotStatus.Available);
    }

    [Fact]
    public async Task CreateTimeslot_InvalidDuration_ReturnsBadRequest()
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

        var request = new
        {
            petWalkerId = petWalkerId,
            date = date.ToString("yyyy-MM-dd"),
            startTime = "09:00",
            durationInMinutes = 20 // Invalid - below 30
        };

        // Act
        var response = await _client.PostAsJsonAsync(URL, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateTimeslot_InvalidPetWalkerId_ReturnsBadRequest()
    {
        // Arrange
        var petWalkerId = Guid.Empty;
        var date = DateOnly.FromDateTime(DateTime.Today.AddDays(1));

        var request = new
        {
            petWalkerId = petWalkerId,
            date = date.ToString("yyyy-MM-dd"),
            startTime = "09:00",
            durationInMinutes = 30
        };

        // Act
        var response = await _client.PostAsJsonAsync(URL, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateTimeslot_PastDate_ReturnsBadRequest()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Today.AddDays(-1)); // Past date

        var request = new
        {
            petWalkerId = petWalkerId,
            date = date.ToString("yyyy-MM-dd"),
            startTime = "09:00",
            durationInMinutes = 30
        };

        // Act
        var response = await _client.PostAsJsonAsync(URL, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}

public class CreateTimeslotResponse
{
    public Guid Id { get; set; }
    public Guid PetWalkerId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int DurationInMinutes { get; set; }
    public TimeslotStatus Status { get; set; }
}
