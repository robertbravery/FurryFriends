using FurryFriends.BlazorUI.Client.Services.Interfaces;

namespace FurryFriends.BlazorUI.Services.Implementation;

/// <summary>
/// Server-side implementation of <see cref="IClientContextService"/>.
/// Tracks the current client in memory for the lifetime of the scope, falling back to a
/// configured default while real authentication is not yet wired up.
/// When authentication is added, replace the fallback with
/// <c>AuthenticationStateProvider</c> / JWT claims.
/// </summary>
public class ClientContextService : IClientContextService
{
    /// <summary>
    /// Configuration key for the fallback client used in testing.
    /// Deliberately only read in Development - see Program.cs.
    /// </summary>
    public const string DefaultClientConfigKey = "Dev:CurrentClientId";

    private readonly ILogger<ClientContextService> _logger;
    private readonly Guid _defaultClientId;

    private Guid _currentClientId;

    public ClientContextService(
        IConfiguration configuration,
        ILogger<ClientContextService> logger)
    {
        _logger = logger;

        var configured = configuration[DefaultClientConfigKey];
        _defaultClientId = Guid.TryParse(configured, out var parsed) ? parsed : Guid.Empty;

        if (_defaultClientId != Guid.Empty)
        {
            _logger.LogInformation(
                "No authentication is configured; falling back to the development client {ClientId} from {ConfigKey}",
                _defaultClientId, DefaultClientConfigKey);
        }
    }

    public Task<Guid> GetCurrentClientIdAsync()
    {
        // An explicitly set client always wins; the configured default is only a fallback.
        if (_currentClientId != Guid.Empty)
        {
            return Task.FromResult(_currentClientId);
        }

        return Task.FromResult(_defaultClientId);
    }

    public Task SetCurrentClientIdAsync(Guid clientId)
    {
        _currentClientId = clientId;
        return Task.CompletedTask;
    }
}

/// <summary>
/// Used outside Development: there is no fallback client, so callers must have a real
/// client context. Prevents a development default from ever applying in a deployed environment.
/// </summary>
public class NoClientContextService : IClientContextService
{
    public Task<Guid> GetCurrentClientIdAsync() => Task.FromResult(Guid.Empty);

    public Task SetCurrentClientIdAsync(Guid clientId) => Task.CompletedTask;
}
