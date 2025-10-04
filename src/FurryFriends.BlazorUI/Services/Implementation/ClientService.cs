using System.Text.Json;
using FurryFriends.BlazorUI.Client.Models.Clients;
using FurryFriends.BlazorUI.Client.Models.Common;
using FurryFriends.BlazorUI.Client.Services.Interfaces;

namespace FurryFriends.BlazorUI.Services.Implementation;

// DTO that matches the API response structure
public record WebClientDto(
    Guid Id,
    string Name,
    string Email,
    string City,
    int Pets,
    Dictionary<string, int> PetsBySpecies);

public class ClientService : IClientService
{
  private readonly HttpClient _httpClient;
  private readonly HttpClient _dogClient;
  private readonly ILogger<ClientService> _logger;

  public ClientService(HttpClient httpClient, HttpClient dogClient, IConfiguration configuration, ILogger<ClientService> logger)
  {
    _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    _dogClient = dogClient ?? throw new ArgumentNullException(nameof(dogClient));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
  }


  public async Task<List<ClientDto>> GetClientsAsync()
  {
    var response = await GetClientsAsync(1, 10);
    if (response is null || response.RowsData is null)
    {
      return new List<ClientDto>();
    }
    return response.RowsData;
  }


  public async Task<ClientResponseBase> GetClientAsync(Guid value)
  {
    var response = await _httpClient.GetFromJsonAsync<ClientResponseBase>($"Clients/id/{value}");
    if (response is null)
    {
      return new ClientResponseBase();
    }
    return response;
  }

  public async Task<ListResponse<ClientDto>> GetClientsAsync(int page, int pageSize, string? searchTerm = null)
  {
    try
    {
      var url = $"Clients/list?page={page}&pageSize={pageSize}";
      if (!string.IsNullOrEmpty(searchTerm))
      {
        url += $"&searchTerm={Uri.EscapeDataString(searchTerm)}";
      }

      _logger.LogInformation("Making API request to {Url}", url);

      // Directly deserialize the API response which returns ListResponse<Web.ClientDto>
      var response = await _httpClient.GetFromJsonAsync<ListResponse<WebClientDto>>(url,
        new System.Text.Json.JsonSerializerOptions
        {
          PropertyNameCaseInsensitive = true,
          PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
        });

      if (response is null || response.RowsData is null)
      {
        _logger.LogWarning("API returned a null response");
        return new ListResponse<ClientDto>();
      }

      // Map the Web.ClientDto to BlazorUI.Client.ClientDto
      var clientDtos = response.RowsData.Select(MapWebClientDtoToClientDto).ToList();

      return new ListResponse<ClientDto>
      {
        RowsData = clientDtos,
        PageNumber = response.PageNumber,
        PageSize = response.PageSize,
        TotalCount = response.TotalCount,
        TotalPages = response.TotalPages,
        HasPreviousPage = response.HasPreviousPage,
        HasNextPage = response.HasNextPage
      };
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error loading clients from API");
      throw;
    }
  }

  public ClientDto MapClientDataToDto(ClientData clientData)
  {
    // Implement the mapping logic here, for example:
    return new ClientDto
    {
      Id = clientData.Id,
      FullName = clientData.Name,
      EmailAddress = clientData.Email,
      City = clientData.City,
      PhoneNumber = clientData.PhoneNumber,
      TotalPets = clientData.Pets?.Length ?? 0,
      PetsBySpecies = clientData.Pets?
        .Where(p => p.isActive)
        .GroupBy(p => p.Species)
        .ToDictionary(g => g.Key, g => g.Count()) ?? new Dictionary<string, int>()
    };
  }

  private ClientDto MapWebClientDtoToClientDto(WebClientDto webClientDto)
  {
    return new ClientDto
    {
      Id = webClientDto.Id,
      FullName = webClientDto.Name,
      EmailAddress = webClientDto.Email,
      City = webClientDto.City,
      PhoneNumber = string.Empty, // Not available in the list API response
      TotalPets = webClientDto.Pets,
      PetsBySpecies = webClientDto.PetsBySpecies
    };
  }

  public async Task<ClientResponseBase> GetClientByEmailAsync(string email)
  {
    var response = await _httpClient.GetFromJsonAsync<ClientResponseBase>($"Clients/email/{email}");
    if (response is null)
    {
      return new ClientResponseBase();
    }
    return response;
  }

  public async Task CreateClientAsync(ClientRequestDto clientModel)
  {
    _logger.LogInformation("Creating new client with email: {Email}", clientModel.Email);

    var response = await _httpClient.PostAsJsonAsync("Clients", clientModel);

    if (!response.IsSuccessStatusCode)
    {
      var errorContent = await response.Content.ReadAsStringAsync();
      _logger.LogWarning("Failed to create client. Status: {StatusCode}, Error: {Error}",
        response.StatusCode, errorContent);
      throw new HttpRequestException($"Failed to create client: {errorContent}", null, response.StatusCode);
    }

    _logger.LogInformation("Successfully created client with email: {Email}", clientModel.Email);
  }

  public async Task UpdateClientAsync(ClientRequestDto clientModel)
  {
    _logger.LogInformation("Updating client with ID: {ClientId}, Email: {Email}", clientModel.Id, clientModel.Email);

    var response = await _httpClient.PutAsJsonAsync($"Clients/{clientModel.Id}", clientModel);

    if (!response.IsSuccessStatusCode)
    {
      var errorContent = await response.Content.ReadAsStringAsync();
      _logger.LogWarning("Failed to update client. Status: {StatusCode}, Error: {Error}",
        response.StatusCode, errorContent);
      throw new HttpRequestException($"Failed to update client: {errorContent}", null, response.StatusCode);
    }

    _logger.LogInformation("Successfully updated client with ID: {ClientId}", clientModel.Id);
  }

  public async Task<string> GetDogImageAsync()
  {
    var dogImage = await _dogClient.GetFromJsonAsync<DogImageResponse>("https://dog.ceo/api/breeds/image/random");
    if (dogImage is null)
    {
      return string.Empty;
    }
    else
    {
      return dogImage.Message.Replace("\\/", "/");
    }
  }

  public async Task<List<BreedDto>> GetBreedsAsync()
  {
    try
    {
      var response = await _httpClient.GetFromJsonAsync<List<BreedDto>>("Clients/breeds");
      if (response is null || response.Count == 0)
      {
        _logger.LogInformation("No breeds found or empty response from API");
        return [];
      }
      _logger.LogDebug("Successfully retrieved {Count} breeds from API", response.Count);
      return response;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error fetching breeds from API");
      return new List<BreedDto>();
    }
  }

  public async Task UpdatePetAsync(string clientEmail, PetDto pet)
  {
    _logger.LogInformation("Updating pet with ID: {PetId} for client: {ClientEmail}", pet.Id, clientEmail);

    // Create the request object for the API
    var updatePetRequest = new
    {
      ClientEmail = clientEmail,
      PetId = pet.Id,
      Name = pet.Name,
      BreedId = pet.BreedId, // Include the breed ID
      Age = pet.Age,
      Weight = pet.Weight,
      Color = "Unknown", // This field is required by the API but not in our UI model
      SpecialNeeds = pet.SpecialNeeds,
      MedicalConditions = pet.MedicalConditions,
      IsActive = pet.isActive,
      Photo = pet.Photo
    };

    // Send the update request to the API
    var response = await _httpClient.PutAsJsonAsync("Clients/pets", updatePetRequest);

    if (!response.IsSuccessStatusCode)
    {
      var errorContent = await response.Content.ReadAsStringAsync();
      _logger.LogWarning("Failed to update pet. Status: {StatusCode}, Error: {Error}",
        response.StatusCode, errorContent);
      throw new HttpRequestException($"Failed to update pet: {errorContent}", null, response.StatusCode);
    }

    _logger.LogInformation("Successfully updated pet with ID: {PetId}", pet.Id);
  }

  public async Task<Guid> AddPetAsync(Guid clientId, PetDto pet)
  {
    _logger.LogInformation("Adding new pet for client with ID: {ClientId}", clientId);

    // Create the request object for the API
    var addPetRequest = new
    {
      ClientId = clientId,
      Name = pet.Name,
      BreedId = pet.BreedId, // Use the selected breed ID
      Age = pet.Age,
      Weight = pet.Weight,
      Color = string.IsNullOrEmpty(pet.Breed) ? "Unknown" : pet.Breed, // Using breed as color if available
      MedicalHistory = pet.MedicalConditions,
      IsVaccinated = false, // Default value
      FavoriteActivities = string.Empty,
      DietaryRestrictions = string.Empty,
      SpecialNeeds = pet.SpecialNeeds,
      Photo = pet.Photo,
      Species = string.IsNullOrEmpty(pet.Species) ? "Dog" : pet.Species // Default to Dog if not specified
    };

    // Send the add request to the API
    var response = await _httpClient.PostAsJsonAsync("/Clients/pets", addPetRequest);

    if (!response.IsSuccessStatusCode)
    {
      var errorContent = await response.Content.ReadAsStringAsync();
      _logger.LogWarning("Failed to add pet. Status: {StatusCode}, Error: {Error}",
        response.StatusCode, errorContent);
      throw new HttpRequestException($"Failed to add pet: {errorContent}", null, response.StatusCode);
    }

    // Parse the response to get the new pet ID
    var responseContent = await response.Content.ReadAsStringAsync();
    var responseObject = JsonSerializer.Deserialize<AddPetResponse>(responseContent);
    var petId = responseObject?.PetId ?? Guid.Empty;

    if (petId != Guid.Empty)
    {
      _logger.LogInformation("Successfully added pet with ID: {PetId} for client: {ClientId}", petId, clientId);
    }
    else
    {
      _logger.LogWarning("Added pet but received empty pet ID for client: {ClientId}", clientId);
    }

    return petId;
  }


}
