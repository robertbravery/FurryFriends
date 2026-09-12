using FurryFriends.UseCases.Domain.Clients.DTO;
using FurryFriends.UseCases.Services.ClientService;

namespace FurryFriends.UseCases.Domain.Clients.Query.GetClient;

public class GetClientByIdQueryHandler(IClientService clientService)
  : IQueryHandler<GetClientByIdQuery, Result<ClientDTO>>
{
  private readonly IClientService _clientService = clientService;

  public async Task<Result<ClientDTO>> Handle(GetClientByIdQuery request, CancellationToken cancellationToken)
  {
    var entityResult = await _clientService.GetClientByIdAsync(request.Id, cancellationToken);

    if (!entityResult.IsSuccess)
    {
      return Result.NotFound("Client not found");
    }

    return entityResult.Value.ToDto();
  }
}
