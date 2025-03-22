using Ardalis.GuardClauses;
using FurryFriends.Core.ClientAggregate;
using FurryFriends.Core.ClientAggregate.Enums;
using FurryFriends.Core.ClientAggregate.Specifications;
using FurryFriends.Core.ValueObjects;

namespace FurryFriends.UseCases.Services.ClientService;

public class ClientService(IRepository<Client> repository) : IClientService
{
  private readonly IRepository<Client> _repository = repository;

  public async Task<Result<Client>> CreateClientAsync(
    Name name, 
    Email email, 
    PhoneNumber phoneNumber, 
    Address address,
    ClientType clientType = ClientType.Regular,
    ReferralSource referralSource = ReferralSource.None,
    CancellationToken cancellationToken = default)
  {
    var existingClientSpec = new ClientByEmailSpec(email.EmailAddress);
    if (await _repository.AnyAsync(existingClientSpec))
    {
      return Result.Error("A client with this email already exists");
    }

    var client = Client.Create(name, email, phoneNumber, address, clientType, referralSource);

    await _repository.AddAsync(client, cancellationToken);
    await _repository.SaveChangesAsync(cancellationToken);

    return Result.Success(client);
  }

  public async Task<Result<Client>> UpdateClientAsync(Client client)
  {
    var updatedClient = await _repository.GetByIdAsync(client.Id)
      ?? throw new NotFoundException(nameof(Client), client.Id.ToString());
    updatedClient.UpdateDetails(
        client.Name,
        client.Email,
        client.PhoneNumber,
        client.Address
    );

    client.UpdateClientType(client.ClientType);
    client.UpdatePreferredContactTime(client.PreferredContactTime);
    client.UpdateReferralSource(client.ReferralSource);

    await _repository.UpdateAsync(client);

    return client;
  }

  public async Task<Result<Client>> GetClientAsync(string emailAddress, CancellationToken cancellationToken)
  {
    var existingClientSpec = new ClientByEmailSpec(emailAddress, true);
    var client = await _repository.FirstOrDefaultAsync(existingClientSpec);
    if (client == null)
    {
      return Result.Error("Client not found");
    }
    return Result.Success(client);

  }

  public async Task<Result<IEnumerable<Client>>> ListClientsAsync(string? searchTerm, int page, int pageSize, CancellationToken cancellationToken)
  {
    var listSpec = new ListClientsSpec(searchTerm, page, pageSize);
    IEnumerable<Client> clients = await _repository.ListAsync(listSpec, cancellationToken);
    if (clients == null)
    {
      return Result.Error("No Clients found");
    }
    return Result.Success(clients);
  }
}


