using FurryFriends.BlazorUI.Client.Services.Interfaces;

namespace FurryFriends.BlazorUI.Services.Implementation;

/// <summary>
/// Server-side implementation of <see cref="IClientContextService"/>.
/// Stores the current client ID in-memory (scoped per DI lifetime).
/// When real authentication is added, swap this to read from
/// <c>AuthenticationStateProvider</c> / JWT claims instead.
/// </summary>
public class ClientContextService : IClientContextService
{
    private Guid _currentClientId;

    public Task<Guid> GetCurrentClientIdAsync()
    {
        return Task.FromResult(_currentClientId);
    }

    public Task SetCurrentClientIdAsync(Guid clientId)
    {
        _currentClientId = clientId;
        return Task.CompletedTask;
    }
}
