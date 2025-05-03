using Microsoft.Extensions.Logging;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace FurryFriends.BlazorUI.Services.Implementation;

/// <summary>
/// Delegating handler for logging HTTP requests and responses
/// </summary>
public class LoggingDelegatingHandler : DelegatingHandler
{
    private readonly ILogger<LoggingDelegatingHandler> _logger;

    public LoggingDelegatingHandler(ILogger<LoggingDelegatingHandler> logger)
    {
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            // Log the request
            _logger.LogInformation("HTTP {Method} Request: {Uri}",
                request.Method, request.RequestUri);

            // Track timing
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // Send the request
            var response = await base.SendAsync(request, cancellationToken);

            stopwatch.Stop();

            // Log the response
            _logger.LogInformation("HTTP {Method} Response: {StatusCode} from {Uri} took {ElapsedMs}ms",
                request.Method, response.StatusCode, request.RequestUri, stopwatch.ElapsedMilliseconds);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during HTTP {Method} request to {Uri}",
                request?.Method, request?.RequestUri);
            throw;
        }
    }
}
