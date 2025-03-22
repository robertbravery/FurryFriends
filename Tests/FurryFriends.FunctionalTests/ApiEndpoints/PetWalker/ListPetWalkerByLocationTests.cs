using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using FurryFriends.Web.Endpoints.PetWalkerEndpoints.List;

namespace FurryFriends.FunctionalTests.ApiEndpoints.PetWalker;

public class ListPetWalkerByLocationTests(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
  private readonly HttpClient _client = factory.CreateClient();
  private const string URL = "/PetWalker/location";

  [Fact]
  public async Task ReturnsUsersForValidLocation()
  {
    // Arrange
    var location = "929ccaf2-8c74-49bb-b9a0-ce26db0611ab";
    var endpoint = $"{URL}?locationId={location}&page=1&pageSize=10";
    var expectedEmail = "test@u.com";

    // Act
    var response = await _client.GetAsync(endpoint, CancellationToken.None);
    var result = await response.Content.ReadFromJsonAsync<ListPetWalkerByLocationResponse>(cancellationToken: CancellationToken.None);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
    result.Should().NotBeNull();
    result!.RowsData.Should().NotBeNull();
    result.RowsData.Count.Should().BeGreaterThan(0);
    result.RowsData.Should().Contain(user => user.EmailAddress == expectedEmail);
  }

  [Fact]
  public async Task ReturnsPaginatedResults()
  {
    // Arrange
    var location = "929ccaf2-8c74-49bb-b9a0-ce26db0611ab";
    var pageSize = 5;
    var endpoint = $"{URL}?locationId={location}&page=1&pageSize={pageSize}";

    // Act
    var response = await _client.GetAsync(endpoint, CancellationToken.None);
    var result = await response.Content.ReadFromJsonAsync<ListPetWalkerByLocationResponse>(cancellationToken: CancellationToken.None);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
    result!.RowsData?.Count.Should().BeGreaterThan(0);
    result!.RowsData?.Count.Should().BeLessOrEqualTo(pageSize);
    result.PageSize.Should().Be(pageSize);
  }

  [Theory]
  [InlineData(0, 10)]
  [InlineData(1, 0)]
  [InlineData(-1, -1)]
  public async Task ReturnsBadRequestForInvalidPagination(int page, int pageSize)
  {
    // Arrange
    var location = "44eec69e-c38a-4111-8825-bdc52a9303af";
    var endpoint = $"{URL}?locationId={location}&page={page}&pageSize={pageSize}";

    // Act
    var response = await _client.GetAsync(endpoint, CancellationToken.None);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
  }

  [Fact]
  public async Task ReturnsEmptyListForNonExistentLocation()
  {
    // Arrange
    var location = Guid.NewGuid().ToString();
    var endpoint = $"{URL}?locationId={location}&page=1&pageSize=10";

    // Act
    var response = await _client.GetAsync(endpoint, CancellationToken.None);
    var result = await response.Content.ReadFromJsonAsync<ListPetWalkerByLocationResponse>(cancellationToken: CancellationToken.None);
    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
    result!.RowsData.Should().BeEmpty();
  }

  [Fact]
  public async Task ReturnsFilteredResultsWithSearchTerm()
  {
    // Arrange
    var searchTerm = "test";
    var endpoint = $"{URL}?searchTerm={searchTerm}&page=1&pageSize=10";

    // Act
    var response = await _client.GetAsync(endpoint, CancellationToken.None);
    var result = await response.Content.ReadFromJsonAsync<ListPetWalkerByLocationResponse>(cancellationToken: CancellationToken.None);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
    result.Should().NotBeNull();
    result!.RowsData.Should().NotBeNull();
  }
}
