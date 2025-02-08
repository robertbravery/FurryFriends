namespace FurryFriends.UseCases.Clients.GetClient;
public record GetClientQuery(string EmailAddress) : IQuery<Result<ClientDTO>>;
