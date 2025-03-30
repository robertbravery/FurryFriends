using System.Net.Http.Json;
using FurryFriends.BlazorUI.Client.Models.Clients;
using FurryFriends.BlazorUI.Client.Services.Interfaces;

namespace FurryFriends.BlazorUI.Client.Services.Implementation;
public class ClientService : IClientService
{
  private readonly HttpClient _httpClient;

  public ClientService(HttpClient httpClient)
  {
    _httpClient = httpClient;
  }

  //public async Task<List<ClientDto>> GetClientsAsync()
  public async Task<List<ClientDto>> GetClientsAsync()
  {
    //var res = await _httpClient.GetFromJsonAsync<object>("api/Clients/list?page=1&pageSize=10");
    var response = await _httpClient.GetFromJsonAsync<ListResponse>("Clients/list?page=1&pageSize=10");
    if (response is null || response.RowsData is null)
    {
      return new List<ClientDto>();
    }
    return response.RowsData;
  }
}
