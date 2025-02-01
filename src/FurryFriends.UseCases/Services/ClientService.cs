using FurryFriends.Core.ClientAggregate;
using FurryFriends.Core.ClientAggregate.Specifications;
using FurryFriends.Core.ValueObjects;
using FurryFriends.UseCase.Services;

namespace FurryFriends.UseCases.Services;

public class ClientService : IClientService
{
  private readonly IRepository<Client> _repository;

  public ClientService(IRepository<Client> repository)
  {
    _repository = repository;
  }

  public async Task<Result<Client>> CreateClientAsync(Name name, Email email, PhoneNumber phoneNumber, Address address)
  {
    var existingClientSpec = new ClientByEmailSpec(email.EmailAddress);
    if (await _repository.AnyAsync(existingClientSpec))
    {
      return Result.Error("A client with this email already exists");
    }

    var client = Client.Create(name, email, phoneNumber, address);

    await _repository.AddAsync(client);
    await _repository.SaveChangesAsync();

    return Result.Success(client);
  }


}


