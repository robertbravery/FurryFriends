namespace FurryFriends.BlazorUI.Client.Models.Common;

/// <summary>
/// Base response type for paginated lists of data from the API.
/// </summary>
/// <typeparam name="T">The type of data in the list</typeparam>
public class ListResponse<T> where T : class
{
    /// <summary>
    /// The list of data items returned from the API
    /// </summary>
    public List<T>? RowsData { get; set; }

    /// <summary>
    /// The current page number (1-based)
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// The number of items per page
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// The total number of items across all pages
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// The total number of pages
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Whether there is a previous page available
    /// </summary>
    public bool HasPreviousPage { get; set; }

    /// <summary>
    /// Whether there is a next page available
    /// </summary>
    public bool HasNextPage { get; set; }

    /// <summary>
    /// Columns to hide in the UI (optional)
    /// </summary>
    public string[]? HideColumns { get; set; }
}
