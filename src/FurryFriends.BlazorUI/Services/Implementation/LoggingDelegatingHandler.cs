using Microsoft.Extensions.Logging;

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
}
