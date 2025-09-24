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
  protected readonly ILogger Logger;

  protected BaseListService(HttpClient httpClient, IConfiguration configuration, string endpointPath, ILogger logger)
  {
    HttpClient = httpClient;
    ApiBaseUrl = configuration["ApiBaseUrl"] ?? string.Empty;
    EndpointPath = endpointPath;
    Logger = logger;
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
    try
    {
      var url = $"{ApiBaseUrl}/{EndpointPath}?page={page}&pageSize={pageSize}";

      if (!string.IsNullOrEmpty(searchTerm))
      {
        url += $"&searchTerm={Uri.EscapeDataString(searchTerm)}";
      }

      Logger.LogInformation("Making API request to {Url}", url);

      // Use GetFromJsonAsync for better performance
      var response = await HttpClient.GetFromJsonAsync<ListResponse<T>>(url,
        new System.Text.Json.JsonSerializerOptions
        {
          PropertyNameCaseInsensitive = false,
          PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
        });

      if (response is null || response.RowsData is null)
      {
        Logger.LogWarning("API returned a null response");
        return CreateEmptyResponse(page, pageSize);
      }

      Logger.LogInformation("Successfully retrieved {Count} items from API", response.RowsData.Count);
      return response;
    }
    catch (HttpRequestException ex)
    {
      // Log the HTTP exception
      Logger.LogError(ex, "HTTP error in GetListAsync for endpoint {EndpointPath}: {Message}", EndpointPath, ex.Message);

      // Throw the exception to allow the UI to handle it
      throw;
    }
    catch (System.Text.Json.JsonException ex)
    {
      // Log the JSON deserialization exception
      Logger.LogError(ex, "JSON deserialization error in GetListAsync for endpoint {EndpointPath}: {Message}", EndpointPath, ex.Message);

      // Throw the exception to allow the UI to handle it
      throw;
    }
    catch (Exception ex)
    {
      // Log the general exception
      Logger.LogError(ex, "Exception in GetListAsync for endpoint {EndpointPath}: {Message}", EndpointPath, ex.Message);

      // Throw the exception to allow the UI to handle it
      throw;
    }
  }

  private ListResponse<T> CreateEmptyResponse(int page, int pageSize)
  {
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
}
