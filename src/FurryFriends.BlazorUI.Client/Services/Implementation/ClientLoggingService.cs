using FurryFriends.BlazorUI.Client.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace FurryFriends.BlazorUI.Client.Services.Implementation;

public class ClientLoggingService : IClientLoggingService
{
  private readonly HttpClient _httpClient;
  private readonly ILogger<ClientLoggingService> _logger;

  public ClientLoggingService(HttpClient httpClient, ILogger<ClientLoggingService> logger)
  {
    _httpClient = httpClient;
    _logger = logger;
  }

  public async Task LogInformation(string message, Dictionary<string, string>? data = null)
  {
    await SendLogToServer("Information", message, null, data);
    _logger.LogInformation(message);
  }

  public async Task LogWarning(string message, Dictionary<string, string>? data = null)
  {
    await SendLogToServer("Warning", message, null, data);
    _logger.LogWarning(message);
  }

  public async Task LogError(string message, Exception? exception = null, Dictionary<string, string>? data = null)
  {
    await SendLogToServer("Error", message, exception?.ToString(), data);
    _logger.LogError(exception, message);
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

      await _httpClient.PostAsJsonAsync("logging", logMessage);
    }
    catch
    {
      // Fail silently - don't create cascading errors from logging failures
    }
  }
}
