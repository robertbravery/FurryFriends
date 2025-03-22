using FurryFriends.UseCases.Domain.Clients.DTO;
using FurryFriends.UseCases.Services.ClientService;

namespace FurryFriends.UseCases.Domain.Clients.Query.ListClients;
public class ListClientHandler(IClientService clientService)
  : IQueryHandler<ListClientQuery, Result<List<ClientDTO>>>
{
  private readonly IClientService _clientService = clientService;

  public async Task<Result<List<ClientDTO>>> Handle(ListClientQuery request, CancellationToken cancellationToken)
  {
    var result = await _clientService.ListClientsAsync(request.SearchTerm, request.Page, request.PageSize, cancellationToken);
    if (!result.IsSuccess)
    {
      return Result.NotFound();
    }
    var clients = result.Value.Select(client => new ClientDTO(
      client.Id,
      client.Name.FullName,
      client.Email.EmailAddress,
      client.PhoneNumber.ToString(),
      client.Address.Street,
      client.Address.City,
      string.Empty,
      client.Address.ZipCode,
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
    return Result<List<ClientDTO>>.Success(clients);
  }
}
