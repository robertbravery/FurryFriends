namespace FurryFriends.BlazorUI.Client.Services.Interfaces;

public interface IClientLoggingService
{
  Task LogError(string message, Exception? exception = null, Dictionary<string, string>? data = null);
  Task LogInformation(string message, Dictionary<string, string>? data = null);
  Task LogWarning(string message, Dictionary<string, string>? data = null);
}
