using FurryFriends.Core.ClientAggregate;

namespace FurryFriends.UseCases.Services.ClientService;

public record ClientListDto(List<Client> Clients, int TotalCount);
