using System.Net;
using System.Net.Http.Json;
using Ardalis.Result;
using FluentAssertions;
using FurryFriends.Web.Endpoints.TimeslotEndpoints.WorkingHours;

namespace FurryFriends.FunctionalTests.ApiEndpoints.TimeslotEndpoints.WorkingHours;

public class DeleteWorkingHoursTests(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task DeletesWorkingHours_WhenValidIdProvided()
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

        // Act - Delete the working hours
        var url = $"/working-hours/{workingHoursId}";
        var deleteResponse = await _client.DeleteAsync(url);
        var deleteResult = await deleteResponse.Content.ReadFromJsonAsync<Result<DeleteWorkingHoursResponse>>();

        // Assert
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        deleteResult.Should().NotBeNull();
        deleteResult!.Value.Success.Should().BeTrue();
    }

    [Fact]
    public async Task ReturnsNotFound_WhenWorkingHoursNotFound()
    {
        // Arrange
        var workingHoursId = Guid.NewGuid();

        // Act
        var url = $"/working-hours/{workingHoursId}";
        var response = await _client.DeleteAsync(url);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ReturnsNotFound_AfterDeletingWorkingHours()
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

        // First delete
        var deleteUrl = $"/working-hours/{workingHoursId}";
        await _client.DeleteAsync(deleteUrl);

        // Act - Try to delete again
        var secondDeleteResponse = await _client.DeleteAsync(deleteUrl);

        // Assert
        secondDeleteResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task SuccessfullyDeletes_AndVerifiesWithGet()
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

        // Act - Delete the working hours
        var deleteUrl = $"/working-hours/{workingHoursId}";
        await _client.DeleteAsync(deleteUrl);

        // Verify it's deleted - should still return OK for petwalker but no working hours
        var getUrl = $"/working-hours/{petWalkerId}";
        var getResponse = await _client.GetAsync(getUrl);
        var getResult = await getResponse.Content.ReadFromJsonAsync<Result<GetWorkingHoursResponse>>();

        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        getResult.Should().NotBeNull();
        getResult!.Value.WorkingHours.Should().BeEmpty();
    }
}
