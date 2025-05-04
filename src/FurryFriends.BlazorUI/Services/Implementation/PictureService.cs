using System.Net.Http.Headers;
using System.Text.Json;
using FurryFriends.BlazorUI.Client.Models.Common;
using FurryFriends.BlazorUI.Client.Models.Picture;
using FurryFriends.BlazorUI.Client.Services.Interfaces;
using Microsoft.AspNetCore.Components.Forms;

namespace FurryFriends.BlazorUI.Services.Implementation;

public class PictureService : IPictureService
{
  private readonly HttpClient _httpClient;
  private readonly string _apiBaseUrl;
  private readonly ILogger<PictureService> _logger;
  private readonly JsonSerializerOptions _jsonOptions;
  private const int MaxFileSize = 5 * 1024 * 1024; // 5MB

  private static class ErrorMessages
  {
    public const string JsonDeserializationError = "Failed to read JSON response";
    public const string NetworkError = "Network error occurred: {0}";
    public const string DeleteError = "Failed to delete photo. Status: {0}. Details: {1}";
    public const string UpdateError = "Failed to update pet walker bio picture. Status: {0}. Details: {1}";
    public const string PhotoUploadError = "Error updating photos: {0}";
    public const string NullResponseData = "Response data is null or invalid";
  }

  public PictureService(HttpClient httpClient, IConfiguration configuration, ILogger<PictureService> logger)
  {
    _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    _apiBaseUrl = configuration["ApiBaseUrl"] ?? string.Empty;
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    _jsonOptions = new JsonSerializerOptions
    {
      PropertyNameCaseInsensitive = true,
      PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
  }

  public async Task<ApiResponse<DetailedPhotoDto>> UpdateBioPictureAsync(Guid petWalkerId, IBrowserFile file)
  {
    try
    {
      // First, get the current pictures to find the bio picture ID
      var picturesResponse = await GetPetWalkerPicturesAsync(petWalkerId);
      if (!picturesResponse.Success || picturesResponse.Data == null)
      {
        return CreateErrorResponse<DetailedPhotoDto>("Failed to retrieve pet walker pictures");
      }

      // Get the bio picture ID
      var bioPicture = picturesResponse.Data.ProfilePicture;
      if (bioPicture == null)
      {
        // If there's no bio picture yet, we should add one instead of updating
        _logger.LogInformation("No existing bio picture found for pet walker {PetWalkerId}, adding a new one", petWalkerId);

        // Create a new bio picture
        return await AddBioPictureAsync(petWalkerId, file);
      }

      // Create the content without using a using statement to avoid premature disposal
      var content = await CreateFileContent(file, "file");

      // Use the UpdatePhoto endpoint with the bio picture ID
      var apiUrl = $"{_apiBaseUrl}/PetWalker/{petWalkerId}/photos/{bioPicture.Id}";

      // Send the request
      var response = await _httpClient.PutAsync(apiUrl, content);

      // Process the response
      var result = await HandleApiResponseAsync<DetailedPhotoDto>(
          response,
          string.Format(ErrorMessages.UpdateError, "{0}", "{1}")
      );

      // Dispose the content after we're done with it
      content.Dispose();

      return result;
    }
    catch (HttpRequestException ex)
    {
      _logger.LogError(ex, "Network error while updating bio picture for pet walker {PetWalkerId}", petWalkerId);
      return CreateErrorResponse<DetailedPhotoDto>(string.Format(ErrorMessages.NetworkError, ex.Message));
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Unexpected error while updating bio picture for pet walker {PetWalkerId}: {Message}", petWalkerId, ex.Message);
      return CreateErrorResponse<DetailedPhotoDto>($"Unexpected error: {ex.Message}");
    }
  }

  public async Task<ApiResponse<DetailedPhotoDto>> AddBioPictureAsync(Guid petWalkerId, IBrowserFile file)
  {
    try
    {
      // Create the content without using a using statement to avoid premature disposal
      var content = new MultipartFormDataContent();

      // Create a memory stream that will be owned by the StreamContent
      var memoryStream = new MemoryStream();

      // Open the file stream with a using statement to ensure it's properly disposed
      using (var fileStream = file.OpenReadStream(maxAllowedSize: MaxFileSize))
      {
        await fileStream.CopyToAsync(memoryStream);
      }

      // Reset the position to the beginning
      memoryStream.Position = 0;

      var fileContent = new StreamContent(memoryStream);
      fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
      content.Add(fileContent, "file", file.Name);

      // Add a form field to indicate this is a bio picture
      content.Add(new StringContent("BioPic"), "photoType");

      var apiUrl = $"{_apiBaseUrl}/PetWalker/{petWalkerId}/photos";
      var response = await _httpClient.PostAsync(apiUrl, content);

      var result = await HandleApiResponseAsync<DetailedPhotoDto>(
          response,
          string.Format(ErrorMessages.PhotoUploadError, "{0}")
      );

      // Dispose the content after we're done with it
      content.Dispose();

      return result;
    }
    catch (HttpRequestException ex)
    {
      _logger.LogError(ex, "Network error while adding bio picture for pet walker {PetWalkerId}", petWalkerId);
      return CreateErrorResponse<DetailedPhotoDto>(string.Format(ErrorMessages.NetworkError, ex.Message));
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Unexpected error while adding bio picture for pet walker {PetWalkerId}: {Message}", petWalkerId, ex.Message);
      return CreateErrorResponse<DetailedPhotoDto>($"Unexpected error: {ex.Message}");
    }
  }

  public async Task<ApiResponse<List<DetailedPhotoDto>>> AddPhotosAsync(Guid petWalkerId, IEnumerable<IBrowserFile> files)
  {
    // Create the content without using a using statement to avoid premature disposal
    var content = new MultipartFormDataContent();

    try
    {
      foreach (var file in files)
      {
        // Create a memory stream that will be owned by the StreamContent
        var memoryStream = new MemoryStream();

        // Open the file stream with a using statement to ensure it's properly disposed
        using (var fileStream = file.OpenReadStream(maxAllowedSize: MaxFileSize))
        {
          await fileStream.CopyToAsync(memoryStream);
        }

        // Reset the position to the beginning
        memoryStream.Position = 0;

        var fileContent = new StreamContent(memoryStream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
        content.Add(fileContent, "files", file.Name);
      }

      var apiUrl = $"{_apiBaseUrl}/PetWalker/{petWalkerId}/photos";
      var response = await _httpClient.PostAsync(apiUrl, content);

      var result = await HandleApiResponseAsync<List<DetailedPhotoDto>>(
          response,
          string.Format(ErrorMessages.PhotoUploadError, "{0}")
      );

      // Dispose the content after we're done with it
      content.Dispose();

      return result;
    }
    catch (HttpRequestException ex)
    {
      _logger.LogError(ex, "Network error while adding photos for pet walker {PetWalkerId}", petWalkerId);
      content.Dispose();
      return CreateErrorResponse<List<DetailedPhotoDto>>(string.Format(ErrorMessages.NetworkError, ex.Message));
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Unexpected error while adding photos for pet walker {PetWalkerId}: {Message}", petWalkerId, ex.Message);
      content.Dispose();
      return CreateErrorResponse<List<DetailedPhotoDto>>($"Unexpected error: {ex.Message}");
    }
  }

  public async Task<ApiResponse<bool>> DeletePhotoAsync(Guid petWalkerId, Guid photoId)
  {
    try
    {
      var apiUrl = $"{_apiBaseUrl}/PetWalker/{petWalkerId}/photos/{photoId}";
      var response = await _httpClient.DeleteAsync(apiUrl);

      if (response.IsSuccessStatusCode)
      {
        return new ApiResponse<bool>
        {
          Success = true,
          Message = "Photo deleted successfully.",
          Data = true,
          Timestamp = DateTime.UtcNow
        };
      }

      var errorContent = await response.Content.ReadAsStringAsync();
      return CreateErrorResponse<bool>(
          string.Format(ErrorMessages.DeleteError, response.StatusCode, errorContent)
      );
    }
    catch (HttpRequestException ex)
    {
      _logger.LogError(ex, "Network error while deleting photo {PhotoId} for pet walker {PetWalkerId}", photoId, petWalkerId);
      return CreateErrorResponse<bool>(string.Format(ErrorMessages.NetworkError, ex.Message));
    }
  }

  public async Task<ApiResponse<PictureViewModel>> GetPetWalkerPicturesAsync(Guid petWalkerId)
  {
    try
    {
      var apiUrl = $"{_apiBaseUrl}/PetWalker/{petWalkerId}/photos";
      _logger.LogInformation("Fetching pet walker photos for ID: {PetWalkerId}", petWalkerId);

      var response = await _httpClient.GetAsync(apiUrl);
      if (response.IsSuccessStatusCode)
      {
        var rawResponse = await response.Content.ReadAsStringAsync();
        _logger.LogDebug("Raw API response: {RawResponse}", rawResponse);

        try
        {
          var pictureData = JsonSerializer.Deserialize<PictureViewModel>(rawResponse, _jsonOptions);
          if (pictureData != null)
          {
            pictureData.Photos ??= new List<DetailedPhotoDto>();
            LogPictureData(pictureData, petWalkerId);
            return new ApiResponse<PictureViewModel>
            {
              Success = true,
              Data = pictureData,
              Timestamp = DateTime.UtcNow
            };
          }
        }
        catch (JsonException ex)
        {
          return await HandleJsonDeserializationError<PictureViewModel>(rawResponse, ex, petWalkerId);
        }
      }

      _logger.LogWarning("Failed to get pet walker photos. Status: {StatusCode}", response.StatusCode);
      var errorContent = await response.Content.ReadAsStringAsync();
      return CreateErrorResponse<PictureViewModel>($"Error: {response.StatusCode} - {errorContent}");
    }
    catch (HttpRequestException ex)
    {
      _logger.LogError(ex, "Network error while fetching photos for pet walker {PetWalkerId}", petWalkerId);
      return CreateErrorResponse<PictureViewModel>(string.Format(ErrorMessages.NetworkError, ex.Message));
    }
  }

  private async Task<MultipartFormDataContent> CreateFileContent(IBrowserFile file, string name)
  {
    var content = new MultipartFormDataContent();

    // Create a memory stream that will be owned by the StreamContent
    var memoryStream = new MemoryStream();

    // Open the file stream with a using statement to ensure it's properly disposed
    using (var fileStream = file.OpenReadStream(maxAllowedSize: MaxFileSize))
    {
      await fileStream.CopyToAsync(memoryStream);
    }

    // Reset the position to the beginning
    memoryStream.Position = 0;

    // Create a StreamContent that will take ownership of the memory stream
    var fileContent = new StreamContent(memoryStream);
    fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
    content.Add(fileContent, name, file.Name);

    return content;
  }

  private async Task<ApiResponse<T>> HandleApiResponseAsync<T>(HttpResponseMessage response, string errorMessageTemplate)
  {
    if (!response.IsSuccessStatusCode)
    {
      var errorContent = await response.Content.ReadAsStringAsync();
      return CreateErrorResponse<T>(string.Format(errorMessageTemplate, response.StatusCode, errorContent));
    }

    try
    {
      var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseBase>(_jsonOptions);
      if (apiResponse == null)
      {
        throw new HttpRequestException(ErrorMessages.JsonDeserializationError);
      }

      if (!apiResponse.Success)
      {
        return CreateErrorResponse<T>(apiResponse.Message ?? "Operation failed", apiResponse.Errors);
      }

      // Check if Data property exists and is not null
      if (apiResponse.Data.ValueKind == JsonValueKind.Null || apiResponse.Data.ValueKind == JsonValueKind.Undefined)
      {
        return CreateErrorResponse<T>(ErrorMessages.NullResponseData);
      }

      try
      {
        var data = JsonSerializer.Deserialize<T>(apiResponse.Data.GetRawText(), _jsonOptions);
        if (data == null)
        {
          return CreateErrorResponse<T>(ErrorMessages.NullResponseData);
        }

        return new ApiResponse<T>
        {
          Success = true,
          Message = apiResponse.Message,
          Data = data,
          Timestamp = DateTime.UtcNow
        };
      }
      catch (JsonException ex)
      {
        _logger.LogError(ex, "Error deserializing response data");
        return CreateErrorResponse<T>($"{ErrorMessages.JsonDeserializationError}: {ex.Message}");
      }
    }
    catch (JsonException ex)
    {
      _logger.LogError(ex, "JSON deserialization error");
      return CreateErrorResponse<T>($"{ErrorMessages.JsonDeserializationError}: {ex.Message}");
    }
  }

  private async Task<ApiResponse<T>> HandleJsonDeserializationError<T>(string rawResponse, JsonException originalEx, Guid petWalkerId)
  {
    _logger.LogWarning(originalEx, "Primary JSON deserialization failed for {Type}, attempting fallback", typeof(T).Name);

    try
    {
      var apiResponse = await Task.Run(() => JsonSerializer.Deserialize<ApiResponseBase>(rawResponse, _jsonOptions));
      if (apiResponse == null)
      {
        return CreateErrorResponse<T>(ErrorMessages.JsonDeserializationError);
      }

      // First ensure Data property exists and is valid
      if (apiResponse.Data.ValueKind != JsonValueKind.Null && apiResponse.Data.ValueKind != JsonValueKind.Undefined)
      {
        try
        {
          var data = JsonSerializer.Deserialize<T>(apiResponse.Data.GetRawText(), _jsonOptions);
          if (data != null)
          {
            return new ApiResponse<T>
            {
              Success = true,
              Data = data,
              Timestamp = DateTime.UtcNow
            };
          }
        }
        catch
        {
          // If deserialization fails, continue to fallback
          _logger.LogDebug("Secondary deserialization failed, continuing to fallback");
        }
      }

      // For PictureViewModel, create a default instance
      if (typeof(T) == typeof(PictureViewModel))
      {
        var defaultModel = new PictureViewModel
        {
          PetWalkerId = petWalkerId,
          PetWalkerName = "Pet Walker",
          Photos = new List<DetailedPhotoDto>()
        };
        return new ApiResponse<T>
        {
          Success = true,
          Data = (T)(object)defaultModel,
          Timestamp = DateTime.UtcNow
        };
      }
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Fallback deserialization failed");
    }

    return CreateErrorResponse<T>($"{ErrorMessages.JsonDeserializationError}: {originalEx.Message}");
  }

  private ApiResponse<T> CreateErrorResponse<T>(string message, IEnumerable<string>? errors = null)
  {
    return new ApiResponse<T>
    {
      Success = false,
      Message = message,
      Errors = errors?.ToList(),
      Timestamp = DateTime.UtcNow
    };
  }

  private void LogPictureData(PictureViewModel pictureData, Guid petWalkerId)
  {
    _logger.LogDebug("Picture data for {PetWalkerId}: Name={Name}, ProfilePic={HasProfilePic}, Photos={PhotoCount}",
        petWalkerId,
        pictureData.PetWalkerName,
        pictureData.ProfilePicture != null ? "Present" : "Null",
        pictureData.Photos.Count);

    foreach (var photo in pictureData.Photos)
    {
      _logger.LogTrace("Photo: Id={PhotoId}, Type={PhotoType}, Url={Url}",
          photo.Id, photo.PhotoType, photo.Url);
    }
  }
}
