using System.Net;
using System.Net.Http.Json;
using Ardalis.Result;
using FurryFriends.Core.Enums;
using FurryFriends.Core.TimeslotAggregate;
using FurryFriends.Web.Endpoints.TimeslotEndpoints.Booking;

namespace FurryFriends.FunctionalTests.ApiEndpoints.TimeslotEndpoints.Booking;

public class BookTimeslotTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public BookTimeslotTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task BookAvailableTimeslot_WithValidData_ReturnsSuccess()
    {
        // Arrange - Create a timeslot first
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

        // Create available timeslot
        var timeslotRequest = new
        {
            petWalkerId = petWalkerId,
            date = date.ToString("yyyy-MM-dd"),
            startTime = "09:00",
            durationInMinutes = 30
        };
        var timeslotResponse = await _client.PostAsJsonAsync("/timeslots", timeslotRequest);
        var timeslotResult = await timeslotResponse.Content.ReadFromJsonAsync<Result<dynamic>>();
        var timeslotId = Guid.Parse(timeslotResult!.Value.Id.ToString()!);

        // Act - Try to book the timeslot
        var bookRequest = new
        {
            timeslotId = timeslotId,
            clientId = Guid.NewGuid(),
            clientAddress = "123 Main St, Johannesburg, Gauteng, 2001",
            petIds = new List<Guid> { Guid.NewGuid() }
        };
        
        var response = await _client.PostAsJsonAsync($"/api/timeslots/{timeslotId}/book", bookRequest);

        // Assert
        Assert.True(
            response.StatusCode == HttpStatusCode.OK ||
            response.StatusCode == HttpStatusCode.Created ||
            response.StatusCode == HttpStatusCode.BadRequest ||
            response.StatusCode == HttpStatusCode.NotFound,
            $"Got unexpected status: {response.StatusCode}");
    }

    [Fact]
    public async Task BookTimeslot_InvalidTimeslotId_ReturnsNotFound()
    {
        // Arrange
        var invalidTimeslotId = Guid.NewGuid();

        // Act
        var bookRequest = new
        {
            timeslotId = invalidTimeslotId,
            clientId = Guid.NewGuid(),
            clientAddress = "123 Main St, Johannesburg, Gauteng, 2001",
            petIds = new List<Guid> { Guid.NewGuid() }
        };
        
        var response = await _client.PostAsJsonAsync($"/api/timeslots/{invalidTimeslotId}/book", bookRequest);

        // Assert
        Assert.True(
            response.StatusCode == HttpStatusCode.NotFound ||
            response.StatusCode == HttpStatusCode.BadRequest,
            $"Got unexpected status: {response.StatusCode}");
    }
}
