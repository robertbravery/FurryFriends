using FurryFriends.UseCases.Domain.Clients.DTO;
using FurryFriends.UseCases.Services.ClientService;

namespace FurryFriends.UseCases.Domain.Clients.Query.ListClients;
public record ListClientQuery(string? SearchTerm, int Page, int PageSize) : IQuery<Result<ClientsDto>>;
