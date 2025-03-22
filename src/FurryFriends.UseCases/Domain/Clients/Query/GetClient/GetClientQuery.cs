using FurryFriends.UseCases.Domain.Clients.DTO;

namespace FurryFriends.UseCases.Domain.Clients.Query.GetClient;
public record GetClientQuery(string EmailAddress) : IQuery<Result<ClientDTO>>;
