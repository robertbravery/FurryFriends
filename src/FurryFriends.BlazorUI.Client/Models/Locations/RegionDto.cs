namespace FurryFriends.BlazorUI.Client.Models.Locations;

/// <summary>
/// Data transfer object for a region
/// </summary>
public class RegionDto
{
    /// <summary>
    /// The region ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The region name
    /// </summary>
    public string RegionName { get; set; } = string.Empty;

    /// <summary>
    /// The country ID
    /// </summary>
    public Guid CountryId { get; set; }
}
