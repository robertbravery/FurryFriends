using System.Text.Json;
using FurryFriends.BlazorUI.Client.Models.Clients;
using FurryFriends.BlazorUI.Client.Models.Common;
using FurryFriends.BlazorUI.Client.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace FurryFriends.BlazorUI.Services.Implementation;

public class ClientService : BaseListService<ClientDto>, IClientService
{
  private readonly HttpClient _httpClient;
  private readonly HttpClient _dogClient;
  private readonly ILogger<ClientService> _logger;

  public ClientService(HttpClient httpClient, HttpClient dogClient, IConfiguration configuration, ILogger<ClientService> logger)
    : base(httpClient, configuration, "Clients/list")
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

  public async Task<ListResponse<ClientDto>> GetClientsAsync(int page, int pageSize, string? searchTerm = null)
  {
    return await GetListAsync(page, pageSize, searchTerm);
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

  public async Task UpdatePetAsync(string clientEmail, Pet pet)
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

  public async Task<Guid> AddPetAsync(Guid clientId, Pet pet)
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
