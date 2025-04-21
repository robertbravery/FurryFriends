
using FurryFriends.BlazorUI.Client.Models.Clients;

namespace FurryFriends.BlazorUI.Client.Services.Interfaces;
public interface IClientService
{
  Task<List<ClientDto>> GetClientsAsync();
  Task<ClientResponseBase> GetClientByEmailAsync(string email);
  Task CreateClientAsync(ClientRequestDto clientModel);
  Task UpdateClientAsync(ClientRequestDto clientModel);

  Task<string> GetDogImageAsync();
}
