using System.Text;
using System.Text.Json;
using FurryFriends.BlazorUI.Client.Models.Common;
using FurryFriends.BlazorUI.Client.Models.Timeslots;
using FurryFriends.BlazorUI.Client.Services.Interfaces;

namespace FurryFriends.BlazorUI.Services.Implementation;

/// <summary>
/// Service implementation for managing timeslots
/// </summary>
public class TimeslotService : ITimeslotService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TimeslotService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public TimeslotService(HttpClient httpClient, ILogger<TimeslotService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }

    #region Working Hours

    /// <summary>
    /// Create working hours for a petwalker
    /// </summary>
    public async Task<ApiResponse<WorkingHoursDto>> CreateWorkingHoursAsync(CreateWorkingHoursRequest request)
    {
        try
        {
            _logger.LogInformation("Creating working hours for PetWalker: {PetWalkerId}, Day: {DayOfWeek}",
                request.PetWalkerId, request.DayOfWeek);

            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"/petwalkers/{request.PetWalkerId}/workinghours", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<WorkingHoursDto>(responseContent, _jsonOptions);

                _logger.LogInformation("Successfully created working hours: {WorkingHoursId}", result?.WorkingHoursId);

                return new ApiResponse<WorkingHoursDto>
                {
                    Success = true,
                    Message = "Working hours created successfully",
                    Data = result,
                    Timestamp = DateTime.Now
                };
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to create working hours. Status: {StatusCode}, Error: {Error}",
                    response.StatusCode, errorContent);

                return new ApiResponse<WorkingHoursDto>
                {
                    Success = false,
                    Message = $"Failed to create working hours. Status: {response.StatusCode}",
                    Errors = new List<string> { errorContent },
                    Data = null,
                    Timestamp = DateTime.Now
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating working hours for PetWalker: {PetWalkerId}", request.PetWalkerId);

            return new ApiResponse<WorkingHoursDto>
            {
                Success = false,
                Message = "An error occurred while creating working hours",
                Errors = new List<string> { ex.Message },
                Data = null,
                Timestamp = DateTime.Now
            };
        }
    }

    /// <summary>
    /// Get working hours for a petwalker
    /// </summary>
    public async Task<ApiResponse<GetWorkingHoursResponse>> GetWorkingHoursAsync(Guid petWalkerId, DayOfWeek? dayOfWeek = null)
    {
        try
        {
            _logger.LogInformation("Getting working hours for PetWalker: {PetWalkerId}, Day: {DayOfWeek}",
                petWalkerId, dayOfWeek?.ToString() ?? "All");

            var url = $"/petwalkers/{petWalkerId}/workinghours";
            if (dayOfWeek.HasValue)
            {
                url += $"?dayOfWeek={(int)dayOfWeek.Value}";
            }

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<GetWorkingHoursResponse>(responseContent, _jsonOptions);

                _logger.LogInformation("Successfully retrieved {Count} working hours for PetWalker: {PetWalkerId}",
                    result?.WorkingHours.Count ?? 0, petWalkerId);

                return new ApiResponse<GetWorkingHoursResponse>
                {
                    Success = true,
                    Message = "Working hours retrieved successfully",
                    Data = result,
                    Timestamp = DateTime.Now
                };
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to get working hours. Status: {StatusCode}, Error: {Error}",
                    response.StatusCode, errorContent);

                return new ApiResponse<GetWorkingHoursResponse>
                {
                    Success = false,
                    Message = $"Failed to get working hours. Status: {response.StatusCode}",
                    Errors = new List<string> { errorContent },
                    Data = null,
                    Timestamp = DateTime.Now
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting working hours for PetWalker: {PetWalkerId}", petWalkerId);

            return new ApiResponse<GetWorkingHoursResponse>
            {
                Success = false,
                Message = "An error occurred while getting working hours",
                Errors = new List<string> { ex.Message },
                Data = null,
                Timestamp = DateTime.Now
            };
        }
    }

    /// <summary>
    /// Update working hours
    /// </summary>
    public async Task<ApiResponse<WorkingHoursDto>> UpdateWorkingHoursAsync(UpdateWorkingHoursRequest request)
    {
        try
        {
            _logger.LogInformation("Updating working hours: {WorkingHoursId}", request.WorkingHoursId);

            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"/workinghours/{request.WorkingHoursId}", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<WorkingHoursDto>(responseContent, _jsonOptions);

                _logger.LogInformation("Successfully updated working hours: {WorkingHoursId}", request.WorkingHoursId);

                return new ApiResponse<WorkingHoursDto>
                {
                    Success = true,
                    Message = "Working hours updated successfully",
                    Data = result,
                    Timestamp = DateTime.Now
                };
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to update working hours. Status: {StatusCode}, Error: {Error}",
                    response.StatusCode, errorContent);

                return new ApiResponse<WorkingHoursDto>
                {
                    Success = false,
                    Message = $"Failed to update working hours. Status: {response.StatusCode}",
                    Errors = new List<string> { errorContent },
                    Data = null,
                    Timestamp = DateTime.Now
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating working hours: {WorkingHoursId}", request.WorkingHoursId);

            return new ApiResponse<WorkingHoursDto>
            {
                Success = false,
                Message = "An error occurred while updating working hours",
                Errors = new List<string> { ex.Message },
                Data = null,
                Timestamp = DateTime.Now
            };
        }
    }

    /// <summary>
    /// Delete working hours
    /// </summary>
    public async Task<ApiResponse<bool>> DeleteWorkingHoursAsync(Guid workingHoursId)
    {
        try
        {
            _logger.LogInformation("Deleting working hours: {WorkingHoursId}", workingHoursId);

            var response = await _httpClient.DeleteAsync($"/workinghours/{workingHoursId}");

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Successfully deleted working hours: {WorkingHoursId}", workingHoursId);

                return new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Working hours deleted successfully",
                    Data = true,
                    Timestamp = DateTime.Now
                };
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to delete working hours. Status: {StatusCode}, Error: {Error}",
                    response.StatusCode, errorContent);

                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = $"Failed to delete working hours. Status: {response.StatusCode}",
                    Errors = new List<string> { errorContent },
                    Data = false,
                    Timestamp = DateTime.Now
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting working hours: {WorkingHoursId}", workingHoursId);

            return new ApiResponse<bool>
            {
                Success = false,
                Message = "An error occurred while deleting working hours",
                Errors = new List<string> { ex.Message },
                Data = false,
                Timestamp = DateTime.Now
            };
        }
    }

    #endregion

    #region Timeslots

    /// <summary>
    /// Create a new timeslot
    /// </summary>
    public async Task<ApiResponse<CreateTimeslotResponse>> CreateTimeslotAsync(CreateTimeslotRequest request)
    {
        try
        {
            _logger.LogInformation("Creating timeslot for PetWalker: {PetWalkerId}, Date: {Date}, Time: {StartTime}",
                request.PetWalkerId, request.Date, request.StartTime);

            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"/api/timeslots", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<CreateTimeslotResponse>(responseContent, _jsonOptions);

                _logger.LogInformation("Successfully created timeslot: {TimeslotId}", result?.TimeslotId);

                return new ApiResponse<CreateTimeslotResponse>
                {
                    Success = true,
                    Message = "Timeslot created successfully",
                    Data = result,
                    Timestamp = DateTime.Now
                };
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to create timeslot. Status: {StatusCode}, Error: {Error}",
                    response.StatusCode, errorContent);

                return new ApiResponse<CreateTimeslotResponse>
                {
                    Success = false,
                    Message = $"Failed to create timeslot. Status: {response.StatusCode}",
                    Errors = new List<string> { errorContent },
                    Data = null,
                    Timestamp = DateTime.Now
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating timeslot for PetWalker: {PetWalkerId}", request.PetWalkerId);

            return new ApiResponse<CreateTimeslotResponse>
            {
                Success = false,
                Message = "An error occurred while creating the timeslot",
                Errors = new List<string> { ex.Message },
                Data = null,
                Timestamp = DateTime.Now
            };
        }
    }

    /// <summary>
    /// Get timeslots for a petwalker
    /// </summary>
    public async Task<ApiResponse<List<TimeslotDto>>> GetTimeslotsAsync(GetTimeslotsRequest request)
    {
        try
        {
            _logger.LogInformation("Getting timeslots for PetWalker: {PetWalkerId}", request.PetWalkerId);

            var url = $"/timeslots";
            var queryParams = new List<string>();

            if (request.PetWalkerId.HasValue)
                queryParams.Add($"petWalkerId={request.PetWalkerId.Value}");
            if (request.StartDate.HasValue)
                queryParams.Add($"startDate={request.StartDate.Value}");
            if (request.EndDate.HasValue)
                queryParams.Add($"endDate={request.EndDate.Value}");
            if (!string.IsNullOrEmpty(request.Status))
                queryParams.Add($"status={request.Status}");
            queryParams.Add($"page={request.Page}");
            queryParams.Add($"pageSize={request.PageSize}");

            if (queryParams.Any())
                url += "?" + string.Join("&", queryParams);

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<List<TimeslotDto>>(responseContent, _jsonOptions);

                _logger.LogInformation("Successfully retrieved {Count} timeslots", result?.Count ?? 0);

                return new ApiResponse<List<TimeslotDto>>
                {
                    Success = true,
                    Message = "Timeslots retrieved successfully",
                    Data = result ?? new List<TimeslotDto>(),
                    Timestamp = DateTime.Now
                };
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to get timeslots. Status: {StatusCode}, Error: {Error}",
                    response.StatusCode, errorContent);

                return new ApiResponse<List<TimeslotDto>>
                {
                    Success = false,
                    Message = $"Failed to get timeslots. Status: {response.StatusCode}",
                    Errors = new List<string> { errorContent },
                    Data = null,
                    Timestamp = DateTime.Now
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting timeslots for PetWalker: {PetWalkerId}", request.PetWalkerId);

            return new ApiResponse<List<TimeslotDto>>
            {
                Success = false,
                Message = "An error occurred while getting timeslots",
                Errors = new List<string> { ex.Message },
                Data = null,
                Timestamp = DateTime.Now
            };
        }
    }

    /// <summary>
    /// Get available timeslots for a petwalker on a specific date
    /// </summary>
    public async Task<ApiResponse<GetAvailableTimeslotsResponse>> GetAvailableTimeslotsAsync(Guid petWalkerId, DateTime date)
    {
        try
        {
            _logger.LogInformation("Getting available timeslots for PetWalker: {PetWalkerId}, Date: {Date}",
                petWalkerId, date);

            var url = $"/timeslots/available?petWalkerId={petWalkerId}&date={date:yyyy-MM-dd}";

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<GetAvailableTimeslotsResponse>(responseContent, _jsonOptions);

                _logger.LogInformation("Successfully retrieved {Count} available timeslots",
                    result?.AvailableTimeslots.Count ?? 0);

                return new ApiResponse<GetAvailableTimeslotsResponse>
                {
                    Success = true,
                    Message = "Available timeslots retrieved successfully",
                    Data = result,
                    Timestamp = DateTime.Now
                };
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to get available timeslots. Status: {StatusCode}, Error: {Error}",
                    response.StatusCode, errorContent);

                return new ApiResponse<GetAvailableTimeslotsResponse>
                {
                    Success = false,
                    Message = $"Failed to get available timeslots. Status: {response.StatusCode}",
                    Errors = new List<string> { errorContent },
                    Data = null,
                    Timestamp = DateTime.Now
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available timeslots for PetWalker: {PetWalkerId}", petWalkerId);

            return new ApiResponse<GetAvailableTimeslotsResponse>
            {
                Success = false,
                Message = "An error occurred while getting available timeslots",
                Errors = new List<string> { ex.Message },
                Data = null,
                Timestamp = DateTime.Now
            };
        }
    }

    /// <summary>
    /// Update a timeslot
    /// </summary>
    public async Task<ApiResponse<TimeslotDto>> UpdateTimeslotAsync(UpdateTimeslotRequest request)
    {
        try
        {
            _logger.LogInformation("Updating timeslot: {TimeslotId}", request.TimeslotId);

            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"/api/timeslots/{request.TimeslotId}", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<TimeslotDto>(responseContent, _jsonOptions);

                _logger.LogInformation("Successfully updated timeslot: {TimeslotId}", request.TimeslotId);

                return new ApiResponse<TimeslotDto>
                {
                    Success = true,
                    Message = "Timeslot updated successfully",
                    Data = result,
                    Timestamp = DateTime.Now
                };
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to update timeslot. Status: {StatusCode}, Error: {Error}",
                    response.StatusCode, errorContent);

                return new ApiResponse<TimeslotDto>
                {
                    Success = false,
                    Message = $"Failed to update timeslot. Status: {response.StatusCode}",
                    Errors = new List<string> { errorContent },
                    Data = null,
                    Timestamp = DateTime.Now
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating timeslot: {TimeslotId}", request.TimeslotId);

            return new ApiResponse<TimeslotDto>
            {
                Success = false,
                Message = "An error occurred while updating timeslot",
                Errors = new List<string> { ex.Message },
                Data = null,
                Timestamp = DateTime.Now
            };
        }
    }

    /// <summary>
    /// Delete a timeslot
    /// </summary>
    public async Task<ApiResponse<bool>> DeleteTimeslotAsync(Guid timeslotId)
    {
        try
        {
            _logger.LogInformation("Deleting timeslot: {TimeslotId}", timeslotId);

            var response = await _httpClient.DeleteAsync($"/api/timeslots/{timeslotId}");

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Successfully deleted timeslot: {TimeslotId}", timeslotId);

                return new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Timeslot deleted successfully",
                    Data = true,
                    Timestamp = DateTime.Now
                };
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to delete timeslot. Status: {StatusCode}, Error: {Error}",
                    response.StatusCode, errorContent);

                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = $"Failed to delete timeslot. Status: {response.StatusCode}",
                    Errors = new List<string> { errorContent },
                    Data = false,
                    Timestamp = DateTime.Now
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting timeslot: {TimeslotId}", timeslotId);

            return new ApiResponse<bool>
            {
                Success = false,
                Message = "An error occurred while deleting timeslot",
                Errors = new List<string> { ex.Message },
                Data = false,
                Timestamp = DateTime.Now
            };
        }
    }

    /// <summary>
    /// Book a timeslot
    /// </summary>
    public async Task<ApiResponse<TimeslotBookingResponseDto>> BookTimeslotAsync(Guid timeslotId, Guid clientId, List<Guid> petIds)
    {
        try
        {
            _logger.LogInformation("Booking timeslot: {TimeslotId} for client: {ClientId}", timeslotId, clientId);

            var request = new
            {
                TimeslotId = timeslotId,
                ClientId = clientId,
                PetIds = petIds
            };

            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"/api/timeslots/{timeslotId}/book", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<TimeslotBookingResponseDto>(responseContent, _jsonOptions);

                _logger.LogInformation("Successfully booked timeslot: {TimeslotId}, BookingId: {BookingId}",
                    timeslotId, result?.BookingId);

                return new ApiResponse<TimeslotBookingResponseDto>
                {
                    Success = true,
                    Message = "Timeslot booked successfully",
                    Data = result,
                    Timestamp = DateTime.Now
                };
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to book timeslot. Status: {StatusCode}, Error: {Error}",
                    response.StatusCode, errorContent);

                return new ApiResponse<TimeslotBookingResponseDto>
                {
                    Success = false,
                    Message = $"Failed to book timeslot. Status: {response.StatusCode}",
                    Errors = new List<string> { errorContent },
                    Data = null,
                    Timestamp = DateTime.Now
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error booking timeslot: {TimeslotId}", timeslotId);

            return new ApiResponse<TimeslotBookingResponseDto>
            {
                Success = false,
                Message = "An error occurred while booking the timeslot",
                Errors = new List<string> { ex.Message },
                Data = null,
                Timestamp = DateTime.Now
            };
        }
    }

    #endregion

    #region Custom Time Requests

    /// <summary>
    /// Request a custom time slot
    /// </summary>
    public async Task<ApiResponse<RequestCustomTimeResponse>> RequestCustomTimeAsync(RequestCustomTimeRequest request)
    {
        try
        {
            _logger.LogInformation("Requesting custom time for PetWalker: {PetWalkerId}, Client: {ClientId}, Date: {Date}",
                request.PetWalkerId, request.ClientId, request.RequestedDate);

            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"/api/timeslots/request-custom", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<RequestCustomTimeResponse>(responseContent, _jsonOptions);

                _logger.LogInformation("Successfully created custom time request: {RequestId}", result?.RequestId);

                return new ApiResponse<RequestCustomTimeResponse>
                {
                    Success = true,
                    Message = "Custom time request submitted successfully",
                    Data = result,
                    Timestamp = DateTime.Now
                };
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to request custom time. Status: {StatusCode}, Error: {Error}",
                    response.StatusCode, errorContent);

                return new ApiResponse<RequestCustomTimeResponse>
                {
                    Success = false,
                    Message = $"Failed to request custom time. Status: {response.StatusCode}",
                    Errors = new List<string> { errorContent },
                    Data = null,
                    Timestamp = DateTime.Now
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error requesting custom time for PetWalker: {PetWalkerId}", request.PetWalkerId);

            return new ApiResponse<RequestCustomTimeResponse>
            {
                Success = false,
                Message = "An error occurred while requesting custom time",
                Errors = new List<string> { ex.Message },
                Data = null,
                Timestamp = DateTime.Now
            };
        }
    }

    /// <summary>
    /// Get custom time requests for a petwalker
    /// </summary>
    public async Task<ApiResponse<List<CustomTimeRequestDto>>> GetPetWalkerCustomTimeRequestsAsync(Guid petWalkerId, string? status = null)
    {
        try
        {
            _logger.LogInformation("Getting custom time requests for PetWalker: {PetWalkerId}, Status: {Status}",
                petWalkerId, status ?? "All");

            var url = $"/petwalkers/{petWalkerId}/custom-time-requests";
            if (!string.IsNullOrEmpty(status))
            {
                url += $"?status={status}";
            }

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<List<CustomTimeRequestDto>>(responseContent, _jsonOptions);

                _logger.LogInformation("Successfully retrieved {Count} custom time requests",
                    result?.Count ?? 0);

                return new ApiResponse<List<CustomTimeRequestDto>>
                {
                    Success = true,
                    Message = "Custom time requests retrieved successfully",
                    Data = result ?? new List<CustomTimeRequestDto>(),
                    Timestamp = DateTime.Now
                };
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to get custom time requests. Status: {StatusCode}, Error: {Error}",
                    response.StatusCode, errorContent);

                return new ApiResponse<List<CustomTimeRequestDto>>
                {
                    Success = false,
                    Message = $"Failed to get custom time requests. Status: {response.StatusCode}",
                    Errors = new List<string> { errorContent },
                    Data = null,
                    Timestamp = DateTime.Now
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting custom time requests for PetWalker: {PetWalkerId}", petWalkerId);

            return new ApiResponse<List<CustomTimeRequestDto>>
            {
                Success = false,
                Message = "An error occurred while getting custom time requests",
                Errors = new List<string> { ex.Message },
                Data = null,
                Timestamp = DateTime.Now
            };
        }
    }

    /// <summary>
    /// Get custom time requests for a client
    /// </summary>
    public async Task<ApiResponse<List<CustomTimeRequestDto>>> GetClientCustomTimeRequestsAsync(Guid clientId, string? status = null)
    {
        try
        {
            _logger.LogInformation("Getting custom time requests for Client: {ClientId}, Status: {Status}",
                clientId, status ?? "All");

            var url = $"/clients/{clientId}/custom-time-requests";
            if (!string.IsNullOrEmpty(status))
            {
                url += $"?status={status}";
            }

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<List<CustomTimeRequestDto>>(responseContent, _jsonOptions);

                _logger.LogInformation("Successfully retrieved {Count} custom time requests",
                    result?.Count ?? 0);

                return new ApiResponse<List<CustomTimeRequestDto>>
                {
                    Success = true,
                    Message = "Custom time requests retrieved successfully",
                    Data = result ?? new List<CustomTimeRequestDto>(),
                    Timestamp = DateTime.Now
                };
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to get custom time requests. Status: {StatusCode}, Error: {Error}",
                    response.StatusCode, errorContent);

                return new ApiResponse<List<CustomTimeRequestDto>>
                {
                    Success = false,
                    Message = $"Failed to get custom time requests. Status: {response.StatusCode}",
                    Errors = new List<string> { errorContent },
                    Data = null,
                    Timestamp = DateTime.Now
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting custom time requests for Client: {ClientId}", clientId);

            return new ApiResponse<List<CustomTimeRequestDto>>
            {
                Success = false,
                Message = "An error occurred while getting custom time requests",
                Errors = new List<string> { ex.Message },
                Data = null,
                Timestamp = DateTime.Now
            };
        }
    }

    /// <summary>
    /// Respond to a custom time request (accept/decline/counter-offer)
    /// </summary>
    public async Task<ApiResponse<RespondToCustomTimeRequestResponse>> RespondToCustomTimeRequestAsync(RespondToCustomTimeRequestRequest request)
    {
        try
        {
            _logger.LogInformation("Responding to custom time request: {RequestId}, Action: {Action}",
                request.RequestId, request.Action);

            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"/api/timeslots/custom-time-requests/{request.RequestId}/respond", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<RespondToCustomTimeRequestResponse>(responseContent, _jsonOptions);

                _logger.LogInformation("Successfully responded to custom time request: {RequestId}", request.RequestId);

                return new ApiResponse<RespondToCustomTimeRequestResponse>
                {
                    Success = true,
                    Message = "Response submitted successfully",
                    Data = result,
                    Timestamp = DateTime.Now
                };
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to respond to custom time request. Status: {StatusCode}, Error: {Error}",
                    response.StatusCode, errorContent);

                return new ApiResponse<RespondToCustomTimeRequestResponse>
                {
                    Success = false,
                    Message = $"Failed to respond to custom time request. Status: {response.StatusCode}",
                    Errors = new List<string> { errorContent },
                    Data = null,
                    Timestamp = DateTime.Now
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error responding to custom time request: {RequestId}", request.RequestId);

            return new ApiResponse<RespondToCustomTimeRequestResponse>
            {
                Success = false,
                Message = "An error occurred while responding to custom time request",
                Errors = new List<string> { ex.Message },
                Data = null,
                Timestamp = DateTime.Now
            };
        }
    }

    #endregion
}
