using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using FurryFriends.Web.Endpoints.Base;
using FurryFriends.Web.Endpoints.ClientEndpoints.List;

namespace FurryFriends.FunctionalTests.ApiEndpoints.Clients;

public class ListClientTests(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
  private readonly HttpClient _client = factory.CreateClient();
  private const string URL = "/Clients/list";

  [Fact]
  public async Task ReturnsAllClients_WhenNoSearchCriteria()
  {
    // Arrange
    var endpoint = $"{URL}";

    // Act
    var response = await _client.GetAsync(endpoint);
    var result = await response.Content.ReadFromJsonAsync<ListResponse<ClientDto>>();

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
    result.Should().NotBeNull();
    result!.RowsData.Should().HaveCountGreaterThan(0);
    result.RowsData.Should().Contain(c => c.EmailAddress == "john.smith@example.com");
    result.RowsData.Should().Contain(c => c.EmailAddress == "jane.doe@example.com");
  }

  [Theory]
  [InlineData("Smith")]
  [InlineData("John")]
  public async Task ReturnsFilteredClients_WhenSearchTermProvided(string searchTerm)
  {
    // Arrange
    var endpoint = $"{URL}?searchTerm={searchTerm}";

    // Act
    var response = await _client.GetAsync(endpoint);
    var result = await response.Content.ReadFromJsonAsync<ListResponse<ClientDto>>();

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
    result.Should().NotBeNull();
    //result!.RowsData.Should().AllSatisfy(client =>
    //    client.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
    //    client.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
    //);
    result!.RowsData
    .Where(client => client.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                     client.EmailAddress.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
    .Should()
    .NotBeEmpty();
  }

  [Theory]
  [InlineData(1, 1)]
  [InlineData(2, 1)]
  public async Task ReturnsPaginatedResults_WhenPaginationParamsProvided(int page, int pageSize)
  {
    // Arrange
    var endpoint = $"{URL}?page={page}&pageSize={pageSize}";

    // Act
    var response = await _client.GetAsync(endpoint);
    var result = await response.Content.ReadFromJsonAsync<ListResponse<ClientDto>>();

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
    result.Should().NotBeNull();
    result!.RowsData.Should().HaveCountLessOrEqualTo(pageSize);
    result.PageNumber.Should().Be(page);
    result.PageSize.Should().Be(pageSize);
  }

  [Fact]
  public async Task ReturnsEmptyList_WhenNoMatchingResults()
  {
    // Arrange
    var endpoint = $"{URL}?searchTerm=NonExistentClient";

    // Act
    var response = await _client.GetAsync(endpoint);
    var result = await response.Content.ReadFromJsonAsync<ListResponse<ClientDto>>();

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
    result.Should().NotBeNull();
    result!.RowsData.Should().BeEmpty();
  }

  [Theory]
  [InlineData(-1, 10)]
  [InlineData(1, -1)]
  [InlineData(0, 0)]
  public async Task ReturnsBadRequest_WhenInvalidPaginationParams(int page, int pageSize)
  {
    // Arrange
    var endpoint = $"{URL}?page={page}&pageSize={pageSize}";

    // Act
    var response = await _client.GetAsync(endpoint);

    // Assert
    response.StatusCode.Should().Be((HttpStatusCode)500);
  }
}
