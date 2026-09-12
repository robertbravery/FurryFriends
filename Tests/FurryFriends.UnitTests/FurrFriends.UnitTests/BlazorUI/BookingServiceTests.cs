using System.Net;
using System.Text;
using FurryFriends.BlazorUI.Client.Models.Bookings;
using FurryFriends.BlazorUI.Client.Models.Common;
using FurryFriends.BlazorUI.Services.Implementation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace FurryFriends.UnitTests.BlazorUI;

/// <summary>
/// Locks down the exact URLs the Blazor UI calls for booking lifecycle actions.
/// A regression here (e.g. a duplicated "/api" segment) would silently break the UI.
/// </summary>
public class BookingServiceTests
{
  private const string ApiBaseUrl = "https://localhost:57679/api";

  [Theory]
  [InlineData("ConfirmBookingAsync", "/bookings/{id}/confirm")]
  [InlineData("StartBookingAsync", "/bookings/{id}/start")]
  [InlineData("CompleteBookingAsync", "/bookings/{id}/complete")]
  [InlineData("MarkBookingNoShowAsync", "/bookings/{id}/no-show")]
  public async Task LifecycleAction_PostsToExpectedUrl(string methodName, string expectedPathTemplate)
  {
    // Arrange
    var bookingId = Guid.NewGuid();
    var handler = StubHttpMessageHandler.WithJson(HttpStatusCode.OK,
      """{"bookingId":"00000000-0000-0000-0000-000000000000","status":"Confirmed","message":"ok"}""");
    var service = CreateService(handler);

    // Act
    var response = await InvokeAsync(service, methodName, bookingId);

    // Assert
    var expected = ApiBaseUrl + expectedPathTemplate.Replace("{id}", bookingId.ToString());
    handler.LastRequestUri.Should().Be(expected);
    handler.LastRequestMethod.Should().Be(HttpMethod.Post);
    response.Success.Should().BeTrue();
  }

  [Fact]
  public async Task ConfirmBookingAsync_DoesNotDuplicateApiSegment()
  {
    // Arrange - guards the historical "$"{_apiBaseUrl}/api/bookings/..." bug
    var bookingId = Guid.NewGuid();
    var handler = StubHttpMessageHandler.WithJson(HttpStatusCode.OK, "{}");
    var service = CreateService(handler);

    // Act
    await service.ConfirmBookingAsync(bookingId);

    // Assert
    handler.LastRequestUri.Should().NotContain("/api/api/");
    handler.LastRequestUri.Should().StartWith(ApiBaseUrl);
  }

  [Fact]
  public async Task LifecycleAction_ReturnsSuccess_WithServerStatusAndMessage()
  {
    // Arrange
    var bookingId = Guid.NewGuid();
    var handler = StubHttpMessageHandler.WithJson(HttpStatusCode.OK,
      $$"""{"bookingId":"{{bookingId}}","status":"InProgress","message":"Booking started successfully"}""");
    var service = CreateService(handler);

    // Act
    var response = await service.StartBookingAsync(bookingId);

    // Assert
    response.Success.Should().BeTrue();
    response.Data.Should().NotBeNull();
    response.Data!.Status.Should().Be("InProgress");
    response.Message.Should().Be("Booking started successfully");
  }

  [Fact]
  public async Task LifecycleAction_ReturnsFailure_WithProblemDetailsMessage_OnBadRequest()
  {
    // Arrange
    var handler = StubHttpMessageHandler.WithJson(HttpStatusCode.BadRequest,
      """{"title":"Validation Error","errors":{"Booking":"Cannot complete booking in Confirmed status"}}""");
    var service = CreateService(handler);

    // Act
    var response = await service.CompleteBookingAsync(Guid.NewGuid());

    // Assert
    response.Success.Should().BeFalse();
    response.Errors.Should().Contain("Cannot complete booking in Confirmed status");
  }

  [Fact]
  public async Task LifecycleAction_ReturnsFailure_WithRawContent_WhenPayloadIsNotJson()
  {
    // Arrange
    var handler = StubHttpMessageHandler.WithContent(HttpStatusCode.InternalServerError, "boom");
    var service = CreateService(handler);

    // Act
    var response = await service.MarkBookingNoShowAsync(Guid.NewGuid());

    // Assert
    response.Success.Should().BeFalse();
    response.Errors.Should().Contain("boom");
  }

  [Fact]
  public async Task LifecycleAction_ReturnsFailure_WhenHttpClientThrows()
  {
    // Arrange
    var handler = StubHttpMessageHandler.Throwing(new HttpRequestException("connection refused"));
    var service = CreateService(handler);

    // Act
    var response = await service.ConfirmBookingAsync(Guid.NewGuid());

    // Assert
    response.Success.Should().BeFalse();
    response.Errors.Should().Contain(e => e.Contains("connection refused"));
  }

  [Fact]
  public async Task CancelBookingAsync_PostsToExpectedUrl()
  {
    // Arrange
    var bookingId = Guid.NewGuid();
    var handler = StubHttpMessageHandler.WithJson(HttpStatusCode.OK,
      "{\"success\":true,\"message\":\"Booking cancelled successfully\",\"bookingId\":\"" + bookingId + "\"}");
    var service = CreateService(handler);

    // Act
    var response = await service.CancelBookingAsync(bookingId, new CancelBookingRequestDto
    {
      Reason = CancellationReason.ClientRequest,
      CancelledBy = CancelledBy.Client,
      AdditionalNotes = "Client changed plans"
    });

    // Assert
    handler.LastRequestUri.Should().Be($"{ApiBaseUrl}/bookings/{bookingId}/cancel");
    handler.LastRequestMethod.Should().Be(HttpMethod.Post);
    response.Success.Should().BeTrue();
    response.Data!.Message.Should().Be("Booking cancelled successfully");
  }

  [Fact]
  public async Task GetClientBookingsAsync_ReturnsBookingListItems()
  {
    // Arrange
    var clientId = Guid.NewGuid();
    var handler = StubHttpMessageHandler.WithJson(HttpStatusCode.OK,
      """
      [{"id":"11111111-1111-1111-1111-111111111111","startTime":"2026-05-04T10:00:00","endTime":"2026-05-04T11:30:00","status":"Confirmed","price":35.50}]
      """);
    var service = CreateService(handler);

    // Act
    var response = await service.GetClientBookingsAsync(clientId, new DateTime(2026, 5, 1), new DateTime(2026, 5, 31));

    // Assert
    handler.LastRequestUri.Should().StartWith($"{ApiBaseUrl}/bookings/client/{clientId}");
    response.Success.Should().BeTrue();
    response.Data.Should().HaveCount(1);
    response.Data![0].Status.Should().Be("Confirmed");
    response.Data[0].Price.Should().Be(35.50m);
    response.Data[0].DurationDisplay.Should().Be("1h 30m");
  }

  private static Task<ApiResponse<BookingStatusResponseDto>> InvokeAsync(
    BookingService service,
    string methodName,
    Guid bookingId) => methodName switch
    {
      "ConfirmBookingAsync" => service.ConfirmBookingAsync(bookingId),
      "StartBookingAsync" => service.StartBookingAsync(bookingId),
      "CompleteBookingAsync" => service.CompleteBookingAsync(bookingId),
      "MarkBookingNoShowAsync" => service.MarkBookingNoShowAsync(bookingId),
      _ => throw new ArgumentOutOfRangeException(nameof(methodName), methodName, null)
    };

  private static BookingService CreateService(HttpMessageHandler handler)
  {
    var configuration = new ConfigurationBuilder()
      .AddInMemoryCollection(new Dictionary<string, string?> { ["ApiBaseUrl"] = ApiBaseUrl })
      .Build();

    var httpClient = new HttpClient(handler) { BaseAddress = new Uri(ApiBaseUrl) };

    return new BookingService(
      httpClient,
      configuration,
      Mock.Of<ILogger<BookingService>>());
  }

  /// <summary>
  /// Minimal HttpMessageHandler that records the last request and returns a canned response.
  /// </summary>
  private sealed class StubHttpMessageHandler : HttpMessageHandler
  {
    private readonly HttpStatusCode _statusCode;
    private readonly string _content;
    private readonly Exception? _exception;

    private StubHttpMessageHandler(HttpStatusCode statusCode, string content, Exception? exception = null)
    {
      _statusCode = statusCode;
      _content = content;
      _exception = exception;
    }

    public string? LastRequestUri { get; private set; }
    public HttpMethod? LastRequestMethod { get; private set; }

    public static StubHttpMessageHandler WithJson(HttpStatusCode statusCode, string json) =>
      new(statusCode, json);

    public static StubHttpMessageHandler WithContent(HttpStatusCode statusCode, string content) =>
      new(statusCode, content);

    public static StubHttpMessageHandler Throwing(Exception exception) =>
      new(HttpStatusCode.OK, string.Empty, exception);

    protected override Task<HttpResponseMessage> SendAsync(
      HttpRequestMessage request,
      CancellationToken cancellationToken)
    {
      LastRequestUri = request.RequestUri?.ToString();
      LastRequestMethod = request.Method;

      if (_exception != null)
      {
        throw _exception;
      }

      var response = new HttpResponseMessage(_statusCode)
      {
        Content = new StringContent(_content, Encoding.UTF8, "application/json")
      };

      return Task.FromResult(response);
    }
  }
}
