using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using FurryFriends.Web.Endpoints.Base;
using FurryFriends.Web.Endpoints.ClientEnpoints.Get;

namespace FurryFriends.FunctionalTests.ApiEndpoints.Clients;

public class GetClientByEmailTests(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
  private readonly HttpClient _client = factory.CreateClient();
  private const string URL = "/Clients/email/";

  [Fact]
  public async Task ReturnsClientByEmail_WhenEmailExists()
  {
    // Arrange
    var email = "john.smith@example.com"; // Ensure this email exists in your test data
    var endpoint = $"{URL}{email}";

    // Act
    var response = await _client.GetAsync(endpoint, CancellationToken.None);
    var result = await response.Content.ReadFromJsonAsync<ResponseBase<ClientRecord>>();

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
    result.Should().NotBeNull();
    result!.Data.Should().NotBeNull();
    result.Data!.Email.Should().Be(email);
  }

  [Fact]
  public async Task ReturnsNotFound_WhenClientDoesNotExist()
  {
    // Arrange
    var email = "nonexistentclient@example.com";
    var endpoint = $"{URL}{email}";

    // Act
    var response = await _client.GetAsync(endpoint, CancellationToken.None);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.NotFound);
  }

  [Theory]
  [InlineData("invalid-email")]
  [InlineData("@.com")]
  [InlineData("test@")]
  public async Task ReturnsBadRequest_GivenInvalidEmailFormat(string invalidEmail)
  {
    // Arrange
    var endpoint = $"{URL}{invalidEmail}";

    // Act
    var response = await _client.GetAsync(endpoint, CancellationToken.None);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
  }

  [Theory]
  [InlineData("")]
  [InlineData("   ")]
  public async Task ReturnsNotFound_GivenEmptyEmail(string emptyEmail)
  {
    // Arrange
    var endpoint = $"{URL}{emptyEmail}";

    // Act
    var response = await _client.GetAsync(endpoint, CancellationToken.None);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.MethodNotAllowed);
  }

  [Fact]
  public async Task ReturnsCorrectClientDetails_WhenEmailExists()
  {
    // Arrange
    var email = "jane.doe@example.com"; // Ensure this email exists in your test data
    var endpoint = $"{URL}{email}";

    // Act
    var response = await _client.GetAsync(endpoint, CancellationToken.None);
    var result = await response.Content.ReadFromJsonAsync<ResponseBase<ClientRecord>>();

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
    result.Should().NotBeNull();
    result!.Data.Should().NotBeNull();
    result.Data!.Should().Match<ClientRecord>(c =>
        c.Email == email &&
        !string.IsNullOrEmpty(c.Name) &&
        !string.IsNullOrEmpty(c.PhoneNumber) &&
        !string.IsNullOrEmpty(c.City)
    );
  }
}
