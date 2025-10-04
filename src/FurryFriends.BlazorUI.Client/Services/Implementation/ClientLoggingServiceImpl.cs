using FurryFriends.BlazorUI.Client.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace FurryFriends.BlazorUI.Client.Services.Implementation;

/// <summary>
/// Client-side logging service implementation that only logs locally.
/// This implementation does NOT make HTTP calls directly to the server.
/// </summary>
public class ClientLoggingServiceImpl : IClientLoggingService
{
  private readonly ILogger<ClientLoggingServiceImpl> _logger;

  public ClientLoggingServiceImpl(ILogger<ClientLoggingServiceImpl> logger)
  {
    _logger = logger;
  }

  public Task LogInformation(string message, Dictionary<string, string>? data = null)
  {
    // Log locally only, no HTTP calls
    _logger.LogInformation("{Message} {Data}", message, data != null ? System.Text.Json.JsonSerializer.Serialize(data) : null);
    return Task.CompletedTask;
  }

  public Task LogWarning(string message, Dictionary<string, string>? data = null)
  {
    // Log locally only, no HTTP calls
    _logger.LogWarning("{Message} {Data}", message, data != null ? System.Text.Json.JsonSerializer.Serialize(data) : null);
    return Task.CompletedTask;
  }

  public Task LogError(string message, Exception? exception = null, Dictionary<string, string>? data = null)
  {
    // Log locally only, no HTTP calls
    _logger.LogError(exception, "{Message} {Data}", message, data != null ? System.Text.Json.JsonSerializer.Serialize(data) : null);
    return Task.CompletedTask;
  }
}
