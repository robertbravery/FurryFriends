using FurryFriends.BlazorUI.Client.Models.Clients;
using FurryFriends.BlazorUI.Client.Services.Interfaces;

namespace FurryFriends.BlazorUI.Services.Implementation;

public class ClientService : IClientService
{
  private readonly HttpClient _httpClient;
  private readonly HttpClient _dogClient;

  public ClientService(HttpClient httpClient, HttpClient dogClient)
  {
    _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    _dogClient = dogClient ?? throw new ArgumentNullException(nameof(dogClient));
  }

  public async Task<List<ClientDto>> GetClientsAsync()
  {
    var response = await _httpClient.GetFromJsonAsync<ListResponse>("Clients/list?page=1&pageSize=10");
    if (response is null || response.RowsData is null)
    {
      return new List<ClientDto>();
    }
    return response.RowsData;
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
    var response = await _httpClient.PostAsJsonAsync("Clients", clientModel);

    if (!response.IsSuccessStatusCode)
    {
      var errorContent = await response.Content.ReadAsStringAsync();
      throw new HttpRequestException($"Failed to create client: {errorContent}", null, response.StatusCode);
    }
  }

  public async Task UpdateClientAsync(ClientRequestDto clientModel)
  {
    var response = await _httpClient.PutAsJsonAsync($"Clients/email/{clientModel.Email}", clientModel);

    if (!response.IsSuccessStatusCode)
    {
      var errorContent = await response.Content.ReadAsStringAsync();
      throw new HttpRequestException($"Failed to update client: {errorContent}", null, response.StatusCode);
    }
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

  public async Task UpdatePetAsync(string clientEmail, Pet pet)
  {
    // Create the request object for the API
    var updatePetRequest = new
    {
      ClientEmail = clientEmail,
      PetId = pet.Id,
      Name = pet.Name,
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
      throw new HttpRequestException($"Failed to update pet: {errorContent}", null, response.StatusCode);
    }
  }
}

public class DogImageResponse
{
  public string Message { get; set; } = default!;
  public string Status { get; set; } = default!;
}
