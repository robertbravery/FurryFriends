using FurryFriends.BlazorUI.Client.Models.Clients;
using FurryFriends.BlazorUI.Client.Services.Interfaces;

namespace FurryFriends.BlazorUI.Services.Implementation;

public class ClientService(HttpClient httpClient) : IClientService
{
  private readonly HttpClient _httpClient = httpClient;

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
    if (response is null || response is null)
    {
      return new ClientResponseBase();
    }
    //return new ClientModel();
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
}
