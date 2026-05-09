using System.Net.Http.Json;
using FurryFriends.BlazorUI.Client.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FurryFriends.BlazorUI.Services.Implementation;

public class RatingService : IRatingService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiBaseUrl;
    private readonly ILogger<RatingService> _logger;

    public RatingService(HttpClient httpClient, IConfiguration configuration, ILogger<RatingService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _apiBaseUrl = configuration["ApiBaseUrl"] ?? "http://api";
    }

    public async Task<RatingSummaryDto?> GetRatingSummaryAsync(Guid petWalkerId)
    {
        try
        {
            _logger.LogInformation("Fetching rating summary for PetWalker: {PetWalkerId}", petWalkerId);
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/petwalkers/{petWalkerId}/ratings/summary");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to get rating summary. Status: {StatusCode}", response.StatusCode);
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<RatingSummaryDto>();
            _logger.LogInformation("Got rating summary for PetWalker: {PetWalkerId}, Average: {Average}, Total: {Total}",
                petWalkerId, result?.AverageRating, result?.TotalRatings);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting rating summary for PetWalker: {PetWalkerId}", petWalkerId);
            return null;
        }
    }

    public async Task<PaginatedRatingResponse> GetRatingsAsync(Guid petWalkerId, int page = 1, int pageSize = 10)
    {
        try
        {
            _logger.LogInformation("Fetching ratings for PetWalker: {PetWalkerId}, Page: {Page}, PageSize: {PageSize}",
                petWalkerId, page, pageSize);

            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/petwalkers/{petWalkerId}/ratings?page={page}&pageSize={pageSize}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to get ratings. Status: {StatusCode}", response.StatusCode);
                return new PaginatedRatingResponse(new List<RatingDto>(), page, pageSize, 0, 0, false, false);
            }

            // The API returns a List<RatingDto> directly, not wrapped in a paginated response
            // We build the paginated response from the data we receive
            var items = await response.Content.ReadFromJsonAsync<List<RatingDto>>();
            items ??= new List<RatingDto>();

            var totalCount = items.Count;
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            _logger.LogInformation("Got {Count} ratings for PetWalker: {PetWalkerId}",
                items.Count, petWalkerId);

            return new PaginatedRatingResponse(
                items,
                page,
                pageSize,
                totalCount,
                totalPages,
                page > 1,
                page < totalPages);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting ratings for PetWalker: {PetWalkerId}", petWalkerId);
            return new PaginatedRatingResponse(new List<RatingDto>(), page, pageSize, 0, 0, false, false);
        }
    }

    public async Task<RatingResult> CreateRatingAsync(CreateRatingRequest request)
    {
        try
        {
            _logger.LogInformation("Creating rating for PetWalker: {PetWalkerId}, Client: {ClientId}, Rating: {RatingValue}",
                request.PetWalkerId, request.ClientId, request.RatingValue);

            var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/ratings", request);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await TryReadErrorAsync(response);
                _logger.LogWarning("Failed to create rating. Status: {StatusCode}, Error: {Error}",
                    response.StatusCode, errorBody);
                return new RatingResult(false, errorBody ?? "Failed to create rating.");
            }

            _logger.LogInformation("Rating created successfully for PetWalker: {PetWalkerId}", request.PetWalkerId);
            return new RatingResult(true, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating rating for PetWalker: {PetWalkerId}", request.PetWalkerId);
            return new RatingResult(false, $"Error creating rating: {ex.Message}");
        }
    }

    public async Task<RatingResult> UpdateRatingAsync(Guid ratingId, UpdateRatingRequest request)
    {
        try
        {
            _logger.LogInformation("Updating rating: {RatingId}", ratingId);

            var response = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/ratings/{ratingId}", request);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await TryReadErrorAsync(response);
                _logger.LogWarning("Failed to update rating. Status: {StatusCode}, Error: {Error}",
                    response.StatusCode, errorBody);
                return new RatingResult(false, errorBody ?? "Failed to update rating.");
            }

            _logger.LogInformation("Rating updated successfully: {RatingId}", ratingId);
            return new RatingResult(true, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating rating: {RatingId}", ratingId);
            return new RatingResult(false, $"Error updating rating: {ex.Message}");
        }
    }

    public async Task<RatingResult> DeleteRatingAsync(Guid ratingId, Guid clientId)
    {
        try
        {
            _logger.LogInformation("Deleting rating: {RatingId}", ratingId);

            var deleteBody = new { RatingId = ratingId, ClientId = clientId };
            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, $"{_apiBaseUrl}/ratings/{ratingId}")
            {
                Content = JsonContent.Create(deleteBody)
            };
            var response = await _httpClient.SendAsync(requestMessage);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await TryReadErrorAsync(response);
                _logger.LogWarning("Failed to delete rating. Status: {StatusCode}, Error: {Error}",
                    response.StatusCode, errorBody);
                return new RatingResult(false, errorBody ?? "Failed to delete rating.");
            }

            _logger.LogInformation("Rating deleted successfully: {RatingId}", ratingId);
            return new RatingResult(true, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting rating: {RatingId}", ratingId);
            return new RatingResult(false, $"Error deleting rating: {ex.Message}");
        }
    }

    private async Task<string?> TryReadErrorAsync(HttpResponseMessage response)
    {
        try
        {
            var body = await response.Content.ReadAsStringAsync();
            return string.IsNullOrWhiteSpace(body) ? null : body;
        }
        catch
        {
            return null;
        }
    }
}
