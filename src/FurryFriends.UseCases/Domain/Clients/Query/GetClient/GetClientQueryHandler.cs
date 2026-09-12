using FurryFriends.UseCases.Domain.Clients.DTO;
using FurryFriends.UseCases.Services.ClientService;

namespace FurryFriends.UseCases.Domain.Clients.Query.GetClient;

public class GetClientQueryHandler(IClientService clientService)
  : IQueryHandler<GetClientQuery, Result<ClientDTO>>
{
  private readonly IClientService _clientService = clientService;

  public async Task<Result<ClientDTO>> Handle(GetClientQuery request, CancellationToken cancellationToken)
  {
    var entityResult = await _clientService.GetClientAsync(request.EmailAddress, cancellationToken);

    if (!entityResult.IsSuccess)
    {
      return Result.NotFound("Client not found");
    }

    return entityResult.Value.ToDto();
  }
}
