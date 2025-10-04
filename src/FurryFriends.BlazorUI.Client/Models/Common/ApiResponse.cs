using System;
using System.Collections.Generic;

namespace FurryFriends.BlazorUI.Client.Models.Common;

/// <summary>
/// Generic API response class for non-paginated data
/// </summary>
/// <typeparam name="T">The type of data in the response</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// Whether the request was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// A message describing the result of the request
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// The data returned from the API
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Any errors that occurred during the request
    /// </summary>
    public List<string>? Errors { get; set; }

    /// <summary>
    /// An error code if an error occurred
    /// </summary>
    public string? ErrorCode { get; set; }

    /// <summary>
    /// The timestamp of the response
    /// </summary>
    public DateTime Timestamp { get; set; }
}
