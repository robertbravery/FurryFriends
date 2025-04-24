using FurryFriends.BlazorUI.Client.Models.Common;
using FurryFriends.BlazorUI.Client.Services.Interfaces;

namespace FurryFriends.BlazorUI.Services.Implementation;

/// <summary>
/// Base implementation for services that provide paginated lists of data
/// </summary>
/// <typeparam name="T">The type of data in the list</typeparam>
public abstract class BaseListService<T> : IListService<T> where T : class
{
  protected readonly HttpClient HttpClient;
  protected readonly string ApiBaseUrl;
  protected readonly string EndpointPath;

  protected BaseListService(HttpClient httpClient, IConfiguration configuration, string endpointPath)
  {
    HttpClient = httpClient;
    ApiBaseUrl = configuration["ApiBaseUrl"] ?? string.Empty;
    EndpointPath = endpointPath;
  }

  /// <summary>
  /// Gets a paginated list of items
  /// </summary>
  /// <param name="page">The page number (1-based)</param>
  /// <param name="pageSize">The number of items per page</param>
  /// <param name="searchTerm">Optional search term to filter results</param>
  /// <returns>A ListResponse containing the items and pagination metadata</returns>
  public virtual async Task<ListResponse<T>> GetListAsync(int page, int pageSize, string? searchTerm = null)
  {
    var url = $"{ApiBaseUrl}/{EndpointPath}?page={page}&pageSize={pageSize}";

    if (!string.IsNullOrEmpty(searchTerm))
    {
      url += $"&searchTerm={Uri.EscapeDataString(searchTerm)}";
    }

    var response = await HttpClient.GetFromJsonAsync<ListResponse<T>>(url);

    if (response is null)
    {
      // Return an empty response with default values
      return new ListResponse<T>
      {
        RowsData = [],
        PageNumber = page,
        PageSize = pageSize,
        TotalCount = 0,
        TotalPages = 0,
        HasPreviousPage = false,
        HasNextPage = false
      };
    }

    return response;
  }
}
