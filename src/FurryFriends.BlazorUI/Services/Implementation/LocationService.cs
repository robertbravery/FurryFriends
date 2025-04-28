using FurryFriends.BlazorUI.Client.Models.Locations;
using FurryFriends.BlazorUI.Client.Services.Interfaces;

namespace FurryFriends.BlazorUI.Services.Implementation;

/// <summary>
/// Service for managing location data
/// </summary>
public class LocationService : ILocationService
{
  private readonly HttpClient _httpClient;
  private readonly string _apiBaseUrl;

  public LocationService(HttpClient httpClient, IConfiguration configuration)
  {
    _httpClient = httpClient;
    _apiBaseUrl = configuration["ApiBaseUrl"] ?? string.Empty;
  }

  /// <summary>
  /// Gets all regions
  /// </summary>
  public async Task<List<RegionDto>> GetRegionsAsync()
  {
    try
    {
      var response = await _httpClient.GetFromJsonAsync<List<RegionDto>>($"{_apiBaseUrl}/Locations/regions");
      return response ?? new List<RegionDto>();
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Error getting regions: {ex.Message}");
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
      var response = await _httpClient.GetFromJsonAsync<List<LocalityDto>>($"{_apiBaseUrl}/Locations/regions/{regionId}/localities");
      return response ?? new List<LocalityDto>();
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Error getting localities: {ex.Message}");
      return new List<LocalityDto>();
    }
  }
}
