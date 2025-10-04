namespace FurryFriends.BlazorUI.Client.Models.Clients.Enums;

/// <summary>
/// Represents how a client discovered our services
/// </summary>
public enum ReferralSource
{
    /// <summary>
    /// No referral source
    /// </summary>
    None = 0,

    /// <summary>
    /// Found through our website
    /// </summary>
    Website = 1,

    /// <summary>
    /// Referred by an existing client
    /// </summary>
    ExistingClient,

    /// <summary>
    /// Found through social media platforms
    /// </summary>
    SocialMedia,

    /// <summary>
    /// Found through online search (Google, Bing, etc.)
    /// </summary>
    SearchEngine,

    /// <summary>
    /// Referred by a veterinarian
    /// </summary>
    Veterinarian,

    /// <summary>
    /// Found through local advertising
    /// </summary>
    LocalAdvertising,

    /// <summary>
    /// Other sources not listed above
    /// </summary>
    Other
}
