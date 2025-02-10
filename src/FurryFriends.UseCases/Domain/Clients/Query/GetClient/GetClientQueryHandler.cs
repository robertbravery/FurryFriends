using FurryFriends.UseCases.Services.ClientService;
using MediatR;

namespace FurryFriends.UseCases.Domain.Clients.Query.GetClient;
public class GetClientQueryHandler(IClientService clientService)
  : IQueryHandler<GetClientQuery, Result<ClientDTO>>
{
  private readonly IClientService _clientService = clientService;

  async Task<Result<ClientDTO>> IRequestHandler<GetClientQuery, Result<ClientDTO>>.Handle(GetClientQuery request, CancellationToken cancellationToken)
  {
    var entityResult = await _clientService.GetClientAsync(request.EmailAddress, cancellationToken);

    if (!entityResult.IsSuccess)
    {
      return Result.NotFound("Client not found");
    }

    return new ClientDTO(
      entityResult.Value.Id,
      entityResult.Value.Name.FullName,
      entityResult.Value.Email.EmailAddress,
      entityResult.Value.PhoneNumber.Number,
      entityResult.Value.Address.Street,
      entityResult.Value.Address.City,
      entityResult.Value.Address.StateProvinceRegion,
      entityResult.Value.Address.ZipCode,
      entityResult.Value.ClientType,
      entityResult.Value.PreferredContactTime,
      entityResult.Value.ReferralSource

      );
  }
}
