using FurryFriends.UseCases.Domain.Clients.DTO;

namespace FurryFriends.UseCases.Domain.Clients.Query.ListClients;
public record ListClientQuery(string? SearchTerm, int Page, int PageSize) : IQuery<Result<List<ClientDTO>>>;
