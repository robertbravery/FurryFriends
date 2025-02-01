using FurryFriends.Core.ClientAggregate;
using FurryFriends.Core.ValueObjects;

namespace FurryFriends.UseCase.Services;

public interface IClientService
{
  Task<Result<Client>> CreateClientAsync(Name name, Email email, PhoneNumber phoneNumber, Address address);
}

