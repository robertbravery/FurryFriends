using System.Net;
using System.Net.Http.Json;
using Ardalis.Result;
using FluentAssertions;
using FurryFriends.Core.Enums;
using FurryFriends.Web.Endpoints.TimeslotEndpoints.Timeslot;

namespace FurryFriends.FunctionalTests.ApiEndpoints.TimeslotEndpoints.Timeslot;

public class GetTimeslotsTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private const string BASE_URL = "/timeslots";

    public GetTimeslotsTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetTimeslots_ValidRequest_ReturnsSuccess()
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

        // Create available timeslots
        var timeslotRequest1 = new
        {
            petWalkerId = petWalkerId,
            date = date.ToString("yyyy-MM-dd"),
            startTime = "09:00",
            durationInMinutes = 30
        };
        await _client.PostAsJsonAsync("/timeslots", timeslotRequest1);

        var timeslotRequest2 = new
        {
            petWalkerId = petWalkerId,
            date = date.ToString("yyyy-MM-dd"),
            startTime = "10:00",
            durationInMinutes = 30
        };
        await _client.PostAsJsonAsync("/timeslots", timeslotRequest2);

        // Act
        var response = await _client.GetAsync($"{BASE_URL}?petWalkerId={petWalkerId}&date={date.ToString("yyyy-MM-dd")}");
        var result = await response.Content.ReadFromJsonAsync<Result<GetTimeslotsResponse>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.PetWalkerId.Should().Be(petWalkerId);
        result.Value.Date.Should().Be(date);
        result.Value.Timeslots.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetTimeslots_NoTimeslots_ReturnsEmptyList()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Today.AddDays(1));

        // Act
        var response = await _client.GetAsync($"{BASE_URL}?petWalkerId={petWalkerId}&date={date.ToString("yyyy-MM-dd")}");
        var result = await response.Content.ReadFromJsonAsync<Result<GetTimeslotsResponse>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Timeslots.Should().BeEmpty();
    }

    [Fact]
    public async Task GetTimeslots_InvalidPetWalker_ReturnsNotFound()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.Today.AddDays(1));

        // Act
        var response = await _client.GetAsync($"{BASE_URL}?petWalkerId={petWalkerId}&date={date.ToString("yyyy-MM-dd")}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
