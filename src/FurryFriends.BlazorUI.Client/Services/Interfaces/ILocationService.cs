using FurryFriends.BlazorUI.Client.Models.Locations;

namespace FurryFriends.BlazorUI.Client.Services.Interfaces;

/// <summary>
/// Service for managing location data
/// </summary>
public interface ILocationService
{
    /// <summary>
    /// Gets all regions
    /// </summary>
    Task<List<RegionDto>> GetRegionsAsync();

    /// <summary>
    /// Gets localities by region ID
    /// </summary>
    Task<List<LocalityDto>> GetLocalitiesByRegionAsync(Guid regionId);
}
