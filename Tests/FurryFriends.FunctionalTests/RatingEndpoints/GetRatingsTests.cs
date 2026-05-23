using System.Net;
using System.Net.Http.Json;

namespace FurryFriends.FunctionalTests.RatingEndpoints;

public class GetRatingsTests(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetRatings_WithData_ReturnsPaginatedList()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var url = $"/petwalkers/{petWalkerId}/ratings";

        // Act
        var response = await _client.GetAsync(url);
        var result = await response.Content.ReadFromJsonAsync<Result<List<GetRatingsForPetWalkerResponse>>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task GetRatings_WithNoData_ReturnsEmptyList()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var url = $"/petwalkers/{petWalkerId}/ratings";

        // Act
        var response = await _client.GetAsync(url);
        var result = await response.Content.ReadFromJsonAsync<Result<List<GetRatingsForPetWalkerResponse>>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.Value.Should().BeEmpty();
    }
}
