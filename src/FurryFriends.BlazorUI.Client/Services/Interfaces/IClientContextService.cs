namespace FurryFriends.BlazorUI.Client.Services.Interfaces;

/// <summary>
/// Provides the currently logged-in client's identity.
/// This is a seam that can be backed by real authentication later
/// (e.g., AuthenticationStateProvider, JWT claims).
/// </summary>
public interface IClientContextService
{
    /// <summary>
    /// Returns the GUID of the currently logged-in client,
    /// or <see cref="Guid.Empty"/> if no client is logged in.
    /// </summary>
    Task<Guid> GetCurrentClientIdAsync();

    /// <summary>
    /// Sets the current client ID (used by navigation/onboarding
    /// until real auth is wired up).
    /// </summary>
    Task SetCurrentClientIdAsync(Guid clientId);
}
