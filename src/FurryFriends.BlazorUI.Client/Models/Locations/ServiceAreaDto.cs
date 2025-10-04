namespace FurryFriends.BlazorUI.Client.Models.Locations;

/// <summary>
/// Data transfer object for a service area
/// </summary>
public class ServiceAreaDto
{
    /// <summary>
    /// The service area ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The locality ID
    /// </summary>
    public Guid LocalityId { get; set; }

    /// <summary>
    /// The locality name (for display purposes)
    /// </summary>
    public string LocalityName { get; set; } = string.Empty;

    /// <summary>
    /// The region ID
    /// </summary>
    public Guid RegionId { get; set; }

    /// <summary>
    /// The region name (for display purposes)
    /// </summary>
    public string RegionName { get; set; } = string.Empty;
}
