using FurryFriends.UseCases.Domain.Clients.DTO;

namespace FurryFriends.UseCases.Domain.Clients.Query.ListClients;

public record ClientsDto(List<ClientDTO> Clients, int TotalCount);
