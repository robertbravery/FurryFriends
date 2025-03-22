using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using FurryFriends.Web.Endpoints.Base;
using FurryFriends.Web.Endpoints.PetWalkerEndpoints.Records;

namespace FurryFriends.FunctionalTests.ApiEndpoints.PetWalker;

public class GetPetWalkerByEmailTests(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
  private readonly HttpClient _client = factory.CreateClient();
  private const string URL = "/PetWalker/email/";

  [Fact]
  public async Task ReturnsUserByEmail_WhenEmailExists()
  {
    // Arrange
    var email = "test2@u.com"; // Ensure this email exists in your test data
    var endpoint = $"{URL}{email}";

    // Act
    var response = await _client.GetAsync(endpoint, CancellationToken.None);
    var result = await response.Content.ReadFromJsonAsync<ResponseBase<PetWalkerRecord>>();

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
    result.Should().NotBeNull();
    result!.Data.Should().NotBeNull();
    result.Data!.Email.Should().Be(email);
  }

  [Fact]
  public async Task ReturnsNotFoundForNonExistentUser()
  {
    // Arrange
    var email = "nonexistentuser@example.com";
    var endpoint = $"{URL}{email}";

    // Act
    var response = await _client.GetAsync(endpoint, CancellationToken.None);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.NotFound);
  }

  [Theory]
  [InlineData("invalid-email")]
  [InlineData("@.com")]
  public async Task ReturnsBadRequestGivenInvalidEmailFormat(string invalidEmail)
  {
    //Arrange 
    var endpoint = $"{URL}{invalidEmail}";

    // Act
    var response = await _client.GetAsync(endpoint, CancellationToken.None);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
  }

  [Theory]
  [InlineData("")]
  [InlineData("   ")]
  public async Task ReturnsNotFoundRequestGivenNoEmail(string invalidEmail)
  {
    //Arrange 
    var endpoint = $"{URL}{invalidEmail}";

    // Act
    var response = await _client.GetAsync(endpoint, CancellationToken.None);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.NotFound);
  }

}

