using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using FurryFriends.Web.Endpoints.BookingEndpoints.GetClientBookings;

namespace FurryFriends.FunctionalTests.BookingEndpoints;

/// <summary>
/// Guards the query-string date binding on the booking list endpoints.
/// These endpoints previously returned HTTP 500 with
/// "The type initializer for 'FastEndpoints.RequestBinder`1' threw an exception"
/// when a date range was supplied, because the DateTime properties were marked
/// with [FromQuery] (complex query object binding) instead of binding as scalars.
/// </summary>
public class GetBookingsDateBindingTests(CustomWebApplicationFactory<Program> factory)
  : IClassFixture<CustomWebApplicationFactory<Program>>
{
  private readonly HttpClient _client = factory.CreateClient();

  private const string DateQuery = "?startDate=2025-08-01&endDate=2026-10-12";

  [Fact]
  public async Task GetClientBookings_WithDateRange_DoesNotFail()
  {
    // Arrange
    var clientId = Guid.NewGuid();

    // Act
    var response = await _client.GetAsync($"/bookings/client/{clientId}{DateQuery}");

    // Assert - binding the dates must succeed; no bookings for a random id is fine.
    response.StatusCode.Should().Be(HttpStatusCode.OK);
  }

  [Fact]
  public async Task GetPetWalkerBookings_WithDateRange_DoesNotFail()
  {
    // Arrange
    var petWalkerId = Guid.NewGuid();

    // Act
    var response = await _client.GetAsync($"/bookings/petwalker/{petWalkerId}{DateQuery}");

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
  }

  [Fact]
  public async Task GetClientBookings_WithDateRange_ReturnsEmptyList_WhenNoBookingsExist()
  {
    // Arrange
    var clientId = Guid.NewGuid();

    // Act
    var response = await _client.GetAsync($"/bookings/client/{clientId}{DateQuery}");
    var bookings = await response.Content.ReadFromJsonAsync<List<ClientBookingResponse>>();

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
    bookings.Should().NotBeNull();
    bookings.Should().BeEmpty();
  }

  [Fact]
  public async Task GetClientBookings_WithDateOnlyFormat_BindsCorrectly()
  {
    // Arrange - the Blazor client sends yyyy-MM-dd, which carries no time component
    var clientId = Guid.NewGuid();

    // Act
    var response = await _client.GetAsync($"/bookings/client/{clientId}?startDate=2025-08-01&endDate=2025-08-31");

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
  }
}
