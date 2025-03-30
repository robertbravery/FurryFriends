
using FurryFriends.BlazorUI.Client.Models.Clients;

namespace FurryFriends.BlazorUI.Client.Services.Interfaces;
public interface IClientService
{
  Task<List<ClientDto>> GetClientsAsync();
}
