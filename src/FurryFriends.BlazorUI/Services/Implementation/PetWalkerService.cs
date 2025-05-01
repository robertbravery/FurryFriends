using System.Text.Json;
using FurryFriends.BlazorUI.Client.Models;
using FurryFriends.BlazorUI.Client.Models.Common;
using FurryFriends.BlazorUI.Client.Models.Locations;
using FurryFriends.BlazorUI.Client.Models.PetWalkers;
using FurryFriends.BlazorUI.Client.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace FurryFriends.BlazorUI.Services.Implementation;

public class PetWalkerService : BaseListService<PetWalkerDto>, IPetWalkerService
{
  private readonly HttpClient _httpClient;
  private readonly string _apiBaseUrl;
  private readonly ILogger<PetWalkerService> _logger;

  public PetWalkerService(HttpClient httpClient, IConfiguration configuration, ILogger<PetWalkerService> logger)
      : base(httpClient, configuration, "PetWalker/list")
  {
    _httpClient = httpClient;
    _apiBaseUrl = configuration["ApiBaseUrl"] ?? string.Empty;
    _logger = logger;
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
      var response = await _httpClient.GetAsync($"{_apiBaseUrl}/PetWalker/email/{email}");

      if (!response.IsSuccessStatusCode)
      {
        return new ApiResponse<PetWalkerDetailDto>
        {
          Success = false,
          Message = $"Failed to load pet walker details: {response.StatusCode}",
          Errors = [$"API returned status code: {response.StatusCode}"],
          Timestamp = DateTime.Now
        };
      }

      var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
      var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseBase>(options);

      if (apiResponse == null || !apiResponse.Success)
      {
        return new ApiResponse<PetWalkerDetailDto>
        {
          Success = false,
          Message = apiResponse?.Message ?? "Failed to load pet walker details",
          Errors = apiResponse?.Errors,
          Timestamp = DateTime.Now
        };
      }

      var petWalkerData = JsonSerializer.Deserialize<PetWalkerApiResponse>(apiResponse.Data.GetRawText(), options);

      if (petWalkerData == null)
      {
        return new ApiResponse<PetWalkerDetailDto>
        {
          Success = false,
          Message = "Failed to deserialize pet walker data",
          Timestamp = DateTime.Now
        };
      }

      var petWalkerDetail = new PetWalkerDetailDto
      {
        Id = petWalkerData.Id,
        Name = petWalkerData.FullName,
        EmailAddress = petWalkerData.Email,
        CountryCode = petWalkerData.CountryCode,
        PhoneNumber = petWalkerData.PhoneNumber,
        City = petWalkerData.City,
        HourlyRate = petWalkerData.HourlyRate,
        Currency = petWalkerData.Currency,
        YearsOfExperience = petWalkerData.YearsOfExperience,
        DailyPetWalkLimit = petWalkerData.DailyPetWalkLimit,
        IsVerified = petWalkerData.IsVerified,
        HasInsurance = petWalkerData.HasInsurance,
        HasFirstAidCertification = petWalkerData.HasFirstAidCertification,
        Gender = petWalkerData.Gender,
        Biography = petWalkerData.Biography,
        Street = petWalkerData.Street,
        State = petWalkerData.State,
        ZipCode = petWalkerData.ZipCode,
        Country = petWalkerData.Country,
        ServiceAreas = petWalkerData.Locations.Concat(petWalkerData.ServiceLocation)
              .Where(x => !string.IsNullOrEmpty(x))
              .Distinct()
              .ToList(),
        ProfilePicture = petWalkerData.BioPicture,
        Photos = petWalkerData.Photos
      };

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
      _logger.LogError(ex, "Error getting pet walker details for email: {Email}", email);
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

  public async Task<ApiResponse<bool>> UpdatePetWalkerAsync(PetWalkerDetailDto petWalkerModel)
  {
    try
    {
      var nameParts = petWalkerModel.Name.Split(' ', 2);
      var firstName = nameParts[0];
      var lastName = nameParts.Length > 1 ? nameParts[1] : string.Empty;

      var serviceAreas = petWalkerModel.ServiceAreas
          .Where(area => !string.IsNullOrWhiteSpace(area))
          .ToList();

      var updateRequest = new
      {
        PetWalkerId = petWalkerModel.Id,
        FirstName = firstName,
        LastName = lastName,
        CountryCode = petWalkerModel.CountryCode.TrimStart('+'),
        PhoneNumber = petWalkerModel.PhoneNumber,
        ServiceAreas = serviceAreas,
        Street = petWalkerModel.Street,
        City = petWalkerModel.City,
        State = petWalkerModel.State,
        ZipCode = petWalkerModel.ZipCode,
        Country = petWalkerModel.Country,
        Biography = petWalkerModel.Biography,
        DateOfBirth = petWalkerModel.DateOfBirth,
        Gender = ConvertGenderToInt(petWalkerModel.Gender),
        HourlyRate = petWalkerModel.HourlyRate,
        Currency = petWalkerModel.Currency,
        IsActive = petWalkerModel.IsActive,
        IsVerified = petWalkerModel.IsVerified,
        YearsOfExperience = petWalkerModel.YearsOfExperience,
        HasInsurance = petWalkerModel.HasInsurance,
        HasFirstAidCertification = petWalkerModel.HasFirstAidCertification,
        DailyPetWalkLimit = petWalkerModel.DailyPetWalkLimit,
        ServiceLocation = string.Join(", ", petWalkerModel.ServiceAreas)
      };

      var response = await _httpClient.PutAsJsonAsync(
          $"{_apiBaseUrl}/PetWalker/{petWalkerModel.Id}",
          updateRequest);

      if (response.IsSuccessStatusCode)
      {
        return new ApiResponse<bool>
        {
          Success = true,
          Message = "Pet walker updated successfully",
          Data = true,
          Timestamp = DateTime.Now
        };
      }

      var errorContent = await response.Content.ReadAsStringAsync();
      return new ApiResponse<bool>
      {
        Success = false,
        Message = $"Failed to update pet walker. Status: {response.StatusCode}. Details: {errorContent}",
        Data = false,
        Timestamp = DateTime.Now
      };
    }
    catch (Exception ex)
    {
      return new ApiResponse<bool>
      {
        Success = false,
        Message = $"Error updating pet walker: {ex.Message}",
        Data = false,
        Errors = new List<string> { ex.Message },
        Timestamp = DateTime.Now
      };
    }
  }

  private int ConvertGenderToInt(string gender)
  {
    return gender.ToLower() switch
    {
      "male" => 0,
      "female" => 1,
      "other" => 2,
      _ => 0
    };
  }

  public async Task DeletePetWalkerAsync(string email)
  {
    await _httpClient.DeleteAsync($"{_apiBaseUrl}/PetWalkers/{email}");
  }

  public async Task<ApiResponse<bool>> UpdateServiceAreasAsync(Guid petWalkerId, List<ServiceAreaDto> serviceAreas)
  {
    try
    {
      var validServiceAreas = serviceAreas
          .Where(sa => sa.LocalityId != Guid.Empty)
          .Select(sa => new { sa.LocalityId })
          .ToList();

      var request = new { ServiceAreas = validServiceAreas };

      var response = await _httpClient.PutAsJsonAsync(
          $"{_apiBaseUrl}/PetWalker/{petWalkerId}/ServiceAreas",
          request);

      if (response.IsSuccessStatusCode)
      {
        return new ApiResponse<bool>
        {
          Success = true,
          Message = "Service areas updated successfully",
          Data = true,
          Timestamp = DateTime.Now
        };
      }

      var errorContent = await response.Content.ReadAsStringAsync();
      return new ApiResponse<bool>
      {
        Success = false,
        Message = $"Failed to update service areas. Status: {response.StatusCode}. Details: {errorContent}",
        Data = false,
        Timestamp = DateTime.Now
      };
    }
    catch (Exception ex)
    {
      return new ApiResponse<bool>
      {
        Success = false,
        Message = $"Error updating service areas: {ex.Message}",
        Data = false,
        Errors = [ex.Message],
        Timestamp = DateTime.Now
      };
    }
  }

  public Task<ApiResponse<PetWalkerDetailDto>> GetPetWalkerByIdAsync(Guid petWalkerId)
  {
    throw new NotImplementedException();
  }
}

