using FurryFriends.BlazorUI.Client.Services.Interfaces;

namespace FurryFriends.BlazorUI.Services.Implementation;

/// <summary>
/// Server-side implementation of the logging service.
/// This service handles sending logs to the backend API.
/// </summary>
public class ServerClientLoggingService : IClientLoggingService
{
  private readonly HttpClient _httpClient;
  private readonly ILogger<ServerClientLoggingService> _logger;

  public ServerClientLoggingService(HttpClient httpClient, ILogger<ServerClientLoggingService> logger)
  {
    _httpClient = httpClient;
    _logger = logger;
  }

  public async Task LogInformation(string message, Dictionary<string, string>? data = null)
  {
    // Log locally
    _logger.LogInformation("{Message} {Data}", message, data != null ? System.Text.Json.JsonSerializer.Serialize(data) : null);

    // Send to server
    await SendLogToServer("Information", message, null, data);
  }

  public async Task LogWarning(string message, Dictionary<string, string>? data = null)
  {
    // Log locally
    _logger.LogWarning("{Message} {Data}", message, data != null ? System.Text.Json.JsonSerializer.Serialize(data) : null);

    // Send to server
    await SendLogToServer("Warning", message, null, data);
  }

  public async Task LogError(string message, Exception? exception = null, Dictionary<string, string>? data = null)
  {
    // Log locally
    _logger.LogError(exception, "{Message} {Data}", message, data != null ? System.Text.Json.JsonSerializer.Serialize(data) : null);

    // Send to server
    await SendLogToServer("Error", message, exception?.ToString(), data);
  }

  private async Task SendLogToServer(string level, string message, string? exception = null, Dictionary<string, string>? data = null)
  {
    try
    {
      var logMessage = new
      {
        Level = level,
        Message = message,
        Exception = exception,
        Data = data
      };

      // Send log to the backend API
      await _httpClient.PostAsJsonAsync("api/logging", logMessage);

    }
    catch (Exception ex)
    {
      // Log the error locally but don't throw - we don't want logging failures to break the application
      _logger.LogError(ex, "Failed to send log to server: {Message}", message);
    }

  }
}
