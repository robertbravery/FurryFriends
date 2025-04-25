using System.Text.Json;
using FurryFriends.BlazorUI.Client.Models;
using FurryFriends.BlazorUI.Client.Models.Common;
using FurryFriends.BlazorUI.Client.Models.PetWalkers;
using FurryFriends.BlazorUI.Client.Services.Interfaces;

namespace FurryFriends.BlazorUI.Services.Implementation;

public class PetWalkerService : BaseListService<PetWalkerDto>, IPetWalkerService
{
  private readonly HttpClient _httpClient;
  private readonly string _apiBaseUrl;

  public PetWalkerService(HttpClient httpClient, IConfiguration configuration)
    : base(httpClient, configuration, "PetWalker/list")
  {
    _httpClient = httpClient;
    _apiBaseUrl = configuration["ApiBaseUrl"] ?? string.Empty;
  }

  public async Task<ListResponse<PetWalkerDto>> GetPetWalkersAsync(int page, int pageSize)
  {
    return await GetListAsync(page, pageSize);
  }

  public async Task<PetWalkerDto> GetPetWalkerByEmailAsync(string email)
  {
    var response = await _httpClient.GetFromJsonAsync<PetWalkerDto>($"{_apiBaseUrl}/PetWalkers/{email}");
    return response ?? new PetWalkerDto();
  }

  public async Task<ApiResponse<PetWalkerDetailDto>> GetPetWalkerDetailsByEmailAsync(string email)
  {
    try
    {
      // Call the API endpoint to get detailed pet walker information
      var response = await _httpClient.GetAsync($"{_apiBaseUrl}/PetWalker/email/{email}");

      if (!response.IsSuccessStatusCode)
      {
        return new ApiResponse<PetWalkerDetailDto>
        {
          Success = false,
          Message = $"Failed to load pet walker details: {response.StatusCode}",
          Errors = new List<string> { $"API returned status code: {response.StatusCode}" },
          Timestamp = DateTime.Now
        };
      }

      // Read the response content as a string
      var jsonString = await response.Content.ReadAsStringAsync();

      // Deserialize the JSON string into our ApiResponseBase to extract the data
      var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
      var apiResponse = JsonSerializer.Deserialize<ApiResponseBase>(jsonString, options);

      if (apiResponse == null || !apiResponse.Success || apiResponse.Data.ValueKind == JsonValueKind.Null)
      {
        return new ApiResponse<PetWalkerDetailDto>
        {
          Success = false,
          Message = apiResponse?.Message ?? "Failed to load pet walker details",
          Errors = apiResponse?.Errors,
          Timestamp = DateTime.Now
        };
      }

      // Extract the data from the JsonElement and map it to our PetWalkerDetailDto
      var petWalkerDetail = new PetWalkerDetailDto
      {
        Id = apiResponse.Data.GetProperty("id").GetGuid(),
        Name = apiResponse.Data.GetProperty("fullName").GetString() ?? string.Empty,
        EmailAddress = apiResponse.Data.GetProperty("email").GetString() ?? string.Empty,
        PhoneNumber = apiResponse.Data.GetProperty("phoneNumber").GetString() ?? string.Empty,
        City = apiResponse.Data.GetProperty("city").GetString() ?? string.Empty,
        ServiceAreas = new List<string>(),
        // Default values for properties that might not be in the API response
        HourlyRate = 25.00m,
        Currency = "USD",
        YearsOfExperience = 1,
        DailyPetWalkLimit = 5,
        IsVerified = true,
        HasInsurance = true,
        HasFirstAidCertification = true,
        Gender = "Not specified",
        Biography = "This pet walker has not provided a biography yet."
      };

      // Try to get additional fields if they exist
      if (apiResponse.Data.TryGetProperty("hourlyRate", out JsonElement hourlyRateElement) &&
          hourlyRateElement.ValueKind == JsonValueKind.Number)
      {
        petWalkerDetail.HourlyRate = hourlyRateElement.GetDecimal();
      }

      if (apiResponse.Data.TryGetProperty("currency", out JsonElement currencyElement) &&
          currencyElement.ValueKind == JsonValueKind.String)
      {
        petWalkerDetail.Currency = currencyElement.GetString() ?? "USD";
      }

      if (apiResponse.Data.TryGetProperty("yearsOfExperience", out JsonElement experienceElement) &&
          experienceElement.ValueKind == JsonValueKind.Number)
      {
        petWalkerDetail.YearsOfExperience = experienceElement.GetInt32();
      }

      if (apiResponse.Data.TryGetProperty("dailyPetWalkLimit", out JsonElement limitElement) &&
          limitElement.ValueKind == JsonValueKind.Number)
      {
        petWalkerDetail.DailyPetWalkLimit = limitElement.GetInt32();
      }

      if ((apiResponse.Data.TryGetProperty("isVerified", out JsonElement verifiedElement) &&
          verifiedElement.ValueKind == JsonValueKind.True) || verifiedElement.ValueKind == JsonValueKind.False)
      {
        petWalkerDetail.IsVerified = verifiedElement.GetBoolean();
      }

      if ((apiResponse.Data.TryGetProperty("hasInsurance", out JsonElement insuranceElement) &&
          insuranceElement.ValueKind == JsonValueKind.True) || insuranceElement.ValueKind == JsonValueKind.False)
      {
        petWalkerDetail.HasInsurance = insuranceElement.GetBoolean();
      }

      if ((apiResponse.Data.TryGetProperty("hasFirstAidCertification", out JsonElement certElement) &&
          certElement.ValueKind == JsonValueKind.True) || certElement.ValueKind == JsonValueKind.False)
      {
        petWalkerDetail.HasFirstAidCertification = certElement.GetBoolean();
      }

      if (apiResponse.Data.TryGetProperty("gender", out JsonElement genderElement) &&
          genderElement.ValueKind == JsonValueKind.String)
      {
        petWalkerDetail.Gender = genderElement.GetString() ?? "Not specified";
      }

      if (apiResponse.Data.TryGetProperty("biography", out JsonElement bioElement) &&
          bioElement.ValueKind == JsonValueKind.String)
      {
        petWalkerDetail.Biography = bioElement.GetString() ?? "This pet walker has not provided a biography yet.";
      }

      // Try to get the service locations if they exist
      if (apiResponse.Data.TryGetProperty("serviceLocation", out JsonElement locationsElement) &&
          locationsElement.ValueKind == JsonValueKind.Array)
      {
        foreach (var location in locationsElement.EnumerateArray())
        {
          petWalkerDetail.ServiceAreas.Add(location.GetString() ?? string.Empty);
        }
      }

      // Try to get address details if they exist
      if (apiResponse.Data.TryGetProperty("street", out JsonElement streetElement) &&
          streetElement.ValueKind == JsonValueKind.String)
      {
        petWalkerDetail.Street = streetElement.GetString() ?? string.Empty;
      }

      if (apiResponse.Data.TryGetProperty("state", out JsonElement stateElement) &&
          stateElement.ValueKind == JsonValueKind.String)
      {
        petWalkerDetail.State = stateElement.GetString() ?? string.Empty;
      }

      if (apiResponse.Data.TryGetProperty("zipCode", out JsonElement zipCodeElement) &&
          zipCodeElement.ValueKind == JsonValueKind.String)
      {
        petWalkerDetail.ZipCode = zipCodeElement.GetString() ?? string.Empty;
      }

      if (apiResponse.Data.TryGetProperty("country", out JsonElement countryElement) &&
          countryElement.ValueKind == JsonValueKind.String)
      {
        petWalkerDetail.Country = countryElement.GetString() ?? string.Empty;
      }

      // Try to get the profile picture if it exists
      if (apiResponse.Data.TryGetProperty("bioPicture", out JsonElement bioPictureElement) &&
          bioPictureElement.ValueKind == JsonValueKind.Object)
      {
        petWalkerDetail.ProfilePicture = new PhotoDto
        {
          Url = bioPictureElement.GetProperty("url").GetString() ?? string.Empty,
          Description = bioPictureElement.TryGetProperty("desciption", out JsonElement descElement) ?
                       descElement.GetString() ?? string.Empty : string.Empty
        };
      }

      // Try to get the photos if they exist
      if (apiResponse.Data.TryGetProperty("photos", out JsonElement photosElement) &&
          photosElement.ValueKind == JsonValueKind.Array)
      {
        petWalkerDetail.Photos = new List<PhotoDto>();
        foreach (var photo in photosElement.EnumerateArray())
        {
          petWalkerDetail.Photos.Add(new PhotoDto
          {
            Url = photo.GetProperty("url").GetString() ?? string.Empty,
            Description = photo.TryGetProperty("desciption", out JsonElement descElement) ?
                         descElement.GetString() ?? string.Empty : string.Empty
          });
        }
      }

      return new ApiResponse<PetWalkerDetailDto>
      {
        Success = true,
        Message = "Success",
        Data = petWalkerDetail,
        Timestamp = apiResponse.Timestamp
      };
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Error getting pet walker details: {ex.Message}");
      return new ApiResponse<PetWalkerDetailDto>
      {
        Success = false,
        Message = "Failed to load pet walker details",
        Errors = new List<string> { ex.Message },
        Timestamp = DateTime.Now
      };
    }
  }

  public async Task CreatePetWalkerAsync(PetWalkerRequestDto petWalkerModel)
  {
    await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/PetWalkers", petWalkerModel);
  }

  public async Task UpdatePetWalkerAsync(PetWalkerRequestDto petWalkerModel)
  {
    await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/PetWalkers/{petWalkerModel.Id}", petWalkerModel);
  }

  public async Task DeletePetWalkerAsync(string email)
  {
    await _httpClient.DeleteAsync($"{_apiBaseUrl}/PetWalkers/{email}");
  }
}
