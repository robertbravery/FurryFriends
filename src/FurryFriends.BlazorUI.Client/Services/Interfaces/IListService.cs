using FurryFriends.BlazorUI.Client.Models.Common;

namespace FurryFriends.BlazorUI.Client.Services.Interfaces;

/// <summary>
/// Base interface for services that provide paginated lists of data
/// </summary>
/// <typeparam name="T">The type of data in the list</typeparam>
public interface IListService<T> where T : class
{
    /// <summary>
    /// Gets a paginated list of items
    /// </summary>
    /// <param name="page">The page number (1-based)</param>
    /// <param name="pageSize">The number of items per page</param>
    /// <param name="searchTerm">Optional search term to filter results</param>
    /// <returns>A ListResponse containing the items and pagination metadata</returns>
    Task<ListResponse<T>> GetListAsync(int page, int pageSize, string? searchTerm = null);
}
