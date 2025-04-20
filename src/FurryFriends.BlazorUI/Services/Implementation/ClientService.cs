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

  public async Task CreateClientAsync(ClientRequestDto clientModel)
  {
    var response = await _httpClient.PostAsJsonAsync("Clients", clientModel);

    if (!response.IsSuccessStatusCode)
    {
      var errorContent = await response.Content.ReadAsStringAsync();
      throw new HttpRequestException($"Failed to create client: {errorContent}", null, response.StatusCode);
    }
  }
}
