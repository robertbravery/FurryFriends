using Ardalis.GuardClauses;
using FurryFriends.Core.ClientAggregate;
using FurryFriends.Core.ValueObjects;
using FurryFriends.UseCases.Services.ClientService;
using MediatR;

namespace FurryFriends.UseCases.Domain.Clients.Command.UpdateClient;

public class UpdateClientCommandHandler : IRequestHandler<UpdateClientCommand, Client>
{
  private readonly IReadRepository<Client> _clientRepository;
  private readonly IClientService _clientService;

  public UpdateClientCommandHandler(IReadRepository<Client> _clientRepository, IClientService clientService)
  {
    this._clientRepository = _clientRepository;
    _clientService = clientService;
  }

  public async Task<Client> Handle(UpdateClientCommand request, CancellationToken cancellationToken)
  {
    Guard.Against.Null(request, nameof(request));
    Guard.Against.NullOrEmpty(request.ClientId, nameof(request.ClientId));
    Client client = await _clientRepository.GetByIdAsync(request.ClientId, cancellationToken) ?? throw new NotFoundException(nameof(Client), request.ClientId.ToString());
    client.UpdateDetails(
        Name.Create(request.FirstName, request.LastName).Value,
        Email.Create(request.Email).Value,
        await PhoneNumber.Create(request.CountryCode, request.PhoneNumber),
        Address.Create(request.Street, request.City, request.StateProvinceRegion, request.Country, request.ZipCode).Value
    );

    client.UpdateClientType(request.ClientType);
    client.UpdatePreferredContactTime(request.PreferredContactTime);
    client.UpdateReferralSource(request.ReferralSource);

    await _clientService.UpdateClientAsync(client);

    return client;
  }
}
