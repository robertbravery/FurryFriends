using System.Net;
using System.Net.Http.Json;
using Ardalis.Result;
using FluentAssertions;
using FurryFriends.Web.Endpoints.TimeslotEndpoints.WorkingHours;

namespace FurryFriends.FunctionalTests.ApiEndpoints.TimeslotEndpoints.WorkingHours;

public class UpdateWorkingHoursTests(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task UpdatesWorkingHours_WhenValidDataProvided()
    {
        // Arrange - First create working hours
        var petWalkerId = Guid.NewGuid();
        var createRequest = new CreateWorkingHoursRequest
        {
            PetWalkerId = petWalkerId,
            DayOfWeek = DayOfWeek.Monday,
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(17, 0),
            IsActive = true
        };
        var createResponse = await _client.PostAsJsonAsync("/working-hours", createRequest);
        var createResult = await createResponse.Content.ReadFromJsonAsync<Result<CreateWorkingHoursResponse>>();
        var workingHoursId = createResult!.Value.Id;

        // Act - Update the working hours
        var updateRequest = new UpdateWorkingHoursRequest
        {
            Id = workingHoursId,
            StartTime = new TimeOnly(8, 0),
            EndTime = new TimeOnly(16, 0),
            IsActive = true
        };
        var url = $"/working-hours/{workingHoursId}";
        var updateResponse = await _client.PutAsJsonAsync(url, updateRequest);
        var updateResult = await updateResponse.Content.ReadFromJsonAsync<Result<UpdateWorkingHoursResponse>>();

        // Assert
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        updateResult.Should().NotBeNull();
        updateResult!.Value.Should().NotBeNull();
        updateResult.Value.StartTime.Should().Be(new TimeOnly(8, 0));
        updateResult.Value.EndTime.Should().Be(new TimeOnly(16, 0));
    }

    [Fact]
    public async Task ReturnsNotFound_WhenWorkingHoursNotFound()
    {
        // Arrange
        var workingHoursId = Guid.NewGuid();
        var updateRequest = new UpdateWorkingHoursRequest
        {
            Id = workingHoursId,
            StartTime = new TimeOnly(8, 0),
            EndTime = new TimeOnly(16, 0),
            IsActive = true
        };

        // Act
        var url = $"/working-hours/{workingHoursId}";
        var response = await _client.PutAsJsonAsync(url, updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ReturnsBadRequest_WhenEndTimeBeforeStartTime()
    {
        // Arrange - First create working hours
        var petWalkerId = Guid.NewGuid();
        var createRequest = new CreateWorkingHoursRequest
        {
            PetWalkerId = petWalkerId,
            DayOfWeek = DayOfWeek.Monday,
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(17, 0),
            IsActive = true
        };
        var createResponse = await _client.PostAsJsonAsync("/working-hours", createRequest);
        var createResult = await createResponse.Content.ReadFromJsonAsync<Result<CreateWorkingHoursResponse>>();
        var workingHoursId = createResult!.Value.Id;

        // Act - Try to update with invalid time
        var updateRequest = new UpdateWorkingHoursRequest
        {
            Id = workingHoursId,
            StartTime = new TimeOnly(17, 0),
            EndTime = new TimeOnly(9, 0),
            IsActive = true
        };
        var url = $"/working-hours/{workingHoursId}";
        var updateResponse = await _client.PutAsJsonAsync(url, updateRequest);

        // Assert
        updateResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdatesIsActiveStatus()
    {
        // Arrange - First create working hours
        var petWalkerId = Guid.NewGuid();
        var createRequest = new CreateWorkingHoursRequest
        {
            PetWalkerId = petWalkerId,
            DayOfWeek = DayOfWeek.Monday,
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(17, 0),
            IsActive = true
        };
        var createResponse = await _client.PostAsJsonAsync("/working-hours", createRequest);
        var createResult = await createResponse.Content.ReadFromJsonAsync<Result<CreateWorkingHoursResponse>>();
        var workingHoursId = createResult!.Value.Id;

        // Act - Update isActive to false
        var updateRequest = new UpdateWorkingHoursRequest
        {
            Id = workingHoursId,
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(17, 0),
            IsActive = false
        };
        var url = $"/working-hours/{workingHoursId}";
        var updateResponse = await _client.PutAsJsonAsync(url, updateRequest);
        var updateResult = await updateResponse.Content.ReadFromJsonAsync<Result<UpdateWorkingHoursResponse>>();

        // Assert
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        updateResult.Should().NotBeNull();
        updateResult!.Value.IsActive.Should().BeFalse();
    }
}
