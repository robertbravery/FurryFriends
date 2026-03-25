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

    public async Task<List<RatingDto>> GetRatingsAsync(Guid petWalkerId, int page = 1, int pageSize = 20)
    {
        try
        {
            _logger.LogInformation("Fetching ratings for PetWalker: {PetWalkerId}, Page: {Page}, PageSize: {PageSize}", 
                petWalkerId, page, pageSize);
            
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/petwalkers/{petWalkerId}/ratings?page={page}&pageSize={pageSize}");
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to get ratings. Status: {StatusCode}", response.StatusCode);
                return new List<RatingDto>();
            }

            var result = await response.Content.ReadFromJsonAsync<List<RatingDto>>();
            _logger.LogInformation("Got {Count} ratings for PetWalker: {PetWalkerId}", 
                result?.Count ?? 0, petWalkerId);
            
            return result ?? new List<RatingDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting ratings for PetWalker: {PetWalkerId}", petWalkerId);
            return new List<RatingDto>();
        }
    }

    public async Task<bool> CreateRatingAsync(CreateRatingRequest request)
    {
        try
        {
            _logger.LogInformation("Creating rating for Booking: {BookingId}, Rating: {RatingValue}", 
                request.BookingId, request.RatingValue);
            
            var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/ratings", request);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to create rating. Status: {StatusCode}", response.StatusCode);
                return false;
            }

            _logger.LogInformation("Rating created successfully for Booking: {BookingId}", request.BookingId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating rating for Booking: {BookingId}", request.BookingId);
            return false;
        }
    }

    public async Task<bool> UpdateRatingAsync(Guid ratingId, UpdateRatingRequest request)
    {
        try
        {
            _logger.LogInformation("Updating rating: {RatingId}", ratingId);
            
            var response = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/ratings/{ratingId}", request);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to update rating. Status: {StatusCode}", response.StatusCode);
                return false;
            }

            _logger.LogInformation("Rating updated successfully: {RatingId}", ratingId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating rating: {RatingId}", ratingId);
            return false;
        }
    }
}
