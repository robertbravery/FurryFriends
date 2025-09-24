using System.Text;
using System.Text.Json;
using FurryFriends.BlazorUI.Client.Models.Bookings;
using FurryFriends.BlazorUI.Client.Models.Common;
using FurryFriends.BlazorUI.Client.Models.PetWalkers;
using FurryFriends.BlazorUI.Client.Services.Interfaces;

namespace FurryFriends.BlazorUI.Services.Implementation;

/// <summary>
/// Service implementation for managing bookings
/// </summary>
public class BookingService : IBookingService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<BookingService> _logger;
    private readonly string _apiBaseUrl;
    private readonly JsonSerializerOptions _jsonOptions;

    public BookingService(
        HttpClient httpClient, 
        IConfiguration configuration, 
        ILogger<BookingService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        _apiBaseUrl = _configuration["ApiBaseUrl"] ?? "http://api";
        
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }

    /// <summary>
    /// Create a new booking
    /// </summary>
    public async Task<ApiResponse<BookingResponseDto>> CreateBookingAsync(BookingRequestDto bookingRequest)
    {
        try
        {
            _logger.LogInformation("Creating booking for PetWalker: {PetWalkerId}, Client: {ClientId}", 
                bookingRequest.PetWalkerId, bookingRequest.PetOwnerId);

            var json = JsonSerializer.Serialize(bookingRequest, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_apiBaseUrl}/Bookings", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<BookingResponseDto>(responseContent, _jsonOptions);

                _logger.LogInformation("Successfully created booking: {BookingId}", result?.BookingId);

                return new ApiResponse<BookingResponseDto>
                {
                    Success = true,
                    Message = "Booking created successfully",
                    Data = result,
                    Timestamp = DateTime.Now
                };
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to create booking. Status: {StatusCode}, Error: {Error}",
                    response.StatusCode, errorContent);

                return new ApiResponse<BookingResponseDto>
                {
                    Success = false,
                    Message = $"Failed to create booking. Status: {response.StatusCode}",
                    Errors = new List<string> { errorContent },
                    Data = null,
                    Timestamp = DateTime.Now
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking for PetWalker: {PetWalkerId}", bookingRequest.PetWalkerId);

            return new ApiResponse<BookingResponseDto>
            {
                Success = false,
                Message = "An error occurred while creating the booking",
                Errors = new List<string> { ex.Message },
                Data = null,
                Timestamp = DateTime.Now
            };
        }
    }

    /// <summary>
    /// Get available time slots for a PetWalker on a specific date
    /// </summary>
    public async Task<ApiResponse<AvailableSlotsResponseDto>> GetAvailableSlotsAsync(Guid petWalkerId, DateTime date)
    {
        try
        {
            _logger.LogInformation("Getting available slots for PetWalker: {PetWalkerId} on {Date}",
                petWalkerId, date.ToString("yyyy-MM-dd"));

            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/petwalker/{petWalkerId}/available-slots?date={date:yyyy-MM-dd}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<AvailableSlotsResponseDto>(content, _jsonOptions);

                _logger.LogInformation("Successfully retrieved {Count} available slots for PetWalker: {PetWalkerId}",
                    result?.AvailableSlots?.Count ?? 0, petWalkerId);

                return new ApiResponse<AvailableSlotsResponseDto>
                {
                    Success = true,
                    Message = "Available slots retrieved successfully",
                    Data = result,
                    Timestamp = DateTime.Now
                };
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to get available slots for PetWalker: {PetWalkerId}. Status: {StatusCode}, Error: {Error}",
                    petWalkerId, response.StatusCode, errorContent);

                return new ApiResponse<AvailableSlotsResponseDto>
                {
                    Success = false,
                    Message = $"Failed to get available slots. Status: {response.StatusCode}",
                    Errors = new List<string> { errorContent },
                    Data = null,
                    Timestamp = DateTime.Now
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available slots for PetWalker: {PetWalkerId}", petWalkerId);

            return new ApiResponse<AvailableSlotsResponseDto>
            {
                Success = false,
                Message = "An error occurred while getting available slots",
                Errors = new List<string> { ex.Message },
                Data = null,
                Timestamp = DateTime.Now
            };
        }
    }

    /// <summary>
    /// Check if a specific time slot can be booked
    /// </summary>
    public async Task<ApiResponse<bool>> CanBookTimeSlotAsync(Guid petWalkerId, DateTime startTime, DateTime endTime)
    {
        try
        {
            _logger.LogInformation("Checking if time slot can be booked for PetWalker: {PetWalkerId} from {StartTime} to {EndTime}",
                petWalkerId, startTime, endTime);

            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/petwalker/{petWalkerId}/can-book?startTime={startTime:yyyy-MM-ddTHH:mm:ss}&endTime={endTime:yyyy-MM-ddTHH:mm:ss}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<bool>(content, _jsonOptions);

                _logger.LogInformation("Time slot availability check result for PetWalker: {PetWalkerId} - {Available}",
                    petWalkerId, result);

                return new ApiResponse<bool>
                {
                    Success = true,
                    Message = result ? "Time slot is available" : "Time slot is not available",
                    Data = result,
                    Timestamp = DateTime.Now
                };
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to check time slot availability for PetWalker: {PetWalkerId}. Status: {StatusCode}, Error: {Error}",
                    petWalkerId, response.StatusCode, errorContent);

                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = $"Failed to check time slot availability. Status: {response.StatusCode}",
                    Errors = new List<string> { errorContent },
                    Data = false,
                    Timestamp = DateTime.Now
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking time slot availability for PetWalker: {PetWalkerId}", petWalkerId);

            return new ApiResponse<bool>
            {
                Success = false,
                Message = "An error occurred while checking time slot availability",
                Errors = new List<string> { ex.Message },
                Data = false,
                Timestamp = DateTime.Now
            };
        }
    }

    /// <summary>
    /// Get bookings for a specific client
    /// </summary>
    public async Task<ApiResponse<List<BookingDto>>> GetClientBookingsAsync(Guid clientId, DateTime startDate, DateTime endDate)
    {
        try
        {
            _logger.LogInformation("Getting bookings for Client: {ClientId} from {StartDate} to {EndDate}",
                clientId, startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));

            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/bookings/client/{clientId}?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<List<BookingDto>>(content, _jsonOptions) ?? new List<BookingDto>();

                _logger.LogInformation("Successfully retrieved {Count} bookings for Client: {ClientId}",
                    result.Count, clientId);

                return new ApiResponse<List<BookingDto>>
                {
                    Success = true,
                    Message = "Client bookings retrieved successfully",
                    Data = result,
                    Timestamp = DateTime.Now
                };
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to get bookings for Client: {ClientId}. Status: {StatusCode}, Error: {Error}",
                    clientId, response.StatusCode, errorContent);

                return new ApiResponse<List<BookingDto>>
                {
                    Success = false,
                    Message = $"Failed to get client bookings. Status: {response.StatusCode}",
                    Errors = new List<string> { errorContent },
                    Data = new List<BookingDto>(),
                    Timestamp = DateTime.Now
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting bookings for Client: {ClientId}", clientId);

            return new ApiResponse<List<BookingDto>>
            {
                Success = false,
                Message = "An error occurred while getting client bookings",
                Errors = new List<string> { ex.Message },
                Data = new List<BookingDto>(),
                Timestamp = DateTime.Now
            };
        }
    }

    /// <summary>
    /// Get bookings for a specific PetWalker
    /// </summary>
    public async Task<ApiResponse<List<BookingDto>>> GetPetWalkerBookingsAsync(Guid petWalkerId, DateTime startDate, DateTime endDate)
    {
        try
        {
            _logger.LogInformation("Getting bookings for PetWalker: {PetWalkerId} from {StartDate} to {EndDate}",
                petWalkerId, startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));

            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/bookings/petwalker/{petWalkerId}?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<List<BookingDto>>(content, _jsonOptions) ?? new List<BookingDto>();

                _logger.LogInformation("Successfully retrieved {Count} bookings for PetWalker: {PetWalkerId}",
                    result.Count, petWalkerId);

                return new ApiResponse<List<BookingDto>>
                {
                    Success = true,
                    Message = "PetWalker bookings retrieved successfully",
                    Data = result,
                    Timestamp = DateTime.Now
                };
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to get bookings for PetWalker: {PetWalkerId}. Status: {StatusCode}, Error: {Error}",
                    petWalkerId, response.StatusCode, errorContent);

                return new ApiResponse<List<BookingDto>>
                {
                    Success = false,
                    Message = $"Failed to get PetWalker bookings. Status: {response.StatusCode}",
                    Errors = new List<string> { errorContent },
                    Data = new List<BookingDto>(),
                    Timestamp = DateTime.Now
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting bookings for PetWalker: {PetWalkerId}", petWalkerId);

            return new ApiResponse<List<BookingDto>>
            {
                Success = false,
                Message = "An error occurred while getting PetWalker bookings",
                Errors = new List<string> { ex.Message },
                Data = new List<BookingDto>(),
                Timestamp = DateTime.Now
            };
        }
    }

    /// <summary>
    /// Get available PetWalkers for booking selection (legacy method for backward compatibility)
    /// </summary>
    public async Task<ApiResponse<List<PetWalkerSummaryDto>>> GetAvailablePetWalkersAsync(string? serviceArea = null)
    {
        var request = new GetAvailablePetWalkersRequest
        {
            ServiceArea = serviceArea ?? string.Empty,
            Page = 1,
            PageSize = 100 // Get a large number for backward compatibility
        };

        var paginatedResponse = await GetAvailablePetWalkersAsync(request);

        if (paginatedResponse.Success && paginatedResponse.Data != null)
        {
            return new ApiResponse<List<PetWalkerSummaryDto>>
            {
                Success = true,
                Message = paginatedResponse.Message,
                Data = paginatedResponse.Data.PetWalkers,
                Timestamp = paginatedResponse.Timestamp
            };
        }

        return new ApiResponse<List<PetWalkerSummaryDto>>
        {
            Success = false,
            Message = paginatedResponse.Message,
            Errors = paginatedResponse.Errors,
            Timestamp = paginatedResponse.Timestamp
        };
    }

    /// <summary>
    /// Get available PetWalkers for booking selection with pagination, sorting, and filtering
    /// </summary>
    public async Task<ApiResponse<PaginatedPetWalkersResponse>> GetAvailablePetWalkersAsync(GetAvailablePetWalkersRequest request)
    {
        try
        {
            _logger.LogInformation("Getting available PetWalkers - Page: {Page}, PageSize: {PageSize}, ServiceArea: {ServiceArea}, SortBy: {SortBy}",
                request.Page, request.PageSize, request.ServiceArea ?? "All", request.SortBy);

            var queryParams = new List<string>();

            if (!string.IsNullOrEmpty(request.ServiceArea))
                queryParams.Add($"serviceArea={Uri.EscapeDataString(request.ServiceArea)}");

            if (!string.IsNullOrEmpty(request.SearchTerm))
                queryParams.Add($"searchTerm={Uri.EscapeDataString(request.SearchTerm)}");

            queryParams.Add($"page={request.Page}");
            queryParams.Add($"pageSize={request.PageSize}");
            queryParams.Add($"sortBy={Uri.EscapeDataString(request.SortBy)}");
            queryParams.Add($"sortDirection={Uri.EscapeDataString(request.SortDirection)}");

            var url = $"{_apiBaseUrl}/petwalkers/available";
            if (queryParams.Any())
            {
                url += "?" + string.Join("&", queryParams);
            }

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                // First deserialize to the API response format
                var apiResponse = JsonSerializer.Deserialize<GetAvailablePetWalkersApiResponse>(content, _jsonOptions);

                if (apiResponse != null)
                {
                    // Map API response to client model
                    var clientResponse = new PaginatedPetWalkersResponse
                    {
                        PetWalkers = apiResponse.PetWalkers.Select(MapApiResponseToClientModel).ToList(),
                        PageNumber = apiResponse.PageNumber,
                        PageSize = apiResponse.PageSize,
                        TotalCount = apiResponse.TotalCount,
                        TotalPages = apiResponse.TotalPages,
                        HasPreviousPage = apiResponse.HasPreviousPage,
                        HasNextPage = apiResponse.HasNextPage,
                        AvailableServiceAreas = apiResponse.AvailableServiceAreas
                    };

                    _logger.LogInformation("Successfully retrieved {Count} available PetWalkers (Page {Page} of {TotalPages})",
                        clientResponse.PetWalkers.Count, clientResponse.PageNumber, clientResponse.TotalPages);

                    return new ApiResponse<PaginatedPetWalkersResponse>
                    {
                        Success = true,
                        Message = "Available PetWalkers retrieved successfully",
                        Data = clientResponse,
                        Timestamp = DateTime.Now
                    };
                }
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Failed to get available PetWalkers. Status: {StatusCode}, Error: {Error}",
                response.StatusCode, errorContent);

            return new ApiResponse<PaginatedPetWalkersResponse>
            {
                Success = false,
                Message = $"Failed to get available PetWalkers. Status: {response.StatusCode}",
                Errors = new List<string> { errorContent },
                Timestamp = DateTime.Now
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available PetWalkers");

            return new ApiResponse<PaginatedPetWalkersResponse>
            {
                Success = false,
                Message = "An error occurred while getting available PetWalkers",
                Errors = new List<string> { ex.Message },
                Timestamp = DateTime.Now
            };
        }
    }

    private static PetWalkerSummaryDto MapApiResponseToClientModel(PetWalkerSummaryApiResponse apiModel)
    {
        return new PetWalkerSummaryDto
        {
            Id = apiModel.Id,
            FullName = apiModel.FullName,
            Email = apiModel.Email,
            Biography = apiModel.Biography,
            HourlyRate = apiModel.HourlyRate,
            Currency = apiModel.Currency,
            IsActive = apiModel.IsActive,
            IsVerified = apiModel.IsVerified,
            YearsOfExperience = apiModel.YearsOfExperience,
            HasInsurance = apiModel.HasInsurance,
            HasFirstAidCertification = apiModel.HasFirstAidCertification,
            DailyPetWalkLimit = apiModel.DailyPetWalkLimit,
            BioPicture = !string.IsNullOrWhiteSpace(apiModel.BioPictureUrl)
                ? new PetWalkerBasicPhotoDto
                {
                    Uri = apiModel.BioPictureUrl,
                    Description = "Profile Picture"
                }
                : null,
            Rating = apiModel.Rating,
            ReviewCount = apiModel.ReviewCount,
            ServiceAreas = apiModel.ServiceAreas
        };
    }
}

// API Response Models (matching the actual API response structure)
public class GetAvailablePetWalkersApiResponse
{
    public List<PetWalkerSummaryApiResponse> PetWalkers { get; set; } = new();
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }
    public List<string> AvailableServiceAreas { get; set; } = new();
}

public class PetWalkerSummaryApiResponse
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Biography { get; set; }
    public decimal HourlyRate { get; set; }
    public string Currency { get; set; } = "USD";
    public bool IsActive { get; set; }
    public bool IsVerified { get; set; }
    public int YearsOfExperience { get; set; }
    public bool HasInsurance { get; set; }
    public bool HasFirstAidCertification { get; set; }
    public int DailyPetWalkLimit { get; set; }
    public string? BioPictureUrl { get; set; }
    public double Rating { get; set; } = 0.0;
    public int ReviewCount { get; set; } = 0;
    public List<string> ServiceAreas { get; set; } = new();
}
