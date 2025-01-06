using FurryFriends.Web.Endpoints.UserEndpoints.Get;
using System.Net;
using FluentAssertions;

namespace FurryFriends.FunctionalTests.ApiEndpoints.PetWalker;

public class GetPetWalkerByEmailTests(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
  private readonly HttpClient _client = factory.CreateClient();
  private const string URL = "/user/email/";

  [Fact]
  public async Task ReturnsUserByEmail()
  {
    // Arrange
    var email = "test2@u.com"; //Should be inserted with test data when data seed is run
    var endpoint = $"{URL}{email}";

    // Act
    var response = await _client.GetAsync(endpoint, TestContext.Current.CancellationToken);
    var result = await _client.GetAndDeserializeAsync<GetUserByEmailResponse>(endpoint);

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
    var response = await _client.GetAsync(endpoint, TestContext.Current.CancellationToken);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.NotFound);
  }

  [Fact]
  public async Task ReturnsBadRequestForInvalidEmail()
  {
    // Arrange
    var email = "invalid-email";
    var endpoint = $"{URL}{email}";

    // Act
    var response = await _client.GetAsync(endpoint, TestContext.Current.CancellationToken);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
  }

  [Theory]
  [InlineData("")]
  [InlineData("invalid-email")]
  [InlineData("@.com")]
  public async Task ReturnsBadRequestGivenInvalidEmailFormat(string invalidEmail)
  {
    //Arrange 
    var endpoint = $"{URL}{invalidEmail}";

    // Act
    var response = await _client.GetAsync(endpoint, TestContext.Current.CancellationToken);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
  }

}

