using System.Net;
using System.Net.Http.Json;
using Ardalis.Result;
using FluentAssertions;
using FurryFriends.Web.Endpoints.TimeslotEndpoints.WorkingHours;

namespace FurryFriends.FunctionalTests.ApiEndpoints.TimeslotEndpoints.WorkingHours;

public class GetWorkingHoursTests(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task ReturnsEmptyList_WhenNoWorkingHoursExist()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var url = $"/working-hours/{petWalkerId}";

        // Act
        var response = await _client.GetAsync(url);
        var result = await response.Content.ReadFromJsonAsync<Result<GetWorkingHoursResponse>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.Value.Should().NotBeNull();
        result.Value.WorkingHours.Should().BeEmpty();
    }

    [Fact]
    public async Task ReturnsWorkingHours_WhenTheyExist()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        
        // First create working hours
        var createRequest = new CreateWorkingHoursRequest
        {
            PetWalkerId = petWalkerId,
            DayOfWeek = DayOfWeek.Monday,
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(17, 0),
            IsActive = true
        };
        await _client.PostAsJsonAsync("/working-hours", createRequest);

        // Act
        var url = $"/working-hours/{petWalkerId}";
        var response = await _client.GetAsync(url);
        var result = await response.Content.ReadFromJsonAsync<Result<GetWorkingHoursResponse>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.Value.Should().NotBeNull();
        result.Value.WorkingHours.Should().NotBeEmpty();
    }

    [Fact]
    public async Task ReturnsWorkingHoursForSpecificDay()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        
        // Create working hours for different days
        var mondayRequest = new CreateWorkingHoursRequest
        {
            PetWalkerId = petWalkerId,
            DayOfWeek = DayOfWeek.Monday,
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(17, 0),
            IsActive = true
        };
        await _client.PostAsJsonAsync("/working-hours", mondayRequest);

        var fridayRequest = new CreateWorkingHoursRequest
        {
            PetWalkerId = petWalkerId,
            DayOfWeek = DayOfWeek.Friday,
            StartTime = new TimeOnly(8, 0),
            EndTime = new TimeOnly(16, 0),
            IsActive = true
        };
        await _client.PostAsJsonAsync("/working-hours", fridayRequest);

        // Act
        var url = $"/working-hours/{petWalkerId}";
        var response = await _client.GetAsync(url);
        var result = await response.Content.ReadFromJsonAsync<Result<GetWorkingHoursResponse>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.Value.Should().NotBeNull();
        result.Value.WorkingHours.Should().HaveCount(2);
    }
}
