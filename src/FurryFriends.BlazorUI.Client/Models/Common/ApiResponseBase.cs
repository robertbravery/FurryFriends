using System;
using System.Collections.Generic;
using System.Text.Json;

namespace FurryFriends.BlazorUI.Client.Models.Common;

/// <summary>
/// Base API response class for non-paginated data with JsonElement for dynamic data
/// </summary>
public class ApiResponseBase
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
    /// The data returned from the API as a JsonElement for dynamic access
    /// </summary>
    public JsonElement Data { get; set; }
    
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
