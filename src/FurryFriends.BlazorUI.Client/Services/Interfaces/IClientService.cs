
using FurryFriends.BlazorUI.Client.Models.Clients;
using FurryFriends.BlazorUI.Client.Models.Common;

namespace FurryFriends.BlazorUI.Client.Services.Interfaces;
public interface IClientService
{
  Task<List<ClientDto>> GetClientsAsync();
  Task<ListResponse<ClientDto>> GetClientsAsync(int page, int pageSize, string? searchTerm = null);
  Task<ClientResponseBase> GetClientByEmailAsync(string email);
  Task CreateClientAsync(ClientRequestDto clientModel);
  Task UpdateClientAsync(ClientRequestDto clientModel);
  Task UpdatePetAsync(string clientEmail, PetDto pet);
  Task<Guid> AddPetAsync(Guid clientId, PetDto pet);
  Task<List<BreedDto>> GetBreedsAsync();

  Task<string> GetDogImageAsync();
  Task<ClientResponseBase> GetClientAsync(Guid value);
  ClientDto MapClientDataToDto(ClientData clientData);
}
