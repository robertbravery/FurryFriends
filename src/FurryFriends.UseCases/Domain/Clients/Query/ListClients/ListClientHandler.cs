using FurryFriends.UseCases.Domain.Clients.DTO;
using FurryFriends.UseCases.Services.ClientService;

namespace FurryFriends.UseCases.Domain.Clients.Query.ListClients;
public class ListClientHandler(IClientService clientService)
  : IQueryHandler<ListClientQuery, Result<ClientsDto>>
{
  private readonly IClientService _clientService = clientService;

  public async Task<Result<ClientsDto>> Handle(ListClientQuery request, CancellationToken cancellationToken)
  {
    // Get the clients for the current page
    var result = await _clientService.ListClientsAsync(request, cancellationToken);
    if (!result.IsSuccess)
    {
      return Result.NotFound();
    }



    var clients = result.Value.Clients.Select(client => new ClientDTO(
      client.Id,
      client.Name.FullName,
      client.Email.EmailAddress,
      client.PhoneNumber.CountryCode,
      client.PhoneNumber.Number,
      client.Address.Street,
      client.Address.City,
      client.Address.StateProvinceRegion,
      client.Address.ZipCode,
      client.Address.Country,
      client.ClientType,
      client.PreferredContactTime,
      client.ReferralSource,
      [.. client.Pets.Select(p=> new ClientPetDto(
        p.Id,
        p.Name,
        p.BreedType.Species.Name,
        p.BreedType.Name,
        p.Age,
        p.Weight,
        p.IsSterilized,
        p.IsVaccinated,
        p.SpecialNeeds,
        p.MedicalConditions,
        p.IsActive))]

      )).ToList();

    return Result<ClientsDto>.Success(new ClientsDto(clients, result.Value.TotalCount));
  }
}
