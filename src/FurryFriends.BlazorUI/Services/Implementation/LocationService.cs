using FurryFriends.BlazorUI.Client.Models.Locations;
using FurryFriends.BlazorUI.Client.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace FurryFriends.BlazorUI.Services.Implementation;

/// <summary>
/// Service for managing location data
/// </summary>
public class LocationService : ILocationService
{
  private readonly HttpClient _httpClient;
  private readonly string _apiBaseUrl;
  private readonly ILogger<LocationService> _logger;

  public LocationService(HttpClient httpClient, IConfiguration configuration, ILogger<LocationService> logger)
  {
    _httpClient = httpClient;
    _apiBaseUrl = configuration["ApiBaseUrl"] ?? string.Empty;
    _logger = logger;
  }

  /// <summary>
  /// Gets all regions
  /// </summary>
  public async Task<List<RegionDto>> GetRegionsAsync()
  {
    try
    {
      _logger.LogInformation("Fetching all regions");
      var response = await _httpClient.GetFromJsonAsync<List<RegionDto>>($"{_apiBaseUrl}/Locations/regions");

      if (response == null)
      {
        _logger.LogWarning("Received null response when fetching regions");
        return new List<RegionDto>();
      }

      _logger.LogInformation("Successfully retrieved {Count} regions", response.Count);
      return response;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error fetching regions from API");
      return new List<RegionDto>();
    }
  }

  /// <summary>
  /// Gets localities by region ID
  /// </summary>
  public async Task<List<LocalityDto>> GetLocalitiesByRegionAsync(Guid regionId)
  {
    try
    {
      _logger.LogInformation("Fetching localities for region ID: {RegionId}", regionId);
      var response = await _httpClient.GetFromJsonAsync<List<LocalityDto>>($"{_apiBaseUrl}/Locations/regions/{regionId}/localities");

      if (response == null)
      {
        _logger.LogWarning("Received null response when fetching localities for region ID: {RegionId}", regionId);
        return new List<LocalityDto>();
      }

      _logger.LogInformation("Successfully retrieved {Count} localities for region ID: {RegionId}", response.Count, regionId);
      return response;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error fetching localities for region ID: {RegionId}", regionId);
      return new List<LocalityDto>();
    }
  }
}
