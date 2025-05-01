using System.Net.Http.Headers; // Required for MediaTypeHeaderValue
using System.Text.Json;
using FurryFriends.BlazorUI.Client.Models.Common;
using FurryFriends.BlazorUI.Client.Models.Picture;
using FurryFriends.BlazorUI.Client.Services.Interfaces;
using Microsoft.AspNetCore.Components.Forms; // Required for IBrowserFile
using Microsoft.Extensions.Logging;

namespace FurryFriends.BlazorUI.Services.Implementation;

public class PictureService : IPictureService
{
  private readonly HttpClient _httpClient;
  private readonly string _apiBaseUrl;
  private readonly ILogger<PictureService> _logger;

  public PictureService(HttpClient httpClient, IConfiguration configuration, ILogger<PictureService> logger)
  {
    _httpClient = httpClient;
    _apiBaseUrl = configuration["ApiBaseUrl"] ?? string.Empty;
    _logger = logger;
  }
  public async Task<ApiResponse<DetailedPhotoDto>> UpdateBioPictureAsync(Guid petWalkerId, IBrowserFile file)
  {
    using var content = new MultipartFormDataContent();
    // Limit stream size during copy
    using var fileStream = file.OpenReadStream(maxAllowedSize: 5 * 1024 * 1024); // 5MB limit example
    using var memoryStream = new MemoryStream();
    await fileStream.CopyToAsync(memoryStream);
    memoryStream.Position = 0; // Reset stream position

    var fileContent = new StreamContent(memoryStream);
    fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

    // "file" is the parameter name expected by the backend API endpoint
    content.Add(content: fileContent, name: "\"file\"", fileName: file.Name);

    var apiUrl = $"{_apiBaseUrl}/api/petwalkers/{petWalkerId}/biopicture"; // Adjust route

    try
    {
      // Use PUT for updating/replacing the bio picture
      var response = await _httpClient.PutAsync(apiUrl, content);
      var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
      var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseBase>(options);
      //return await ApiResponse<PhotoDto>.FromHttpResponseAsync(response);
      if (apiResponse is null)
      {
        throw new HttpRequestException("Failed to read JSON information");
      }
      if (!apiResponse.Success)
      {
        return new ApiResponse<DetailedPhotoDto>
        {
          Success = false,
          Message = apiResponse.Message,
          Errors = apiResponse.Errors,
          Timestamp = DateTime.Now
        };
      }
      var photoData = JsonSerializer.Deserialize<DetailedPhotoDto>(apiResponse.Data.GetRawText(), options);
      return new ApiResponse<DetailedPhotoDto>
      {
        Success = apiResponse.Success,
        Message = apiResponse.Message,
        Data = photoData,
        Timestamp = apiResponse.Timestamp
      };
    }
    catch (HttpRequestException ex)
    {
      return new ApiResponse<DetailedPhotoDto>
      {
        Success = false,
        Message = $"Failed to update pet walker bio picture. Status: {ex.StatusCode}. Details: {ex.Message}",
        Timestamp = DateTime.Now
      };
    }
  }

  public async Task<ApiResponse<List<DetailedPhotoDto>>> AddPhotosAsync(Guid petWalkerId, IEnumerable<IBrowserFile> files)
  {
    using var content = new MultipartFormDataContent();
    foreach (var file in files)
    {
      // Limit stream size during copy
      using var fileStream = file.OpenReadStream(maxAllowedSize: 5 * 1024 * 1024); // 5MB limit example
      using var memoryStream = new MemoryStream();
      await fileStream.CopyToAsync(memoryStream);
      memoryStream.Position = 0; // Reset stream position

      var fileContent = new StreamContent(memoryStream);
      fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

      // "files" is the parameter name expected by the backend API endpoint for multiple files
      content.Add(content: fileContent, name: "\"files\"", fileName: file.Name);
    }

    var apiUrl = $"{_apiBaseUrl}/api/petwalkers/{petWalkerId}/photos"; // Adjust route

    try
    {
      // Use POST for adding new photos
      var response = await _httpClient.PostAsync(apiUrl, content);
      //return await ApiResponse<List<PhotoDto>>.FromHttpResponseAsync(response);
      var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
      var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseBase>(options);
      if (apiResponse is null)
      {
        throw new HttpRequestException("Failed to read JSON information");
      }
      if (!apiResponse.Success)
      {
        return new ApiResponse<List<DetailedPhotoDto>>
        {
          Success = false,
          Message = apiResponse.Message,
          Errors = apiResponse.Errors,
          Timestamp = DateTime.Now
        };
      }
      var photoData = JsonSerializer.Deserialize<List<DetailedPhotoDto>>(apiResponse.Data.GetRawText(), options);
      return new ApiResponse<List<DetailedPhotoDto>>
      {
        Success = apiResponse.Success,
        Message = apiResponse.Message,
        Data = photoData,
        Timestamp = apiResponse.Timestamp
      };
    }
    catch (HttpRequestException ex)
    {
      return new ApiResponse<List<DetailedPhotoDto>>
      {
        Success = false,
        Message = $"Error updating photo's: {ex.Message}",
        Errors = [ex.Message],
        Timestamp = DateTime.Now
      };
    }
  }

  public async Task<ApiResponse<bool>> DeletePhotoAsync(Guid petWalkerId, Guid photoId)
  {
    var apiUrl = $"{_apiBaseUrl}/api/petwalkers/{petWalkerId}/photos/{photoId}"; // Adjust route

    try
    {
      var response = await _httpClient.DeleteAsync(apiUrl);
      // Check for success status code (e.g., 200 OK or 204 No Content)
      if (response.IsSuccessStatusCode)
      {
        return new ApiResponse<bool>
        {
          Success = true,
          Message = "Photo deleted successfully.",
          Data = true,
          Timestamp = DateTime.Now
        };
      }
      else
      {
        var errorContent = await response.Content.ReadAsStringAsync();
        return new ApiResponse<bool>
        {
          Success = false,
          Message = $"Failed to delete photo. Status: {response.StatusCode}. Details: {errorContent}",
          Timestamp = DateTime.Now
        };
      }
    }
    catch (HttpRequestException ex)
    {
      return new ApiResponse<bool>
      {
        Success = false,
        Message = $"Network error. Status: {ex.StatusCode}. Details: {ex.Message}",
        Timestamp = DateTime.Now
      };
    }
  }

  public async Task<ApiResponse<PictureViewModel>> GetPetWalkerByIdAsync(Guid currentPetWalkerId)
  {
    var apiUrl = $"{_apiBaseUrl}/PetWalker/{currentPetWalkerId}/photos";
    _logger.LogInformation("Fetching pet walker photos for ID: {PetWalkerId}", currentPetWalkerId);

    try
    {
      var response = await _httpClient.GetAsync(apiUrl);
      var options = new JsonSerializerOptions {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
      };

      if (response.IsSuccessStatusCode)
      {
        // First, log the raw response for debugging
        var rawResponse = await response.Content.ReadAsStringAsync();
        _logger.LogDebug("Raw API response for pet walker {PetWalkerId}: {RawResponse}", currentPetWalkerId, rawResponse);

        // Try to deserialize directly to PictureViewModel
        PictureViewModel? pictureData;
        try
        {
          pictureData = JsonSerializer.Deserialize<PictureViewModel>(rawResponse, options);

          // Ensure Photos collection is initialized
          if (pictureData != null)
          {
            if (pictureData.Photos == null)
            {
              pictureData.Photos = new List<DetailedPhotoDto>();
              _logger.LogWarning("Photos collection was null for pet walker {PetWalkerId}, initialized empty list", currentPetWalkerId);
            }

            _logger.LogDebug("Deserialized PictureViewModel: PetWalkerId={PetWalkerId}, Name={Name}, ProfilePicture={ProfilePicture}, Photos Count={PhotosCount}",
                pictureData.PetWalkerId,
                pictureData.PetwalkerName,
                pictureData.ProfilePicture != null ? "Present" : "Null",
                pictureData.Photos.Count);

            // Log each photo for debugging
            if (pictureData.Photos.Any())
            {
              foreach (var photo in pictureData.Photos)
              {
                _logger.LogDebug("Photo details: Id={PhotoId}, Type={PhotoType}, Url={PhotoUrl}",
                    photo.Id, photo.PhotoType, photo.Url);
              }
            }
          }
        }
        catch (JsonException ex)
        {
          _logger.LogWarning(ex, "JSON deserialization error for pet walker {PetWalkerId}", currentPetWalkerId);

          // Fallback: Try to parse as ApiResponseBase and extract data
          try
          {
            var apiResponse = JsonSerializer.Deserialize<ApiResponseBase>(rawResponse, options);
            if (apiResponse != null && apiResponse.Data.ValueKind != JsonValueKind.Null)
            {
              pictureData = JsonSerializer.Deserialize<PictureViewModel>(apiResponse.Data.GetRawText(), options);
              _logger.LogInformation("Successfully deserialized from ApiResponseBase.Data for pet walker {PetWalkerId}", currentPetWalkerId);
            }
            else
            {
              // Create a default PictureViewModel with the petwalker ID
              pictureData = new PictureViewModel
              {
                PetWalkerId = currentPetWalkerId,
                PetwalkerName = "Pet Walker",
                Photos = new List<DetailedPhotoDto>()
              };
              _logger.LogInformation("Created default PictureViewModel as fallback for pet walker {PetWalkerId}", currentPetWalkerId);
            }
          }
          catch (Exception innerEx)
          {
            _logger.LogError(innerEx, "Fallback deserialization error for pet walker {PetWalkerId}", currentPetWalkerId);
            return new ApiResponse<PictureViewModel>
            {
              Success = false,
              Message = $"Failed to parse response: {ex.Message}",
              Timestamp = DateTime.Now
            };
          }
        }

        _logger.LogInformation("Successfully retrieved picture data for pet walker {PetWalkerId}", currentPetWalkerId);
        return new ApiResponse<PictureViewModel>
        {
          Success = true,
          Data = pictureData,
          Timestamp = DateTime.UtcNow
        };
      }

      _logger.LogWarning("Failed to get pet walker photos. Status: {StatusCode}, Reason: {ReasonPhrase}",
          response.StatusCode, response.ReasonPhrase);
      return new ApiResponse<PictureViewModel>
      {
        Success = false,
        Message = $"Error: {response.StatusCode} - {response.ReasonPhrase}",
        Timestamp = DateTime.Now
      };
    }
    catch (HttpRequestException ex)
    {
      _logger.LogError(ex, "Failed to get pet walker photos for ID: {PetWalkerId}", currentPetWalkerId);
      return new ApiResponse<PictureViewModel>
      {
        Success = false,
        Message = $"Failed to get pet walker photos. Status: {ex.StatusCode}. Details: {ex.Message}",
        Timestamp = DateTime.Now
      };
    }
  }
}

