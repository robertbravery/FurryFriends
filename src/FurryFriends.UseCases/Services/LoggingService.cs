using Serilog;

namespace FurryFriends.UseCases.Services;

public class LoggingService
{
  private readonly ILogger _logger;

  public LoggingService()
  {
    _logger = Log.Logger;
  }

  public ILogger GetLogger()
  {
    return _logger;
  }
}
