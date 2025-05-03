namespace FurryFriends.Web.Endpoints.LoggingEndpoints;

public class LogMessageRequest
{
  public string Level { get; set; } = "Information";
  public string Message { get; set; } = string.Empty;
  public string? Exception { get; set; }
  public Dictionary<string, string>? Data { get; set; }
}

public class LogMessageResponse
{
  public bool Success { get; set; }
}

public class LogMessage : Endpoint<LogMessageRequest, LogMessageResponse>
{
  private readonly ILogger<LogMessage> _logger;

  public LogMessage(ILogger<LogMessage> logger)
  {
    _logger = logger;
  }

  public override void Configure()
  {
    Post("/logging");
    AllowAnonymous();
    Options(o => o.WithName("ClientLogging_" + Guid.NewGuid().ToString()));
    Summary(s =>
    {
      s.Summary = "Get All Regions";
      s.Description = "Returns a list of all regions";
    });
  }

  public override Task HandleAsync(LogMessageRequest req, CancellationToken ct)
  {
    try
    {
      // Log the message with the appropriate level
      switch (req.Level.ToLowerInvariant())
      {
        case "error":
          _logger.LogError(req.Exception != null ? new Exception(req.Exception) : null,
              "Client Log: {Message} {Data}",
              req.Message,
              req.Data != null ? System.Text.Json.JsonSerializer.Serialize(req.Data) : null);
          break;
        case "warning":
          _logger.LogWarning("Client Log: {Message} {Data}",
              req.Message,
              req.Data != null ? System.Text.Json.JsonSerializer.Serialize(req.Data) : null);
          break;
        default:
          _logger.LogInformation("Client Log: {Message} {Data}",
              req.Message,
              req.Data != null ? System.Text.Json.JsonSerializer.Serialize(req.Data) : null);
          break;
      }

      return SendAsync(new LogMessageResponse { Success = true });
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error processing client log: {Message}", req.Message);
      return SendAsync(new LogMessageResponse { Success = false });
    }
  }
}
