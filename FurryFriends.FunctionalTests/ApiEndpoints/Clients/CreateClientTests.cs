using System.Net;
using System.Net.Http.Json;
using Ardalis.Result;
using FluentAssertions;
using FurryFriends.Core.ClientAggregate.Enums;
using FurryFriends.Web.Endpoints.ClientEnpoints.Create;

namespace FurryFriends.FunctionalTests.ApiEndpoints.Clients;

public class CreateClientTests(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
  private readonly HttpClient _client = factory.CreateClient();
  private const string URL = "/Clients";

  [Fact]
  public async Task CreatesClient_WhenValidDataProvided()
  {
    // Arrange
    var request = new CreateClientRequest
    {
      FirstName = "Test",
      LastName = "Client",
      Email = "testclient@example.com",
      PhoneCountryCode = "027",
      PhoneNumber = "555-123-4567",
      Street = "123 Test Street",
      City = "Test City",
      State = "Test State",
      Country = "US",
      ZipCode = "12345",
      ClientType = ClientType.Regular,
      PreferredContactTime = new TimeOnly(9, 0),
      ReferralSource = ReferralSource.Website,

    };

    // Act
    var response = await _client.PostAsJsonAsync(URL, request);
    var result = await response.Content.ReadFromJsonAsync<Result<CreateClientReponse>>();

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
    result.Should().NotBeNull();
    result!.Value.ClientId.Should().NotBeNull();
  }

  [Fact]
  public async Task CreatesPremiumClient_WhenValidDataProvided()
  {
    // Arrange
    var request = new CreateClientRequest
    {
      FirstName = "Premium",
      LastName = "Client",
      Email = "premium@example.com",
      PhoneCountryCode = "027",
      PhoneNumber = "555-999-8888",
      Street = "456 Premium Ave",
      City = "Premium City",
      State = "Premium State",
      Country = "US",
      ZipCode = "54321",
      ClientType = ClientType.Premium,
      PreferredContactTime = new TimeOnly(14, 0),
      ReferralSource = ReferralSource.Website
    };

    // Act
    var response = await _client.PostAsJsonAsync(URL, request);
    var result = await response.Content.ReadFromJsonAsync<CreateClientReponse>();

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
    result.Should().NotBeNull();
  }

  [Theory]
  [InlineData("", "LastName", "invalid@")]
  [InlineData("FirstName", "", "email@test.com")]
  [InlineData("FirstName", "LastName", "")]
  public async Task ReturnsBadRequest_WhenInvalidBasicData(string firstName, string lastName, string email)
  {
    // Arrange
    var request = new CreateClientRequest
    {
      FirstName = firstName,
      LastName = lastName,
      Email = email,
      PhoneCountryCode = "027",
      PhoneNumber = "555-123-4567",
      Street = "123 Test Street",
      City = "Test City",
      State = "Test State",
      Country = "US",
      ZipCode = "12345"
    };

    // Act
    var response = await _client.PostAsJsonAsync(URL, request);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
  }

  [Theory]
  [InlineData("123", "invalid-phone")]
  [InlineData("", "555-123-4567")]
  [InlineData("027", "")]
  public async Task ReturnsBadRequest_WhenInvalidPhoneData(string countryCode, string phoneNumber)
  {
    // Arrange
    var request = new CreateClientRequest
    {
      FirstName = "Test",
      LastName = "Client",
      Email = "test@example.com",
      PhoneCountryCode = countryCode,
      PhoneNumber = phoneNumber,
      Street = "123 Test Street",
      City = "Test City",
      State = "Test State",
      Country = "US",
      ZipCode = "12345"
    };

    // Act
    var response = await _client.PostAsJsonAsync(URL, request);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
  }

  [Theory]
  [InlineData("", "City", "State", "12345")]
  [InlineData("Street", "", "State", "12345")]
  [InlineData("Street", "City", "", "12345")]
  [InlineData("Street", "City", "State", "")]
  public async Task ReturnsBadRequest_WhenInvalidAddressData(string street, string city, string state, string zipCode)
  {
    // Arrange
    var request = new CreateClientRequest
    {
      FirstName = "Test",
      LastName = "Client",
      Email = "test@example.com",
      PhoneCountryCode = "027",
      PhoneNumber = "555-123-4567",
      Street = street,
      City = city,
      State = state,
      Country = "US",
      ZipCode = zipCode
    };

    // Act
    var response = await _client.PostAsJsonAsync(URL, request);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
  }

  [Fact]
  public async Task ReturnsBadRequest_WhenDuplicateEmail()
  {
    // Arrange
    var request = new CreateClientRequest
    {
      FirstName = "Test",
      LastName = "Client",
      Email = "john.smith@example.com", // Email from SeedData
      PhoneCountryCode = "027",
      PhoneNumber = "555-123-4567",
      Street = "123 Test Street",
      City = "Test City",
      State = "Test State",
      Country = "US",
      ZipCode = "12345"
    };

    // Act
    var response = await _client.PostAsJsonAsync(URL, request);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
  }
}
