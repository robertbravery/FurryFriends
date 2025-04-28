namespace FurryFriends.BlazorUI.Client.Models.Locations;

/// <summary>
/// Data transfer object for a locality
/// </summary>
public class LocalityDto
{
    /// <summary>
    /// The locality ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The locality name
    /// </summary>
    public string LocalityName { get; set; } = string.Empty;

    /// <summary>
    /// The region ID
    /// </summary>
    public Guid RegionId { get; set; }
}
