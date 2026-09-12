
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using FurryFriends.BlazorUI.Client.Models.Common;
using FurryFriends.BlazorUI.Client.Models.PetWalkers;
using FurryFriends.BlazorUI.Client.Services.Interfaces;

namespace FurryFriends.BlazorUI.Services.Implementation;

// API response DTOs for deserialization
public class ApiResult<T>
{
  public bool IsSuccess { get; set; }
  public T? Value { get; set; }
  public List<string> Errors { get; set; } = new();
}

public class GetScheduleApiResponse
{
  public Guid PetWalkerId { get; set; }
  public List<ScheduleApiItem> Schedules { get; set; } = new();
}

public class ScheduleApiItem
{
  public DayOfWeek DayOfWeek { get; set; }
  public TimeOnly StartTime { get; set; }
  public TimeOnly EndTime { get; set; }
}


/// <summary>
/// Service implementation for managing PetWalker schedules
/// Handles HTTP communication with the backend API
/// </summary>
public class ScheduleService : IScheduleService
{
  private readonly HttpClient _httpClient;
  private readonly string _apiBaseUrl;
  private readonly ILogger<ScheduleService> _logger;
  private readonly JsonSerializerOptions _jsonOptions;

  public ScheduleService(HttpClient httpClient, IConfiguration configuration, ILogger<ScheduleService> logger)
  {
    _httpClient = httpClient;
    _apiBaseUrl = configuration["ApiBaseUrl"] ?? "http://api";
    _logger = logger;
    _jsonOptions = new JsonSerializerOptions
    {
      PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
      WriteIndented = true
    };
  }

  /// <summary>
  /// Get schedules for a specific PetWalker
  /// </summary>
  public async Task<ApiResponse<GetScheduleResponseDto>> GetScheduleAsync(Guid petWalkerId)
  {
    try
    {
      _logger.LogInformation("Getting schedule for PetWalker: {PetWalkerId}", petWalkerId);

      var response = await _httpClient.GetAsync($"{_apiBaseUrl}/petwalker/{petWalkerId}/schedule");

      if (response.IsSuccessStatusCode)
      {
        var content = await response.Content.ReadAsStringAsync();

        // Parse the API response
        var apiResult = JsonSerializer.Deserialize<ApiResult<GetScheduleApiResponse>>(content, _jsonOptions);

        var scheduleResponse = new GetScheduleResponseDto
        {
          PetWalkerId = petWalkerId,
          Schedules = apiResult?.Value?.Schedules?.Select(s => new ScheduleItemDto
          {
            DayOfWeek = s.DayOfWeek,
            StartTime = s.StartTime,
            EndTime = s.EndTime,
            IsActive = true
          }).ToList() ?? new List<ScheduleItemDto>()
        };

        _logger.LogInformation("Successfully retrieved schedule for PetWalker: {PetWalkerId} with {Count} items",
          petWalkerId, scheduleResponse.Schedules.Count);

        return new ApiResponse<GetScheduleResponseDto>
        {
          Success = true,
          Message = "Schedule retrieved successfully",
          Data = scheduleResponse,
          Timestamp = DateTime.Now
        };
      }
      else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
      {
        // PetWalker not found or no schedule set - return empty schedule
        var emptyScheduleResponse = new GetScheduleResponseDto
        {
          PetWalkerId = petWalkerId,
          Schedules = new List<ScheduleItemDto>()
        };

        return new ApiResponse<GetScheduleResponseDto>
        {
          Success = true,
          Message = "No schedule found for this PetWalker",
          Data = emptyScheduleResponse,
          Timestamp = DateTime.Now
        };
      }
      else
      {
        _logger.LogWarning("Failed to get schedule for PetWalker: {PetWalkerId}. Status: {StatusCode}",
          petWalkerId, response.StatusCode);

        return new ApiResponse<GetScheduleResponseDto>
        {
          Success = false,
          Message = $"Failed to retrieve schedule. Status: {response.StatusCode}",
          Errors = new List<string> { response.ReasonPhrase ?? "Unknown error" },
          Timestamp = DateTime.Now
        };
      }
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error getting schedule for PetWalker: {PetWalkerId}", petWalkerId);

      return new ApiResponse<GetScheduleResponseDto>
      {
        Success = false,
        Message = "An error occurred while retrieving the schedule",
        Errors = new List<string> { ex.Message },
        Timestamp = DateTime.Now
      };
    }
  }

  /// <summary>
  /// Set/update schedules for a specific PetWalker
  /// </summary>
  public async Task<ApiResponse<bool>> SetScheduleAsync(Guid petWalkerId, List<ScheduleItemDto> schedules)
  {
    try
    {
      _logger.LogInformation("Setting schedule for PetWalker: {PetWalkerId} with {ScheduleCount} items",
        petWalkerId, schedules.Count);

      // Convert to API request format - send list directly, not wrapped in object
      var requestBody = schedules.Where(s => s.IsActive).Select(s => new
      {
        DayOfWeek = s.DayOfWeek,
        StartTime = s.StartTime,
        EndTime = s.EndTime
      }).ToList();

      var json = JsonSerializer.Serialize(requestBody, _jsonOptions);
      var content = new StringContent(json, Encoding.UTF8, "application/json");
      content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
      _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

      var response = await _httpClient.PostAsync($"{_apiBaseUrl}/petwalker/{petWalkerId}/schedule", content);

      if (response.IsSuccessStatusCode)
      {
        _logger.LogInformation("Successfully set schedule for PetWalker: {PetWalkerId}", petWalkerId);

        return new ApiResponse<bool>
        {
          Success = true,
          Message = "Schedule updated successfully",
          Data = true,
          Timestamp = DateTime.Now
        };
      }
      else
      {
        var errorContent = await response.Content.ReadAsStringAsync();
        _logger.LogWarning("Failed to set schedule for PetWalker: {PetWalkerId}. Status: {StatusCode}, Error: {Error}",
          petWalkerId, response.StatusCode, errorContent);

        return new ApiResponse<bool>
        {
          Success = false,
          Message = $"Failed to update schedule. Status: {response.StatusCode}",
          Errors = new List<string> { errorContent },
          Data = false,
          Timestamp = DateTime.Now
        };
      }
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error setting schedule for PetWalker: {PetWalkerId}", petWalkerId);

      return new ApiResponse<bool>
      {
        Success = false,
        Message = "An error occurred while updating the schedule",
        Errors = new List<string> { ex.Message },
        Data = false,
        Timestamp = DateTime.Now
      };
    }
  }

  /// <summary>
  /// Clear all schedules for a specific PetWalker
  /// </summary>
  public async Task<ApiResponse<bool>> ClearScheduleAsync(Guid petWalkerId)
  {
    try
    {
      _logger.LogInformation("Clearing schedule for PetWalker: {PetWalkerId}", petWalkerId);

      // Send empty schedule list to clear all schedules
      return await SetScheduleAsync(petWalkerId, new List<ScheduleItemDto>());
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error clearing schedule for PetWalker: {PetWalkerId}", petWalkerId);

      return new ApiResponse<bool>
      {
        Success = false,
        Message = "An error occurred while clearing the schedule",
        Errors = new List<string> { ex.Message },
        Data = false,
        Timestamp = DateTime.Now
      };
    }
  }

  /// <summary>
  /// Get the date-specific availability overrides for a PetWalker
  /// </summary>
  public async Task<ApiResponse<List<PetWalkerDateExceptionDto>>> GetDateExceptionsAsync(Guid petWalkerId)
  {
    try
    {
      _logger.LogInformation("Getting date exceptions for PetWalker: {PetWalkerId}", petWalkerId);

      var response = await _httpClient.GetAsync($"{_apiBaseUrl}/petwalker/{petWalkerId}/date-exceptions");

      if (response.IsSuccessStatusCode)
      {
        var content = await response.Content.ReadAsStringAsync();
        var apiResult = JsonSerializer.Deserialize<ApiResult<List<PetWalkerDateExceptionDto>>>(content, _jsonOptions);

        return new ApiResponse<List<PetWalkerDateExceptionDto>>
        {
          Success = true,
          Message = "Date exceptions retrieved successfully",
          Data = apiResult?.Value ?? new List<PetWalkerDateExceptionDto>(),
          Timestamp = DateTime.Now
        };
      }

      return Failure<List<PetWalkerDateExceptionDto>>("Failed to retrieve date exceptions", response);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error getting date exceptions for PetWalker: {PetWalkerId}", petWalkerId);
      return ExceptionResponse<List<PetWalkerDateExceptionDto>>("An error occurred while retrieving date exceptions", ex);
    }
  }

  /// <summary>
  /// Block a whole day, or replace the hours for that day
  /// </summary>
  public async Task<ApiResponse<PetWalkerDateExceptionDto>> SetDateExceptionAsync(
      Guid petWalkerId,
      DateTime date,
      bool isUnavailable,
      TimeOnly? startTime,
      TimeOnly? endTime,
      string? reason)
  {
    try
    {
      _logger.LogInformation(
          "Setting date exception for PetWalker: {PetWalkerId} on {Date:yyyy-MM-dd} (unavailable: {IsUnavailable})",
          petWalkerId, date, isUnavailable);

      var body = new
      {
        Date = date.Date,
        IsUnavailable = isUnavailable,
        StartTime = startTime,
        EndTime = endTime,
        Reason = reason
      };

      var content = new StringContent(JsonSerializer.Serialize(body, _jsonOptions), Encoding.UTF8, "application/json");
      var response = await _httpClient.PostAsync($"{_apiBaseUrl}/petwalker/{petWalkerId}/date-exceptions", content);

      if (response.IsSuccessStatusCode)
      {
        var responseContent = await response.Content.ReadAsStringAsync();
        var apiResult = JsonSerializer.Deserialize<ApiResult<PetWalkerDateExceptionDto>>(responseContent, _jsonOptions);

        return new ApiResponse<PetWalkerDateExceptionDto>
        {
          Success = true,
          Message = "Date exception saved",
          Data = apiResult?.Value,
          Timestamp = DateTime.Now
        };
      }

      return Failure<PetWalkerDateExceptionDto>("Failed to save the date exception", response);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error setting date exception for PetWalker: {PetWalkerId}", petWalkerId);
      return ExceptionResponse<PetWalkerDateExceptionDto>("An error occurred while saving the date exception", ex);
    }
  }

  /// <summary>
  /// Remove the override for a date, reverting to the weekly pattern
  /// </summary>
  public async Task<ApiResponse<bool>> DeleteDateExceptionAsync(Guid petWalkerId, DateTime date)
  {
    try
    {
      _logger.LogInformation("Deleting date exception for PetWalker: {PetWalkerId} on {Date:yyyy-MM-dd}",
          petWalkerId, date);

      var url = $"{_apiBaseUrl}/petwalker/{petWalkerId}/date-exceptions/{date:yyyy-MM-dd}";
      var response = await _httpClient.DeleteAsync(url);

      if (response.IsSuccessStatusCode)
      {
        return new ApiResponse<bool>
        {
          Success = true,
          Message = "Date exception removed",
          Data = true,
          Timestamp = DateTime.Now
        };
      }

      return Failure<bool>("Failed to remove the date exception", response);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error deleting date exception for PetWalker: {PetWalkerId}", petWalkerId);
      return ExceptionResponse<bool>("An error occurred while removing the date exception", ex);
    }
  }

  /// <summary>
  /// Get the settings used to derive bookable slots
  /// </summary>
  public async Task<ApiResponse<AvailabilitySettingsDto>> GetAvailabilitySettingsAsync(Guid petWalkerId)
  {
    try
    {
      var response = await _httpClient.GetAsync($"{_apiBaseUrl}/petwalker/{petWalkerId}/availability-settings");

      if (response.IsSuccessStatusCode)
      {
        var content = await response.Content.ReadAsStringAsync();
        var apiResult = JsonSerializer.Deserialize<ApiResult<AvailabilitySettingsDto>>(content, _jsonOptions);

        return new ApiResponse<AvailabilitySettingsDto>
        {
          Success = true,
          Message = "Availability settings retrieved successfully",
          Data = apiResult?.Value,
          Timestamp = DateTime.Now
        };
      }

      return Failure<AvailabilitySettingsDto>("Failed to retrieve availability settings", response);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error getting availability settings for PetWalker: {PetWalkerId}", petWalkerId);
      return ExceptionResponse<AvailabilitySettingsDto>("An error occurred while retrieving availability settings", ex);
    }
  }

  /// <summary>
  /// Update slot length, inter-booking gap, and the daily walk cap
  /// </summary>
  public async Task<ApiResponse<AvailabilitySettingsDto>> UpdateAvailabilitySettingsAsync(
      Guid petWalkerId,
      int slotDurationMinutes,
      int interBookingGapMinutes,
      int dailyPetWalkLimit)
  {
    try
    {
      _logger.LogInformation("Updating availability settings for PetWalker: {PetWalkerId}", petWalkerId);

      var body = new
      {
        SlotDurationMinutes = slotDurationMinutes,
        InterBookingGapMinutes = interBookingGapMinutes,
        DailyPetWalkLimit = dailyPetWalkLimit
      };

      var content = new StringContent(JsonSerializer.Serialize(body, _jsonOptions), Encoding.UTF8, "application/json");
      var response = await _httpClient.PutAsync($"{_apiBaseUrl}/petwalker/{petWalkerId}/availability-settings", content);

      if (response.IsSuccessStatusCode)
      {
        var responseContent = await response.Content.ReadAsStringAsync();
        var apiResult = JsonSerializer.Deserialize<ApiResult<AvailabilitySettingsDto>>(responseContent, _jsonOptions);

        return new ApiResponse<AvailabilitySettingsDto>
        {
          Success = true,
          Message = "Availability settings updated",
          Data = apiResult?.Value,
          Timestamp = DateTime.Now
        };
      }

      return Failure<AvailabilitySettingsDto>("Failed to update availability settings", response);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error updating availability settings for PetWalker: {PetWalkerId}", petWalkerId);
      return ExceptionResponse<AvailabilitySettingsDto>("An error occurred while updating availability settings", ex);
    }
  }

  private static ApiResponse<T> Failure<T>(string message, HttpResponseMessage response) =>
      new()
      {
        Success = false,
        Message = $"{message}. Status: {response.StatusCode}",
        Errors = new List<string> { response.ReasonPhrase ?? "Unknown error" },
        Data = default,
        Timestamp = DateTime.Now
      };

  private static ApiResponse<T> ExceptionResponse<T>(string message, Exception ex) =>
      new()
      {
        Success = false,
        Message = message,
        Errors = new List<string> { ex.Message },
        Data = default,
        Timestamp = DateTime.Now
      };
}

