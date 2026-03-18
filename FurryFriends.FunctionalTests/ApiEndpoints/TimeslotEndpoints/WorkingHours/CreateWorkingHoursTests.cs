using System.Net;
using System.Net.Http.Json;
using Ardalis.Result;
using FluentAssertions;
using FurryFriends.Web.Endpoints.TimeslotEndpoints.WorkingHours;

namespace FurryFriends.FunctionalTests.ApiEndpoints.TimeslotEndpoints.WorkingHours;

public class CreateWorkingHoursTests(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private const string URL = "/working-hours";

    [Fact]
    public async Task CreatesWorkingHours_WhenValidDataProvided()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var request = new CreateWorkingHoursRequest
        {
            PetWalkerId = petWalkerId,
            DayOfWeek = DayOfWeek.Monday,
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(17, 0),
            IsActive = true
        };

        // Act
        var response = await _client.PostAsJsonAsync(URL, request);
        var result = await response.Content.ReadFromJsonAsync<Result<CreateWorkingHoursResponse>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.Value.Should().NotBeNull();
        result.Value.Id.Should().NotBeEmpty();
    }

    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000000", DayOfWeek.Monday)]
    [InlineData("00000000-0000-0000-0000-000000000000", DayOfWeek.Sunday)]
    public async Task ReturnsBadRequest_WhenInvalidPetWalkerId(Guid petWalkerId, DayOfWeek dayOfWeek)
    {
        // Arrange
        var request = new CreateWorkingHoursRequest
        {
            PetWalkerId = petWalkerId,
            DayOfWeek = dayOfWeek,
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(17, 0),
            IsActive = true
        };

        // Act
        var response = await _client.PostAsJsonAsync(URL, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ReturnsBadRequest_WhenEndTimeBeforeStartTime()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var request = new CreateWorkingHoursRequest
        {
            PetWalkerId = petWalkerId,
            DayOfWeek = DayOfWeek.Monday,
            StartTime = new TimeOnly(17, 0),
            EndTime = new TimeOnly(9, 0),
            IsActive = true
        };

        // Act
        var response = await _client.PostAsJsonAsync(URL, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ReturnsBadRequest_WhenOverlappingShifts()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        
        // First create a working hours entry
        var firstRequest = new CreateWorkingHoursRequest
        {
            PetWalkerId = petWalkerId,
            DayOfWeek = DayOfWeek.Monday,
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(12, 0),
            IsActive = true
        };
        await _client.PostAsJsonAsync(URL, firstRequest);

        // Try to create overlapping shift
        var overlappingRequest = new CreateWorkingHoursRequest
        {
            PetWalkerId = petWalkerId,
            DayOfWeek = DayOfWeek.Monday,
            StartTime = new TimeOnly(11, 0),
            EndTime = new TimeOnly(14, 0),
            IsActive = true
        };

        // Act
        var response = await _client.PostAsJsonAsync(URL, overlappingRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
