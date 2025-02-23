using FurryFriends.Core.ClientAggregate;
using FurryFriends.Core.ValueObjects;

namespace FurryFriends.UseCases.Services.ClientService;

public interface IClientService
{
  Task<Result<Client>> CreateClientAsync(Name name, Email email, PhoneNumber phoneNumber, Address address, CancellationToken cancellationToken);
  Task<Result<Client>> GetClientAsync(string emailAddress, CancellationToken cancellationToken);
  Task<Result<IEnumerable<Client>>> ListClientsAsync(string? searchTerm, int page, int pageSize, CancellationToken cancellationToken);
  Task<Result<Client>> UpdateClientAsync(Client client);
}

